using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.TFS
{
    public abstract class TfsBaseHandlers
    {
        public Action<Exception> ActionToHandleError { get; set; }
        public void InvokeErroHandler(Exception ex)
        {
            if (ActionToHandleError != null)
                ActionToHandleError(ex);
        }
    }
}
