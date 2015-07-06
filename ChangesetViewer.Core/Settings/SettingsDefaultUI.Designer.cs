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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtDefaultSearchPath = new System.Windows.Forms.TextBox();
            this.grpServerCredentials = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.chkUseVStfsInfo = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnJIRAdefault = new System.Windows.Forms.Button();
            this.txtJiraSearchRegex = new System.Windows.Forms.TextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtJiraTicketLink = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.grpServerCredentials.SuspendLayout();
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
            this.grpServerCredentials.Controls.Add(this.textBox3);
            this.grpServerCredentials.Controls.Add(this.textBox2);
            this.grpServerCredentials.Controls.Add(this.textBox1);
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
            // textBox3
            // 
            this.textBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox3.Location = new System.Drawing.Point(223, 45);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(72, 20);
            this.textBox3.TabIndex = 2;
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(81, 45);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(72, 20);
            this.textBox2.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(81, 19);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(214, 20);
            this.textBox1.TabIndex = 0;
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
            this.btnJIRAdefault.Location = new System.Drawing.Point(237, 255);
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
            this.txtJiraSearchRegex.Location = new System.Drawing.Point(119, 205);
            this.txtJiraSearchRegex.Name = "txtJiraSearchRegex";
            this.txtJiraSearchRegex.Size = new System.Drawing.Size(193, 20);
            this.txtJiraSearchRegex.TabIndex = 9;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(19, 176);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(163, 17);
            this.checkBox2.TabIndex = 8;
            this.checkBox2.Text = "Find JIRA tickets in comment";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 208);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 13);
            this.label5.TabIndex = 7;
            this.label5.Text = "JIRA search Regex";
            // 
            // txtJiraTicketLink
            // 
            this.txtJiraTicketLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtJiraTicketLink.Location = new System.Drawing.Point(119, 231);
            this.txtJiraTicketLink.Name = "txtJiraTicketLink";
            this.txtJiraTicketLink.Size = new System.Drawing.Size(193, 20);
            this.txtJiraTicketLink.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 234);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(82, 13);
            this.label6.TabIndex = 11;
            this.label6.Text = "JIRA Ticket link";
            // 
            // SettingsDefaultUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.txtJiraTicketLink);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnJIRAdefault);
            this.Controls.Add(this.txtJiraSearchRegex);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.label5);
            this.Name = "SettingsDefaultUI";
            this.Size = new System.Drawing.Size(342, 308);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.grpServerCredentials.ResumeLayout(false);
            this.grpServerCredentials.PerformLayout();
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
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.CheckBox chkUseVStfsInfo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnJIRAdefault;
        private System.Windows.Forms.TextBox txtJiraSearchRegex;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtJiraTicketLink;
        private System.Windows.Forms.Label label6;
    }
}