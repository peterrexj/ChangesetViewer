using ChangesetViewer.Core.TFS;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using System.Collections.ObjectModel;

namespace ChangesetViewer.Core.UI
{
    public class ChangesetViewerModel
    {
        public ObservableCollection<Identity> UserCollectionInTfs { get; set; }
        public ObservableCollection<ChangesetViewModel> ChangeSetCollection { get; set; }

        public ChangesetViewerModel()
        {
            UserCollectionInTfs = new ObservableCollection<Identity>();
            ChangeSetCollection = new ObservableCollection<ChangesetViewModel>();
        }


        public int ChangeSetCollectionCount()
        {
            return ChangeSetCollection.Count;
        }
    }
}
