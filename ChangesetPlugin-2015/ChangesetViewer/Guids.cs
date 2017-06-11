// Guids.cs
// MUST match guids.h
using System;

namespace PeterRexJoseph.ChangesetViewer
{
    static class GuidList
    {
        public const string guidChangesetViewerPkgString = "77b916dd-5930-4798-908c-0a317565b3d4";
        public const string guidChangesetViewerCmdSetString = "54fe5d53-84bb-4cce-b49e-27f6b1513637";
        public const string guidToolWindowPersistanceString = "cd1a9663-b078-4811-a298-c2073eccbead";
        public const string guidchangesetviewerTeamExplorerNavigationItem = "04681CA1-45DD-4E24-B5D2-8633AF3ECA4E";
        public const string guidchangesetviewerTeamExplorerPage = "D40AAE04-82AC-4338-8517-03596BACCCCF";

        public static readonly Guid guidChangesetViewerCmdSet = new Guid(guidChangesetViewerCmdSetString);
    };
}