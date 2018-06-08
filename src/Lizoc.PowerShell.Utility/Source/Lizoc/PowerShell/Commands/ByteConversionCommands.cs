using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using Lizoc.PowerShell;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    [Cmdlet(VerbsData.ConvertFrom, "Bytes", 
        HelpUri = "http://docs.lizoc.com/powerextend/convertfrom-bytes"
    )]
    [OutputType(typeof(string))]
    public class ConvertFromBytesCommand : Cmdlet, IDynamicParameters
    {
        private byte[] inputValue;
        private string encoding = "UTF8";
        private ConvertFromBytesCommandDynamicParameters context;

        [Parameter(Mandatory = true, Position = 0)]
        public byte[] InputObject
        {
            get { return this.inputValue; }
            set { this.inputValue = value; }
        }

        [Parameter(Mandatory = false)]
        [ValidateSet("Base64", "Base16", "Base85", "ASCII", "BigEndianUnicode", "Unicode", "UTF32", "UTF7", "UTF8")]
        public string Encoding
        {
            get { return this.encoding; }
            set { this.encoding = value; }
        }

        // Implement GetDynamicParameters to retrieve the dynamic parameter.
        public object GetDynamicParameters()
        {
            if (encoding != null && (encoding.ToUpperInvariant() == "BASE64" || encoding.ToUpperInvariant() == "BASE16" || encoding.ToUpperInvariant() == "BASE85"))
            {
                context = new ConvertFromBytesCommandDynamicParameters();
                return context;
            }
            return null;
        }

        protected override void ProcessRecord()
        {
            if (encoding.ToUpperInvariant() == "BASE85")
            {
                const int lineLength = 76;

                if (context.Prefix != null || context.Suffix != null)
                {
                    if (context.NewLine == true)
                    {
                        base.WriteObject(Standard.FastConvert.ToBase85String(
                            inputValue, lineLength, 
                            (context.Prefix == null ? string.Empty : context.Prefix), 
                            (context.Suffix == null ? string.Empty : context.Suffix)));
                    }
                    else
                    {
                        base.WriteObject(Standard.FastConvert.ToBase85String(inputValue, 
                            context.Prefix == null ? string.Empty : context.Prefix, 
                            context.Suffix == null ? string.Empty : context.Suffix));
                    }
                }
                else
                {
                    if (context.NewLine == true)
                        base.WriteObject(Standard.FastConvert.ToBase85String(inputValue, lineLength));
                    else
                        base.WriteObject(Standard.FastConvert.ToBase85String(inputValue));                    
                }
            }
            else if (encoding.ToUpperInvariant() == "BASE64")
            {
                string prefix = string.Empty;
                if (!string.IsNullOrEmpty(context.Prefix)) 
                    prefix = context.Prefix + Environment.NewLine;

                string suffix = string.Empty;
                if (!string.IsNullOrEmpty(context.Suffix)) 
                    suffix = Environment.NewLine + context.Suffix;

                if (context.NewLine == true) 
                {
                    // netstandard1.3 doesn't have Base64FormattingOptions
                    string output = Convert.ToBase64String(inputValue);
                    StringBuilder result = new StringBuilder();
                    int currentPosition = 0;
                    int interval = 76;
                    while (currentPosition + interval < output.Length)
                    {
                        result.Append(output.Substring(currentPosition, interval)).Append(Environment.NewLine);
                        currentPosition += interval;
                    }
                    if (currentPosition < output.Length)
                        result.Append(output.Substring(currentPosition));

                    base.WriteObject(prefix + result.ToString() + suffix);
                }
                else
                {
                    base.WriteObject(prefix + Convert.ToBase64String(inputValue) + suffix);
                }
            }
            else if (encoding.ToUpperInvariant() == "BASE16")
            {
                StringBuilder sb = new StringBuilder();

                // add prefix
                if (!string.IsNullOrEmpty(context.Prefix))
                    sb.Append(context.Prefix + Environment.NewLine);

                // content
                for (int i = 0; i < inputValue.Length; i++)
                {
                    // convert each byte in input to base16 (lower case)
                    sb.Append(inputValue[i].ToString("x2", CultureInfo.InvariantCulture));
                    if (context.NewLine == true)
                    {
                        // break after every 76 position (each base16 char takes 2 spaces, so 76/2)
                        if (((i + 1) % 38) == 0)
                            sb.Append(Environment.NewLine);                       
                    }
                }

                // add suffix
                if (!string.IsNullOrEmpty(context.Suffix))
                {
                    // add a line break if sb doesn't already ends with it
                    if ((inputValue.Length % 76) != 0)
                        sb.Append(Environment.NewLine);

                    // now add the suffix
                    sb.Append(context.Suffix);
                }

                base.WriteObject(sb.ToString());
            }
            else
            {
                string output;

                if (encoding.ToUpperInvariant() == "ASCII")
                    output = System.Text.Encoding.ASCII.GetString(inputValue);
                else if (encoding.ToUpperInvariant() == "BIGENDIANUNICODE")
                    output = System.Text.Encoding.BigEndianUnicode.GetString(inputValue);
                else if (encoding.ToUpperInvariant() == "UNICODE")
                    output = System.Text.Encoding.Unicode.GetString(inputValue);
                else if (encoding.ToUpperInvariant() == "UTF7")
                    output = System.Text.Encoding.UTF7.GetString(inputValue);
                else if (encoding.ToUpperInvariant() == "UTF8")
                    output = System.Text.Encoding.UTF8.GetString(inputValue);
                else if (encoding.ToUpperInvariant() == "UTF32")
                    output = System.Text.Encoding.UTF32.GetString(inputValue);
                else 
                { 
                    // throw
                    throw new ArgumentException(string.Format(RS.UnsupportedEncoding, encoding), "Encoding");
                }

                base.WriteObject(output);
            }
        }
    }

    [Cmdlet(VerbsData.ConvertTo, "Bytes", 
        HelpUri = "http://docs.lizoc.com/powerextend/convertto-bytes"
    ), OutputType(typeof(byte[]))]
    public class ConvertToBytesCommand : Cmdlet, IDynamicParameters
    {
        private string inputValue;
        private string encoding = "UTF8";
        private ConvertBytesCommandDynamicParameters context;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string InputObject
        {
            get { return this.inputValue; }
            set { this.inputValue = value; }
        }

        [Parameter(Mandatory = false)]
        [ValidateSet("Base64", "Base16", "Base85", "ASCII", "BigEndianUnicode", "Unicode", "UTF32", "UTF7", "UTF8")]
        public string Encoding
        {
            get { return this.encoding; }
            set { this.encoding = value; }
        }

        // Implement GetDynamicParameters to retrieve the dynamic parameter.
        public object GetDynamicParameters()
        {
            if (encoding != null && (encoding.ToUpperInvariant() == "BASE64" || encoding.ToUpperInvariant() == "BASE16" || encoding.ToUpperInvariant() == "BASE85"))
            {
                context = new ConvertBytesCommandDynamicParameters();
                return context;
            }

            return null;
        }

        protected override void ProcessRecord()
        {
            if (encoding.ToUpperInvariant() == "BASE85")
            {
                if ((context.Prefix != null) || (context.Suffix != null))
                {
                    base.WriteObject(Standard.FastConvert.FromBase85String(inputValue, 
                        (context.Prefix == null ? string.Empty : context.Prefix), 
                        (context.Suffix == null ? string.Empty : context.Suffix)));
                }
                else
                {
                    base.WriteObject(Standard.FastConvert.FromBase85String(inputValue));                    
                }
            }
            else if (encoding.ToUpperInvariant() == "BASE64")
            {
                // support trim off prefix
                int encodeStartIndex = 0;
                if (context.Prefix != null && inputValue.StartsWith(context.Prefix))
                    encodeStartIndex = context.Prefix.Length;

                // support trim off suffix
                int encodeLength = inputValue.Length - encodeStartIndex;
                if (context.Suffix != null && inputValue.EndsWith(context.Suffix))
                    encodeLength = encodeLength - context.Suffix.Length;

                // trim off prefix/suffix, remove line break
                base.WriteObject(Convert.FromBase64String(
                    inputValue.Substring(encodeStartIndex, encodeLength).Replace(Environment.NewLine, string.Empty)
                ));
            }
            else if (encoding.ToUpperInvariant() == "BASE16")
            {
                // support trim off prefix
                int encodeStartIndex = 0;
                if (context.Prefix != null && inputValue.StartsWith(context.Prefix))
                    encodeStartIndex = context.Prefix.Length;

                // support trim off suffix
                int encodeLength = inputValue.Length - encodeStartIndex;
                if (context.Suffix != null && inputValue.EndsWith(context.Suffix))
                    encodeLength = encodeLength - context.Suffix.Length;

                // trim off prefix/suffix, remove line break, **capitalize**
                string adjustedInput = inputValue.Substring(encodeStartIndex, encodeLength).Replace(Environment.NewLine, string.Empty).ToUpperInvariant();

                // validate characters
                Match match = Regex.Match(adjustedInput, "[^0-9A-F]");
                if (match.Success)
                    throw new InvalidDataException(RS.InvalidBase16Chars);

                // validate length
                if ((adjustedInput.Length % 2) != 0)
                    throw new InvalidDataException(RS.InvalidBase16Length);

                int length = adjustedInput.Length / 2;
                try
                {
                    byte[] buffer = new byte[length];
                    for (int i = 0; i < length; i++)
                    {
                        buffer[i] = byte.Parse(adjustedInput.Substring(i * 2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
                    }

                    base.WriteObject(buffer);
                }
                catch
                {
                    throw new OutOfMemoryException(string.Format(RS.BufferAllocFailure, length.ToString()));
                }
            }
            else
            {
                byte[] output;

                if (encoding.ToUpperInvariant() == "ASCII")
                    output = System.Text.Encoding.ASCII.GetBytes(inputValue);
                else if (encoding.ToUpperInvariant() == "BIGENDIANUNICODE")
                    output = System.Text.Encoding.BigEndianUnicode.GetBytes(inputValue);
                else if (encoding.ToUpperInvariant() == "UNICODE")
                    output = System.Text.Encoding.Unicode.GetBytes(inputValue);
                else if (encoding.ToUpperInvariant() == "UTF7")
                    output = System.Text.Encoding.UTF7.GetBytes(inputValue);
                else if (encoding.ToUpperInvariant() == "UTF8")
                    output = System.Text.Encoding.UTF8.GetBytes(inputValue);
                else if (encoding.ToUpperInvariant() == "UTF32")
                    output = System.Text.Encoding.UTF32.GetBytes(inputValue);
                else 
                { 
                    // this shouldn't happen
                    throw new ArgumentException(string.Format(RS.UnsupportedEncoding, encoding), "Encoding");
                }

                base.WriteObject(output);
            }
        }
    }

    // Define the dynamic parameters to be added
    public class ConvertBytesCommandDynamicParameters 
    {
        private string prefix;
        private string suffix;

        [Parameter]
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        [Parameter]
        public string Suffix
        {
            get { return suffix; }
            set { suffix = value; }
        }
    }

    // Define the dynamic parameters to be added
    // ConvertFrom-Bytes has an addition -NewLine dynam parameter
    public class ConvertFromBytesCommandDynamicParameters : ConvertBytesCommandDynamicParameters
    {
        private bool newLine;

        [Parameter]
        public SwitchParameter NewLine
        {
            get { return newLine; }
            set { newLine = value; }
        }
    }
}
