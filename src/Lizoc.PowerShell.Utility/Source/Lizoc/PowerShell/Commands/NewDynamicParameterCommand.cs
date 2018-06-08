using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Reflection;
using System.Linq;
using Lizoc.PowerShell.Utility;

namespace Lizoc.PowerShell.Commands
{
    [Cmdlet(
        VerbsCommon.New, "DynamicParameter", 
        DefaultParameterSetName = "__AllParameterSets",
        HelpUri = "http://docs.lizoc.com/powerextend/new-dynamicparameter",
        RemotingCapability = RemotingCapability.None
    )]
    [OutputType(typeof(System.Management.Automation.RuntimeDefinedParameterDictionary))]
    public class NewDynamicParameterCommand : PSCmdlet
    {
        private bool _valueFromPipelineByPropertyName = false;
        private bool _valueFromPipeline = false;
        private bool _valueFromRemainingArguments = false;
        private bool _ignoreCase = true;
        private bool _mandatory = false;
        private string _parameterSetName = "__AllParameterSets";
        private int _position;
        private bool _positionSpecified = false;
        private bool _addToExistingPipeline = false;
        private object _defaultValue;
        private bool _defaultValueSpecified = false;
        private bool _allowEmptyCollection = false;
        private bool _allowEmptyString = false;
        private bool _allowNull = false;
        private bool _hidden = false;
        //private bool _validateUserDrive = false;
        //private bool _validateDrive = false;
        private bool _validateNotNullOrEmpty = false;
        private bool _validateNotNull = false;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        public string Name { get; set; }

        [Parameter(Mandatory = true)]
        public Type Type { get; set; }

        [Parameter(Mandatory = false)]
        public object DefaultValue
        { 
            get { return _defaultValue; }
            set 
            { 
                _defaultValue = value; 
                _defaultValueSpecified = true;
            }
        }

        [Parameter(Mandatory = false)]
        public string ParameterSet
        { 
            get { return _parameterSetName; }
            set { _parameterSetName = value; } 
        }

