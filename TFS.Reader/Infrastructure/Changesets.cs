using Microsoft.TeamFoundation.VersionControl.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFS.Reader.Infrastructure;

namespace TFS.Reader.Infrastructure
{
    public interface IChangsets
    {
        IEnumerable<Changeset> Get(string projectPath, DateTime from);
        IEnumerable<Changeset> Get(string projectPath, int topN, string containsCheck);

        Changeset Get(int changesetId);
    }

    public class Changesets : IChangsets
    {
        private readonly ITfsServer _tfsServer;

        public Changesets(ITfsServer tfsServer)
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
            Trace.WriteLine(String.Format("Searching history for project {0}", projectPath));

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
            Trace.WriteLine(String.Format("Searching history for project {0}", projectPath));

            var projectHistory = source.QueryHistory(projectPath, VersionSpec.Latest, 0, RecursionType.Full,
                null, null, null, topN,
                false, false, false, false)
                    .OfType<Changeset>();

            if (containsCheck != string.Empty)
                projectHistory = projectHistory.Where(p => p.Comment.Contains(containsCheck));
            //.Reverse();

            return projectHistory;
        }

        //public async Task<ExtendedMerge> TrackChangesetInAsync(Changeset changeset, string projectPath, string branch)
        //{
        //    var result = await TrackChangesetInAsync(changeset.ChangesetId, projectPath, new[] { branch });
        //    return result.FirstOrDefault();
        //}

        //public async Task<ExtendedMerge[]> TrackChangesetInAsync(Changeset changeset, string projectPath, IEnumerable<string> branches)
        //{
        //    var result = await TrackChangesetInAsync(changeset.ChangesetId, projectPath, branches);
        //    return result;
        //}

        ///// <summary>
        ///// Track a changeset merged into a possible list of branches.
        ///// </summary>
        ///// <param name="changesetId">The changeset ID to track</param>
        ///// <param name="projectPath">The source path of the changeset eg $/project/dev</param>
        ///// <param name="branches">A list of paths to check if the changeset has been merged into</param>
        ///// <returns>An array of Extended Merge</returns>
        //public async Task<ExtendedMerge[]> TrackChangesetInAsync(int changesetId, string projectPath, IEnumerable<string> branches)
        //{
        //    var t = await Task.Run(() =>
        //    {
        //        var result = TrackChangesetIn(changesetId, projectPath, branches);
        //        return result;
        //    });
        //    return t;
        //}

        //public ExtendedMerge TrackChangesetIn(Changeset changeset, string projectPath, string branch)
        //{
        //    var result = TrackChangesetIn(changeset.ChangesetId, projectPath, new[] { branch });
        //    return result.FirstOrDefault();
        //}

        //public ExtendedMerge[] TrackChangesetIn(int changesetId, string projectPath, string branch)
        //{
        //    var result = TrackChangesetIn(changesetId, projectPath, new[] { branch });
        //    return result;
        //}

        ///// <summary>
        ///// Track a changeset merged into a possible list of branches.
        ///// </summary>
        ///// <param name="changesetId">The changeset ID to track</param>
        ///// <param name="projectPath">The source path of the changeset eg $/project/dev</param>
        ///// <param name="branches">A list of paths to check if the changeset has been merged into</param>
        ///// <returns>An array of Extended Merge</returns>
        //public ExtendedMerge[] TrackChangesetIn(int changesetId, string projectPath, IEnumerable<string> branches)
        //{
        //    var projectCollection = _tfsServer.Connection();
        //    if (projectCollection.HasAuthenticated == false)
        //        projectCollection.Authenticate();

        //    // Get the Changeset list from the TFS API.
        //    var source = projectCollection.GetService<VersionControlServer>();

        //    var merges = source.TrackMerges(new int[] { changesetId },
        //        new ItemIdentifier(projectPath),
        //        branches.Select(b => new ItemIdentifier(b)).ToArray(),
        //        null);

        //    return merges;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <see cref="http://stackoverflow.com/questions/9514204/tfs2010-track-merges"/>
        public Changeset Get(int changesetId)
        {
            var projectCollection = _tfsServer.GetCollection();
            if (projectCollection.HasAuthenticated == false)
                projectCollection.Authenticate();

            var server = projectCollection.GetService<VersionControlServer>();

            var changeset = server.GetChangeset(changesetId);
            return changeset;
        }
    }
}
