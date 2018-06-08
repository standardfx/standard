using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Lizoc.PowerShell;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    [Cmdlet(VerbsDiagnostic.Test, "WebConnection", 
        DefaultParameterSetName = "__AllParameterSets",
        HelpUri = "http://docs.lizoc.com/powerextend/test-webconnection"
    )]
    [OutputType(typeof(bool))]
    public class TestWebConnectionCommand : PSCmdlet
    {
        private bool _lanMode = false;
        private bool _resolveMode = false;

        [Parameter(Mandatory = true, ParameterSetName = "LanModeSet")]
        [Alias(new string[] { "LAN" })]
        public SwitchParameter LocalNetwork
        {
            get { return _lanMode; }
            set { _lanMode = value; }
        }

        [Parameter(Mandatory = true, ParameterSetName = "ResolveModeSet")]
        public SwitchParameter Resolve
        {
            get { return _resolveMode; }
            set { _resolveMode = value; }
        }

        protected override void ProcessRecord()
        {
            bool isConnected = false;

            if (base.ParameterSetName == "LanModeSet")
            {
                isConnected = System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable();                
            }
            else if (base.ParameterSetName == "ResolveModeSet")
            {
                string testNetConnectionScript = "Test-NetConnection -InformationLevel Quiet -ErrorAction SilentlyContinue";
                Collection<PSObject> psout = PSScriptInvoker.Invoke(testNetConnectionScript);
                if (psout.Count > 0)
                {
                    try
                    {
                        isConnected = bool.Parse(psout[0].ToString());
                    }
                    catch {}
                }
            }
            else
            {
                isConnected = NativeMethods.IsConnectedToInternet();
            }

            base.WriteObject(isConnected);
        }
    }
}
