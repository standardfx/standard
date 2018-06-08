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
    //      Provides detailed information on PowerShell paths.
    //
    //  .REMARKS
    //      Registry paths are only available on PowerShell and PowerShell Core for 
    //      Nano Server. It is not available for Linux platforms.
    //
    //  .DEVDOC
    //      - RelativePath reverts to AbsolutePath when path contains wildcards.
    //#>
    [Cmdlet(VerbsCommon.Get, "PathInfo", 
        HelpUri = "http://docs.lizoc.com/powerextend/get-pathinfo"
    )]
    [OutputType(typeof(PSObject))]
    public class GetPathInfoCommand : PathInfoCommandBase
    {
        private string _path;
        private string _relativeTo;
        private string _relativeToAbsolutePath;

        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true)]
        [Alias(new string[] { "PSPath", "FullName" })]
        public string Path
        {
            get { return _path; }
            set { _path = value; }
        }

        [Parameter(Mandatory = false)]
        public string RelativeTo
        {
            get { return _relativeTo; }
            set { _relativeTo = value; }
        }

        protected override void BeginProcessing()
        {
            if (string.IsNullOrEmpty(_relativeTo))
            {
                _relativeTo = base.SessionState.Path.CurrentLocation.ProviderPath;
            }
            else
            {
                try
                {
                    Dictionary<string, object> relativePathInfo = GenericPathHandling(_relativeTo, base.SessionState.Path.CurrentLocation.ProviderPath);
                    if (relativePathInfo.ContainsKey("AbsolutePath"))
                        _relativeToAbsolutePath = (string)relativePathInfo["AbsolutePath"];
                    else
                        _relativeToAbsolutePath = _relativeTo;
                }
                catch
                {
                    _relativeToAbsolutePath = _relativeTo;                    
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
                _pathInfoDic = GenericPathHandling(_path, _relativeToAbsolutePath);
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
            PSObject psInfoObj = new PSObject();

            foreach(string pathInfoField in _pathInfoDic.Keys)
            {
                if (pathInfoField.StartsWith("Is") || pathInfoField.Contains("Exists") || pathInfoField.Contains("Has") || pathInfoField.StartsWith("Assume"))
                    psInfoObj.Members.Add(new PSNoteProperty(pathInfoField, (bool)_pathInfoDic[pathInfoField]));
                else if (pathInfoField == "Provider")
                    psInfoObj.Members.Add(new PSNoteProperty(pathInfoField, (ProviderInfo)_pathInfoDic[pathInfoField]));
                else if (pathInfoField == "Drive")
                    psInfoObj.Members.Add(new PSNoteProperty(pathInfoField, (PSDriveInfo)_pathInfoDic[pathInfoField]));
                else if (pathInfoField == "ItemType")
                    psInfoObj.Members.Add(new PSNoteProperty(pathInfoField, (Type)_pathInfoDic[pathInfoField]));
                else if (pathInfoField == "FileInfo")
                    psInfoObj.Members.Add(new PSNoteProperty(pathInfoField, (System.IO.FileInfo)_pathInfoDic[pathInfoField]));
                else if (pathInfoField == "DirectoryInfo")
                    psInfoObj.Members.Add(new PSNoteProperty(pathInfoField, (System.IO.DirectoryInfo)_pathInfoDic[pathInfoField]));
                else
                    psInfoObj.Members.Add(new PSNoteProperty(pathInfoField, (string)_pathInfoDic[pathInfoField]));
            }

            base.WriteObject(psInfoObj);
        }
    }
}
