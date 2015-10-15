namespace ChangesetViewer.Core.Settings
{
    partial class SettingsDefaultUI
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDefaultUI));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDefaultSearchPath = new System.Windows.Forms.TextBox();
            this.grpServerCredentials = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServerCredPassword = new System.Windows.Forms.TextBox();
            this.txtServerCredUserName = new System.Windows.Forms.TextBox();
            this.txtServerUrl = new System.Windows.Forms.TextBox();
            this.chkUseVStfsInfo = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnJIRAdefault = new System.Windows.Forms.Button();
            this.txtJiraSearchRegex = new System.Windows.Forms.TextBox();
            this.chkFindJiraTicketsInComment = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtJiraTicketLink = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.grpJiraSettings = new System.Windows.Forms.GroupBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.groupBox1.SuspendLayout();
            this.grpServerCredentials.SuspendLayout();
            this.grpJiraSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtDefaultSearchPath);
            this.groupBox1.Controls.Add(this.grpServerCredentials);
            this.groupBox1.Controls.Add(this.chkUseVStfsInfo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(330, 149);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // txtDefaultSearchPath
            // 
            this.txtDefaultSearchPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDefaultSearchPath.Location = new System.Drawing.Point(116, 122);
            this.txtDefaultSearchPath.Name = "txtDefaultSearchPath";
            this.txtDefaultSearchPath.Size = new System.Drawing.Size(193, 20);
            this.txtDefaultSearchPath.TabIndex = 3;
            // 
            // grpServerCredentials
            // 
            this.grpServerCredentials.Controls.Add(this.label4);
            this.grpServerCredentials.Controls.Add(this.label3);
            this.grpServerCredentials.Controls.Add(this.label2);
            this.grpServerCredentials.Controls.Add(this.txtServerCredPassword);
            this.grpServerCredentials.Controls.Add(this.txtServerCredUserName);
            this.grpServerCredentials.Controls.Add(this.txtServerUrl);
            this.grpServerCredentials.Location = new System.Drawing.Point(14, 40);
            this.grpServerCredentials.Name = "grpServerCredentials";
            this.grpServerCredentials.Size = new System.Drawing.Size(305, 76);
            this.grpServerCredentials.TabIndex = 2;
            this.grpServerCredentials.TabStop = false;
            this.grpServerCredentials.Text = "Server Settings";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(164, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Password";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 48);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "User Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(63, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Server URL";
            // 
            // txtServerCredPassword
            // 
            this.txtServerCredPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerCredPassword.Location = new System.Drawing.Point(223, 45);
            this.txtServerCredPassword.Name = "txtServerCredPassword";
            this.txtServerCredPassword.PasswordChar = '*';
            this.txtServerCredPassword.Size = new System.Drawing.Size(72, 20);
            this.txtServerCredPassword.TabIndex = 2;
            // 
            // txtServerCredUserName
            // 
            this.txtServerCredUserName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerCredUserName.Location = new System.Drawing.Point(81, 45);
            this.txtServerCredUserName.Name = "txtServerCredUserName";
            this.txtServerCredUserName.Size = new System.Drawing.Size(72, 20);
            this.txtServerCredUserName.TabIndex = 1;
            // 
            // txtServerUrl
            // 
            this.txtServerUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerUrl.Location = new System.Drawing.Point(81, 19);
            this.txtServerUrl.Name = "txtServerUrl";
            this.txtServerUrl.Size = new System.Drawing.Size(214, 20);
            this.txtServerUrl.TabIndex = 0;
            // 
            // chkUseVStfsInfo
            // 
            this.chkUseVStfsInfo.AutoSize = true;
            this.chkUseVStfsInfo.Location = new System.Drawing.Point(14, 17);
            this.chkUseVStfsInfo.Name = "chkUseVStfsInfo";
            this.chkUseVStfsInfo.Size = new System.Drawing.Size(240, 17);
            this.chkUseVStfsInfo.TabIndex = 1;
            this.chkUseVStfsInfo.Text = "Use Visual Studio connected TFS information";
            this.chkUseVStfsInfo.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 125);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Default Server path";
            // 
            // btnJIRAdefault
            // 
            this.btnJIRAdefault.Location = new System.Drawing.Point(229, 65);
            this.btnJIRAdefault.Name = "btnJIRAdefault";
            this.btnJIRAdefault.Size = new System.Drawing.Size(75, 23);
            this.btnJIRAdefault.TabIndex = 10;
            this.btnJIRAdefault.Text = "Use Default";
            this.btnJIRAdefault.UseVisualStyleBackColor = true;
            this.btnJIRAdefault.Click += new System.EventHandler(this.btnJIRAdefault_Click);
            // 
            // txtJiraSearchRegex
            // 
            this.txtJiraSearchRegex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJiraSearchRegex.Location = new System.Drawing.Point(116, 13);
            this.txtJiraSearchRegex.Name = "txtJiraSearchRegex";
            this.txtJiraSearchRegex.Size = new System.Drawing.Size(188, 20);
            this.txtJiraSearchRegex.TabIndex = 9;
            // 
            // chkFindJiraTicketsInComment
            // 
            this.chkFindJiraTicketsInComment.AutoSize = true;
            this.chkFindJiraTicketsInComment.Location = new System.Drawing.Point(17, 167);
            this.chkFindJiraTicketsInComment.Name = "chkFindJiraTicketsInComment";
            this.chkFindJiraTicketsInComment.Size = new System.Drawing.Size(130, 17);
            this.chkFindJiraTicketsInComment.TabIndex = 8;
            this.chkFindJiraTicketsInComment.Text = "Find items in comment";
            this.chkFindJiraTicketsInComment.UseVisualStyleBackColor = true;
            this.chkFindJiraTicketsInComment.CheckedChanged += new System.EventHandler(this.chkFindJiraTicketsInComment_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 16);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(96, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "Item search Regex";
            this.toolTip1.SetToolTip(this.label5, "Regex that will match the changeset comment and hightlight.");
            // 
            // txtJiraTicketLink
            // 
            this.txtJiraTicketLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJiraTicketLink.Location = new System.Drawing.Point(116, 39);
            this.txtJiraTicketLink.Name = "txtJiraTicketLink";
            this.txtJiraTicketLink.Size = new System.Drawing.Size(188, 20);
            this.txtJiraTicketLink.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(14, 42);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "Item link";
            this.toolTip1.SetToolTip(this.label6, resources.GetString("label6.ToolTip"));
            // 
            // grpJiraSettings
            // 
            this.grpJiraSettings.Controls.Add(this.label5);
            this.grpJiraSettings.Controls.Add(this.txtJiraTicketLink);
            this.grpJiraSettings.Controls.Add(this.btnJIRAdefault);
            this.grpJiraSettings.Controls.Add(this.txtJiraSearchRegex);
            this.grpJiraSettings.Controls.Add(this.label6);
            this.grpJiraSettings.Location = new System.Drawing.Point(8, 190);
            this.grpJiraSettings.Name = "grpJiraSettings";
            this.grpJiraSettings.Size = new System.Drawing.Size(314, 93);
            this.grpJiraSettings.TabIndex = 13;
            this.grpJiraSettings.TabStop = false;
            this.grpJiraSettings.Text = "Item Settings";
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 10;
            this.toolTip1.AutoPopDelay = 25000;
            this.toolTip1.InitialDelay = 10;
            this.toolTip1.IsBalloon = true;
            this.toolTip1.ReshowDelay = 2;
            this.toolTip1.ShowAlways = true;
            this.toolTip1.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            // 
            // SettingsDefaultUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpJiraSettings);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.chkFindJiraTicketsInComment);
            this.Name = "SettingsDefaultUI";
            this.Size = new System.Drawing.Size(342, 308);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpServerCredentials.ResumeLayout(false);
            this.grpServerCredentials.PerformLayout();
            this.grpJiraSettings.ResumeLayout(false);
            this.grpJiraSettings.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtDefaultSearchPath;
        private System.Windows.Forms.GroupBox grpServerCredentials;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerCredPassword;
        private System.Windows.Forms.TextBox txtServerCredUserName;
        private System.Windows.Forms.TextBox txtServerUrl;
        private System.Windows.Forms.CheckBox chkUseVStfsInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnJIRAdefault;
        private System.Windows.Forms.TextBox txtJiraSearchRegex;
        private System.Windows.Forms.CheckBox chkFindJiraTicketsInComment;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtJiraTicketLink;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox grpJiraSettings;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}