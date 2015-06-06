using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PeterRexJoseph.ChangesetViewer.SettingsPages
{
    public partial class SettingsPageUI : UserControl
    {
        internal SettingsPageModel optionsPage;

        public SettingsPageUI()
        {
            InitializeComponent();
            txtDefaultSearchPath.Leave += txtDefaultSearchPath_Leave;
            chkUseVStfsInfo.CheckedChanged += chkUseVStfsInfo_CheckedChanged;
        }

        void chkUseVStfsInfo_CheckedChanged(object sender, EventArgs e)
        {
            grpServerCredentials.Enabled = !chkUseVStfsInfo.Checked;
        }

        void txtDefaultSearchPath_Leave(object sender, EventArgs e)
        {
            optionsPage.DefaultSearchPath = txtDefaultSearchPath.Text;
        }

        public void Initialize()
        {
            txtDefaultSearchPath.Text = optionsPage.DefaultSearchPath;
        }



    }
}
