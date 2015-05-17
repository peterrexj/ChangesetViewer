using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TFS.Reader.Infrastructure;
using PluginCore.Extensions;
using System.Reactive.Linq;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.ComponentModel;
using System.Reflection;
using System.Threading;

namespace ChangesetViewer.UI.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public IEnumerable<Identity> AllUsersInTfs { get; set; }
        public ObservableCollection<Identity> UserCollectionInTFS { get; set; }
        public ObservableCollection<Changeset> ChangeSetCollection { get; set; }

        private readonly BackgroundWorker workerChangesetFetch = new BackgroundWorker();
        private readonly BackgroundWorker workerUsersFetch = new BackgroundWorker();
        private CancellationTokenSource _cts;
        private TFS.Reader.Infrastructure.IChangsets _changesets;
        private ChangesetSearchModel searchModel;

        public MainWindow()
        {
            InitializeComponent();

            InitializeWindow();

        }

        public void InitializeWindow()
        {
            UserCollectionInTFS = new ObservableCollection<Identity>();
            ChangeSetCollection = new ObservableCollection<Microsoft.TeamFoundation.VersionControl.Client.Changeset>();

            searchModel = new ChangesetSearchModel();

            workerChangesetFetch.WorkerSupportsCancellation = true;
            workerChangesetFetch.WorkerReportsProgress = true;
            workerChangesetFetch.DoWork += workerChangesetFetch_DoWork;
            workerChangesetFetch.RunWorkerCompleted += workerChangesetFetch_RunWorkerCompleted;

            workerUsersFetch.DoWork += workerUsersFetch_DoWork;
            workerUsersFetch.RunWorkerCompleted += workerUsersFetch_RunWorkerCompleted;

            loaderUser_Gif.Visibility = System.Windows.Visibility.Hidden;
        }
        

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            Test();
        }


        #region Changeset listing
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.Equals("Search"))
            {
                _cts = new CancellationTokenSource();
                loader_Gif.Play();
                loader_Gif.Visibility = System.Windows.Visibility.Visible;
                searchModel = new ChangesetSearchModel
                {
                    ProjectSourcePath = txtSource.Text.Trim(),
                    TopN = System.Int32.MaxValue,
                    SearchKeyword = txtSearchText.Text.Trim(),
                    Committer = lstUsers.Text
                };
                if (lstContainer.ItemsSource == null)
                {
                    lstContainer.Items.Clear();
                    lstContainer.ItemsSource = ChangeSetCollection;
                }
                ChangeSetCollection.Clear();
                lblTotalCount.Content = "";
                btnSearch.Content = "Stop";

                workerChangesetFetch.RunWorkerAsync();
            }
            else if (((Button)sender).Content.Equals("Stop"))
            {
                _cts.Cancel();
                _changesets.CancelQueryHistorySearch();
            }
        }

        void workerChangesetFetch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
        void workerChangesetFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            GetChangesetAsync(e, _cts.Token);
        }


        public async void GetChangesetAsync(DoWorkEventArgs e, CancellationToken ct)
        {
            TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();
            _changesets = new TFS.Reader.Infrastructure.Changesets(tfs);

            IEnumerable<Changeset> changesets = await _changesets.GetAsync(searchModel);

            IObservable<Changeset> changesetToLoad = changesets.ToObservable<Changeset>();

            Action<Changeset> AddChangesetToCollection = (changeset) =>
            {
                this.ChangeSetCollection.Add(changeset);
                lblTotalCount.Content = this.ChangeSetCollection.Count;
            };

            Action OnErrorOrComplete = () => {
                App.Current.Dispatcher.Invoke(
                   DispatcherPriority.Background,
                   new Action(() =>
                   {
                       loader_Gif.Stop();
                       loader_Gif.Visibility = System.Windows.Visibility.Hidden;
                       btnSearch.Content = "Search";
                       //loader_Gif.Source = null;
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
            }, () => {
                //this block will execute once the iteration is over.
                OnErrorOrComplete();  
            },
            ct
            );

            
        }
        #endregion

        #region User List

        private void lstUsers_DropDownOpened(object sender, EventArgs e)
        {
            InitializeUserList();
        }
        private void lstUsers_GotFocus(object sender, RoutedEventArgs e)
        {
            InitializeUserList();
        }

        void workerUsersFetch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }
        void workerUsersFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            LoadUsersInTFSasync();
        }

        public void InitializeUserList()
        {
            
            if (lstUsers.ItemsSource == null)
            {
                loaderUser_Gif.Play();
                loaderUser_Gif.Visibility = System.Windows.Visibility.Visible;

                lstUsers.ItemsSource = UserCollectionInTFS;
                workerUsersFetch.RunWorkerAsync();
            }
        }
        public async void LoadUsersInTFSasync()
        {
            TfsUsers users = new TfsUsers();
            Identity[] ident = await users.GetAllUsersInTFSBasedOnIdentityAsync();
            IObservable<Identity> usertoLoad = ident.ToObservable<Identity>();

            Action<Identity> AddUserToCollection = (user) =>
            {
                this.UserCollectionInTFS.Add(user);
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
                        loaderUser_Gif.Stop();
                        loaderUser_Gif.Visibility = System.Windows.Visibility.Hidden;
                    }));
            });
        }
        #endregion

        private void loader_Gif_MediaEnded(object sender, RoutedEventArgs e)
        {
            var obj = (MediaElement)sender;
            obj.Position = new TimeSpan(0, 0, 1);
            obj.Play();
        }

        private void loader_Gif_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            MessageBox.Show(e.ErrorException.Message);
        }


        private void Test()
        {
            TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();
            tfs.GetCollection();

            StringBuilder sp = new StringBuilder();
             
            //var projectCollection = (ICommonStructureService)tfs.Collection.GetService(typeof(ICommonStructureService));

            VersionControlServer version = tfs.Collection.GetService(typeof(VersionControlServer)) as VersionControlServer;

            ItemSet items = version.GetItems(@"$\ProjectName", RecursionType.Full);
            //ItemSet items = version.GetItems(@"$\ProjectName\FileName.cs", RecursionType.Full);

            foreach (Item item in items.Items)
            {
                MessageBox.Show(item.ItemId.ToString());
                //System.Console.WriteLine(item.ServerItem);
                sp.Append(item.ServerItem).Append(",");
            }

            MessageBox.Show(sp.ToString());

            //projects.DataContext = new
            //{
            //    Projects = projectCollection.ListProjects().Select(x => x.Name)
            //};
            //projects.ItemsSource = projectCollection.ListProjects();

            //MessageBox.Show(sp.ToString());
        }


    }

}
