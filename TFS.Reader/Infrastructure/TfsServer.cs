using Microsoft.TeamFoundation.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFS.Reader.Infra
{
    public interface ITfsServer
    {
        TfsTeamProjectCollection Connection();
    }

    public class TfsServer : ITfsServer
    {
        public TfsTeamProjectCollection Connection()
        {
            //var credentials = new System.Net.NetworkCredential("tfsbuild2010", "7f3.bu1ld", "janison");
            const string tfsUrl = "https://tfscls.janison.com.au:8081/tfs/clscollection";
            var credentials = System.Net.CredentialCache.DefaultCredentials;
            var server = new TfsTeamProjectCollection(new Uri(tfsUrl), credentials);
            server.Authenticate();
            return server;
        }
    }
}
