//using ChangesetViewer.Core.TFS;
using Microsoft.VisualStudio.TeamFoundation.VersionControl;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace PeterRexJoseph.ChangesetViewer
{
    /// <summary>
    /// Interaction logic for MyControl.xaml
    /// </summary>
    public partial class MyControl : UserControl
    {
        public MyControl()
        {
            InitializeComponent();
            
        }

        public EnvDTE80.DTE2 DTE { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "Changesets Viewer");

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            //ITfsServer tfs = new TfsServer();
            //tfs.GetCollection();

            //var _changesets = new Changesets(tfs);
            //var c = _changesets.Get(23969);

            //EnvDTE.IVsExtensibility extensibility = ChangesetViewerPackage.GetGlobalService(typeof(EnvDTE.IVsExtensibility)) as EnvDTE.IVsExtensibility;
            //DTE = extensibility.GetGlobalsObject(null).DTE as EnvDTE80.DTE2;

            //VersionControlExt vce;
            //vce = DTE.GetObject("Microsoft.VisualStudio.TeamFoundation.VersionControl.VersionControlExt") as VersionControlExt;
            //vce.ViewChangesetDetails(c.ChangesetId);

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            
            //lstContainer.Items.Clear();
            //TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();

            //TFS.Reader.Infrastructure.IChangsets cs = new TFS.Reader.Infrastructure.Changesets(tfs);

            //var dd = cs.Get(txtSource.Text.Trim(), 1500, txtSearchText.Text.Trim());
            //foreach (var d in dd)
            //{
            //    lstContainer.Items.Add(new { Title = d.ChangesetId, Name = d.Committer });
            //}

            //lblCount.Text = dd.Count().ToString();

            
            //vstfs:///VersionControl/Changeset/23969
        }
    }
}