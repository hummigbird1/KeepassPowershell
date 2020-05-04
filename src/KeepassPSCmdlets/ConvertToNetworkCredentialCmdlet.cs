using KeepassPSCmdlets.Extensions;
using System;
using System.Linq;
using System.Management.Automation;
using System.Security;

namespace KeepassPSCmdlets
{
    [Cmdlet(VerbsData.ConvertTo, "NetworkCredential")]
    public class ConvertToNetworkCredentialCmdlet : PSCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string DomainPropertyName { get; set; } = "Domain";

        [Parameter(Mandatory = true, ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 0)]
        public PSObject InputObject { get; set; }

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string PasswordPropertyName { get; set; } = "Password";

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public SwitchParameter UpnUserName { get; set; } = true;

        [Parameter(Mandatory = false, ValueFromPipelineByPropertyName = true)]
        public string UserPropertyName { get; set; } = "UserName";

        protected override void ProcessRecord()
        {
            var properties = InputObject.Properties.Where(x => x.IsGettable).ToDictionary(x => x.Name, x => x.Value);
            if (properties.ContainsKey(UserPropertyName) && properties.ContainsKey(PasswordPropertyName))
            {
                var userName = properties[UserPropertyName];
                var password = properties[PasswordPropertyName];
                string domain = null;
                if (properties.ContainsKey(DomainPropertyName))
                {
                    domain = properties[DomainPropertyName].ToString();
                }
                if (!string.IsNullOrEmpty(domain))
                {
                    userName = string.Format(UpnUserName ? "{1}@{0}" : "{0}\\{1}", domain, userName);
                }
                if (password is SecureString)
                    WriteObject(new PSCredential(userName.ToString(), (SecureString)password));
                else
                    WriteObject(new PSCredential(userName.ToString(), password.ToString().ToSecureString()));
            }
            else
            {
                WriteError(new ErrorRecord(new Exception($"The InputObject does not contain the required properties '{UserPropertyName}' and '{PasswordPropertyName}'"), "UnsupportedInputObject", ErrorCategory.InvalidArgument, InputObject));
            }
        }
    }
}