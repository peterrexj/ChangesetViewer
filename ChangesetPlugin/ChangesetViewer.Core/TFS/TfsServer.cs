using Microsoft.TeamFoundation.Client;
using Microsoft.VisualStudio.TeamFoundation;
using System;

namespace ChangesetViewer.Core.TFS
{
    public class TfsServer : ITfsServer
    {
        private string _serverUrl;
        private string _username;
        private string _password;

        public TfsTeamProjectCollection Collection { get; set; }

        public TfsServer() { }

        public TfsServer(string tfsServerUrl, string username, string password)
        {
            _serverUrl = tfsServerUrl;
            _username = username;
            _password = password;
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
