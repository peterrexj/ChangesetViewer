using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(string.Format(System.Globalization.CultureInfo.CurrentUICulture, "We are inside {0}.button1_Click()", this.ToString()),
                            "Changesets Viewer");

        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            lstContainer.Items.Clear();
            TFS.Reader.Infrastructure.TfsServer tfs = new TFS.Reader.Infrastructure.TfsServer();

            TFS.Reader.Infrastructure.IChangsets cs = new TFS.Reader.Infrastructure.Changesets(tfs);

            var dd = cs.Get(txtSource.Text.Trim(), 1500, txtSearchText.Text.Trim());
            foreach (var d in dd)
            {
                lstContainer.Items.Add(new { Title = d.ChangesetId, Name = d.Committer });
            }

            //lblCount.Text = dd.Count().ToString();
        }
    }
}