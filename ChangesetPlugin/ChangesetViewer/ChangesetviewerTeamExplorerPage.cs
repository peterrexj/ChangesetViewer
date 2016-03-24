using System;
using System.ComponentModel;
using System.Linq;

using Microsoft.TeamFoundation.Controls;
using ChangesetViewer.UI.View;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.PlatformUI;
using PluginCore.Extensions;
using ChangesetViewer.Core.UI;

namespace PeterRexJoseph.ChangesetViewer
{
    [TeamExplorerPage(GuidList.guidchangesetviewerTeamExplorerPage)]
    public class ChangesetviewerTeamExplorerPage : ITeamExplorerPage
    {
        private IServiceProvider serviceProvider;

        private bool isBusy;

        public void Cancel()
        {
        }

        public object GetExtensibilityService(Type serviceType)
        {
            return null;
        }

        public void Initialize(object sender, PageInitializeEventArgs e)
        {
            this.serviceProvider = e.ServiceProvider;
            VSColorTheme.ThemeChanged += VSColorTheme_ThemeChanged;
        }

        void VSColorTheme_ThemeChanged(ThemeChangedEventArgs e)
        {
            Refresh();
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }
            private set
            {
                this.isBusy = value;
                this.FirePropertyChanged("IsBusy");
            }
        }

        public void Loaded(object sender, PageLoadedEventArgs e)
        {
        }

        private static object _pageContent;
        public object PageContent
        {
            get
            {
                if (_pageContent == null)
                {
                    _pageContent = CreateContent();
                }
                return _pageContent;
            }
        }

        public void Refresh()
        {
            var service = this.GetService<ITeamExplorer>();
            if (service == null)
            {
                return;
            }
            _pageContent = null;
            service.NavigateToPage(new Guid(GuidList.guidchangesetviewerTeamExplorerPage), null);
            
        }

        private object CreateContent()
        {
            var extensibility = ChangesetViewerPackage.GetGlobalService(typeof(EnvDTE.IVsExtensibility)) as EnvDTE.IVsExtensibility;
            var content = new ChangesetViewerMainWindow();

            content.UIController.DTE = extensibility.GetGlobalsObject(null).DTE as EnvDTE80.DTE2;
            ChangesetViewerUIController.TeamExplorer = (ITeamExplorer)ChangesetViewerPackage.GetGlobalService(typeof(ITeamExplorer));
            content.InitializeWindow();
            content.UIController.VisualStudioColors = GetVisualStudioFilteredColors();
            content.ApplyTheme();

            return content;
        }

        public void SaveContext(object sender, PageSaveContextEventArgs e)
        {
        }

        public string Title
        {
            get
            {
                return "Changeset Viewer";
            }
        }

        public void Dispose()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void FirePropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        #region Extra Plugin Access Functions from VS
        private List<Color> GetVisualStudioBasicColorList()
        {
            IVsUIShell uiShell = (IVsUIShell)ChangesetViewerPackage.GetGlobalService(typeof(IVsUIShell));

            List<Color> result = new List<Color>();

            foreach (VSSYSCOLOR vsSysColor in Enum.GetValues(typeof(VSSYSCOLOR)))
            {
                uint win32Color;
                uiShell.GetVSSysColor(vsSysColor, out win32Color);
                Color color = ColorTranslator.FromWin32((int)win32Color);
                result.Add(color);
            }

            return result;
        }

        private Dictionary<string, Color> GetVisualStudioDetailedColorList()
        {
            IVsUIShell2 uiShell = (IVsUIShell2)ChangesetViewerPackage.GetGlobalService(typeof(IVsUIShell));

            Dictionary<string, Color> result = new Dictionary<string, Color>();

            foreach (__VSSYSCOLOREX vsSysColor in Enum.GetValues(typeof(__VSSYSCOLOREX)))
            {
                uint win32Color;
                uiShell.GetVSSysColorEx((int)vsSysColor, out win32Color);
                Color color = ColorTranslator.FromWin32((int)win32Color);

                if (!result.Any(c => c.Key == vsSysColor.ToString()))
                    result.Add(vsSysColor.ToString(), color);
            }

            return result;
        }

        private Dictionary<string, Color> GetVisualStudioFilteredColors()
        {
            Dictionary<string, Color> result = new Dictionary<string, Color>();

            result.Add("ToolboxBackgroundColorKey", VSColorTheme.GetThemedColor(EnvironmentColors.ToolboxBackgroundColorKey));
            result.Add("ControlLinkTextColorKey", VSColorTheme.GetThemedColor(EnvironmentColors.ControlLinkTextColorKey));
            result.Add("ControlOutlineColorKey", VSColorTheme.GetThemedColor(EnvironmentColors.ControlOutlineColorKey));
            result.Add("PanelTextColorKey", VSColorTheme.GetThemedColor(EnvironmentColors.PanelTextColorKey));
            result.Add("CommandBarTextInactiveColorKey", VSColorTheme.GetThemedColor(EnvironmentColors.CommandBarTextInactiveColorKey));

            return result;
        }

        #endregion
    }
}
