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
    //<#
    //  .SYNOPSIS
    //      Tests for common output preference parameters.
    //
    //  .DESCRIPTION
    //      This command is used to determine what kind of output will be shown.
    //
    //      For example, `foo -Verbose` will display verbose messages. In the function `foo`, you
    //      can determine whether verbose messages will be displayed by using this command.
    //
    //  .EXAMPLE
    //      function foo {
    //          [CmdletBinding()]param()
    //          if (Test-CallerPreference -Name Debug) { "will show debug info" }
    //          else { "will not show debug info" }
    //      }
    //      foo -Debug # -debug switch used!
    //
    //      DESCRIPTION
    //      -----------
    //      To use this command, your function needs to be decorated with the `CmdletBinding` attribute.
    //#>
    [Cmdlet(VerbsDiagnostic.Test, "CallerPreference", 
        HelpUri = "http://docs.lizoc.com/powerextend/test-callerpreference"
    )]
    [OutputType(typeof(bool))]
    public class TestCallerPreferenceCommand : PSCmdlet
    {
        private string _preferenceName;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [ValidateSet(new string[] { 
            "Debug", "Verbose",
            "Confirm", "WhatIf",
            "Error", "Warning", "Information" //,
            //"Progress"
        })]
        [Alias(new string[] { "Name", "PreferenceName" })]
        public string ParameterName
        {
            get { return _preferenceName; }
            set { _preferenceName = value; }
        }

        protected override void ProcessRecord()
        {
            bool testResult = false;

            switch (_preferenceName)
            {
                case "Confirm":
                    testResult = (ConfirmImpact)base.SessionState.PSVariable.GetValue("ConfirmPreference") != ConfirmImpact.None;
                    break;

                case "WhatIf":
                    try
                    {
                        testResult = ((SwitchParameter)base.SessionState.PSVariable.GetValue("WhatIfPreference")).ToBool();
                    }
                    catch {} // InvalidCastException if -WhatIf is not specified. Default if no -WhatIf => WhatIfPreference = false
                    break;

                case "Debug":
                    testResult = (ActionPreference)base.SessionState.PSVariable.GetValue("DebugPreference") == ActionPreference.Inquire;
                    break;

                case "Verbose":
                    testResult = (ActionPreference)base.SessionState.PSVariable.GetValue("VerbosePreference") != ActionPreference.SilentlyContinue;
                    break;

                case "Error":
                    testResult = (ActionPreference)base.SessionState.PSVariable.GetValue("ErrorActionPreference") != ActionPreference.SilentlyContinue;
                    break;

                case "Warning":
                    testResult = (ActionPreference)base.SessionState.PSVariable.GetValue("WarningPreference") != ActionPreference.SilentlyContinue;
                    break;

                case "Information":
                    testResult = (ActionPreference)base.SessionState.PSVariable.GetValue("InformationPreference") != ActionPreference.SilentlyContinue;
                    break;

                default:
                    throw new ArgumentException(string.Format("Bad validate set (_preferenceName): {0}", _preferenceName));
            }

            base.WriteObject(testResult);
        }
    }
}
