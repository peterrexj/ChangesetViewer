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
            txtSourceControlName.DataContext = UIController.Model;
            btnExportToExcel.DataContext = UIController.Model;
            btnSearch.DataContext = UIController.Model;
            lblTotalCount.DataContext = UIController.Model;

        }

        public ChangesetViewerUIController UIController { get; set; }

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
                DisableLoadNotificationUsers = DiableUINotificationUsers,
                EnableLoadNotificationChangeset = EnableUINotificationChangeset,
                DisableLoadNotificatioChangeset = DisableUINotificationChangeset,
                SearchButtonTextLoading = SearchButtonTextLoading,
                SearchButtonTextReset = SearchButtonTextReset,
            };

            UIController.TfsServerContextChanged += _cController_TfsServerContextChanged;

            DiableUINotificationUsers();
            DisableUINotificationChangeset();
        }
        public void InitializeWindow()
        {
            cboSearchType.ItemsSource = Enum.GetValues(typeof(Consts.SearchCommentType));
            txtSource.Text = UIController.GlobalSettings.DefaultTFSSearchPath;
            HandleErrorInUI();
        }

        void _cController_TfsServerContextChanged(object sender, EventArgs e)
        {
            try
            {
                UIController.Model.UserCollectionInTfs.Clear();
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
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

        private ChangesetSearchOptions ReadOptionsValueFromUI()
        {
            var options = new ChangesetSearchOptions
            {
                ProjectSourcePath = txtSource.Text.Trim(),
                TopN = Int32.MaxValue,
                SearchKeyword = txtSearchText.Text.Trim(),
                Committer = lstUsers.Text,
                IsSearchBasedOnRegex = chkSearchBasedOnRegex.IsChecked.HasValue && chkSearchBasedOnRegex.IsChecked.Value,
                SearchCommentType = (Consts.SearchCommentType)Enum.Parse(typeof(Consts.SearchCommentType), cboSearchType.SelectedValue.ToString()),
                StartDate = startDate.SelectedDate
            };

            //End date should have time till the end of the day
            if (endDate.SelectedDate.HasValue)
                options.EndDate = DateExtensions.GetEndOfDay(endDate.SelectedDate.Value);

            if (chkToday.IsChecked.HasValue && chkToday.IsChecked.Value)
            {
                options.StartDate = DateExtensions.GetStartOfDay(System.DateTime.Now);
                options.EndDate = DateExtensions.GetEndOfDay(System.DateTime.Now);
            }
            else if (chkWeek.IsChecked.HasValue && chkWeek.IsChecked.Value)
            {
                options.StartDate = DateExtensions.GetStartOfDay(System.DateTime.Now.AddDays(-7));
                options.EndDate = DateExtensions.GetEndOfDay(System.DateTime.Now);
            }
            else if (chkMonth.IsChecked.HasValue && chkMonth.IsChecked.Value)
            {
                options.StartDate = DateExtensions.GetStartOfDay(System.DateTime.Now.AddDays(-30));
                options.EndDate = DateExtensions.GetEndOfDay(System.DateTime.Now);
            }

            return options;
        }
        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UIController.UpdateSettingModel();

                var processChangesetsPull = ActionExtensions.Create(() =>
                {
                    if (!UIController.IsVisualStudioIsConnectedToTFS())
                        return;

                    _cancelHit = 0;
                    EnableUINotificationChangeset();
                    var searchModel = ReadOptionsValueFromUI();
                    UIController.GetChangesets(searchModel);

                    if (lstContainer.ItemsSource == null)
                    {
                        lstContainer.Items.Clear();
                        lstContainer.ItemsSource = UIController.Model.ChangeSetCollection;
                    }
                    UIController.Model.ChangeSetCollection.Clear();
                    btnSearch.Content = "Stop";
                });

                var requestProcessingBreak = ActionExtensions.Create(() =>
                {
                    UIController.StopProcessingChangesetFetch();
                    _cancelHit = _cancelHit + 1;

                    if (_cancelHit <= 2) return;

                    UIController.DisableLoadNotificatioChangeset.Invoke();
                    UIController.SearchButtonTextReset.Invoke();
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
            if (!UIController.IsVisualStudioIsConnectedToTFS())
                return;

            if (lstUsers.ItemsSource != null && UIController.Model.UserCollectionInTfs.Count > 0) return;

            spinnerUser.IsSpinning = true;
            spinnerUser.Visibility = Visibility.Visible;

            lstUsers.ItemsSource = UIController.Model.UserCollectionInTfs;

            UIController.LoadUsersAsync();
        }


        public void DiableUINotificationUsers()
        {
            try
            {
                spinnerUser.IsSpinning = false;
                spinnerUser.Visibility = System.Windows.Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        public void EnableUINotificationChangeset()
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
        public void DisableUINotificationChangeset()
        {
            try
            {
                spinnerchangeset.IsSpinning = false;
                spinnerchangeset.Visibility = System.Windows.Visibility.Collapsed;
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

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
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
                var disableControls = ActionExtensions.Create(() =>
                {
                    btnExportToExcel.IsEnabled = false;
                    btnSearch.IsEnabled = false;
                    btnExportToExcel.Content = "Exporting..";
                });

                var enableControls1 = ActionExtensions.Create(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        btnSearch.IsEnabled = true;
                    });
                });

                var enableControls2 = ActionExtensions.Create(() =>
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        btnExportToExcel.IsEnabled = true;
                        btnExportToExcel.Content = "Export to Excel";
                    });
                });

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
                UIController.OpenChangesetWindow(txtChangesetId.Text, true);
            }
            catch (Exception ex)
            {
                HandleErrorInUI(ex);
            }
        }
        private void btnSelectServerPath_Click(object sender, RoutedEventArgs e)
        {

        }
        private void RichTextboxCustomized_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e) 
        { 
            //dummy event which is req. Do not delete 
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

            if (dtType != null)
                ClearDateRangeUIFields();

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
                default:
                    break;
            }
        }
        private void ClearDateRangeUIFields()
        {
            startDate.SelectedDate = null;
            endDate.SelectedDate = null;
        }
        #endregion
    }
}
