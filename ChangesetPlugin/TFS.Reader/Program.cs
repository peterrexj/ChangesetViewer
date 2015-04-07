using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFS.Reader
{
    class Program
    {
        static void Main(string[] args)
        {
            Infra.TfsServer tfs = new Infra.TfsServer();
            var pp = tfs.Connection();

            if (pp.HasAuthenticated == false)
                pp.Authenticate();



            //TFS.Reader.Infrastructure.IChangsets cs = new TFS.Reader.Infrastructure.Changesets(tfs);

            //var dd = cs.Get("$/CLS/Src/Dev", 50);
            //foreach (var d in dd)
            //{
            //    Console.WriteLine(d.Comment);
            //}

            ////var source = tfs.GetService<VersionControlServer>();
            //Console.ReadLine();

            

        }
    }
}
