using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.TFS
{
    public class TfsWorkItem : ITFSWorkItem
    {

        public string GetWorkItemTitle(string id)
        {
            Changeset m;
            WorkItem mm;

            return string.Empty;
        }
    }
}
