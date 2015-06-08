using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFS.Reader.Infrastructure;
using System.Reactive.Linq;
using System.Windows.Threading;
using System.Threading;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace ChangesetViewer.UI.Test.Infra
{
    public class ChangesetController
    {
        private ChangesetSearchModel _searchOptions;
        private IChangsets _changesets;

        public ChangesetModel _Model { get; set; }


        public Action EnableLoadNotificationUsers { get; set; }
        public Action DisableLoadNotificationUsers { get; set; }
        public Action EnableLoadNotificationChangeset { get; set; }
        public Action DisableLoadNotificatioChangeset { get; set; }
        public Action SearchButtonTextLoading { get; set; }
        public Action SearchButtonTextReset { get; set; }
        public Action<int> UpdateChangesetCount { get; set; }

        private readonly BackgroundWorker workerUsersFetch = new BackgroundWorker();
        private readonly BackgroundWorker workerChangesetFetch = new BackgroundWorker();
        private CancellationTokenSource _cts;
        


        public ChangesetController()
        {
            _Model = new ChangesetModel();
            _searchOptions = new ChangesetSearchModel();

            workerUsersFetch.DoWork += workerUsersFetch_DoWork;
            workerUsersFetch.RunWorkerCompleted += workerUsersFetch_RunWorkerCompleted;

            workerChangesetFetch.WorkerSupportsCancellation = true;
            workerChangesetFetch.WorkerReportsProgress = true;
            workerChangesetFetch.DoWork += workerChangesetFetch_DoWork;
            workerChangesetFetch.RunWorkerCompleted += workerChangesetFetch_RunWorkerCompleted;
        }

        #region User load
        public void LoadUsersAsync()
        {
            workerUsersFetch.RunWorkerAsync();
        }

        void workerUsersFetch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        void workerUsersFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadUsersInTFSasync();
        }

        private async void LoadUsersInTFSasync()
        {
            TfsUsers users = new TfsUsers();
            Identity[] ident = await users.GetAllUsersInTFSBasedOnIdentityAsync();
            IObservable<Identity> usertoLoad = ident.ToObservable<Identity>();

            Action<Identity> AddUserToCollection = (user) =>
            {
                _Model.UserCollectionInTFS.Add(user);
            };

            usertoLoad.Subscribe<Identity>(u =>
            {
                App.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action<Identity>(AddUserToCollection),
                    u);
            }, () =>
            {
                //this block will execute once the iteration is over.
                App.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        DisableLoadNotificationUsers.Invoke();
                        //loaderUser_Gif.Stop();
                        //loaderUser_Gif.Visibility = System.Windows.Visibility.Hidden;
                    }));
            });
        }

        #endregion

        void workerChangesetFetch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        void workerChangesetFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            GetChangesetAsync(e, _cts.Token);
        }

        public void GetChangesets(ChangesetSearchModel changesetSearchModel)
        {
            _searchOptions = changesetSearchModel;
            _cts = new CancellationTokenSource();
            workerChangesetFetch.RunWorkerAsync();
        }
        public void StopProcessingChangesetFetch()
        {
            _cts.Cancel();
            _changesets.CancelQueryHistorySearch();
        }

        private async void GetChangesetAsync(DoWorkEventArgs e, CancellationToken ct)
        {
            TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();
            _changesets = new TFS.Reader.Infrastructure.Changesets(tfs);

            IEnumerable<Changeset> changesets = await _changesets.GetAsync(_searchOptions);

            IObservable<Changeset> changesetToLoad = changesets.ToObservable<Changeset>();

            Action<Changeset> AddChangesetToCollection = (changeset) =>
            {
                _Model.ChangeSetCollection.Add(changeset);
                UpdateChangesetCount.Invoke(_Model.ChangeSetCollectionCount());
            };

            Action OnErrorOrComplete = () =>
            {
                App.Current.Dispatcher.Invoke(
                   DispatcherPriority.Background,
                   new Action(() =>
                   {
                       DisableLoadNotificatioChangeset.Invoke();
                       SearchButtonTextReset.Invoke();
                   }));
            };

            changesetToLoad.Subscribe<Changeset>(c =>
            {
                App.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action<Changeset>(AddChangesetToCollection),
                    c);
            }, (ex) =>
            {
                //this block will execute when there is some error or action cancelled which cause exception
                OnErrorOrComplete();
            }, () =>
            {
                //this block will execute once the iteration is over.
                OnErrorOrComplete();
            },
                ct
            );


        }


    }
}
