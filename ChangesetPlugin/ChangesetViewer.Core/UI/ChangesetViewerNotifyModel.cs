using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.UI
{
    public class ChangesetViewerNotifyModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Notify(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        private string _sourceControlName;
        public string SourceControlName
        {
            get
            {
                return _sourceControlName;
            }
            set
            {
                _sourceControlName = value;
                Notify("SourceControlName");
            }
        }
    }
}
