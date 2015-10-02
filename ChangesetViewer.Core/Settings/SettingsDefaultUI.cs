using System;
using System.Windows.Forms;

namespace ChangesetViewer.Core.Settings
{
    public partial class SettingsDefaultUI : UserControl
    {
        public SettingsDefaultUI()
        {
            InitializeComponent();
            txtDefaultSearchPath.Leave += txtLeaveEvent;
            txtServerUrl.Leave += txtLeaveEvent;
            txtServerCredUserName.Leave += txtLeaveEvent;
            txtServerCredPassword.Leave += txtLeaveEvent;

            chkUseVStfsInfo.CheckedChanged += chkUseVStfsInfo_CheckedChanged;
            chkFindJiraTicketsInComment.CheckedChanged += chkFindJiraTicketsInComment_CheckedChanged;

            txtJiraSearchRegex.Leave += txtLeaveEvent;
            txtJiraTicketLink.Leave += txtLeaveEvent;
        }

        internal SettingsPageModel optionsPage;

        public void Initialize()
        {
            chkUseVStfsInfo.Checked = optionsPage.UseVisualStudioEnvironmentTfsConnection;
            txtServerUrl.Text = optionsPage.TFSServerURL;
            txtServerCredUserName.Text = optionsPage.TFSUsername;
            txtServerCredPassword.Text = optionsPage.TFSPassword;

            txtDefaultSearchPath.Text = optionsPage.DefaultTFSSearchPath;


            chkFindJiraTicketsInComment.Checked = optionsPage.FindJiraTicketsInComment;
            txtJiraTicketLink.Text = optionsPage.JiraTicketBrowseLink;
            txtJiraSearchRegex.Text = optionsPage.JiraSearchRegexPattern;

            chkUseVStfsInfo.Enabled = false;
            grpServerCredentials.Enabled = false;
            txtDefaultSearchPath.Enabled = false;
        }

        private void UpdateSettingsModelBasedonUI()
        {
            optionsPage.TFSServerURL = txtServerUrl.Text;
            optionsPage.TFSUsername = txtServerCredUserName.Text;
            optionsPage.TFSPassword = txtServerCredPassword.Text;
            optionsPage.DefaultTFSSearchPath = txtDefaultSearchPath.Text;

            optionsPage.JiraTicketBrowseLink = txtJiraTicketLink.Text;
            optionsPage.JiraSearchRegexPattern = txtJiraSearchRegex.Text;

            optionsPage.FindJiraTicketsInComment = chkFindJiraTicketsInComment.Checked;
            optionsPage.UseVisualStudioEnvironmentTfsConnection = chkUseVStfsInfo.Checked;

        }

        void txtLeaveEvent(object sender, EventArgs e)
        {
            UpdateSettingsModelBasedonUI();
        }

        void chkUseVStfsInfo_CheckedChanged(object sender, EventArgs e)
        {
            grpServerCredentials.Enabled = !chkUseVStfsInfo.Checked;
            UpdateSettingsModelBasedonUI();
        }
        private void chkFindJiraTicketsInComment_CheckedChanged(object sender, EventArgs e)
        {
            grpJiraSettings.Enabled = chkFindJiraTicketsInComment.Checked;
            UpdateSettingsModelBasedonUI();
        }

        private void btnJIRAdefault_Click(object sender, EventArgs e)
        {
            txtJiraSearchRegex.Text = Consts.DefaultJiraticketsearchregex;
            txtJiraTicketLink.Text = Consts.DefaultJiraticketbrowseurl;
            UpdateSettingsModelBasedonUI();
        }

        
    }
}
