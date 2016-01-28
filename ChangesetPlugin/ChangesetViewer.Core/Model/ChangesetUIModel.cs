using ChangesetViewer.Core.Settings;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ChangesetViewer.Core.Model
{
    public class ChangesetUIModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<IdentityViewModel> UserCollectionInTfs { get; set; }
        public ObservableCollection<ChangesetViewModel> ChangeSetCollection { get; set; }

        

        public ChangesetUIModel()
        {
            UserCollectionInTfs = new ObservableCollection<IdentityViewModel>();
            ChangeSetCollection = new ObservableCollection<ChangesetViewModel>();

            ChangeSetCollection.CollectionChanged += ChangeSetCollection_CollectionChanged;

            FoundMoreItemsAfterInitialSearch = false;
        }

        protected void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        void ChangeSetCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Notify("SearchResultedChangesets");
            Notify("ChangesetCollectionCount");
            Notify("HasMoreItemsToFetch");
        }

        public int ChangeSetCollectionCount() { return ChangeSetCollection.Count; }

        public void InitializeChangesetsModel() { ChangeSetCollection.Clear(); }
        public void InitializeUsersModel() { UserCollectionInTfs.Clear(); }

        #region INotify Property

        private string _sourceControlName;
        public string SourceControlName
        {
            get
            {
                return _sourceControlName;
            }
            set
            {
                _sourceControlName = value;
                Notify("SourceControlName");
                Notify("SearchTooltipRequired");
            }
        }

        public Visibility SearchTooltipRequired { get { return string.IsNullOrEmpty(_sourceControlName) ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility VisibilityBasedOnChangesetCount { get { return (ChangeSetCollection.Count > 0 ? Visibility.Visible : Visibility.Collapsed); } }

        public bool SearchResultedChangesets { get { return ChangeSetCollection.Count > 0; } }

        public int ChangesetCollectionCount { get { return ChangeSetCollection.Count;  }}


        //sets to true if initial search page size is met or if there are more items to fetch
        private bool _foundMoreItemsAfterInitialSearch;

        public bool FoundMoreItemsAfterInitialSearch
        {
            get { return _foundMoreItemsAfterInitialSearch; }
            set
            {
                _foundMoreItemsAfterInitialSearch = value;
                Notify("HasMoreItemsToFetch");
            }
        }

        private bool _isSearchingMode;

        /// <summary>
        /// This property will determine whether the search operation is going on
        /// Will be used mainly in the UI interface to enable/disable some controls or even text
        /// </summary>
        public bool IsSearchingMode
        {
            get
            {
                return _isSearchingMode;
            }
            set
            {
                _isSearchingMode = value;
                Notify("IsSearchingMode");
                Notify("HasMoreItemsToFetch");
            }
        }

        /// <summary>
        /// Used to control the UI control visibility (ex. The fetch more button)
        /// </summary>
        public Visibility HasMoreItemsToFetch { get { return (!IsSearchingMode && (ChangeSetCollection.Count == SettingsStaticModelWrapper.SearchPageSize || FoundMoreItemsAfterInitialSearch)) ? Visibility.Visible : Visibility.Collapsed; } }

        #endregion
    }
}
