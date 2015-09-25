using ChangesetViewer.Core.TFS;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace ChangesetViewer.Core.UI
{
    public class ChangesetUIModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<Identity> UserCollectionInTfs { get; set; }
        public ObservableCollection<ChangesetViewModel> ChangeSetCollection { get; set; }

        

        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ChangesetUIModel()
        {
            UserCollectionInTfs = new ObservableCollection<Identity>();
            ChangeSetCollection = new ObservableCollection<ChangesetViewModel>();

            ChangeSetCollection.CollectionChanged += ChangeSetCollection_CollectionChanged;
        }

        void ChangeSetCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Notify("SearchResultedChangesets");
        }

        public int ChangeSetCollectionCount()
        {
            return ChangeSetCollection.Count;
        }


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

        public Visibility SearchTooltipRequired
        {
            get
            {
                return string.IsNullOrEmpty(_sourceControlName) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool SearchResultedChangesets
        {
            get
            {
                return ChangeSetCollection.Count > 0;
            }
        }
        #endregion
    }
}
