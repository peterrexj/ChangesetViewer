using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.TFS
{
    interface ITFSWorkItem
    {
        string GetWorkItemTitle(string id);
    }
}
