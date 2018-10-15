using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Linq;
using Standard.Data.Confon;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    [Cmdlet(
        VerbsData.ConvertFrom, "BSD", 
        HelpUri = "http://docs.lizoc.com/ps/convertfrombsd",
        RemotingCapability = RemotingCapability.None
    ), OutputType(typeof(PSObject))]
    public class ConvertFromBsdCommand : Cmdlet
    {
        private List<string> _inputObjectBuffer = new List<string>();
        private string[] _fallback;

        [AllowEmptyString, Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string InputObject { get; set; }

        [Parameter(Mandatory = false)]
        public string[] Fallback 
        { 
            get { return _fallback; }
            set { _fallback = value; }
        }

        protected override void BeginProcessing()
        {
        }

        protected override void ProcessRecord()
        {
            _inputObjectBuffer.Add(this.InputObject);
        }

        protected override void EndProcessing()
        {
            // ignore empty entry
            if (_inputObjectBuffer.Count == 0)
                return;

            // It is not actually easy to write syntaxically wrong Confon.
            // So instead of trying to figure out if it is a list of bsd or 
            // just newlines, let's just join the list to a whole big string.

            ConfonContext context;
            ConfonContext userContext;
            try
            {
                if (_inputObjectBuffer.Count == 1)
                    userContext = ConfonFactory.ParseString(string.Join(Environment.NewLine, _inputObjectBuffer[0]));
                else
                    userContext = ConfonFactory.ParseString(string.Join(Environment.NewLine, _inputObjectBuffer.ToArray()));

                if (_fallback == null || _fallback.Length == 0)
                {
                    context = userContext;
                }
                else if (_fallback.Length == 1)
                {
                    if (string.IsNullOrEmpty(_fallback[0]))
                        context = userContext;
                    else
                        context = userContext.WithFallback(ConfonFactory.ParseString(_fallback[0]));
                }
                else
                {
                    ConfonContext baseContext = null;
                    for (int i = _fallback.Length - 1; i >= 0; i--)
                    {
                        if (string.IsNullOrEmpty(_fallback[i]))
                            continue;

                        if (baseContext == null)
                            baseContext = ConfonFactory.ParseString(_fallback[i]);       
                        else
                            baseContext = baseContext.WithFallback(ConfonFactory.ParseString(_fallback[i]));
                    }

                    if (baseContext == null)
                        context = userContext;
                    else
                        context = userContext.WithFallback(baseContext);
                }

                // Handle empty situation
                if (context.IsEmpty)
                    return;

                ErrorRecord populateError;
                object obj = TransverseConfonRoot(context, out populateError);
                if (populateError != null)
                    base.ThrowTerminatingError(populateError);

                base.WriteObject(obj);
            }
            catch (Exception ex)
            {
                ErrorRecord errorRecord = new ErrorRecord(ex, "BsdConversionFailure", ErrorCategory.ParserError, null);
                base.ThrowTerminatingError(errorRecord);
            }
        }

        private static object TransverseConfonRoot(ConfonContext context, out ErrorRecord error)
        {
            // internal exception catching
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            error = null;

            if (context.IsEmpty)
                return null;

            if (!context.Root.IsObject())
            {
                error = new ErrorRecord(new FormatException(RS.Err_BsdRootNotAnObject), "BsdRootNotAnObject", ErrorCategory.ParserError, null);
                return null;
            }

            return PopulateConfonObject(context.Root, context, null, out error);
        }

        private static object PopulateConfonLeaf(ConfonValue jv, string path, out ErrorRecord error)
        {
            if (jv == null)
                throw new ArgumentNullException(nameof(jv));

            error = null;

            if (!jv.IsString())
                throw new ArgumentException("Internal error: Non-leaf object has entered `PopulateConfonLeaf`.");

            try { return jv.GetBoolean(); }
            catch {}

            try { return jv.GetInt32(); }
            catch {}

            try { return jv.GetInt64(); }
            catch {}

            try { return jv.GetSingle(); }
            catch {}
            
            try { return jv.GetDouble(); }
            catch {}

            try { return jv.GetDecimal(); }
            catch {}

            try { return jv.GetTimeSpan(); }
            catch {}

            try { return jv.GetByteSize(); }
            catch {}

            try
            {
                return jv.GetString();
            }
            catch
            {
                error = new ErrorRecord(new FormatException(string.Format("Err_UnrecognizedLeafValue: {0}", path)), "BadBsdValue", ErrorCategory.ParserError, null);
                return null;
            }
        }

        private static PSObject PopulateConfonObject(ConfonValue jv, ConfonContext context, string path, out ErrorRecord error)
        {
            if (jv == null)
                throw new ArgumentNullException(nameof(jv));

            error = null;

            if (!jv.IsObject())
                throw new ArgumentException("Internal error: Non-container object has entered `PopulateConfonObject`.");

            PSObject psObject = new PSObject();
            ConfonObject confonObject = jv.GetObject();

            foreach (string key in confonObject.Items.Keys)
            {
                ConfonValue child = confonObject.Items[key];

                // escape quotes in key
                string safeKey = key;
                if (key.Contains(".") || key.Contains("[") || key.Contains("]"))
                    safeKey = "'" + key.Replace("'", "\\'") + "'";

                string childPath = (path == null) 
                    ? safeKey 
                    : (path + "." + safeKey);

                if (child.IsEmpty)
                {
                    psObject.Properties.Add(new PSNoteProperty(key, null));
                }
                else if (child.IsString())
                {
                    // populate a leaf
                    psObject.Properties.Add(new PSNoteProperty(key, PopulateConfonLeaf(child, childPath, out error)));
                }
                else if (child.IsObject())
                {
                    // populate an object. recurse!
                    psObject.Properties.Add(new PSNoteProperty(key, PopulateConfonObject(child, context, childPath, out error)));
                }
                else if (child.IsArray())
                {
                    // quote the safeKey

                    psObject.Properties.Add(new PSNoteProperty(key, PopulateConfonArray(child, context, path, safeKey, out error)));
                }
                else
                {
                    error = new ErrorRecord(new NotImplementedException(string.Format("Unable to determine object type at {0}", childPath)), "UnhandledDataType", ErrorCategory.ParserError, null);
                }

                if (error != null)
                    return null;
            }

            return psObject;
        }

        private static object[] PopulateConfonArray(ConfonValue jv, ConfonContext context, string parentPath, string childName, out ErrorRecord error)
        {
            if (jv == null)
                throw new ArgumentNullException(nameof(jv));

            error = null;

            if (!jv.IsArray())
                throw new ArgumentException("Internal error: Non-array object has entered `PopulateConfonObject`.");

            IList<ConfonValue> values = jv.GetArray();
            List<object> results = new List<object>();

            int indexPosition = 0;
            foreach (ConfonValue current in values)
            {
                string indexedItemPath = string.Format("{0}.'{1}[{2}]'", parentPath, childName, indexPosition);

                if (current.IsEmpty)
                {
                    results.Add(null);
                }
                else if (current.IsString())
                {
                    results.Add(PopulateConfonLeaf(current, indexedItemPath, out error));
                }
                else if (current.IsObject())
                {
                    results.Add(PopulateConfonObject(current, context, indexedItemPath, out error));
                }
                else if (current.IsArray())
                {
                    // array in array...
                    results.Add(PopulateConfonArray(current, context, parentPath, indexPosition.ToString(), out error));
                }
                else
                {
                    error = new ErrorRecord(new NotImplementedException(string.Format("Unable to determine object type at {0}", indexedItemPath)), "UnhandledDataType", ErrorCategory.ParserError, null);
                }

                if (error != null)
                    return null;

                indexPosition += 1;
            }

            return results.ToArray();
        }
    }
}
