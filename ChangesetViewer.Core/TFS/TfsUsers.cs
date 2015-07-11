using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client;
using PluginCore.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.TFS
{
    public class TfsUsers : ITfsUsers
    {
        public TfsTeamProjectCollection Collection { get; set; }

        public TfsUsers()
        {
            Collection = (new TfsServer()).GetCollection();
        }

        public TfsUsers(string tfsServerUrl, string username, string password)
        {
            Collection = (new TfsServer(tfsServerUrl, username, password)).GetCollection();
        }


        public IEnumerable<TeamFoundationIdentity> GetAllUsersInTFSBasedOnProjectCollection()
        {
            var css4 = Collection.GetService<Microsoft.TeamFoundation.Server.ICommonStructureService4>();
            TfsTeamService teamService = Collection.GetService<TfsTeamService>();

            return from p in css4.ListProjects()
                   let allTeams = teamService.QueryTeams(p.Uri)
                   from a in allTeams
                   let ppls = a.GetMembers(Collection, MembershipQuery.Direct)
                   from ppl in ppls
                   select ppl;
        }
        public IEnumerable<Identity> GetAllUsersInTFSBasedOnIdentity()
        {
            Collection.EnsureAuthenticated();
            IGroupSecurityService gss = Collection.GetService<IGroupSecurityService>();
            Identity SIDS = gss.ReadIdentity(SearchFactor.AccountName, "Project Collection Valid Users", QueryMembership.Expanded);
            return gss.ReadIdentities(SearchFactor.Sid, SIDS.Members, QueryMembership.None);
        }

        public async Task<Identity[]> GetAllUsersInTFSBasedOnIdentityAsync()
        {
            var readUsersTask = Task.Factory.StartNew(() =>
                {
                    Collection.EnsureAuthenticated();

                    IGroupSecurityService gss = Collection.GetService<IGroupSecurityService>();
                    ReadOnlyCollection<CatalogNode> teamProjectCollections = Collection.CatalogNode.QueryChildren(
                        new Guid[] { CatalogResourceTypes.TeamProject },
                        false,
                        CatalogQueryOptions.None);

                    if (teamProjectCollections.Any())
                    {
                        Identity SIDS = gss.ReadIdentity(SearchFactor.AccountName,
                                teamProjectCollections.FirstOrDefault().Resource.DisplayName + " Team",
                                QueryMembership.Expanded);

                        return gss.ReadIdentities(SearchFactor.Sid, SIDS.Members, QueryMembership.None)
                            .Where(u => u != null)
                            .OrderBy(u => u.DisplayName)
                            .Select(u => u)
                            .ToArray();
                    }
                    else
                        return EnumerableExtensions.Empty<Identity>().ToArray();
                });

            return await readUsersTask;

        }

    }
}
