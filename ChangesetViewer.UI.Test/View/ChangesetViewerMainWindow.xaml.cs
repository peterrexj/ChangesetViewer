using System;
using System.Windows;
using System.Windows.Controls;
using ChangesetViewer.Core.TFS;
using ChangesetViewer.Core.UI;
using System.Collections.Generic;
using PluginCore.Extensions;
using ChangesetViewer.Core.Settings;
using ChangesetViewer.Core;

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

            InitializeInternalComponents();

            //Remove this line
            //InitializeWindow();
        }

        public ChangesetViewerController _cController;

        private void InitializeInternalComponents()
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

            _cController.TfsServerContextChanged += _cController_TfsServerContextChanged;
        }

        void _cController_TfsServerContextChanged(object sender, EventArgs e)
        {
            _cController.Model.UserCollectionInTfs.Clear();
            //lstUsers.Items.Clear();
            //lstUsers.ItemsSource = null;
        }


        private void HandleErrorInUI(Exception ex = null)
        {
            if (ex == null)
            {
                txtErrors.Visibility = System.Windows.Visibility.Collapsed;
                txtErrors.Text = string.Empty;
            }
            else
            {
                txtErrors.Visibility = System.Windows.Visibility.Visible;
                txtErrors.Text = ex.Message;
            }
        }
        

        public void InitializeWindow()
        {
            loaderUser_Gif.Visibility = Visibility.Hidden;
            loader_Gif.Visibility = Visibility.Hidden;

            cboSearchType.ItemsSource = Enum.GetValues(typeof(Consts.SearchCommentType));
            txtSource.Text = _cController.GlobalSettings.DefaultTFSSearchPath;
            HandleErrorInUI();
        }

        
        private int _cancelHit;

        private ChangesetSearchOptions ReadOptionsValueFromUI()
        {
            return new ChangesetSearchOptions
            {
                ProjectSourcePath = txtSource.Text.Trim(),
                TopN = Int32.MaxValue,
                SearchKeyword = txtSearchText.Text.Trim(),
                Committer = lstUsers.Text,
                IsSearchBasedOnRegex = chkSearchBasedOnRegex.IsChecked.HasValue && chkSearchBasedOnRegex.IsChecked.Value,
                SearchCommentType = (Consts.SearchCommentType)Enum.Parse(typeof(Consts.SearchCommentType), cboSearchType.SelectedValue.ToString())
            };
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var processChangesetsPull = ActionExtensions.Create(() =>
                {
                    if (!_cController.IsVisualStudioIsConnectedToTFS())
                        return;

                    _cancelHit = 0;
                    //loader_Gif.Source = new System.Uri("ChangesetViewer.UI;resource\\loader01.gif", UriKind.RelativeOrAbsolute);
                    loader_Gif.Play();
                    loader_Gif.Visibility = Visibility.Visible;
                    var searchModel = ReadOptionsValueFromUI();
                    _cController.GetChangesets(searchModel);

                    if (lstContainer.ItemsSource == null)
                    {
                        lstContainer.Items.Clear();
                        lstContainer.ItemsSource = _cController.Model.ChangeSetCollection;
                    }
                    _cController.Model.ChangeSetCollection.Clear();
                    lblTotalCount.Content = "";
                    btnSearch.Content = "Stop";
                });

                var requestProcessingBreak = ActionExtensions.Create(() =>
                {
                    _cController.StopProcessingChangesetFetch();
                    _cancelHit = _cancelHit + 1;

                    if (_cancelHit <= 2) return;

                    _cController.DisableLoadNotificatioChangeset.Invoke();
                    _cController.SearchButtonTextReset.Invoke();
                });

                if (((Button)sender).Content.Equals("Search"))
                {
                    processChangesetsPull();
                }
                else if (((Button)sender).Content.Equals("Stop"))
                {
                    requestProcessingBreak();
                }
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
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
            if (!_cController.IsVisualStudioIsConnectedToTFS())
                return;

            //if (_cController.GlobalSettings.IsTfsServerChanged())
            //{
            //    lstUsers.ItemsSource = null;
            //    lstUsers.Items.Clear();
            //}

            if (lstUsers.ItemsSource != null && _cController.Model.UserCollectionInTfs.Count > 0) return;

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
            HandleErrorInUI(e.ErrorException);
            //MessageBox.Show(e.ErrorException.Message);
        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            _cController.UpdateSettingModel();
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

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            _cController.OpenChangesetWindow(e.Target);
        }

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnGoToChangeet_Click(object sender, RoutedEventArgs e)
        {
            _cController.OpenChangesetWindow(txtChangesetId.Text, true);
        }

        private void btnSelectServerPath_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
