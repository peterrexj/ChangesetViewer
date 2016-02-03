using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Navigation;
using ChangesetViewer.Core.TFS;
using ChangesetViewer.Core.UI;
using PluginCore.Extensions;
using ChangesetViewer.Core;

namespace ChangesetViewer.UI.View
{
    /// <summary>
    /// Interaction logic for ChangesetViewerMainWindow.xaml
    /// </summary>
    public partial class ChangesetViewerMainWindow
    {
        public ChangesetViewerUIController UIController { get; set; }
        private int _currentpage = 1;

        public ChangesetViewerMainWindow()
        {
            InitializeComponent();

            InitializeInternalComponents();

            //Remove this line
            //InitializeWindow();

            txtSourceControlName.DataContext = UIController.Model;
            lblTotalCount.DataContext = UIController.Model;
            numberOfItemsFondPanel.DataContext = UIController.Model;

            buttonsPanel.DataContext = UIController.Model;

            ModelChangesetDataContextBinder();

            //lstContainer.AddHandler(ScrollViewer.ScrollChangedEvent, new ScrollChangedEventHandler(LstScrollView_ScrollChanged));
            //lstContainer.AddHandler(ScrollBar.ScrollEvent, new ScrollEventHandler(LstScrollView_ScrollChanged));
            HandleDisplayOfShelvesetButton();
        }

        public void ModelChangesetDataContextBinder()
        {
            btnFetchMore.DataContext = UIController.Model;
        }

        void LstScrollView_ScrollChanged(object sender, ScrollEventArgs e)
        {
            ScrollBar sb = e.OriginalSource as ScrollBar;
            if (sb == null)
                return;

            if (sb.Orientation == Orientation.Horizontal)
                return;

            if (sb.Value == sb.Maximum)
            {
                //Fetch more items from the scource control
                //_currentpage++;
                //GetChangesetsFromServer(_currentpage);
            }
        }


        private int _cancelHit;

        public enum DateFilterType
        {
            Today,
            Week,
            Month
        }

        private void InitializeInternalComponents()
        {
            UIController = new ChangesetViewerUIController
            {
                DisableLoadNotificationUsers = DiableUiNotificationUsers,
                EnableLoadNotificationChangeset = EnableUiNotificationChangeset,
                DisableLoadNotificatioChangeset = DisableUiNotificationChangeset,
                SearchButtonTextLoading = SearchButtonTextLoading,
                SearchButtonTextReset = SearchButtonTextReset,
                ErrorHandler = HandleErrorInUI,
            };

            UIController.TfsServerContextChanged += _cController_TfsServerContextChanged;

            DiableUiNotificationUsers();
            DisableUiNotificationChangeset();
        }
        public void InitializeWindow()
        {
            cboSearchType.ItemsSource = Enum.GetValues(typeof(Consts.SearchCommentType));
            txtSource.Text = UIController.GlobalSettings.DefaultTFSSearchPath;
            HandleErrorInUI();
            UIController.IsVisualStudioIsConnectedToTfs();
        }

