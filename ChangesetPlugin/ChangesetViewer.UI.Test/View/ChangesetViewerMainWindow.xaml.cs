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
            txtSourceControlName.DataContext = _cController.Model;
            btnExportToExcel.DataContext = _cController.Model;
            btnSearch.DataContext = _cController.Model;
            lblTotalCount.DataContext = _cController.Model;

        }

        public ChangesetViewerUIController _cController;
        public enum DateFilterType
        {
            Today,
            Week,
            Month
        }

        private void InitializeInternalComponents()
        {
            _cController = new ChangesetViewerUIController
            {
                EnableLoadNotificationUsers = EnableUINotificationUsers,
                DisableLoadNotificationUsers = DiableUINotificationUsers,
                EnableLoadNotificationChangeset = EnableUINotificationChangeset,
                DisableLoadNotificatioChangeset = DisableUINotificationChangeset,
                SearchButtonTextLoading = SearchButtonTextLoading,
                SearchButtonTextReset = SearchButtonTextReset,
            };

            _cController.TfsServerContextChanged += _cController_TfsServerContextChanged;

            DiableUINotificationUsers();
            DisableUINotificationChangeset();
        }

        void _cController_TfsServerContextChanged(object sender, EventArgs e)
        {
            _cController.Model.UserCollectionInTfs.Clear();
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
            cboSearchType.ItemsSource = Enum.GetValues(typeof(Consts.SearchCommentType));
            txtSource.Text = _cController.GlobalSettings.DefaultTFSSearchPath;
            HandleErrorInUI();
        }
        
        private int _cancelHit;

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
                var processChangesetsPull = ActionExtensions.Create(() =>
                {
                    if (!_cController.IsVisualStudioIsConnectedToTFS())
                        return;

                    _cancelHit = 0;
                    EnableUINotificationChangeset();
                    var searchModel = ReadOptionsValueFromUI();
                    _cController.GetChangesets(searchModel);

                    if (lstContainer.ItemsSource == null)
                    {
                        lstContainer.Items.Clear();
                        lstContainer.ItemsSource = _cController.Model.ChangeSetCollection;
                    }
                    _cController.Model.ChangeSetCollection.Clear();
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

            spinnerUser.IsSpinning = true;
            spinnerUser.Visibility = Visibility.Visible;

            lstUsers.ItemsSource = _cController.Model.UserCollectionInTfs;

            _cController.LoadUsersAsync();
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
            spinnerUser.IsSpinning = false;
            spinnerUser.Visibility = System.Windows.Visibility.Collapsed;
        }
        public void EnableUINotificationChangeset()
        {
            spinnerchangeset.IsSpinning = true;
            spinnerchangeset.Visibility = Visibility.Visible;
        }
        public void DisableUINotificationChangeset()
        {
            spinnerchangeset.IsSpinning = false;
            spinnerchangeset.Visibility = System.Windows.Visibility.Collapsed;
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
            _cController.OpenChangesetWindow(e.Target);
        }

        private void btnExportToExcel_Click(object sender, RoutedEventArgs e)
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

            _cController.ExportToExcel(enableControls1, enableControls2);
        }

        private void btnGoToChangeet_Click(object sender, RoutedEventArgs e)
        {
            _cController.OpenChangesetWindow(txtChangesetId.Text, true);
        }

        private void btnSelectServerPath_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void RichTextboxCustomized_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MessageBox.Show("inside");
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
        private void HandleCheckDateField(DateFilterType dtType) {
            
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
