using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.TFS
{
    public interface ITfsChangsets
    {
        IEnumerable<Changeset> Get(string projectPath, DateTime from);
        IEnumerable<Changeset> Get(string projectPath, int topN, string containsCheck);

        Task<IEnumerable<ChangesetViewModel>> GetAsync(ChangesetSearchOptions search);
        void CancelAsyncQueryHistorySearch();

        ChangesetViewModel Get(int changesetId);
    }
}
