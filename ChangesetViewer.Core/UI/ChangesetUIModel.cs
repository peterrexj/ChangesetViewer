using ChangesetViewer.Core.TFS;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Collections.ObjectModel;
using System.ComponentModel;

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
            }
        }

        #endregion
    }
}
