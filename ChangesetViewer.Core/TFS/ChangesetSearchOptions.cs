using System.Collections.Generic;
namespace ChangesetViewer.Core.TFS
{
    public class ChangesetSearchOptions
    {
        public string ProjectSourcePath { get; set; }
        public int TopN { get; set; }
        public string SearchKeyword { get; set; }
        public string Committer { get; set; }
        public bool IsSearchBasedOnRegex { get; set; }

        public IEnumerable<string> SearchKeywordSplitMode
        {
            get
            {
                return SearchKeyword.Split(" ".ToCharArray());
            }
        }
    }
}
