using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;

namespace ChangesetViewer.Core.TFS
{
    public interface ITfsUsers
    {
        IEnumerable<Identity> GetAllUsersInTfsBasedOnIdentity();
        Task<Identity[]> GetAllUsersInTfsBasedOnIdentityAsync();
        IEnumerable<TeamFoundationIdentity> GetAllUsersInTfsBasedOnProjectCollection();
    }
}
