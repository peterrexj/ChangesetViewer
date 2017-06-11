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
        public bool FindJiraTicketsInComment { get; set; }
        public string JiraSearchRegexPattern { get; set; }
        public string JiraTicketBrowseLink { get; set; }


        public bool UseVisualStudioEnvironmentTfsConnection { get; set; }
        public string TFSServerURL { get; set; }
        public string TFSUsername { get; set; }
        public string TFSPassword { get; set; }

        public string DefaultTFSSearchPath { get; set; }

        public int SearchPageSize { get; set; }

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
