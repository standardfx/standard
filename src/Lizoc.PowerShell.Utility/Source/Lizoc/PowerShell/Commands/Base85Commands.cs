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
    [Cmdlet(VerbsData.ConvertTo, "Base85", 
        HelpUri = "http://docs.lizoc.com/powerextend/convertto-base85"
    )]
    [OutputType(typeof(string))]
    public class ConvertToBase85Command : Cmdlet
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
            const int lineLength = 76;
            byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(inputValue);

            if (string.IsNullOrEmpty(Prefix) && string.IsNullOrEmpty(Suffix))
            {
                if (NewLine == true)
                    base.WriteObject(Standard.FastConvert.ToBase85String(inputBytes, lineLength));
                else
                    base.WriteObject(Standard.FastConvert.ToBase85String(inputBytes));
            }
            else
            {
                if (NewLine == true)
                {
                    base.WriteObject(Standard.FastConvert.ToBase85String(
                        inputBytes, lineLength, 
                        (Prefix == null ? string.Empty : Prefix), 
                        (Suffix == null ? string.Empty : Suffix)));
                }
                else
                {
                    base.WriteObject(Standard.FastConvert.ToBase85String(
                        inputBytes, 
                        (Prefix == null ? string.Empty : Prefix), 
                        (Suffix == null ? string.Empty : Suffix)));
                }
            }
        }
    }

    [Cmdlet(VerbsData.ConvertFrom, "Base85", 
        HelpUri = "http://docs.lizoc.com/powerextend/convertfrom-base85"
    )]
    [OutputType(typeof(string))]
    public class ConvertFromBase85Command : Cmdlet
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
            if ((!string.IsNullOrEmpty(Prefix)) && (!string.IsNullOrEmpty(Suffix)))
            {
                base.WriteObject(System.Text.Encoding.UTF8.GetString(
                    Standard.FastConvert.FromBase85String(inputValue, 
                        (Prefix == null ? string.Empty : Prefix), 
                        (Suffix == null ? string.Empty : Suffix))));
            }
            else
            {
                base.WriteObject(System.Text.Encoding.UTF8.GetString(
                    Standard.FastConvert.FromBase85String(inputValue)));
            }
        }
    }
}
