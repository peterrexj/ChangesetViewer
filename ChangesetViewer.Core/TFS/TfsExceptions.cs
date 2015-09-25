using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.TFS
{
    public class QueryCancelRequest : Exception
    {
        public QueryCancelRequest(string message) : base(message)
        {

        }
    }
}
