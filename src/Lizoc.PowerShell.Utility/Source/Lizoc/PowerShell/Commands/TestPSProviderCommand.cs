using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using Lizoc.PowerShell;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    [Cmdlet(VerbsDiagnostic.Test, "PSProvider", 
        HelpUri = "http://docs.lizoc.com/powerextend/test-psprovider"
    )]
    [OutputType(typeof(bool))]
    public class TestPSProviderCommand : PSCmdlet
    {
        private string providerName;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string PSProvider
        {
            get { return this.providerName; }
            set { this.providerName = value; }
        }

        protected override void ProcessRecord()
        {
            Hashtable getPSProviderCmdletParams = new Hashtable();
            getPSProviderCmdletParams.Add("PSProvider", new string[] { providerName });

            Dictionary<string, object> scriptParams = new Dictionary<string, object>();
            scriptParams.Add("p1", getPSProviderCmdletParams);

            string psScript = "param($p1) Get-PSProvider @p1";

            Collection<PSObject> psout = PSScriptInvoker.Invoke(psScript, scriptParams);

            bool providerFound = false;
            foreach (PSObject psobj in psout)
            {
                providerFound = true;
                break;
            }

            base.WriteObject(providerFound);
        }
    }
}
