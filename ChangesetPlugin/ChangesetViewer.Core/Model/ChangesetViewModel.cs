using System;

namespace ChangesetViewer.Core.Model
{
    public class ChangesetViewModel
    {
        public int ChangesetId { get; set; }
        public string Comment { get; set; }
        public string CommitterDisplayName { get; set; }
        public DateTime CreationDate { get; set; }
        public string WorkItemIds { get; set; }
        public string WorkItemTitles { get; set; }
        public Uri ArtifactUri { get; set; }
    }
}
