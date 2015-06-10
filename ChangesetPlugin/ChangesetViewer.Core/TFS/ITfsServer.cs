using Microsoft.TeamFoundation.Client;

namespace ChangesetViewer.Core.TFS
{
    public interface ITfsServer
    {
        TfsTeamProjectCollection GetCollection();
    }
}
