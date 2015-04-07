using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TFS.UI
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            listBox1.Items.Clear();
            

            TFS.Reader.Infra.TfsServer tfs = new TFS.Reader.Infra.TfsServer();
            //var pp = tfs.Connection();

            //if (pp.HasAuthenticated == false)
            //    pp.Authenticate();



            TFS.Reader.Infrastructure.IChangsets cs = new TFS.Reader.Infrastructure.Changesets(tfs);

            var dd = cs.Get(textBox2.Text.Trim(), 1500, textBox1.Text.Trim());
            foreach (var d in dd)
            {
                listBox1.Items.Add(d.ChangesetId);
            }

            lblCount.Text = dd.Count().ToString();
            //var source = tfs.GetService<VersionControlServer>();
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            //VersionControlExt 
        }
    }
}
