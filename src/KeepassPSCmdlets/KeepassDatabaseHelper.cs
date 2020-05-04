using KeePassLib;
using KeePassLib.Keys;
using KeePassLib.Serialization;
using KeepassPSCmdlets.Extensions;
using System;
using System.Management.Automation;
using System.Security;
using System.Text;

namespace KeepassPSCmdlets
{
    public static class KeepassDatabaseHelper
    {
        public static CompositeKey CreatePasswordDatabaseKey(SecureString masterPassword, string keyFile, bool windowsUserAccount)
        {
            var key = new CompositeKey();
            if (masterPassword != null)
                key.AddUserKey(new KcpPassword(masterPassword.ToByteArray(Encoding.UTF8)));

            if (!string.IsNullOrEmpty(keyFile))
                key.AddUserKey(new KcpKeyFile(keyFile));

            if (windowsUserAccount)
                key.AddUserKey(new KcpUserAccount());

            return key;
        }

        public static PwDatabase GetDatabaseInstance(object inputObject, CompositeKey databaseCompositeKey)
        {
            if (inputObject == null)
                throw new Exception("The Database Object was not specified!");

            if (inputObject is PwDatabase)
            {
                return (PwDatabase)inputObject;
            }
            if (inputObject is SecureString)
            {
                return OpenPasswordDatabase(((SecureString)inputObject).ToUnsecureString(), databaseCompositeKey);
            }
            if (inputObject is PSObject)
            {
                return GetDatabaseInstance(((PSObject)inputObject).BaseObject, databaseCompositeKey);
            }

            // Assume its a file path
            return OpenPasswordDatabase(inputObject.ToString(), databaseCompositeKey);
        }

        private static PwDatabase OpenPasswordDatabase(string dbPath, CompositeKey key)
        {
            var connectionInfo = new IOConnectionInfo
            {
                Path = dbPath,
                CredSaveMode = IOCredSaveMode.NoSave
            };
            return connectionInfo.OpenDatabase(key);
        }
    }
}