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
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            lstContainer.Items.Clear();
            TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();

            TFS.Reader.Infrastructure.IChangsets cs = new TFS.Reader.Infrastructure.Changesets(tfs);

            var dd = cs.Get(txtSource.Text.Trim(), 1500, txtSearchText.Text.Trim());
            foreach (var d in dd)
            {
                lstContainer.Items.Add(new { Title = d.ChangesetId, Name = d.Committer });
            }
        }

        private void btnUsers_Click(object sender, RoutedEventArgs e)
        {

            TfsUsers users = new TfsUsers();
            var something = users.GetAllUsersInTFS().Select(u => new { Title = u.UniqueName, Name = u.DisplayName });
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
    }
}
