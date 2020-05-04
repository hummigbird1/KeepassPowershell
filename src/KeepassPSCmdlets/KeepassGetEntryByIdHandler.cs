using KeePassLib;
using KeepassPSCmdlets.Extensions;
using System;
using System.Linq;

namespace KeepassPSCmdlets
{
    public class KeepassGetEntryByIdHandler : IKPEntryHandler
    {
        public KeepassGetEntryByIdHandler(string[] ids)
        {
            Ids = ids ?? throw new ArgumentNullException(nameof(ids));
        }

        public Func<PwDatabase, PwEntry, bool> EntryHandler => EntryHandlerFunction;

        public string[] Ids { get; }

        public bool IncludeRecycleBin { get; set; }

        private bool EntryHandlerFunction(PwDatabase db, PwEntry pwEntry)
        {
            if (!IncludeRecycleBin && db.RecycleBinEnabled && db.IsEntryInRecycleBin(pwEntry))
                return false;

            return Ids.Any(x => pwEntry.Uuid.ToHexString() == x);
        }
    }
}