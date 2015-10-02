using Microsoft.VisualStudio.TeamFoundation;

namespace ChangesetViewer.Core.Settings
{
    public class SettingsModelWrapper
    {
        private EnvDTE80.DTE2 _dteInstance;

        public EnvDTE80.DTE2 DTE
        {
            get
            {
                return _dteInstance;
            }
            set
            {
                _dteInstance = value;
            }
        }

        private dynamic getProperties(string propName)
        {
            if (_dteInstance != null)
            {
                var props = DTE.get_Properties(Consts.Pluginname, Consts.SettingspageServer);

                //System.IO.File.WriteAllText(@"D:\1.txt", props.Item(propName).Value.ToString());

                if (props.Item(propName).Value != null && props.Item(propName).Value.GetType().Name == "String")
                {
                    return props.Item(propName).Value == "" ? null : props.Item(propName).Value;
                }

                return props.Item(propName).Value;
            }
            return string.Empty;
        }

        public bool UseVisualStudioEnvironmentTfsConnection
        {
            get
            {
                var _t = getProperties("UseVisualStudioEnvironmentTfsConnection");
                return _t != null && !string.IsNullOrEmpty(_t.ToString()) ? _t : false;
            }
        }

        public string TFSServerURL
        {
            get
            {
                var tfsUrl = getProperties("TFSServerURL") as string;

                if (UseVisualStudioEnvironmentTfsConnection || string.IsNullOrEmpty(tfsUrl))
                {
                    TeamFoundationServerExt tfsExt = (TeamFoundationServerExt)DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.TeamFoundationServerExt");
                    if (tfsExt != null)
                    {
                        return tfsExt.ActiveProjectContext.DomainUri;
                    }
                }
                return tfsUrl;
            }
        }

        public string TFSUsername
        {
            get
            {
                return getProperties("TFSUsername") as string;
            }
        }

        public string TFSPassword
        {
            get
            {
                return getProperties("TFSPassword") as string;
            }
        }

        public string DefaultTFSSearchPath
        {
            get
            {
                return getProperties("DefaultTFSSearchPath") ?? Consts.DefaultSearchpath;
            }
        }

        public bool FindJiraTicketsInComment
        {
            get
            {
                var _t = getProperties("FindJiraTicketsInComment");
                return _t != null && !string.IsNullOrEmpty(_t.ToString()) ? _t : false;
            }
        }

        public string JiraSearchRegexPattern
        {
            get
            {
                return getProperties("JiraSearchRegexPattern") as string;
            }
        }

        public string JiraTicketBrowseLink
        {
            get
            {
                return getProperties("JiraTicketBrowseLink") as string;
            }
        }

    }

    public static class SettingsStaticModelWrapper
    {
        public static bool FindJiraTicketsInComment { get; set; }
        public static string JiraSearchRegexPattern { get; set; }
        public static string JiraTicketBrowseLink { get; set; }
    }
}
