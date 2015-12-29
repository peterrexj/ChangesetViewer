using ChangesetViewer.Core.Model;
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

        ChangesetViewModel GetWithMinimalInfo(int changesetId);
        Changeset Get(int changesetId);
    }
}
