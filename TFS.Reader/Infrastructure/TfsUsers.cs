using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Framework.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFS.Reader.Infrastructure;

namespace TFS.Reader.Infrastructure
{
    public class TfsUsers
    {
        public TfsTeamProjectCollection Collection { get; set; }

        public TfsUsers()
        {
            Collection = (new TfsServer()).GetCollection();
        }

        public IEnumerable<TeamFoundationIdentity> GetAllUsersInTFS()
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
    }
}
