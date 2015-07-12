using ChangesetViewer.Core.TFS;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using System.Reactive.Linq;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using ChangesetViewer.Core.Settings;
using Microsoft.VisualStudio.TeamFoundation;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Controls;
using Microsoft.TeamFoundation.Controls.WPF.TeamExplorer;


namespace ChangesetViewer.Core.UI
{
    public class ChangesetViewerController
    {

        private IChangsets _changesets;

        private ChangesetSearchOptions _searchOptions;
        private SettingsModelWrapper _globalSettings;
        private TeamFoundationServerExt _tfsServerContext;
        private EnvDTE80.DTE2 _dte;
        private bool _loadingUsers = false;

        public ChangesetViewerModel Model { get; set; }


        public SettingsModelWrapper GlobalSettings
        {
            get
            {
                if (_globalSettings == null)
                    _globalSettings = new SettingsModelWrapper();

                if (_globalSettings.DTE == null)
                    if (DTE != null)
                        _globalSettings.DTE = DTE;

                return _globalSettings;
            }
        }

        public EnvDTE80.DTE2 DTE
        {
            get { return _dte; }
            set
            { 
                _dte = value;
                _tfsServerContext = (TeamFoundationServerExt)_dte.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt");
                _tfsServerContext.ProjectContextChanged += tfsServerContext_ProjectContextChanged;
            }
        }
        public ITeamExplorer TeamExplorer { get; set; }

        public Action EnableLoadNotificationUsers { get; set; }
        public Action DisableLoadNotificationUsers { get; set; }
        public Action EnableLoadNotificationChangeset { get; set; }
        public Action DisableLoadNotificatioChangeset { get; set; }
        public Action SearchButtonTextLoading { get; set; }
        public Action SearchButtonTextReset { get; set; }
        public Action<int> UpdateChangesetCount { get; set; }

        private readonly BackgroundWorker _workerUsersFetch = new BackgroundWorker();
        private readonly BackgroundWorker _workerChangesetFetch = new BackgroundWorker();
        private CancellationTokenSource _cts;

        public ChangesetViewerController()
        {
            Model = new ChangesetViewerModel();
            //Settings = new SettingsModelWrapper();
            _searchOptions = new ChangesetSearchOptions();

            _workerUsersFetch.DoWork += workerUsersFetch_DoWork;
            _workerUsersFetch.RunWorkerCompleted += _workerUsersFetch_RunWorkerCompleted;

            _workerChangesetFetch.WorkerSupportsCancellation = true;
            _workerChangesetFetch.WorkerReportsProgress = true;
            _workerChangesetFetch.DoWork += workerChangesetFetch_DoWork;
            _workerChangesetFetch.RunWorkerCompleted += workerChangesetFetch_RunWorkerCompleted;
        }

        void _workerUsersFetch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _loadingUsers = false;
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

        private async void LoadUsersInTfsAsync()
        {
            _loadingUsers = true;
            var users = new TfsUsers(GlobalSettings.TFSServerURL, GlobalSettings.TFSUsername, GlobalSettings.TFSPassword);
            var ident = await users.GetAllUsersInTFSBasedOnIdentityAsync();
            var usertoLoad = ident.ToObservable();

            Action<Identity> addUserToCollection = (user) => 
            {
                if (!Model.UserCollectionInTfs.Contains(user))
                    Model.UserCollectionInTfs.Add(user);
            };

            usertoLoad.Subscribe(u =>
                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action<Identity>(addUserToCollection),
                    u),
                () => Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action(() => DisableLoadNotificationUsers.Invoke())));
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
            ITfsServer tfs = new TfsServer(GlobalSettings.TFSServerURL, GlobalSettings.TFSUsername, GlobalSettings.TFSPassword);
            _changesets = new Changesets(tfs);

            var changesets = await _changesets.GetAsync(_searchOptions);

            var changesetToLoad = changesets.ToObservable();

            Action<Changeset> addChangesetToCollection = changeset =>
            {
                Model.ChangeSetCollection.Add(changeset);
                UpdateChangesetCount.Invoke(Model.ChangeSetCollectionCount());
            };

            Action onErrorOrComplete = () => Application.Current.Dispatcher.Invoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    DisableLoadNotificatioChangeset.Invoke();
                    SearchButtonTextReset.Invoke();
                }));

            changesetToLoad.Subscribe(c =>
                Application.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action<Changeset>(addChangesetToCollection),
                    c),
                ex => onErrorOrComplete(),
                onErrorOrComplete,
                ct
            );


        }

        #endregion

        public void OpenChangesetWindow(string changesetId, bool requiresVerification = false)
        {
            if (string.IsNullOrEmpty(changesetId))
                return;

            if (changesetId == "0")
                return;

            int intChangesetID;
            int.TryParse(changesetId, out intChangesetID);

            if (!changesetId.Equals(intChangesetID.ToString()))
                return;

            if (!IsVisualStudioIsConnectedToTFS())
                return;

            if (requiresVerification)
            {
                ITfsServer tfs = new TfsServer(GlobalSettings.TFSServerURL, GlobalSettings.TFSUsername, GlobalSettings.TFSPassword);
                _changesets = new Changesets(tfs);
                var changeset = _changesets.Get(int.Parse(changesetId));
                if (changeset == null)
                    return;
            }

            var cId = int.Parse(changesetId);
            if (cId == 0)
                return;

            var pendingChangesPage = (TeamExplorerPageBase)TeamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.ChangesetDetails), cId);
            
        }


        public bool IsVisualStudioIsConnectedToTFS()
        {
            if (string.IsNullOrEmpty(_tfsServerContext.ActiveProjectContext.DomainUri))
            {
                var connectPage = (TeamExplorerPageBase)TeamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.Connect), null);
                return false;
            }

            return true;
        }

        public event EventHandler TfsServerContextChanged;

        protected virtual void OnTfsServerContextChanged(EventArgs e)
        {
            EventHandler handler = TfsServerContextChanged;
            if (handler != null)
                handler(this, e);
        }

        void tfsServerContext_ProjectContextChanged(object sender, EventArgs e)
        {
            OnTfsServerContextChanged(EventArgs.Empty);
        }

        public void Opena()
        {
            //TeamProjectPicker pp = new TeamProjectPicker(TeamProjectPickerMode.SingleProject, false);
            //pp.ShowDialog();

            //VersionControlExt vce;
            //vce = DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            //vce.ViewChangesetDetails(cId);


            //var tfsExt = (TeamFoundationServerExt)DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt");
            //var tfs = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tfsExt.ActiveProjectContext.DomainUri));
            //var pendingChangesPage = (TeamExplorerPageBase)TeamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.ChangesetDetails), cId);

            //var teamExplorer = (ITeamExplorer)GetService(typeof(ITeamExplorer));
            //var pendingChangesPage = (TeamExplorerPageBase)teamExplorer.NavigateToPage(new Guid(TeamExplorerPageIds.PendingChanges), null);

            //var workItemStore = tfs.GetService<WorkItemStore>();
            //var workItem = workItemStore.GetWorkItem(24065); // workItem is not null!

            //var model = (IPendingCheckin)pendingChangesPage.Model;
            //model.PendingChanges.Comment = "Hello, World!"; // Comment saved
            //model.WorkItems.CheckedWorkItems = new[]
            //{
            //    new WorkItemCheckinInfo(workItem, WorkItemCheckinAction.Associate),
            //}; // CheckedWorkItems not saved =(
        }
    }
}
