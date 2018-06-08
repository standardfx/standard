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
    //      Throws an exception when a path does not satisfy the requisites specified.
    //
    //  .DEVDOC
    //      - RelativePath reverts to AbsolutePath when path contains wildcards.
    //#>
    [Cmdlet(VerbsLifecycle.Assert, "Path", 
        HelpUri = "http://docs.lizoc.com/powerextend/assert-path"
    )]
    [OutputType(typeof(void))]
    public class AssertPathCommand : PathInfoCommandBase
    {
        private string _path;
        private string[] _itemType;
        private string[] _providerName;
        private string[] _driveName;

        private Dictionary<string, bool> _switchPredicates = new Dictionary<string, bool>();
        private Dictionary<string, string> _errorReasons = new Dictionary<string, string>()
        {
            // ##DISABLE_AUTO_FORMAT##

            { "PathError", RS.PathError },
            { "MultiErrorReasonBullet",  RS.MultiErrorReasonBullet },
            { "MultiErrorReasonPrefix",  RS.MultiErrorReasonPrefix },
            { "SingleErrorReasonPrefix", RS.SingleErrorReasonPrefix },

            { "ProviderName", RS.ProviderName },
            { "DriveName",    RS.DriveName },
            { "ProviderNameResolutionFailure", RS.ProviderNameResolutionFailure },
            { "DriveNameResolutionFailure",    RS.DriveNameResolutionFailure },
            { "Type", RS.Type },
            { "TypeResolutionFailure", RS.TypeResolutionFailure },

            { "IsUNC_true",      RS.IsUNC_true },
            { "IsUNC_false",     RS.IsUNC_false },
            { "IsValid_true",    RS.IsValid_true },
            { "IsValid_false",   RS.IsValid_false },
            { "IsLiteral_true",  RS.IsLiteral_true },
            { "IsLiteral_false", RS.IsLiteral_false },
            { "IsProviderQualified_true",  RS.IsProviderQualified_true },
            { "IsProviderQualified_false", RS.IsProviderQualified_false },
            { "ProviderExists_true",       RS.ProviderExists_true },
            { "ProviderExists_false",      RS.ProviderExists_false },
            { "IsDriveQualified_true",     RS.IsDriveQualified_true },
            { "IsDriveQualified_false",    RS.IsDriveQualified_false },
            { "DriveExists_true",          RS.DriveExists_true },
            { "DriveExists_false",         RS.DriveExists_false },
            { "IsLeaf_true",       RS.IsLeaf_true },
            { "IsLeaf_false",      RS.IsLeaf_false },
            { "IsContainer_true",  RS.IsContainer_true },
            { "IsContainer_false", RS.IsContainer_false },
            { "Exists_true",       RS.Exists_true },
            { "Exists_false",      RS.Exists_false },
            { "IsRelative_true",   RS.IsRelative_true },
            { "IsRelative_false",  RS.IsRelative_false }

            // ##ENABLE_AUTO_FORMAT##
        };

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Alias(new string[] { "PSPath", "FullName" })]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter IsUNC 
        { 
            get { return _switchPredicates.ContainsKey("IsUNC") ? _switchPredicates["IsUNC"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("IsUNC"))
                    _switchPredicates["IsUNC"] = value;
                else
                    _switchPredicates.Add("IsUNC", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter IsValid
        { 
            get { return _switchPredicates.ContainsKey("IsValid") ? _switchPredicates["IsValid"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("IsValid"))
                    _switchPredicates["IsValid"] = value;
                else
                    _switchPredicates.Add("IsValid", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public string[] ItemType 
        { 
            get { return _itemType; } 
            set { _itemType = value; } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter IsLiteral
        { 
            get { return _switchPredicates.ContainsKey("IsLiteral") ? _switchPredicates["IsLiteral"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("IsLiteral"))
                    _switchPredicates["IsLiteral"] = value;
                else
                    _switchPredicates.Add("IsLiteral", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter IsProviderQualified
        { 
            get { return _switchPredicates.ContainsKey("IsProviderQualified") ? _switchPredicates["IsProviderQualified"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("IsProviderQualified"))
                    _switchPredicates["IsProviderQualified"] = value;
                else
                    _switchPredicates.Add("IsProviderQualified", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public string[] ProviderName 
        { 
            get { return _providerName; }
            set { _providerName = value; } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter ProviderExists 
        { 
            get { return _switchPredicates.ContainsKey("ProviderExists") ? _switchPredicates["ProviderExists"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("ProviderExists"))
                    _switchPredicates["ProviderExists"] = value;
                else
                    _switchPredicates.Add("ProviderExists", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter IsDriveQualified
        { 
            get { return _switchPredicates.ContainsKey("IsDriveQualified") ? _switchPredicates["IsDriveQualified"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("IsDriveQualified"))
                    _switchPredicates["IsDriveQualified"] = value;
                else
                    _switchPredicates.Add("IsDriveQualified", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public string[] DriveName 
        { 
            get { return _driveName; }
            set { _driveName = value; } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter DriveExists
        { 
            get { return _switchPredicates.ContainsKey("DriveExists") ? _switchPredicates["DriveExists"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("DriveExists"))
                    _switchPredicates["DriveExists"] = value;
                else
                    _switchPredicates.Add("DriveExists", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter IsLeaf
        { 
            get { return _switchPredicates.ContainsKey("IsLeaf") ? _switchPredicates["IsLeaf"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("IsLeaf"))
                    _switchPredicates["IsLeaf"] = value;
                else
                    _switchPredicates.Add("IsLeaf", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter IsContainer
        { 
            get { return _switchPredicates.ContainsKey("IsContainer") ? _switchPredicates["IsContainer"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("IsContainer"))
                    _switchPredicates["IsContainer"] = value;
                else
                    _switchPredicates.Add("IsContainer", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter Exists
        { 
            get { return _switchPredicates.ContainsKey("Exists") ? _switchPredicates["Exists"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("Exists"))
                    _switchPredicates["Exists"] = value;
                else
                    _switchPredicates.Add("Exists", value); 
            } 
        }

        [Parameter(Mandatory = false)]
        public SwitchParameter IsRelative
        { 
            get { return _switchPredicates.ContainsKey("IsRelative") ? _switchPredicates["IsRelative"] : false; } 
            set 
            { 
                if (_switchPredicates.ContainsKey("IsRelative"))
                    _switchPredicates["IsRelative"] = value;
                else
                    _switchPredicates.Add("IsRelative", value); 
            } 
        }

        protected override void BeginProcessing()
        {
            // UNC path special handling
            foreach (char pathSepChar in _pathSeparatorChars)
            {
                string uncPathPrefix = pathSepChar.ToString() + pathSepChar.ToString();

                if (_path.StartsWith(uncPathPrefix) || _path.ToUpperInvariant().StartsWith("FILESYSTEM::" + uncPathPrefix))
                {
                    _pathInfoDic = UncPathHandling(_path);
                    return;
                }
            }
        }

        protected override void ProcessRecord()
        {
            // UNC path special handling
            foreach (char pathSepChar in _pathSeparatorChars)
            {
                string uncPathPrefix = pathSepChar.ToString() + pathSepChar.ToString();

                if (_path.StartsWith(uncPathPrefix) || _path.ToUpperInvariant().StartsWith("FILESYSTEM::" + uncPathPrefix))
                {
                    _pathInfoDic = UncPathHandling(_path);
                    return;
                }
            }

            // General handling
            try
            {
                _pathInfoDic = GenericPathHandling(_path, base.SessionState.Path.CurrentLocation.ProviderPath);
            }
            catch
            {
                _pathInfoDic = new Dictionary<string, object>()
                {
                    { "Path", _path }
                };
            }

            // Provider specific handling
            string providerName = string.Empty;
            Dictionary<string, object> extPathInfo = null;

            if (_pathInfoDic.ContainsKey("ProviderName"))
                providerName = (string)_pathInfoDic["ProviderName"];

            switch (providerName)
            {
                case "FileSystem": 
                    extPathInfo = FileSystemPathHandling();
                    break;

                case "Registry": 
                    extPathInfo = RegistryPathHandling();
                    break;

                default:
                    break;
            }

            if (extPathInfo != null)
            {
                foreach (string fieldName in extPathInfo.Keys)
                {
                    _pathInfoDic.Add(fieldName, extPathInfo[fieldName]);
                }
            }
        }

        protected override void EndProcessing()
        {
            bool throwException = false;
            List<string> throwReasons = new List<string>();

            foreach (string key in _switchPredicates.Keys)
            {
                if (_pathInfoDic.ContainsKey(key))
                {
                    if ((bool)_pathInfoDic[key] != _switchPredicates[key])
                    {
                        throwException = true;
                        throwReasons.Add(_errorReasons[key + "_" + _switchPredicates[key]]);
                    }
                }
                else
                {
                    if (_switchPredicates[key] == true)
                    {
                        throwException = true;
                        throwReasons.Add(_errorReasons[key + "_true"]);
                    }
                }
            }

            if (_providerName != null && _providerName.Length > 0)
            {
                if (_pathInfoDic.ContainsKey("ProviderName"))
                {
                    bool providerNameFound = false;
                    foreach (string pn in _providerName)
                    {
                        if (string.Compare((string)_pathInfoDic["ProviderName"], pn, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            providerNameFound = true;
                            break;
                        }
                    }

                    if (providerNameFound == false)
                    {
                        throwException = true;
                        throwReasons.Add(string.Format(_errorReasons["ProviderName"], _pathInfoDic["ProviderName"]));
                    }
                }
                else
                {
                    throwException = true;
                    throwReasons.Add(_errorReasons["ProviderNameResolutionFailure"]);
                }
            }

            if (_driveName != null && _driveName.Length > 0)
            {
                if (_pathInfoDic.ContainsKey("DriveName"))
                {
                    bool driveNameFound = false;
                    foreach (string dn in _driveName)
                    {
                        if (string.Compare((string)_pathInfoDic["DriveName"], dn, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            driveNameFound = true;
                            break;
                        }
                    }

                    if (driveNameFound == false)
                    {
                        throwException = true;
                        throwReasons.Add(string.Format(_errorReasons["DriveName"], _pathInfoDic["DriveName"]));
                    }
                }
                else
                {
                    throwException = true;
                    throwReasons.Add(_errorReasons["DriveNameResolutionFailure"]);
                }
            }

            if (_itemType != null && _itemType.Length > 0)
            {
                if (_pathInfoDic.ContainsKey("ItemType"))
                {
                    string itemTypeName = ((Type)_pathInfoDic["ItemType"]).ToString();
                    bool itemTypeFound = false;

                    foreach (string itn in _itemType)
                    {
                        if (string.Compare(itemTypeName, itn, StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            itemTypeFound = true;
                            break;
                        }
                    }

                    if (itemTypeFound == false)
                    {
                        throwException = true;
                        throwReasons.Add(string.Format(_errorReasons["Type"], itemTypeName));                                      
                    }
                }
                else
                {
                    throwException = true;
                    throwReasons.Add(_errorReasons["TypeResolutionFailure"]);                                      
                }
            }

            if (throwException)
            {
                string allReasons = _errorReasons["PathError"];

                if (throwReasons.Count == 1)
                {
                    allReasons += _errorReasons["SingleErrorReasonPrefix"] + throwReasons[0];
                }
                else
                {
                    allReasons += _errorReasons["MultiErrorReasonPrefix"] + Environment.NewLine;
                    foreach (string reason in throwReasons)
                    {
                        allReasons += _errorReasons["MultiErrorReasonBullet"] + reason + Environment.NewLine;
                    }
                }

                FormatException exObj = new FormatException(allReasons);

                base.WriteError(new ErrorRecord(exObj, "PathAssertionError", ErrorCategory.InvalidData, _path));
            }
        }
    }
}
