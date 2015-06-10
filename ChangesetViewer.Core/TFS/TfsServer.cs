using Microsoft.TeamFoundation.Client;
using System;

namespace ChangesetViewer.Core.TFS
{
    public class TfsServer : ITfsServer
    {
        public TfsTeamProjectCollection Collection { get; set; }
        public TfsTeamProjectCollection GetCollection()
        {
            //var credentials = new System.Net.NetworkCredential("tfsbuild2010", "7f3.bu1ld", "janison");
            const string tfsUrl = "https://tfscls.janison.com.au:8081/tfs/clscollection";
            var credentials = System.Net.CredentialCache.DefaultCredentials;
            Collection = new TfsTeamProjectCollection(new Uri(tfsUrl), credentials);
            Collection.Authenticate();
            return Collection;
        }
    }
}
