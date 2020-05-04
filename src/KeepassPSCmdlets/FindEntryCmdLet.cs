using KeePassLib;
using KeepassPSCmdlets.Base;
using KeepassPSCmdlets.Extensions;
using System.Collections.Generic;
using System.Management.Automation;

namespace KeepassPSCmdlets
{
    [Cmdlet(VerbsCommon.Find, "KeepassEntry")]
    public class FindEntryCmdlet : EntryBaseCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [Alias("Groups", "Grp")]
        public string[] Group { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public FilterMode GroupFilterMode { get; set; } = FilterMode.Wildcard;

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter GroupFilterOnPath { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter IncludeRecycleBin { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [Alias("TagMode")]
        public TagMatchMode TagMatchMode { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [Alias("Tag")]
        public string[] Tags { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [Alias("Name", "Names", "Entry", "Titles", "Entries")]
        public string[] Title { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public FilterMode TitleFilterMode { get; set; } = FilterMode.Wildcard;

        protected override IEnumerable<PwEntry> RetrieveEntries(PwDatabase database)
        {
            var entryHandler = new KeepassFindEntryHandler(new KeepassFindEntryParameter
            {
                Group = Group,
                GroupFilterMode = GroupFilterMode,
                GroupFilterOnPath = GroupFilterOnPath.ToBool(),
                TagMatchMode = TagMatchMode,
                Tags = Tags,
                Title = Title,
                TitleFilterMode = TitleFilterMode
            })
            {
                IncludeRecycleBin = IncludeRecycleBin
            };

            return database.GetEntries(entryHandler.EntryHandler);
        }
    }
}