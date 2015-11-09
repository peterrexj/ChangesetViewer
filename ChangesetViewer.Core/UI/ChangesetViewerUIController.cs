using ChangesetViewer.Core.TFS;
using Microsoft.TeamFoundation.Server;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Reactive.Linq;
using ChangesetViewer.Core.Settings;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.TeamFoundation.Controls;


namespace ChangesetViewer.Core.UI
{
    public class ChangesetViewerUIController
    {
        private ChangesetSearchOptions _searchOptions;
        private SettingsModelWrapper _globalSettings;
        private TeamFoundationServerExt _tfsServerContext;
        private EnvDTE80.DTE2 _dte;
        private bool _loadingUsers;
        private ITfsServer _tfsServer;
        private ITfsUsers _tfsUsers;
        private ITfsChangsets _changesets;

        public SettingsModelWrapper GlobalSettings
        {
            get
            {
                if (_globalSettings == null)
                    _globalSettings = new SettingsModelWrapper();

                if (_globalSettings.DTE != null) return _globalSettings;
                if (DTE != null)
                    _globalSettings.DTE = DTE;

                return _globalSettings;
            }
        }

        private ITfsServer __TFSServer
        {
            get
            {
                return _tfsServer ??
                       (_tfsServer =
                           new TfsServer(GlobalSettings.TFSServerURL));
            }
        }
        private ITfsUsers __TFSUsers
        {
            get { return _tfsUsers ?? (_tfsUsers = new TfsUsers(__TFSServer, ErrorHandler)); }
        }

        private readonly BackgroundWorker _workerUsersFetch;
        private readonly BackgroundWorker _workerChangesetFetch;
        private CancellationTokenSource _cts;

        public ChangesetUIModel Model { get; set; }

        public EnvDTE80.DTE2 DTE
        {
            get { return _dte; }
            set
            {
                _dte = value;
                _tfsServerContext = (TeamFoundationServerExt)_dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt");
                _tfsServerContext.ProjectContextChanged += tfsServerContext_ProjectContextChanged;

                if (string.IsNullOrEmpty(Model.SourceControlName) && _tfsServerContext != null)
                    tfsServerContext_ProjectContextChanged(null, null);
            }
        }
        public ITeamExplorer TeamExplorer { get; set; }

        public Action EnableLoadNotificationUsers { get; set; }
        public Action DisableLoadNotificationUsers { get; set; }
        public Action EnableLoadNotificationChangeset { get; set; }
        public Action DisableLoadNotificatioChangeset { get; set; }
        public Action SearchButtonTextLoading { get; set; }
        public Action SearchButtonTextReset { get; set; }
        public Action<Exception> ErrorHandler { get; set; }

        public ChangesetViewerUIController()
        {
            Model = new ChangesetUIModel();
            _searchOptions = new ChangesetSearchOptions();

            _workerUsersFetch = new BackgroundWorker();
            _workerChangesetFetch = new BackgroundWorker();

            _workerUsersFetch.DoWork += workerUsersFetch_DoWork;
            _workerUsersFetch.RunWorkerCompleted += _workerUsersFetch_RunWorkerCompleted;

            _workerChangesetFetch.WorkerSupportsCancellation = true;
            _workerChangesetFetch.WorkerReportsProgress = true;
            _workerChangesetFetch.DoWork += workerChangesetFetch_DoWork;
            _workerChangesetFetch.RunWorkerCompleted += workerChangesetFetch_RunWorkerCompleted;


            UpdateSettingModel();
        }

        public void UpdateSettingModel()
        {
            _globalSettings = new SettingsModelWrapper();
            _globalSettings.DTE = DTE;
            SettingsStaticModelWrapper.FindJiraTicketsInComment = _globalSettings.FindJiraTicketsInComment;
            SettingsStaticModelWrapper.JiraTicketBrowseLink = _globalSettings.JiraTicketBrowseLink;
            SettingsStaticModelWrapper.JiraSearchRegexPattern = _globalSettings.JiraSearchRegexPattern;
        }


        #region User load
        public void LoadUsersAsync()
        {
            if (!_loadingUsers)
                if (!_workerUsersFetch.IsBusy)
                    _workerUsersFetch.RunWorkerAsync();
        }

