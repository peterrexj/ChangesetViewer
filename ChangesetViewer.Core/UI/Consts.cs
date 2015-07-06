using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesetViewer.Core
{
    public static class Consts
    {
        public enum SearchCommentType
        {
            Exact,
            Keyword
        }

        #region default values
        public const  string __DEFAULT_SEARCHPATH = @"$/";
        public const  string __DEFAULT_JIRATICKETSEARCHREGEX = @"[a-zA-Z]+[-]\d+";
        public const  string __DEFAULT_JIRATICKETBROWSEURL = "https://janisoncls.atlassian.net/browse/";

        public const string __PLUGINNAME = "Changeset Viewer";
        public const string __SETTINGSPAGE_SERVER = "ServerPage";
        
        #endregion
    }
}
