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

        public TfsServer()
        {

        }
        public TfsServer(string tfsServerUrl, string username, string password)
        {
            _serverUrl = tfsServerUrl;
            _username = username;
            _password = password;

            
        }
        public TfsTeamProjectCollection Collection { get; set; }
        public TfsTeamProjectCollection GetCollection()
        {
            //var credentials = new System.Net.NetworkCredential("tfsbuild2010", "7f3.bu1ld", "janison");
            //const string tfsUrl = "https://tfscls.janison.com.au:8081/tfs/clscollection";
            //var credentials = System.Net.CredentialCache.DefaultCredentials;
            //Collection = new TfsTeamProjectCollection(new Uri(tfsUrl), credentials);
            //Collection.Authenticate();
            //return Collection;

            //const string tfsUrl = "https://tfscls.janison.com.au:8081/tfs/clscollection";


            var credentials = System.Net.CredentialCache.DefaultCredentials;
            Collection = new TfsTeamProjectCollection(new Uri(_serverUrl), credentials);
            Collection.Authenticate();
            return Collection;
        }
    }
}
