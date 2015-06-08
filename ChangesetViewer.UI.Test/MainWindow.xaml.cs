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
using ChangesetViewer.UI.Test.Infra;

namespace ChangesetViewer.UI.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ChangesetController cController;

        public MainWindow()
        {
            InitializeComponent();

            InitializeWindow();

        }

        public void InitializeWindow()
        {
            cController = new ChangesetController();

            cController.EnableLoadNotificationUsers = EnableUINotificationUsers;
            cController.DisableLoadNotificationUsers = DiableUINotificationUsers;
            cController.EnableLoadNotificationChangeset = EnableUINotificationChangeset;
            cController.DisableLoadNotificatioChangeset = DisableUINotificationChangeset;
            cController.SearchButtonTextLoading = SearchButtonTextLoading;
            cController.SearchButtonTextReset = SearchButtonTextReset;
            cController.UpdateChangesetCount = UpdateTotalCount;

            loaderUser_Gif.Visibility = System.Windows.Visibility.Hidden;
            loader_Gif.Visibility = System.Windows.Visibility.Hidden;
        }


        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.Equals("Search"))
            {
                loader_Gif.Play();
                loader_Gif.Visibility = System.Windows.Visibility.Visible;
                ChangesetSearchModel searchModel = new ChangesetSearchModel
                {
                    ProjectSourcePath = txtSource.Text.Trim(),
                    TopN = System.Int32.MaxValue,
                    SearchKeyword = txtSearchText.Text.Trim(),
                    Committer = lstUsers.Text,
                    IsSearchBasedOnRegex = chkSearchBasedOnRegex.IsChecked.HasValue ? chkSearchBasedOnRegex.IsChecked.Value : false
                };
                cController.GetChangesets(searchModel);

                if (lstContainer.ItemsSource == null)
                {
                    lstContainer.Items.Clear();
                    lstContainer.ItemsSource = cController._Model.ChangeSetCollection;
                }
                cController._Model.ChangeSetCollection.Clear();
                lblTotalCount.Content = "";
                btnSearch.Content = "Stop";

            }
            else if (((Button)sender).Content.Equals("Stop"))
            {
                cController.StopProcessingChangesetFetch();
            }
        }

        private void lstUsers_DropDownOpened(object sender, EventArgs e)
        {
            InitializeUserList();
        }
        private void lstUsers_GotFocus(object sender, RoutedEventArgs e)
        {
            InitializeUserList();
        }

        public void InitializeUserList()
        {
            if (lstUsers.ItemsSource == null)
            {
                loaderUser_Gif.Play();
                loaderUser_Gif.Visibility = System.Windows.Visibility.Visible;

                lstUsers.ItemsSource = cController._Model.UserCollectionInTFS;

                cController.LoadUsersAsync();
            }
        }

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

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            Test();
        }
        private void Test()
        {

            Changeset s;
            
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

        public void EnableUINotificationUsers()
        {

        }
        public void DiableUINotificationUsers()
        {
            loaderUser_Gif.Stop();
            loaderUser_Gif.Visibility = System.Windows.Visibility.Hidden;
        }
        public void EnableUINotificationChangeset()
        {

        }
        public void DisableUINotificationChangeset()
        {
            loader_Gif.Stop();
            loader_Gif.Visibility = System.Windows.Visibility.Hidden;
        }
        public void SearchButtonTextLoading()
        {
            btnSearch.Content = "Stop";
        }
        public void SearchButtonTextReset()
        {
            btnSearch.Content = "Search";
        }
        public void UpdateTotalCount(int count)
        {
            lblTotalCount.Content = count.ToString();
        }

    }

}
