using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesetViewer.UI.Test.Infra
{
    public class ChangesetModel
    {
        //public IEnumerable<Identity> AllUsersInTfs { get; set; }
        public ObservableCollection<Identity> UserCollectionInTFS { get; set; }
        public ObservableCollection<Changeset> ChangeSetCollection { get; set; }

        public ChangesetModel()
        {
            UserCollectionInTFS = new ObservableCollection<Identity>();
            ChangeSetCollection = new ObservableCollection<Changeset>();
        }


        public int ChangeSetCollectionCount()
        {
            return ChangeSetCollection.Count;
        }

    }
}
