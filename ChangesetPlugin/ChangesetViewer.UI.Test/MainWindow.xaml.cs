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
        }

        public IEnumerable<Identity> AllUsersInTfs { get; set; }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            GetChangesetAsync();
            if (lstContainer.ItemsSource == null)
            {
                lstContainer.Items.Clear();
                lstContainer.ItemsSource = ChangeSetCollection;
                    //.Select(u => new { Id = u.ChangesetId, CommitterName = u.CommitterDisplayName, Comment = u.Comment });
            }

            //lstContainer.ItemsSource = null;
            //lstContainer.Items.Clear();
            //TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();

            //TFS.Reader.Infrastructure.IChangsets cs = new TFS.Reader.Infrastructure.Changesets(tfs);

            ////var ddd  = cs.Get(txtSource.Text.Trim(), 1500, txtSearchText.Text.Trim()).First();
            //var dd = cs.Get(txtSource.Text.Trim(), 1500, txtSearchText.Text.Trim());


            
                     
                     

            //    //.Select(u => new { Id = u.ChangesetId, CommitterName = u.Committer, Comment = u.Comment });


            //lstContainer.ItemsSource = dd
            //    .Select(u => new { Id = u.ChangesetId, CommitterName = u.CommitterDisplayName, Comment = u.Comment });

            ////foreach (var d in dd)
            ////{
            ////    lstContainer.Items.Add(new { Title = d.ChangesetId, Name = d.Committer });
            ////}
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {
            //Microsoft.TeamFoundation.VersionControl.Control vxc;



            LoadUsersInTFSasync();

            lstUsers.ItemsSource = UserCollectionInTFS;

            //TfsUsers users = new TfsUsers();
            ////var something = users.GetAllUsersInTFS().Select(u => new { Id = u.UniqueName, Name = u.DisplayName })
            ////    .DistinctBy(d => d.Id)
            ////    .OrderBy(d => d.Name);

            //var something = users.GetAllUsersInTFSBasedOnIdentity()
            //    .Where(u => !u.Deleted && u.Type == IdentityType.WindowsUser) 
            //    .Select(u => new { Id = u.TeamFoundationId, Name = u.DisplayName })
            //    .DistinctBy(d => d.Name)
            //    .OrderBy(d => d.Name);

            //lstUsers.ItemsSource = something;
            //lstContainer.ItemsSource = something;
            //lstContainer.DataContext
  

            //TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();
            //tfs.GetCollection();

            //var css4 = tfs.Collection.GetService<Microsoft.TeamFoundation.Server.ICommonStructureService4>();
            //TfsTeamService teamService = tfs.Collection.GetService<TfsTeamService>();

            //foreach (var projectInfo in css4.ListProjects())
            //{
            //    var allTeams = teamService.QueryTeams(projectInfo.Uri);
            //    foreach (TeamFoundationTeam team in allTeams)
            //    {
                    
            //        foreach (var mem in team.GetMembers(tfs.Collection, MembershipQuery.Direct))
            //        {
            //            lstContainer.Items.Add(new { Title = mem.UniqueName, Name = mem.DisplayName });
                        
            //        }
            //    }
            //}

            //ProjectInfo projectInfo = css4.GetProjectFromName("$/CLS");
            
            

            //$/CLS


            //tfs.Server.GetService<Microsoft.TeamFoundation.Client.TfsTeamService>();

            //tfs.Server.GetService<
        }

        private void lstUsers_DropDownOpened(object sender, EventArgs e)
        {
            if (lstUsers.ItemsSource == null)
            {
                LoadUsersInTFSasync();
                lstUsers.ItemsSource = UserCollectionInTFS;
            }
        }

        public async void LoadUsersInTFSasync()
        {
            TfsUsers users = new TfsUsers();
            Identity[] ident = await users.GetAllUsersInTFSBasedOnIdentityAsync();
            IObservable<Identity> usertoLoad = ident.ToObservable<Identity>();

            usertoLoad.Subscribe<Identity>(u =>
            {
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action<Identity>(AddUserToCollection), u);
            }, () =>
            {
                //this block will execute once the iteration is over.
            });
        }

        private void AddUserToCollection(Identity user)
        {
            this.UserCollectionInTFS.Add(user);
        }

        public async void GetChangesetAsync()
        {
            ChangeSetCollection.Clear();
            TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();
            TFS.Reader.Infrastructure.IChangsets cs = new TFS.Reader.Infrastructure.Changesets(tfs);

            IEnumerable<Changeset> changesets = await cs.GetAsync(txtSource.Text.Trim(), 1500, txtSearchText.Text.Trim());

            IObservable<Changeset> changesetToLoad = changesets.ToObservable<Changeset>();

            changesetToLoad.Subscribe<Changeset>(c =>
            {
                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new Action<Changeset>(AddChangesetToCollection), c);
            }, () =>
            {
                //this block will execute once the iteration is over.
            });
        }

        private void AddChangesetToCollection(Changeset changeset)
        {
            this.ChangeSetCollection.Add(changeset);
            lblTotalCount.Content = this.ChangeSetCollection.Count;
        }

        public ObservableCollection<Identity> UserCollectionInTFS { get; set; }
        public ObservableCollection<Changeset> ChangeSetCollection { get; set; }
        

    }

}
