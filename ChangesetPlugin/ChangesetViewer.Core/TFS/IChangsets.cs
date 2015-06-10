using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.TFS
{
    public interface IChangsets
    {
        IEnumerable<Changeset> Get(string projectPath, DateTime from);
        IEnumerable<Changeset> Get(string projectPath, int topN, string containsCheck);

        Task<IEnumerable<Changeset>> GetAsync(ChangesetSearchOptions search);
        void CancelAsyncQueryHistorySearch();

        Changeset Get(int changesetId);
    }
}
