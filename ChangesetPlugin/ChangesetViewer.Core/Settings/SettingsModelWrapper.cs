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

        public string DefaultSearchPath
        {
            get
            {
                if (_dteInstance != null) {
                    var props = DTE.get_Properties(Consts.__PLUGINNAME, Consts.__SETTINGSPAGE_SERVER);
                    var defaultPath = props.Item("DefaultSearchPath").Value as string;
                    return defaultPath ?? Consts.__DEFAULT_SEARCHPATH;
                }
                return Consts.__DEFAULT_SEARCHPATH;
            }
        }

    }
}
