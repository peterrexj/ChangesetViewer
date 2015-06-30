using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ChangesetViewer.Core.Settings
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [Guid("1D9ECCF3-5D2F-4112-9B25-264596873DC9")]
    [CLSCompliant(false), ComVisible(true)]  
    public class SettingsPageModel : DialogPage
    {
        private string _defaultSearchPath = @"$/";

        public bool UseTFSconnectedServer { get; set; }

        public string ServerURL { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public string DefaultSearchPath { get; set; }

        private string optionValue = "alpha";

        public string OptionString
        {
            get { return optionValue; }
            set { optionValue = value; }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected override IWin32Window Window
        {
            get
            {
                SettingsDefaultUI page = new SettingsDefaultUI();
                page.optionsPage = this;
                page.Initialize();
                return page;
            }
        }
    }
}
