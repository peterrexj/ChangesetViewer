using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using PluginCore.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.TFS
{
    public class TfsUsers : TfsBaseHandlers, ITfsUsers
    {
        private readonly ITfsServer _tfsServer;

        public TfsUsers(ITfsServer tfsServer)
        {
            _tfsServer = tfsServer;
        }

        public TfsUsers(ITfsServer tfsServer, Action<Exception> actionForError)
        {
            _tfsServer = tfsServer;
            ActionToHandleError = actionForError;
        }

        public IEnumerable<TeamFoundationIdentity> GetAllUsersInTfsBasedOnProjectCollection()
        {
            var css4 = _tfsServer.GetCollection().GetService<ICommonStructureService4>();
            TfsTeamService teamService = _tfsServer.GetCollection().GetService<TfsTeamService>();

            return from p in css4.ListProjects()
                   let allTeams = teamService.QueryTeams(p.Uri)
                   from a in allTeams
                   let ppls = a.GetMembers(_tfsServer.GetCollection(), MembershipQuery.Direct)
                   from ppl in ppls
                   select ppl;
        }
        public IEnumerable<Identity> GetAllUsersInTfsBasedOnIdentity()
        {
            _tfsServer.GetCollection().EnsureAuthenticated();
#pragma warning disable 618
            var gss = _tfsServer.GetCollection().GetService<IGroupSecurityService>();
#pragma warning restore 618
            var sids = gss.ReadIdentity(SearchFactor.AccountName, "Project Collection Valid Users", QueryMembership.Expanded);
            return gss.ReadIdentities(SearchFactor.Sid, sids.Members, QueryMembership.None);
        }

        public async Task<Identity[]> GetAllUsersInTfsBasedOnIdentityAsync()
        {
            try
            {
                var readUsersTask = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            _tfsServer.GetCollection().EnsureAuthenticated();

#pragma warning disable 618
                            var gss = _tfsServer.GetCollection().GetService<IGroupSecurityService>();
#pragma warning restore 618
                            var teamProjectCollections = _tfsServer.GetCollection().CatalogNode.QueryChildren(
                                new[] { CatalogResourceTypes.TeamProject },
                                false,
                                CatalogQueryOptions.None);

                            if (teamProjectCollections.Any())
                            {
                                var sids = gss.ReadIdentity(SearchFactor.AccountName,
                                        teamProjectCollections.FirstOrDefault().Resource.DisplayName + " Team",
                                        QueryMembership.Expanded);

                                return gss.ReadIdentities(SearchFactor.Sid, sids.Members, QueryMembership.None)
                                    .Where(u => u != null)
                                    .OrderBy(u => u.DisplayName)
                                    .Select(u => u)
                                    .ToArray();
                            }
                            return EnumerableExtensions.Empty<Identity>().ToArray();
                        }
                        catch(Exception ex)
                        {
                            InvokeErroHandler(ex);
                            return null;
                        }
                    });

                return await readUsersTask;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
