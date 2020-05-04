using KeePassLib;
using System;

namespace KeepassPSCmdlets
{
    public interface IKPEntryHandler
    {
        Func<PwDatabase, PwEntry, bool> EntryHandler { get; }
    }
}