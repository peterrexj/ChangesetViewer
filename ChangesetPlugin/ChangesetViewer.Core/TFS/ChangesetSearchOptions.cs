using System.Collections;
using System.Collections.Generic;
using PluginCore.Extensions;
using System.Linq;
using System;
using ChangesetViewer.Core.UI;

namespace ChangesetViewer.Core.TFS
{
    public class ChangesetSearchOptions
    {
        private string _projectSourcePath;

        public string ProjectSourcePath
        {
            get
            {
                if (string.IsNullOrEmpty(_projectSourcePath))
                    _projectSourcePath = @"$\";
                return _projectSourcePath;
            }
            set
            {
                _projectSourcePath = value;
            }
        }
        public int TopN { get; set; }
        public string SearchKeyword { get; set; }
        public string Committer { get; set; }
        public bool IsSearchBasedOnRegex { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Consts.SearchCommentType SearchCommentType { get; set; }

        public IEnumerable<string> SearchKeywordSplitMode
        {
            get
            {
                return (IEnumerable<string>)SearchKeyword.Split(" ".ToCharArray()).Select(s => s.Trim());
            }
        }

    }
}
