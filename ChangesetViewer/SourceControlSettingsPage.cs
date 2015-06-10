using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace PeterRexJoseph.ChangesetViewer
{
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [CLSCompliant(false), ComVisible(true)]
    public class SourceControlSettingsPage : DialogPage
    {

        private string _defaultServerSearchPath = @"$/";

        [Category("Source Control Settings")]
        [DisplayName("Server Search Path")]
        [Description("Default server path to search")]
        public string DefaultSearchPath
        {
            get
            {
                return _defaultServerSearchPath;
            }
            set
            {
                _defaultServerSearchPath = value;
            }
        }

        [Category("Source Control Settings")]
        [DisplayName("Search for JIRA tickets in comment")]
        [Description("Search for JIRA tickets in comment")]
        public bool SearchForJIRATicket { get; set; }

    }
}
