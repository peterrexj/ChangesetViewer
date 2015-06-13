using System;
using System.Windows;
using System.Windows.Controls;
using ChangesetViewer.Core.TFS;
using ChangesetViewer.Core.UI;

namespace ChangesetViewer.UI.View
{
    /// <summary>
    /// Interaction logic for ChangesetViewerMainWindow.xaml
    /// </summary>
    public partial class ChangesetViewerMainWindow : UserControl
    {
        public ChangesetViewerMainWindow()
        {
            InitializeComponent();

            InitializeWindow();

        }

        public ChangesetViewerController _cController;

        public void InitializeWindow()
        {
            _cController = new ChangesetViewerController
            {
                EnableLoadNotificationUsers = EnableUINotificationUsers,
                DisableLoadNotificationUsers = DiableUINotificationUsers,
                EnableLoadNotificationChangeset = EnableUINotificationChangeset,
                DisableLoadNotificatioChangeset = DisableUINotificationChangeset,
                SearchButtonTextLoading = SearchButtonTextLoading,
                SearchButtonTextReset = SearchButtonTextReset,
                UpdateChangesetCount = UpdateTotalCount
            };

            loaderUser_Gif.Visibility = Visibility.Hidden;
            loader_Gif.Visibility = Visibility.Hidden;
        }

        private int _cancelHit;

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Content.Equals("Search"))
            {
                _cancelHit = 0;
                loader_Gif.Play();
                loader_Gif.Visibility = Visibility.Visible;
                var searchModel = new ChangesetSearchOptions
                {
                    ProjectSourcePath = txtSource.Text.Trim(),
                    TopN = Int32.MaxValue,
                    SearchKeyword = txtSearchText.Text.Trim(),
                    Committer = lstUsers.Text,
                    IsSearchBasedOnRegex = chkSearchBasedOnRegex.IsChecked.HasValue && chkSearchBasedOnRegex.IsChecked.Value
                };
                _cController.GetChangesets(searchModel);

                if (lstContainer.ItemsSource == null)
                {
                    lstContainer.Items.Clear();
                    lstContainer.ItemsSource = _cController.Model.ChangeSetCollection;
                }
                _cController.Model.ChangeSetCollection.Clear();
                lblTotalCount.Content = "";
                btnSearch.Content = "Stop";

            }
            else if (((Button)sender).Content.Equals("Stop"))
            {
                _cController.StopProcessingChangesetFetch();
                _cancelHit = _cancelHit + 1;

                if (_cancelHit <= 2) return;

                _cController.DisableLoadNotificatioChangeset.Invoke();
                _cController.SearchButtonTextReset.Invoke();
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
            if (lstUsers.ItemsSource != null) return;

            loaderUser_Gif.Play();
            loaderUser_Gif.Visibility = Visibility.Visible;

            lstUsers.ItemsSource = _cController.Model.UserCollectionInTfs;

            _cController.LoadUsersAsync();
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
