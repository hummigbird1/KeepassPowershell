using KeePassLib;
using KeepassPSCmdlets.Base;
using KeepassPSCmdlets.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace KeepassPSCmdlets
{
    [Cmdlet(VerbsCommon.Get, "KeepassEntry")]
    public class GetEntryCmdlet : EntryBaseCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string[] Id { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter IncludeRecycleBin { get; set; }

        protected override IEnumerable<PwEntry> RetrieveEntries(PwDatabase database)
        {
            var entryHandler = new KeepassGetEntryByIdHandler(Id)
            {
                IncludeRecycleBin = IncludeRecycleBin
            };

            var passwordEntries = database.GetEntries(entryHandler.EntryHandler);

            if (!passwordEntries.Any())
            {
                WriteError(new ErrorRecord(new Exception("No records with the given Ids could be found."), "NoRecordsFoundForGivenIds", ErrorCategory.ObjectNotFound, Id));
                return null;
            }
            else
            {
                return passwordEntries;
            }
        }
    }
}