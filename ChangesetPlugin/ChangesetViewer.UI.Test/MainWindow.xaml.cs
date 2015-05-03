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

namespace ChangesetViewer.UI.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            UserCollectionInTFS = new ObservableCollection<Identity>();
            ChangeSetCollection = new ObservableCollection<Microsoft.TeamFoundation.VersionControl.Client.Changeset>();

            workerChangesetFetch.DoWork += workerChangesetFetch_DoWork;
            workerChangesetFetch.RunWorkerCompleted += workerChangesetFetch_RunWorkerCompleted;

            workerUsersFetch.DoWork += workerUsersFetch_DoWork;
            workerUsersFetch.RunWorkerCompleted += workerUsersFetch_RunWorkerCompleted;

            loaderUser_Gif.Visibility = System.Windows.Visibility.Hidden;
        }

        
        public IEnumerable<Identity> AllUsersInTfs { get; set; }
        public ObservableCollection<Identity> UserCollectionInTFS { get; set; }
        public ObservableCollection<Changeset> ChangeSetCollection { get; set; }

        private readonly BackgroundWorker workerChangesetFetch = new BackgroundWorker();
        private readonly BackgroundWorker workerUsersFetch = new BackgroundWorker();

        private ChangesetSearchModel searchModel = new ChangesetSearchModel();

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
        }


        #region Changeset listing
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            loader_Gif.Source = new Uri("E://loader01.gif");
            loader_Gif.Play();
            loader_Gif.Visibility = System.Windows.Visibility.Visible;
            searchModel = new ChangesetSearchModel
            {
                ProjectSourcePath = txtSource.Text.Trim(),
                TopN = 1500,
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

            workerChangesetFetch.RunWorkerAsync();
            //InitializeTFSChangesetList();
        }

        void workerChangesetFetch_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }
        void workerChangesetFetch_DoWork(object sender, DoWorkEventArgs e)
        {
            InitializeTFSChangesetList();
        }

        public void InitializeTFSChangesetList()
        {
            GetChangesetAsync();
        }
        public async void GetChangesetAsync()
        {
            TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();
            TFS.Reader.Infrastructure.IChangsets cs = new TFS.Reader.Infrastructure.Changesets(tfs);

            IEnumerable<Changeset> changesets = await cs.GetAsync(searchModel);

            IObservable<Changeset> changesetToLoad = changesets.ToObservable<Changeset>();

            Action<Changeset> AddChangesetToCollection = (changeset) =>
            {
                
                this.ChangeSetCollection.Add(changeset);
                lblTotalCount.Content = this.ChangeSetCollection.Count;
            };

            changesetToLoad.Subscribe<Changeset>(c =>
            {
                App.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action<Changeset>(AddChangesetToCollection),
                    c);
            }, () =>
            {
                //this block will execute once the iteration is over.
                App.Current.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new Action(() =>
                    {
                        loader_Gif.Stop();
                        loader_Gif.Source = null;
                    }));
            });

            
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
                loaderUser_Gif.Source = new Uri("E://loader01.gif");
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
                        loaderUser_Gif.Source = null;
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


    }

}
