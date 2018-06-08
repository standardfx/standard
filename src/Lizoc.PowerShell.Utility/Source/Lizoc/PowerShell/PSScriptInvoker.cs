using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using System.Collections.ObjectModel;
using SMA = System.Management.Automation;

namespace Lizoc.PowerShell
{
    internal static class PSScriptInvoker
    {
        public static Collection<SMA.PSObject> Invoke(string script)
        {
            SMA.PSDataStreams streams;
            return Invoke(script, null, out streams);
        }

        public static Collection<SMA.PSObject> Invoke(string script, Dictionary<string, object> scriptParams)
        {
            SMA.PSDataStreams streams;
            return Invoke(script, scriptParams, out streams);
        }

        public static Collection<SMA.PSObject> Invoke(string script, Dictionary<string, object> scriptParams, out SMA.PSDataStreams streams)
        {
            Collection<SMA.PSObject> psOutput;
            using (SMA.PowerShell psInstance = SMA.PowerShell.Create())
            {
                psInstance.AddScript(script);
                if (scriptParams != null)
                {
                    foreach (string paramName in scriptParams.Keys)
                    {
                        psInstance.AddParameter(paramName, scriptParams[paramName]);
                    }                    
                }

                psOutput = psInstance.Invoke();
                streams = psInstance.Streams;
            }

            return psOutput;
        }
    }
}
