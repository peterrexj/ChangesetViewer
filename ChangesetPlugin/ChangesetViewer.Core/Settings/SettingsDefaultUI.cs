using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChangesetViewer.Core.Settings
{
    public partial class SettingsDefaultUI : UserControl
    {
        public SettingsDefaultUI()
        {
            InitializeComponent();
            txtDefaultSearchPath.Leave += txtDefaultSearchPath_Leave;
            chkUseVStfsInfo.CheckedChanged += chkUseVStfsInfo_CheckedChanged;
        }

        internal SettingsPageModel optionsPage;

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

        private void btnJIRAdefault_Click(object sender, EventArgs e)
        {
            txtDefaultSearchPath.Text = "";
            txtJiraSearchRegex.Text = "";
        }
    }
}
