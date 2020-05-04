using KeePassLib.Security;
using System;
using System.Linq;
using System.Management.Automation;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace KeepassPSCmdlets.Extensions
{
    public static class ExtensionMethods
    {
        public static void AddProperty(this PSObject obj, string name, object value)
        {
            obj.Properties.Add(new PSVariableProperty(new PSVariable(name, value)));
        }

        public static void AddPropertyIfNotNullOrEmpty(this PSObject obj, string name, string value)
        {
            if (!string.IsNullOrEmpty(value))
                obj.AddProperty(name, value);
        }

        public static void AddPropertyIfNotNullOrEmpty(this PSObject obj, string name, object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (!string.IsNullOrEmpty(stringValue))
                    obj.AddProperty(name, value);
            }
            else if (value != null)
                obj.AddProperty(name, value);
        }

        public static unsafe byte[] ToByteArray(this SecureString secureString, Encoding encoding = null)
        {
            if (secureString == null)
                throw new ArgumentNullException(nameof(secureString));
            if (encoding == null)
                encoding = Encoding.UTF8;

            int maxLength = encoding.GetMaxByteCount(secureString.Length);

            IntPtr bytes = Marshal.AllocHGlobal(maxLength);
            IntPtr str = Marshal.SecureStringToBSTR(secureString);

            try
            {
                char* chars = (char*)str.ToPointer();
                byte* bptr = (byte*)bytes.ToPointer();
                int len = encoding.GetBytes(chars, secureString.Length, bptr, maxLength);

                byte[] _bytes = new byte[len];
                for (int i = 0; i < len; ++i)
                {
                    _bytes[i] = *bptr;
                    bptr++;
                }

                return _bytes;
            }
            finally
            {
                Marshal.FreeHGlobal(bytes);
                Marshal.ZeroFreeBSTR(str);
            }
        }

        public static SecureString ToSecureString(this string s)
        {
            var ss = new SecureString();
            s.ToList().ForEach(c => ss.AppendChar(c));
            ss.MakeReadOnly();
            return ss;
        }

        public static SecureString ToSecureString(this byte[] bytes, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;

            var s = new SecureString();
            var a = encoding.GetChars(bytes);
            foreach (var b in a)
            {
                s.AppendChar(b);
            }
            s.MakeReadOnly();
            return s;
        }

        public static SecureString ToSecureString(this ProtectedString s)
        {
            return s.ReadUtf8().ToSecureString(Encoding.UTF8);
        }

        public static string ToUnsecureString(this SecureString secureString, Encoding encoding = null)
        {
            if (encoding == null)
                encoding = Encoding.UTF8;
            return encoding.GetString(secureString.ToByteArray(encoding));
        }
    }
}