using ChangesetViewer.Core.Model;
using Microsoft.TeamFoundation.Server;

namespace ChangesetViewer.Core.Extensions
{
    internal static class IdentityEx
    {
        public static IdentityViewModel ToIdentityViewModel(this Identity p)
        {
            return new IdentityViewModel { DisplayName = p.DisplayName };
        }
    }
}
