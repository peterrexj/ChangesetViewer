using System;

namespace ChangesetViewer.Core.TFS
{
    public class QueryCancelRequest : Exception
    {
        public QueryCancelRequest(string message) : base(message)
        {

        }
    }
}
