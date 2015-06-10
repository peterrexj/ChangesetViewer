using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;

namespace ChangesetViewer.Core.TFS
{
    public interface ITfsUsers
    {
        IEnumerable<Identity> GetAllUsersInTFSBasedOnIdentity();
        Task<Identity[]> GetAllUsersInTFSBasedOnIdentityAsync();
        IEnumerable<TeamFoundationIdentity> GetAllUsersInTFSBasedOnProjectCollection();
    }
}