        void _cController_TfsServerContextChanged(object sender, EventArgs e)
        {
            try
            {
                UIController.Model.InitializeUsersModel();
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }

        private void HandleErrorInUI(Exception ex = null)
        {
            Dispatcher.Invoke(() =>
            {
                if (ex == null)
                {
                    txtErrors.Visibility = Visibility.Collapsed;
                    txtErrors.Text = string.Empty;
                }
                else
                {
                    txtErrors.Visibility = Visibility.Visible;
                    txtErrors.Text = ex.Message;
                }
            });
        }

        private ChangesetSearchOptions ReadOptionsValueFromUI()
        {
            var options = new ChangesetSearchOptions
            {
                ProjectSourcePath = txtSource.Text.Trim(),
                TopN = Int32.MaxValue,
                SearchKeyword = txtSearchText.Text.Trim(),
                Committer = lstUsers.Text,
                SearchCommentType = (Consts.SearchCommentType)Enum.Parse(typeof(Consts.SearchCommentType), cboSearchType.SelectedValue.ToString()),
                StartDate = startDate.SelectedDate
            };

            //End date should have time till the end of the day
            if (endDate.SelectedDate.HasValue)
                options.EndDate = DateExtensions.GetEndOfDay(endDate.SelectedDate.Value);

            if (chkToday.IsChecked.HasValue && chkToday.IsChecked.Value)
            {
                options.StartDate = DateExtensions.GetStartOfDay(DateTime.Now);
                options.EndDate = DateExtensions.GetEndOfDay(DateTime.Now);
            }
            else if (chkWeek.IsChecked.HasValue && chkWeek.IsChecked.Value)
            {
                options.StartDate = DateExtensions.GetStartOfDay(DateTime.Now.AddDays(-7));
                options.EndDate = DateExtensions.GetEndOfDay(DateTime.Now);
            }
            else if (chkMonth.IsChecked.HasValue && chkMonth.IsChecked.Value)
            {
                options.StartDate = DateExtensions.GetStartOfDay(DateTime.Now.AddDays(-30));
                options.EndDate = DateExtensions.GetEndOfDay(DateTime.Now);
            }

            return options;
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HandleErrorInUI(); //clear the error from the UI

                UIController.UpdateSettingModel();

                if (!UIController.Model.IsSearchingMode)
                {
                    _currentpage = 1;
                    GetChangesetsFromServer();
                }
                else
                {
                    StopGettingChangesetsFromServer();
                }
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }

        private void btnFetchMore_Click(object sender, RoutedEventArgs e)
        {
            _currentpage++;
            GetChangesetsFromServer(_currentpage);
        }

        private void lstUsers_DropDownOpened(object sender, EventArgs e)
        {
            try
            {
                InitializeUserList();
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        private void lstUsers_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeUserList();
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        public void InitializeUserList()
        {
            HandleErrorInUI(); //clear the error from the UI

            if (!UIController.IsVisualStudioIsConnectedToTfs())
                return;

            if (lstUsers.ItemsSource != null && UIController.Model.UserCollectionInTfs.Count > 0) return;

            spinnerUser.IsSpinning = true;
            spinnerUser.Visibility = Visibility.Visible;

            lstUsers.ItemsSource = UIController.Model.UserCollectionInTfs;

            UIController.LoadUsersAsync();
        }


        public void DiableUiNotificationUsers()
        {
            try
            {
                spinnerUser.IsSpinning = false;
                spinnerUser.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        public void EnableUiNotificationChangeset()
        {
            try
            {
                spinnerchangeset.IsSpinning = true;
                spinnerchangeset.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        public void DisableUiNotificationChangeset()
        {
            try
            {
                spinnerchangeset.IsSpinning = false;
                spinnerchangeset.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        public void SearchButtonTextLoading()
        {
            btnSearch.Content = "Stop";
        }
        public void SearchButtonTextReset()
        {
            btnSearch.Content = "Search";
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                HandleErrorInUI(); //clear the error from the UI

                UIController.OpenChangesetWindow(e.Target);
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HandleErrorInUI(); //clear the error from the UI

                var disableControls = ActionExtensions.Create(() =>
                {
                    btnExportToExcel.IsEnabled = false;
                    btnSearch.IsEnabled = false;
                    btnExportToExcel.Content = "Exporting..";
                });

                var enableControls1 = ActionExtensions.Create(() => Dispatcher.Invoke(() =>
                {
                    btnSearch.IsEnabled = true;
                }));

                var enableControls2 = ActionExtensions.Create(() => Dispatcher.Invoke(() =>
                {
                    btnExportToExcel.IsEnabled = true;
                    btnExportToExcel.Content = "Export to Excel";
                }));

                disableControls();

                UIController.ExportToExcel(enableControls1, enableControls2);
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }

        }
        private void btnGoToChangeet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HandleErrorInUI(); //clear the error from the UI

                UIController.OpenChangesetWindow(txtChangesetId.Text, true);
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        private void btnGoToShelveset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HandleErrorInUI(); //clear the error from the UI

                UIController.OpenShelvesetWindow(lstUsers.Text);
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        private void btnSelectServerPath_Click(object sender, RoutedEventArgs e)
        {
            HandleErrorInUI(); //clear the error from the UI
        }
        private void RichTextboxCustomized_MouseUp(object sender, MouseButtonEventArgs e)
        {
            //dummy event which is req. Do not delete 
        }
        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            HandleDisplayOfShelvesetButton();
        }
        private void lstUsers_KeyUp(object sender, KeyEventArgs e)
        {
            HandleDisplayOfShelvesetButton();
        }

        #region Checkbox change event handler

        private void chkToday_Checked(object sender, RoutedEventArgs e)
        {
            if (chkToday.IsChecked.HasValue && chkToday.IsChecked.Value)
                HandleCheckDateField(DateFilterType.Today);
        }
        private void chkWeek_Checked(object sender, RoutedEventArgs e)
        {
            if (chkWeek.IsChecked.HasValue && chkWeek.IsChecked.Value)
                HandleCheckDateField(DateFilterType.Week);
        }
        private void chkMonth_Checked(object sender, RoutedEventArgs e)
        {
            if (chkMonth.IsChecked.HasValue && chkMonth.IsChecked.Value)
                HandleCheckDateField(DateFilterType.Month);
        }
        private void HandleCheckDateField(DateFilterType dtType)
        {
            ClearDateRangeUiFields();

            switch (dtType)
            {
                case DateFilterType.Today:
                    chkWeek.IsChecked = false;
                    chkMonth.IsChecked = false;
                    break;
                case DateFilterType.Week:
                    chkToday.IsChecked = false;
                    chkMonth.IsChecked = false;
                    break;
                case DateFilterType.Month:
                    chkWeek.IsChecked = false;
                    chkToday.IsChecked = false;
                    break;
            }
        }

        private void ClearDateRangeUiFields()
        {
            startDate.SelectedDate = null;
            endDate.SelectedDate = null;
        }
        #endregion

        private void txtChangesetId_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnGoToChangeet_Click(null, null);
            }
        }

        #region Private Methods

        private void GetChangesetsFromServer(int page = 1)
        {
            if (!UIController.IsVisualStudioIsConnectedToTfs())
                return;

            _cancelHit = 0;
            EnableUiNotificationChangeset();
            var searchModel = ReadOptionsValueFromUI();
            searchModel.PagingInfo.Page = page;

            if (page == 1)
            {
                UIController.Model.InitializeChangesetsModel();
                lstContainer.ItemsSource = null;
                lstContainer.ItemsSource = UIController.Model.ChangeSetCollection;
            }

            UIController.GetChangesets(searchModel);

            btnSearch.Content = "Stop";
        }

        private void StopGettingChangesetsFromServer()
        {
            UIController.StopProcessingChangesetFetch();
            _cancelHit = _cancelHit + 1;

            if (_cancelHit <= 2) return;

            UIController.DisableLoadNotificatioChangeset.Invoke();
            UIController.SearchButtonTextReset.Invoke();
        }

        private void HandleDisplayOfShelvesetButton()
        {
            btnGotoShelveset.Visibility = System.Windows.Visibility.Collapsed;
            // Always collapsed since the name in search for shelveset is always the logged in user name

            //if (!string.IsNullOrEmpty(lstUsers.Text))
            //{
            //    btnGotoShelveset.Visibility = System.Windows.Visibility.Visible;
            //}
            //else
            //{
            //    btnGotoShelveset.Visibility = System.Windows.Visibility.Collapsed;
            //}
        }

        #endregion

    }
}
