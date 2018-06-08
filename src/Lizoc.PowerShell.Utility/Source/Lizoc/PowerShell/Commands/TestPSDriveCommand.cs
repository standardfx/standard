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
    [Cmdlet(VerbsDiagnostic.Test, "PSDrive", 
        DefaultParameterSetName = "NameSet",
        HelpUri = "http://docs.lizoc.com/powerextend/test-psdrive"
    )]
    [OutputType(typeof(bool))]
    public class TestPSDriveCommand : PSCmdlet
    {
        private string name;
        private string literalName;
        private string scope;
        private string[] psProvider;
        private bool useTransaction = false;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, ParameterSetName = "NameSet")]
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        [Parameter(Mandatory = true, ParameterSetName = "LiteralNameSet")]
        public string LiteralName
        {
            get { return this.literalName; }
            set { this.literalName = value; }
        }

        [Parameter(Mandatory = false)]
        public string Scope
        {
            get { return this.scope; }
            set { this.scope = value; }
        }

        [Parameter(Mandatory = false)]
        public string[] PSProvider
        {
            get { return this.psProvider; }
            set { this.psProvider = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter UseTransaction
        {
            get { return this.useTransaction; }
            set { this.useTransaction = value; }
        }

        protected override void ProcessRecord()
        {
            Hashtable getPSDriveCmdletParams = new Hashtable();

            switch (ParameterSetName)
            {
                case "NameSet":
                    getPSDriveCmdletParams.Add("Name", new string[] { name });
                    break;

                case "LiteralNameSet":
                    getPSDriveCmdletParams.Add("LiteralName", new string[] { literalName });
                    break;

                default:
                    throw new ArgumentException(string.Format("Bad ParameterSet: {0}", ParameterSetName));
            }

            if (psProvider != null)
                getPSDriveCmdletParams.Add("PSProvider", psProvider);

            if (scope != null)
                getPSDriveCmdletParams.Add("Scope", Scope);

            if (useTransaction == true)
                getPSDriveCmdletParams.Add("UseTransaction", true);

            Dictionary<string, object> scriptParams = new Dictionary<string, object>();
            scriptParams.Add("p1", getPSDriveCmdletParams);

            string psScript = "param($p1) Get-PSDrive @p1";

            Collection<PSObject> psout = PSScriptInvoker.Invoke(psScript, scriptParams);

            bool driveFound = false;
            foreach (PSObject psobj in psout)
            {
                driveFound = true;
                break;
            }

            base.WriteObject(driveFound);
        }
    }
}
