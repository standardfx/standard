using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Linq;
using System.Security;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    [Cmdlet(
        VerbsCommon.New, "Credential", 
        DefaultParameterSetName = "__AllParameterSets",
        HelpUri = "http://docs.lizoc.com/powerextend/new-credential",
        RemotingCapability = RemotingCapability.None
    )]
    [OutputType(typeof(System.Management.Automation.RuntimeDefinedParameterDictionary))]
    public class NewCredentialCommand : Cmdlet
    {
        private SecureString _password = null;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string UserName { get; set; }

        [Parameter(Mandatory = false, Position = 1)]
        public SecureString Password 
        { 
            get { return _password; }
            set { _password = value; }
        }

        protected override void ProcessRecord()
        {
            if (_password == null)
                _password = new SecureString();

            PSCredential pscred = new PSCredential(this.UserName, _password);
            base.WriteObject(pscred);
        }
    }
}
