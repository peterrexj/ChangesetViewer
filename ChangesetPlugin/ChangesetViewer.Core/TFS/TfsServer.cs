using Microsoft.TeamFoundation.Client;
using System;

namespace ChangesetViewer.Core.TFS
{
    public class TfsServer : ITfsServer
    {
        private readonly string _serverUrl;

        public TfsTeamProjectCollection Collection { get; set; }

        public TfsServer() { }

        public TfsServer(string tfsServerUrl)
        {
            _serverUrl = tfsServerUrl;
        }

        public TfsTeamProjectCollection GetCollection()
        {
            Collection = new TfsTeamProjectCollection(new Uri(_serverUrl));
            Collection.EnsureAuthenticated();
            
            if (!Collection.HasAuthenticated)
                Collection.Authenticate();

            return Collection;
        }
    }
}
