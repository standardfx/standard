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
    [Cmdlet(VerbsCommon.Lock, "Computer", 
        HelpUri = "http://docs.lizoc.com/powerextend/lock-computer"
    )]
    [OutputType(typeof(void))]
    public class LockComputerCommand : Cmdlet
    {
        private bool _online;

        [Parameter(Mandatory = true)]
        public SwitchParameter Online
        {
            get { return _online; }
            set { _online = value; }
        }

        private void EnsureOnlineIsNotFalse()
        {
            if (_online == false)
            {                
                base.ThrowTerminatingError(new ErrorRecord(
                    new InvalidOperationException(string.Format(RS.SwitchParamShouldNotBeFalse, "Online")), 
                    "OnlineParamShouldNotBeFalse", ErrorCategory.InvalidOperation, null));
            }
        }

        protected override void ProcessRecord()
        {
            EnsureOnlineIsNotFalse();
            WindowsUtility.LockWorkstation();
        }
    }
}