        [Parameter(Mandatory = false)]
        public string[] Alias { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter Mandatory 
        { 
            get { return _mandatory; }
            set { _mandatory = value; }
        }

        [Parameter(Mandatory = false)]
        [ValidateRange(0, Int32.MaxValue)]
        public int Position 
        { 
            get { return _position; } 
            set 
            { 
                _position = value; 
                _positionSpecified = true;
            }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter AllowEmptyCollection 
        { 
            get { return _allowEmptyCollection; }
            set { _allowEmptyCollection = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter AllowEmptyString
        { 
            get { return _allowEmptyString; }
            set { _allowEmptyString = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter AllowNull
        { 
            get { return _allowNull; }
            set { _allowNull = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter Hidden
        { 
            get { return _hidden; }
            set { _hidden = value; }
        }

        //[Parameter(Mandatory = false)]
        //public SwitchParameter ValidateUserDrive
        //{ 
        //    get { return _validateUserDrive; }
        //    set { _validateUserDrive = value; }
        //}

        //[Parameter(Mandatory = false)]
        //public SwitchParameter ValidateDrive
        //{ 
        //    get { return _validateDrive; }
        //    set { _validateDrive = value; }
        //}

        [Parameter(Mandatory = false)]
        public SwitchParameter ValidateNotNullOrEmpty
        { 
            get { return _validateNotNullOrEmpty; }
            set { _validateNotNullOrEmpty = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter ValidateNotNull
        { 
            get { return _validateNotNull; }
            set { _validateNotNull = value; }
        }

        [Parameter(Mandatory = false)]
        public ScriptBlock ValidateScript { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(1, 2)]
        public int[] ValidateRange { get; set; }

        [Parameter(Mandatory = false)]
        [ValidateRange(1, 2)]
        public int[] ValidateLength { get; set; }

        [Parameter(Mandatory = false)]
        public string ValidatePattern { get; set; }

        [Parameter(Mandatory = false)]
        public SwitchParameter ValueFromPipelineByPropertyName 
        { 
            get { return _valueFromPipelineByPropertyName; }
            set { _valueFromPipelineByPropertyName = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter ValueFromPipeline
        { 
            get { return _valueFromPipeline; }
            set { _valueFromPipeline = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter ValueFromRemainingArguments
        { 
            get { return _valueFromRemainingArguments; }
            set { _valueFromRemainingArguments = value; }
        }

        [Parameter(Mandatory = false)]
        public string HelpMessage { get; set; }

        [Parameter(Mandatory = true, ParameterSetName = "Validate")]
        public string[] ValidateSet { get; set; }

        [Parameter(Mandatory = false, ParameterSetName = "Validate")]
        public SwitchParameter IgnoreCase
        { 
            get { return _ignoreCase; }
            set { _ignoreCase = value; }
        }

        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public RuntimeDefinedParameterDictionary ParameterObject { get; set; }

        protected override void ProcessRecord()
        {
            ParameterAttribute paramAttrib = new ParameterAttribute() 
            {
                ParameterSetName = _parameterSetName,
                Mandatory = _mandatory,
                ValueFromPipelineByPropertyName = _valueFromPipelineByPropertyName,
                ValueFromPipeline = _valueFromPipeline,
                ValueFromRemainingArguments = _valueFromRemainingArguments
            };

            if (_positionSpecified == true)
                paramAttrib.Position = _position;

            if (!string.IsNullOrEmpty(HelpMessage))
                paramAttrib.HelpMessage = HelpMessage;

            // add common param attributes and ValidateSet (if any) to a collection
            Collection<Attribute> attribCollection = new Collection<Attribute>();
            attribCollection.Add(paramAttrib);

            // === Attributes ===

            if (ParameterSetName == "Validate")
            {
                ValidateSetAttribute paramValidateSet = new ValidateSetAttribute(ValidateSet);
                attribCollection.Add(paramValidateSet);
            }

            if (Alias != null && Alias.Length != 0)
                attribCollection.Add(new AliasAttribute(Alias));

            if (_allowEmptyCollection == true)
                attribCollection.Add(new AllowEmptyCollectionAttribute());

            if (_allowEmptyString == true)
                attribCollection.Add(new AllowEmptyStringAttribute());

            if (_allowNull == true)
                attribCollection.Add(new AllowNullAttribute());

            if (_hidden == true)
                attribCollection.Add(new HiddenAttribute());

            //if (_validateUserDrive == true)
            //    attribCollection.Add(new ValidateUserDriveAttribute());

            //if (_validateDrive == true)
            //    attribCollection.Add(new ValidateDriveAttribute());

            if (_validateNotNullOrEmpty == true)
                attribCollection.Add(new ValidateNotNullOrEmptyAttribute());

            if (_validateNotNull == true)
                attribCollection.Add(new ValidateNotNullAttribute());

            if (ValidateScript != null)
                attribCollection.Add(new ValidateScriptAttribute(ValidateScript));

            if (!string.IsNullOrEmpty(ValidatePattern))
                attribCollection.Add(new ValidatePatternAttribute(ValidatePattern));

            if (ValidateRange != null)
            {
                if (ValidateRange.Length == 2)
                    attribCollection.Add(new ValidateRangeAttribute(ValidateRange[0], ValidateRange[1]));
                else
                    attribCollection.Add(new ValidateRangeAttribute(ValidateRange[0], Int32.MaxValue));
            }

            if (ValidateLength != null)
            {
                if (ValidateLength.Length == 2)
                    attribCollection.Add(new ValidateLengthAttribute(ValidateLength[0], ValidateLength[1]));
                else
                    attribCollection.Add(new ValidateLengthAttribute(ValidateLength[0], Int32.MaxValue));
            }

            // === /Attributes ===
            
            // add parameter to parameter list
            RuntimeDefinedParameter dynParam = new RuntimeDefinedParameter(Name, Type, attribCollection);
            if (_defaultValueSpecified == true && _mandatory != true)
                dynParam.Value = _defaultValue;

            if (ParameterObject == null)
            {
                ParameterObject = new RuntimeDefinedParameterDictionary();
                _addToExistingPipeline = false;
            }
            else
            {
                _addToExistingPipeline = true;
            }

            ParameterObject.Add(Name, dynParam);
        }

        protected override void EndProcessing()
        {
            if (!_addToExistingPipeline)
                base.WriteObject(ParameterObject);
        }
    }
}
