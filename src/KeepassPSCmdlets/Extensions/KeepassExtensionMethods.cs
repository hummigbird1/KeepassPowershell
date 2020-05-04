using KeePassLib;
using KeePassLib.Delegates;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace KeepassPSCmdlets.Extensions
{
    public static class KeepassExtensionMethods
    {
        public static string CreateGroupPath(this PwEntry entry)
        {
            return entry.ParentGroup.CreateGroupPath();
        }

        public static string CreateGroupPath(this PwGroup @group)
        {
            List<string> groupNames = new List<string>();
            var grp = @group;
            do
            {
                groupNames.Add(grp.Name);
                grp = grp.ParentGroup;
            } while (grp != null);
            groupNames.Reverse();
            return string.Join(Path.DirectorySeparatorChar.ToString(), groupNames.ToList());
        }

        public static void EnumerateEntries(this PwDatabase db, Action<PwDatabase, PwEntry> entryAction)
        {
            if (entryAction != null)
            {
                db.RootGroup.TraverseTree(TraversalMethod.PreOrder, null, entry =>
                                                                          {
                                                                              entryAction(db, entry);
                                                                              return true;
                                                                          });
            }
        }

        public static List<PwEntry> GetEntries(this PwDatabase db, Func<PwDatabase, PwEntry, bool> matchFunction = null)
        {
            List<PwEntry> entries = new List<PwEntry>();
            db.TraverseEntries(entry =>
            {
                bool isMatch = true;
                if (matchFunction != null)
                {
                    isMatch = matchFunction(db, entry);
                }
                if (isMatch)
                    entries.Add(entry);
                return true;
            });
            return entries;
        }

        public static PwGroup GetRecycleBinGroup(this PwDatabase db)
        {
            if (db.RecycleBinEnabled)
            {
                return db.RootGroup.FindGroup(db.RecycleBinUuid, true);
            }
            return null;
        }

        public static bool IsEntryInRecycleBin(this PwDatabase db, PwEntry entry)
        {
            if (db.RecycleBinEnabled)
            {
                return entry.IsContainedIn(db.GetRecycleBinGroup());
            }
            return false;
        }

        public static PwDatabase OpenDatabase(this IOConnectionInfo connectionInfo, CompositeKey key)
        {
            var database = new PwDatabase();
            database.Open(connectionInfo, key, new NullStatusLogger()); // TODO Check if Powershell output logger instead of NullLogger makes any sense 
            return database;
        }

        public static void TraverseEntries(this PwDatabase db, EntryHandler entryHandler)
        {
            db.RootGroup.TraverseTree(TraversalMethod.PreOrder, null, entryHandler);
        }
    }
}