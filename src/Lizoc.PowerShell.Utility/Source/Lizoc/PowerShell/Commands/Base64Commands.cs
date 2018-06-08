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
    [Cmdlet(VerbsData.ConvertTo, "Base64", 
        HelpUri = "http://docs.lizoc.com/powerextend/convertto-base64"
    )]
    [OutputType(typeof(string))]
    public class ConvertToBase64Command : Cmdlet
    {
        private string inputValue;
        private string prefix;
        private string suffix;
        private bool newLine = true;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string InputObject
        {
            get { return this.inputValue; }
            set { this.inputValue = value; }
        }

        [Parameter()]
        public string Prefix
        {
            get { return this.prefix; }
            set { this.prefix = value; }
        }

        [Parameter()]
        public string Suffix
        {
            get { return this.suffix; }
            set { this.suffix = value; }
        }

        [Parameter()]
        public SwitchParameter NewLine
        {
            get { return this.newLine; }
            set { this.newLine = value; }
        }

        protected override void ProcessRecord()
        {
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(inputValue);
            const int lineLength = 76;

            string outPrefix = string.Empty;
            if (!string.IsNullOrEmpty(Prefix)) 
                outPrefix = Prefix + Environment.NewLine;

            string outSuffix = string.Empty;
            if (!string.IsNullOrEmpty(Suffix)) 
                outSuffix = Environment.NewLine + Suffix;

            if (NewLine == true) 
            {
                // netstandard1.3 doesn't have Base64FormattingOptions
                string output = Convert.ToBase64String(inputBytes);
                StringBuilder result = new StringBuilder();
                int currentPosition = 0;

                while (currentPosition + lineLength < output.Length)
                {
                    result.Append(output.Substring(currentPosition, lineLength)).Append(Environment.NewLine);
                    currentPosition += lineLength;
                }
                
                if (currentPosition < output.Length)
                    result.Append(output.Substring(currentPosition));

                base.WriteObject(prefix + result.ToString() + suffix);
            }
            else
            {
                base.WriteObject(outPrefix + Convert.ToBase64String(inputBytes) + outSuffix);
            }
        }
    }

    [Cmdlet(VerbsData.ConvertFrom, "Base64", 
        HelpUri = "http://docs.lizoc.com/powerextend/convertfrom-base64"
    )]
    [OutputType(typeof(string))]
    public class ConvertFromBase64Command : Cmdlet
    {
        private string inputValue;
        private string prefix;
        private string suffix;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string InputObject
        {
            get { return this.inputValue; }
            set { this.inputValue = value; }
        }

        [Parameter()]
        public string Prefix
        {
            get { return this.prefix; }
            set { this.prefix = value; }
        }

        [Parameter()]
        public string Suffix
        {
            get { return this.suffix; }
            set { this.suffix = value; }
        }

        protected override void ProcessRecord()
        {
            // support trim off prefix
            int encodeStartIndex = 0;
            if (!string.IsNullOrEmpty(Prefix) && inputValue.StartsWith(Prefix))
                encodeStartIndex = Prefix.Length;

            // support trim off suffix
            int encodeLength = inputValue.Length - encodeStartIndex;
            if (!string.IsNullOrEmpty(Suffix) && inputValue.EndsWith(Suffix))
                encodeLength = encodeLength - Suffix.Length;

            // trim off prefix/suffix, remove line break
            base.WriteObject(System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(inputValue.Substring(encodeStartIndex, encodeLength).Replace(Environment.NewLine, string.Empty))));
        }
    }
}
