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
    [Cmdlet(VerbsData.Expand, "MUIString", 
        HelpUri = "http://docs.lizoc.com/powerextend/expand-muistring"
    )]
    [OutputType(typeof(string))]
    public class ExpandMUIStringCommand : Cmdlet
    {
        private string[] _inputObject;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string[] InputObject
        {
            get { return _inputObject; }
            set { _inputObject = value; }
        }

        protected override void ProcessRecord()
        {
            foreach (string item in _inputObject)
            {
                base.WriteObject(WindowsUtility.GetExpandableString(item));
            }
        }
    }
}