        void workerUsersFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadUsersInTfsAsync();
        }
        void _workerUsersFetch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
        }

        private async void LoadUsersInTfsAsync()
        {
            
            //var users = new TfsUsers(GlobalSettings.TFSServerURL, GlobalSettings.TFSUsername, GlobalSettings.TFSPassword);
            //var ident = await users.GetAllUsersInTFSBasedOnIdentityAsync();
            //var usertoLoad = ident.ToObservable();
            if (!_loadingUsers)
            {
                _loadingUsers = true;

                Action onErrorOrComplete = () => Application.Current.Dispatcher.Invoke(
                      DispatcherPriority.Background,
                      new Action(() =>
                      {
                          _loadingUsers = false;
                          DisableLoadNotificationUsers.Invoke();
                      }));

                var identities = await __TFSUsers.GetAllUsersInTfsBasedOnIdentityAsync();

                if (identities == null)
                {
                    onErrorOrComplete();
                    return;
                }

                var usertoLoad = identities.ToObservable();

                Action<TeamFoundationUser> addUserToCollection = user =>
                {
                    if (!Model.UserCollectionInTfs.Contains(user))
                        Model.UserCollectionInTfs.Add(user);
                };

                usertoLoad.Subscribe(u =>
                    Application.Current.Dispatcher.Invoke(
                        DispatcherPriority.Background,
                        new Action<TeamFoundationUser>(addUserToCollection),
                        u),
                        onErrorOrComplete
                    );
            }
        }

        #endregion

        #region Changeset fetch

        void workerChangesetFetch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        void workerChangesetFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            GetChangesetAsync(_cts.Token);
        }

        public void GetChangesets(ChangesetSearchOptions changesetSearchModel)
        {
            if (!_workerChangesetFetch.IsBusy)
            {
                _searchOptions = changesetSearchModel;
                _cts = new CancellationTokenSource();
                _workerChangesetFetch.RunWorkerAsync();
            }
        }
        public void StopProcessingChangesetFetch()
        {
            _cts.Cancel();
            _changesets.CancelAsyncQueryHistorySearch();
        }

        private async void GetChangesetAsync(CancellationToken ct)
        {
            ITfsServer tfs = new TfsServer(GlobalSettings.TFSServerURL);
            _changesets = new TfsChangesets(tfs, ErrorHandler);

            Action onErrorOrComplete = () => Application.Current.Dispatcher.Invoke(
                   DispatcherPriority.Background,
                   new Action(() =>
                   {
                       DisableLoadNotificatioChangeset.Invoke();
                       SearchButtonTextReset.Invoke();
                   }));

            var changesets = await _changesets.GetAsync(_searchOptions);

            if (changesets == null)
            {
                onErrorOrComplete();
                return;
            }

            var changesetToLoad = changesets.ToObservable();

            Action<ChangesetViewModel> addChangesetToCollection = changeset => Model.ChangeSetCollection.Add(changeset);

            try
            {
                changesetToLoad.Subscribe(c =>
                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action<ChangesetViewModel>(addChangesetToCollection),
                    c),
                ex => onErrorOrComplete(),
                onErrorOrComplete,
                ct
            );
            }
            catch (QueryCancelRequest) { } //do nothing
            catch (Exception ex)
            {
                ErrorHandler(ex);
            }

        }

        #endregion

        #region Visual Studio Operations

        public event EventHandler TfsServerContextChanged;

        public void OpenChangesetWindow(string changesetId, bool requiresVerification = false)
        {
            if (string.IsNullOrEmpty(changesetId))
                return;

            if (changesetId == "0")
                return;

            int intChangesetID;
            int.TryParse(changesetId, out intChangesetID);

            if (!changesetId.Trim().Equals(intChangesetID.ToString()))
                return;

            if (!IsVisualStudioIsConnectedToTfs())
                return;

            if (requiresVerification)
            {
                ITfsServer tfs = new TfsServer(GlobalSettings.TFSServerURL);
                _changesets = new TfsChangesets(tfs, ErrorHandler);
                var changeset = _changesets.Get(int.Parse(changesetId));
                if (changeset == null)
                    return;
            }

            var cId = int.Parse(changesetId);
            if (cId == 0)
                return;

            TeamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.ChangesetDetails), cId);
        }

        //Any operation on tfs should pass through the this call
        public bool IsVisualStudioIsConnectedToTfs()
        {
            if (!string.IsNullOrEmpty(_tfsServerContext.ActiveProjectContext.DomainUri)) return true;
            TeamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.Connect), null);
            return false;
        }

        protected virtual void OnTfsServerContextChanged(EventArgs e)
        {
            EventHandler handler = TfsServerContextChanged;
            Model.UserCollectionInTfs.Clear();
            Model.ChangeSetCollection.Clear();
            if (handler != null)
                handler(this, e);

            _tfsServer = null;
            _tfsUsers = null;
        }

        void tfsServerContext_ProjectContextChanged(object sender, EventArgs e)
        {
            Model.SourceControlName = _tfsServerContext.ActiveProjectContext.ProjectName;
            OnTfsServerContextChanged(EventArgs.Empty);
        }

        #endregion

        public static void OpenWorkItemInWindow()
        {

        }

        public void ExportToExcel(Action enableUiControlsLevel1, Action enableUiControlsLevel2)
        {
            //Check the grid with some values then export that
            var s = new ChangesetExportHelper();

            s.ExportToExcel(Model.ChangeSetCollection, enableUiControlsLevel1, enableUiControlsLevel2);
        }
    }
}
