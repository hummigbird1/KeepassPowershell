using KeePass.Util.Spr;
using KeePassLib;
using KeePassLib.Cryptography;
using KeepassPSCmdlets.Extensions;
using System;
using System.Linq;
using System.Management.Automation;
using System.Text;

namespace KeepassPSCmdlets
{
    public class KeepassEntryConverter
    {
        public static PSObject CreateEntryResult(PwDatabase db, PwEntry passwordEntry, bool asUnprotectedStrings, bool resolveReferencedFields)
        {
            var result = new PSObject();
            result.AddProperty("Id", passwordEntry.Uuid.ToHexString());
            result.AddProperty("GroupName", passwordEntry.ParentGroup.Name);
            result.AddProperty("Path", passwordEntry.CreateGroupPath());
            result.AddPropertyIfNotNullOrEmpty(PwDefs.TitleField, GetStringEntry(PwDefs.TitleField, db, passwordEntry, asUnprotectedStrings, resolveReferencedFields));
            result.AddPropertyIfNotNullOrEmpty(PwDefs.UserNameField, GetStringEntry(PwDefs.UserNameField, db, passwordEntry, asUnprotectedStrings, resolveReferencedFields));
            result.AddProperty(PwDefs.PasswordField, asUnprotectedStrings ? (object)passwordEntry.Strings.Get(PwDefs.PasswordField).ReadString() : passwordEntry.Strings.Get(PwDefs.PasswordField).ReadUtf8().ToSecureString(Encoding.UTF8));
            result.AddProperty("EstimatedPasswordQualityBits", QualityEstimation.EstimatePasswordBits(passwordEntry.Strings.Get(PwDefs.PasswordField).ReadUtf8()));

            if (passwordEntry.Tags != null && passwordEntry.Tags.Any())
                result.AddProperty("Tags", passwordEntry.Tags);

            result.AddProperty("Expires", passwordEntry.Expires);
            if (passwordEntry.Expires)
            {
                result.AddProperty("IsExpired", passwordEntry.ExpiryTime < DateTime.Now);
                result.AddProperty("ExpireTime", passwordEntry.ExpiryTime);
            }

            foreach (var key1 in passwordEntry.Strings.GetKeys().Where(x => !new[] { PwDefs.TitleField, PwDefs.PasswordField, PwDefs.UserNameField }.Contains(x)))
            {
                result.AddPropertyIfNotNullOrEmpty(key1, GetStringEntry(key1, db, passwordEntry, asUnprotectedStrings, resolveReferencedFields));
            }
            // Version 2.36
            //foreach (var key1 in passwordEntry.CustomData)
            //{
            //    result.AddPropertyIfNotNullOrEmpty(key1.Key, key1.Value);
            //}
            return result;
        }

        private static object GetStringEntry(string key, PwDatabase db, PwEntry entry, bool asUnprotectedStrings, bool resolveReferencedFields)
        {
            var keyEntry = entry.Strings.Get(key);
            if (keyEntry.IsProtected && !asUnprotectedStrings)
                return keyEntry.ToSecureString();

            var stringValue = keyEntry.ReadString();
            if (resolveReferencedFields && stringValue.Contains("{REF:"))
                return SprEngine.Compile(stringValue, new SprContext(entry, db, SprCompileFlags.All));

            return stringValue;
        }
    }
}