using Microsoft.VisualStudio.TeamFoundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
                var props = DTE.get_Properties(Consts.__PLUGINNAME, Consts.__SETTINGSPAGE_SERVER);

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
                return getProperties("UseVisualStudioEnvironmentTfsConnection") ?? true;
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
                return getProperties("DefaultTFSSearchPath") ?? Consts.__DEFAULT_SEARCHPATH;
            }
        }

        public bool FindJiraTicketsInComment
        {
            get
            {
                return getProperties("FindJiraTicketsInComment") ?? true;
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
}
