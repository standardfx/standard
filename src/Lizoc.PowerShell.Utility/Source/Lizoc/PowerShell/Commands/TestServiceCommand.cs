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
    [Cmdlet(VerbsDiagnostic.Test, "Service", 
        DefaultParameterSetName = "NameSet",
        HelpUri = "http://docs.lizoc.com/powerextend/test-service"
    )]
    [OutputType(typeof(bool))]
    public class TestServiceCommand : PSCmdlet
    {
        private string name;
        private string displayName;
        private string computerName;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "NameSet")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [Parameter(Mandatory = true, ParameterSetName = "DisplayNameSet")]
        public string DisplayName
        {
            get { return this.displayName; }
            set { this.displayName = value; }
        }

        [Parameter(Mandatory = false)]
        public string ComputerName
        {
            get { return this.computerName; }
            set { this.computerName = value; }
        }

        protected override void ProcessRecord()
        {
            Hashtable getServiceCmdletParams = new Hashtable();

            switch (ParameterSetName)
            {
                case "NameSet":
                    getServiceCmdletParams.Add("Name", new string[] { name });
                    break;

                case "DisplayNameSet":
                    getServiceCmdletParams.Add("DisplayName", new string[] { displayName });
                    break;

                default:
                    throw new ArgumentException(string.Format("Bad ParameterSet: {0}", ParameterSetName));
            }

            if (computerName != null)
                getServiceCmdletParams.Add("ComputerName", new string[] { computerName });

            Dictionary<string, object> scriptParams = new Dictionary<string, object>();
            scriptParams.Add("p1", getServiceCmdletParams);

            string psScript = "param($p1) Get-Service @p1";

            Collection<PSObject> psout = PSScriptInvoker.Invoke(psScript, scriptParams);

            bool serviceFound = false;
            foreach (PSObject psobj in psout)
            {
                serviceFound = true;
                break;
            }

            base.WriteObject(serviceFound);
        }
    }
}
