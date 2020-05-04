using KeePassLib;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace KeepassPSCmdlets.Base
{
    public abstract class EntryBaseCmdlet : KeepassCmdLet
    {
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [Alias("Native", "Raw")]
        public SwitchParameter AsNativeKeepassObject { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [Alias("Unsafe", "Unprotected")]
        public SwitchParameter AsUnprotectedStrings { get; set; } = false;

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [Alias("ResolveLinks", "Resolve", "Dereference", "Deref")]
        public SwitchParameter ResolveReferencedFields { get; set; } = true;

        protected override void ProcessRecord()
        {
            var databaseCompositeKey = KeepassDatabaseHelper.CreatePasswordDatabaseKey(MasterPassword, KeyFile, WindowsUserAccount); // TODO ? Make a seperate Cmdlet to create a key and pass in as parameter
            var keepassDb = KeepassDatabaseHelper.GetDatabaseInstance(InputObject, databaseCompositeKey);

            var passwordEntries = RetrieveEntries(keepassDb);

            if (passwordEntries != null)
            {
                if (AsNativeKeepassObject)
                {
                    WarnUserAboutUnusedParameters();
                    WriteObject(passwordEntries, true);
                }
                else
                {
                    WriteObject(passwordEntries.Select(entry => KeepassEntryConverter.CreateEntryResult(keepassDb, entry, AsUnprotectedStrings, ResolveReferencedFields)), true);
                }
            }
        }

        protected abstract IEnumerable<PwEntry> RetrieveEntries(PwDatabase database);

        private bool GetParameterState(string parameterName, bool currentParameterValue)
        {
            if (MyInvocation.BoundParameters.ContainsKey(parameterName))
            {
                return currentParameterValue;
            }

            return false;
        }

        private void WarnUserAboutUnusedParameters()
        {
            var unusedParameters = new List<string>();
            var asUnprotectedStrings = GetParameterState(nameof(AsUnprotectedStrings), AsUnprotectedStrings);
            if (asUnprotectedStrings)
            {
                unusedParameters.Add(nameof(AsUnprotectedStrings));
            }
            var resolveReferencedFields = GetParameterState(nameof(ResolveReferencedFields), ResolveReferencedFields);

            if (resolveReferencedFields)
            {
                unusedParameters.Add(nameof(ResolveReferencedFields));
            }

            if (unusedParameters.Any())
            {
                var message = new StringBuilder($"You have specified some parameters that have no effect since you also specified to use '{nameof(AsNativeKeepassObject)}': ");
                message.Append(string.Join(", ", unusedParameters));
                WriteWarning(message.ToString());
            }
        }
    }
}