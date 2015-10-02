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
            var credentials = System.Net.CredentialCache.DefaultCredentials;
            Collection = new TfsTeamProjectCollection(new Uri(_serverUrl), credentials);
            Collection.Authenticate();
            return Collection;
        }
    }
}
