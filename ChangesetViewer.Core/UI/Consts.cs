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
        public const  string DefaultSearchpath = @"$/";
        public const  string DefaultJiraticketsearchregex = @"[a-zA-Z]+[-]\d+";
        public const  string DefaultJiraticketbrowseurl = "";

        public const string Pluginname = "Changeset Viewer";
        public const string SettingspageServer = "ServerPage";
        
        #endregion
    }
}
