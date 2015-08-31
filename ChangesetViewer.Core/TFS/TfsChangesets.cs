using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using PluginCore.Extensions;

namespace ChangesetViewer.Core.TFS
{
    public class TfsChangesets : ITfsChangsets
    {
        private readonly ITfsServer _tfsServer;
        private VersionControlServer _versionControlServer;

        public TfsChangesets(ITfsServer tfsServer)
        {
            _tfsServer = tfsServer;
        }

        public IEnumerable<Changeset> Get(string projectPath, DateTime from)
        {
            var projectCollection = _tfsServer.GetCollection();
            if (projectCollection.HasAuthenticated == false)
                projectCollection.Authenticate();

            // Get the Changeset list from the TFS API.
            var source = projectCollection.GetService<VersionControlServer>();

            var projectHistory = source.QueryHistory(projectPath, VersionSpec.Latest, 0, RecursionType.Full,
                null, new DateVersionSpec(from.Date), null, int.MaxValue,
                true, false, false, false)
                    .OfType<Changeset>()
                    .Reverse();

            return projectHistory.ToList().AsEnumerable();
        }

        public IEnumerable<Changeset> Get(string projectPath, int topN, string containsCheck = "")
        {
            var projectCollection = _tfsServer.GetCollection();
            if (projectCollection.HasAuthenticated == false)
                projectCollection.Authenticate();

            // Get the Changeset list from the TFS API.
            var source = projectCollection.GetService<VersionControlServer>();

            var projectHistory = source.QueryHistory(projectPath, VersionSpec.Latest, 0, RecursionType.Full,
                null, null, null, topN,
                false, false, false, false)
                    .OfType<Changeset>();

            if (containsCheck != string.Empty)
                projectHistory = projectHistory.Where(p => p.Comment.Contains(containsCheck));
            //.Reverse();

            return projectHistory;
        }

        public async Task<IEnumerable<ChangesetViewModel>> GetAsync(ChangesetSearchOptions search)
        {
            Task<IEnumerable<ChangesetViewModel>> qryHistory;

            qryHistory = Task.Factory.StartNew(() => BuildQuery(search));

            return await qryHistory;
        }

        public void CancelAsyncQueryHistorySearch()
        {
            if (_versionControlServer != null)
                _versionControlServer.Canceled = true;
        }

        private IEnumerable<ChangesetViewModel> BuildQuery(ChangesetSearchOptions search)
        {
            var projectCollection = _tfsServer.GetCollection();
            if (projectCollection.HasAuthenticated == false)
                projectCollection.Authenticate();

            // Get the Changeset list from the TFS API.
            _versionControlServer = projectCollection.GetService<VersionControlServer>();

            var qryHistroy = _versionControlServer.QueryHistory(search.ProjectSourcePath, VersionSpec.Latest, 0, RecursionType.Full,
                null, null, null, search.TopN,
                false, false, false, false)
                    .OfType<Changeset>();

            if (search.IsSearchBasedOnRegex && !string.IsNullOrEmpty(search.SearchKeyword))
            {
                var rx = new Regex(search.SearchKeyword, RegexOptions.IgnoreCase);
                qryHistroy = qryHistroy.Where(p => rx.IsMatch(search.SearchKeyword));
            }
            else if (!search.IsSearchBasedOnRegex && !string.IsNullOrEmpty(search.SearchKeyword))
            {
                if (search.SearchCommentType == Consts.SearchCommentType.Exact)
                    qryHistroy = qryHistroy.Where(c => c.Comment != null && c.Comment.Contains(search.SearchKeyword));
                else if (search.SearchCommentType == Consts.SearchCommentType.Keyword)
                    qryHistroy = qryHistroy.Where(c => c.Comment != null && search.SearchKeywordSplitMode.Any(s => c.Comment.Contains(s)));
            }


            if (!string.IsNullOrEmpty(search.Committer))
                qryHistroy = qryHistroy.Where(c => c.CommitterDisplayName == search.Committer);

            if (search.StartDate.HasValue)
                qryHistroy = qryHistroy.Where(c => c.CreationDate >= search.StartDate.Value);

            if (search.EndDate.HasValue)
                qryHistroy = qryHistroy.Where(c => c.CreationDate <= search.EndDate.Value);

            return qryHistroy.Select(c => ToViewModel(c));
        }

        public ChangesetViewModel Get(int changesetId)
        {
            var projectCollection = _tfsServer.GetCollection();
            if (projectCollection.HasAuthenticated == false)
                projectCollection.Authenticate();

            var server = projectCollection.GetService<VersionControlServer>();

            try
            {
                var changeset = server.GetChangeset(changesetId);
                return ToViewModel(changeset);
            }
            catch (Exception ex)
            {
                if (ex is ChangesetNotFoundException)
                    return null;
                throw ex;
            }
        }

        private ChangesetViewModel ToViewModel(Changeset c)
        {
            return new ChangesetViewModel
            {
                ChangesetId = c.ChangesetId,
                Comment = c.Comment,
                CommitterDisplayName = c.CommitterDisplayName,
                CreationDate = c.CreationDate,
                WorkItemIds = "", //string.Join(", ", c.WorkItems.Select(w => w.Id))
                ArtifactUri = c.ArtifactUri,
         
            };
        }


        private async Task<ChangesetViewModel> ToViewModelAsync(Changeset c)
        {
            return new ChangesetViewModel
            {
                ChangesetId = c.ChangesetId,
                Comment = c.Comment,
                CommitterDisplayName = c.CommitterDisplayName,
                CreationDate = c.CreationDate,
                WorkItemIds = await GetWorkItemsFromChangeset(c), //string.Join(", ", c.WorkItems.Select(w => w.Id))
                ArtifactUri = c.ArtifactUri,
            };
        }

        private async Task<string> GetWorkItemsFromChangeset(Changeset c)
        {
            return await Task.Factory.StartNew(() => string.Join(", ", c.WorkItems.Select(w => w.Id)));
        }
    }
}
