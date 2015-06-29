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


namespace ChangesetViewer.Core.UI
{
    public class ChangesetViewerController
    {
        private ChangesetSearchOptions _searchOptions;
        private IChangsets _changesets;

        public ChangesetViewerModel Model { get; set; }

        public EnvDTE80.DTE2 DTE { get; set; }
        public EnvDTE.IVsExtensibility Extensibility { get; set; }

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
            _searchOptions = new ChangesetSearchOptions();

            _workerUsersFetch.DoWork += workerUsersFetch_DoWork;

            _workerChangesetFetch.WorkerSupportsCancellation = true;
            _workerChangesetFetch.WorkerReportsProgress = true;
            _workerChangesetFetch.DoWork += workerChangesetFetch_DoWork;
            _workerChangesetFetch.RunWorkerCompleted += workerChangesetFetch_RunWorkerCompleted;
        }

        #region User load
        public void LoadUsersAsync()
        {
            _workerUsersFetch.RunWorkerAsync();
        }

        void workerUsersFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadUsersInTfsAsync();
        }

        private async void LoadUsersInTfsAsync()
        {
            var users = new TfsUsers();
            var ident = await users.GetAllUsersInTFSBasedOnIdentityAsync();
            var usertoLoad = ident.ToObservable();

            Action<Identity> addUserToCollection = user => Model.UserCollectionInTfs.Add(user);

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
            _searchOptions = changesetSearchModel;
            _cts = new CancellationTokenSource();
            _workerChangesetFetch.RunWorkerAsync();
        }
        public void StopProcessingChangesetFetch()
        {
            _cts.Cancel();
            _changesets.CancelAsyncQueryHistorySearch();
        }

        private async void GetChangesetAsync(CancellationToken ct)
        {
            ITfsServer tfs = new TfsServer();
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


        public void OpenChangesetWindow(string changesetId)
        {

           
            if (string.IsNullOrEmpty(changesetId))
                return;

            if (Extensibility == null)
                return;

            var cId = int.Parse(changesetId);
            if (cId == 0)
                return;


            DTE = Extensibility.GetGlobalsObject(null).DTE as EnvDTE80.DTE2;

            VersionControlExt vce;
            vce = DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            vce.ViewChangesetDetails(cId);
        }

    }
}
