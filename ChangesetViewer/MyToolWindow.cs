﻿using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using ChangesetViewer.UI.View;
using Microsoft.TeamFoundation.Controls;
using ChangesetViewer.Core.UI;

namespace PeterRexJoseph.ChangesetViewer
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid("cd1a9663-b078-4811-a298-c2073eccbead")]
    public class MyToolWindow : ToolWindowPane
    {
        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public MyToolWindow() :
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;

            // This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
            // we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on 
            // the object returned by the Content property.
            //base.Content = new MyControl();

            var content = new ChangesetViewerMainWindow();

            var extensibility = ChangesetViewerPackage.GetGlobalService(typeof(EnvDTE.IVsExtensibility)) as EnvDTE.IVsExtensibility;
            content.UIController.DTE = extensibility.GetGlobalsObject(null).DTE as EnvDTE80.DTE2;
            ChangesetViewerUIController.TeamExplorer = (ITeamExplorer)ChangesetViewerPackage.GetGlobalService(typeof(ITeamExplorer));

            content.InitializeWindow();

            base.Content = content;
            
        }
    }
}
