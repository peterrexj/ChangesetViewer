using System.Collections.Generic;
using System.Threading.Tasks;
using ChangesetViewer.Core.Model;
using Microsoft.TeamFoundation.Framework.Client;
using Microsoft.TeamFoundation.Server;

namespace ChangesetViewer.Core.TFS
{
    public interface ITfsUsers
    {
        IEnumerable<IdentityViewModel> GetAllUsersInTfsBasedOnIdentity();
        Task<IdentityViewModel[]> GetAllUsersInTfsBasedOnIdentityAsync();
        IEnumerable<TeamFoundationIdentity> GetAllUsersInTfsBasedOnProjectCollection();
    }
}
