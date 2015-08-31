﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangesetViewer.Core.TFS
{
    public class ChangesetViewModel
    {
        public int ChangesetId { get; set; }
        public string Comment { get; set; }
        public string CommitterDisplayName { get; set; }
        public DateTime CreationDate { get; set; }
        public string WorkItemIds { get; set; }
        public Uri ArtifactUri { get; set; }
    }
}