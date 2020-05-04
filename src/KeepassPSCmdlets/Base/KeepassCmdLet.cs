using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Security;

namespace KeepassPSCmdlets.Base
{
    public abstract class KeepassCmdLet : PSCmdlet
    {
        protected KeepassCmdLet()
        {
            // TODO See if entire Keepass Handling can be moved into seperate AppDomain 
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomainOnAssemblyResolve;
        }

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
        [Alias("Path", "DbFile", "File", "DbPath", "Database", "Input")]
        public object InputObject { get; set; }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        [Alias("BinDir", "Binaries", "KeepassBinLocation", "KeepassBin", "KP", "KPBinaries", "KPBin")]
        public string KeepassLocation { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        [Alias("KeyFilePath", "KeyPath")]
        public string KeyFile { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true, Position = 1)]
        [Alias("Password", "Pwd")]
        public SecureString MasterPassword { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter WindowsUserAccount { get; set; }

        private Assembly CurrentDomainOnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var missingAssemblyName = new AssemblyName(args.Name);

            var lookupDirectory = Path.GetDirectoryName(this.GetType().Assembly.Location);
            if (!string.IsNullOrWhiteSpace(KeepassLocation) && Directory.Exists(KeepassLocation))
            {
                lookupDirectory = KeepassLocation;
            }

            var possibleResolvedFileLocation = Path.Combine(lookupDirectory, missingAssemblyName.Name + ".exe");
            if (File.Exists(possibleResolvedFileLocation))
                return Assembly.LoadFile(possibleResolvedFileLocation);

            return null;
        }
    }
}