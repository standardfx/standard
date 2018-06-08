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
    public abstract class PathInfoCommandBase : PSCmdlet
    {
        protected Dictionary<string, object> _pathInfoDic;
        protected char[] _pathSeparatorChars = new char[] 
        { 
            System.IO.Path.DirectorySeparatorChar, 
            System.IO.Path.AltDirectorySeparatorChar 
        };

        protected Dictionary<string, object> UncPathHandling(string path)
        {
            string providerName = "FileSystem";
            bool isProviderQualified = false;

            // ---
            // UNC prefix
            // ---            
            string uncPath = null;

            List<string> uncPathPrefixes = new List<string>();
            foreach (char pathSepChar in _pathSeparatorChars)
            {
                uncPathPrefixes.Add(pathSepChar.ToString() + pathSepChar.ToString());
            }

            foreach (string uncPrefix in uncPathPrefixes)
            {
                if (path.ToUpperInvariant().StartsWith("FILESYSTEM::" + uncPrefix))
                {
                    uncPath = path.Substring("FILESYSTEM::".Length);
                    isProviderQualified = true;
                    break;
                }
            }

            if (uncPath == null)
            {
                uncPath = path;
                isProviderQualified = false;
            }


            // ---
            // Leaf or container
            // ---

            bool isContainer = false;
            bool isLeaf = false;
            bool assumeLeaf = false;
            bool assumeContainer = false;

            // if like "xxx\" => directory
            foreach (char pathSepChar in _pathSeparatorChars)
            {
                if (uncPath.EndsWith(pathSepChar.ToString()))
                {
                    isContainer = true;
                    assumeContainer = true;
                    break;
                }
            }

            System.IO.DirectoryInfo dirInfo = null;
            System.IO.FileInfo fileInfo = null;
            try
            {
                dirInfo = new System.IO.DirectoryInfo(uncPath);

                if (dirInfo.Exists)
                {
                    isContainer = true;
                    assumeContainer = true;
                }
                else
                {
                    if (!isContainer && !string.IsNullOrEmpty(dirInfo.Extension))
                    {
                        dirInfo = null;
                        fileInfo = new System.IO.FileInfo(uncPath);
                        assumeLeaf = true;
                        isLeaf = fileInfo.Exists;
                    }
                }
            }
            catch {}


            // ---
            // computer name
            // ---
            string computerName = null;

            int firstIndexOfPathSep = -1;
            foreach (char pathSepChar in _pathSeparatorChars)
            {
                if (uncPath.Substring(2).IndexOf(pathSepChar) > -1)
                {
                    firstIndexOfPathSep = uncPath.Substring(2).IndexOf(pathSepChar);
                    break;
                }
            }

            if (firstIndexOfPathSep > -1)
                computerName = uncPath.Substring(2).Substring(0, firstIndexOfPathSep);

            // ---
            // Wildcards
            // ---

            bool hasWildcard = false;
            if (uncPath.Contains("*") || uncPath.Contains("?"))
                hasWildcard = true;


            // ---
            // others
            // ---
            bool isPathValid = (dirInfo != null) || (fileInfo != null);
            string driveName = null;
            PSDriveInfo driveObject = null; //#todo
            bool driveExists = false; //#todo
            string parentPath = null;
            string childPath = null;
            Type itemType = null;
            string pathWithoutRoot = null;
            string relativePath = null; // #todo
            string absolutePath = null;
            bool itemExists = false;

            if (isPathValid)
            {
                absolutePath = dirInfo == null ? fileInfo.FullName : dirInfo.FullName;
                itemExists = dirInfo == null ? fileInfo.Exists : dirInfo.Exists;

                foreach (char pathSepChar in _pathSeparatorChars)
                {
                    if (absolutePath.EndsWith(pathSepChar.ToString()))
                    {
                        absolutePath = absolutePath.Substring(0, absolutePath.Length - 1);
                        break;
                    }
                }

                // #todo
                relativePath = absolutePath;

                // populate from DirectoryInfo/FileInfo
                if (dirInfo != null)
                {
                    driveName = dirInfo.Root != null ? dirInfo.Root.Name : null;
                    parentPath = dirInfo.Parent != null ? dirInfo.Parent.FullName : null;
                    childPath = dirInfo.Name;
                    itemType = typeof(System.IO.DirectoryInfo);
                }
                else if (fileInfo != null)
                {
                    driveName = ((fileInfo.Directory != null) && (fileInfo.Directory.Root != null)) ? fileInfo.Directory.Root.Name : null;
                    parentPath = fileInfo.Directory != null ? fileInfo.Directory.FullName : null;
                    childPath = fileInfo.Name;
                    itemType = typeof(System.IO.FileInfo);
                }

                if ((computerName != null) && 
                    (driveName != null) && 
                    (uncPath.Length > ("//".Length + (computerName.Length) + "/".Length + driveName.Length)))
                {
                    pathWithoutRoot = uncPath.Substring("//".Length + computerName.Length + "/".Length + driveName.Length);
                }
            }

            return new Dictionary<string, object>()
            {
                { "Path", path },
                { "IsUNC", true },
                { "IsValid", isPathValid },
                { "FileInfo", fileInfo },
                { "DirectoryInfo", dirInfo },
                { "ComputerName", computerName },
                { "ItemType", itemType },
                { "IsLiteralPath", !hasWildcard },
                { "UnqualifiedPath", pathWithoutRoot },

                { "IsProviderQualified", isProviderQualified },
                { "ProviderName", providerName },
                { "ProviderExists", true },
                { "Provider", base.SessionState.Provider.GetOne("FileSystem") },
                { "ProviderQualifedPath", "FileSystem::" + absolutePath },

                { "IsDriveQualified", false },
                { "DriveName", driveName },
                { "Drive", driveObject },
                { "DriveExists", driveExists },
                { "DriveQualifiedPath", absolutePath },
                
                { "IsContainer", isContainer },
                { "IsLeaf", isLeaf },
                { "AssumeContainer", assumeContainer },
                { "AssumeLeaf", assumeLeaf },
                { "Exists", itemExists },

                { "IsRelativePath", string.Compare(absolutePath, uncPath, StringComparison.OrdinalIgnoreCase) != 0 },
                { "IsAbsolutePath", string.Compare(absolutePath, uncPath, StringComparison.OrdinalIgnoreCase) == 0 },
                { "AbsolutePath", absolutePath },
                { "RelativePath", relativePath },

                { "ParentPath", parentPath },
                { "ChildPath", childPath }
            };         
        }

        protected Dictionary<string, object> FileSystemPathHandling()
        {
            string absolutePath = null;
            bool isLeaf = false;
            bool isContainer = false;

            if (_pathInfoDic.ContainsKey("AbsolutePath"))
                absolutePath = (string)_pathInfoDic["AbsolutePath"];

            if (_pathInfoDic.ContainsKey("IsLeaf"))
                isLeaf = (bool)_pathInfoDic["IsLeaf"];
            
            if (_pathInfoDic.ContainsKey("IsContainer"))
                isContainer = (bool)_pathInfoDic["IsContainer"];

            Dictionary<string, object> retResult = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(absolutePath) && isLeaf)
            {
                retResult.Add("FileName", System.IO.Path.GetFileName(absolutePath));
                retResult.Add("FileBaseName", System.IO.Path.GetFileNameWithoutExtension(absolutePath));
                retResult.Add("HasExtension", System.IO.Path.HasExtension(absolutePath));
                retResult.Add("Extension", System.IO.Path.GetExtension(absolutePath));
                retResult.Add("FileInfo", new System.IO.FileInfo(absolutePath));
                retResult.Add("ItemType", typeof(System.IO.FileInfo));
            }

            if (isContainer)
            {
                retResult.Add("DirectoryInfo", new System.IO.DirectoryInfo(absolutePath));
                retResult.Add("ItemType", typeof(System.IO.DirectoryInfo));
            }

            return retResult;
        }

        protected Dictionary<string, object> RegistryPathHandling()
        {
#if NETSTANDARD2
            throw new NotSupportedException();
#else
            Dictionary<string, object> retResult = new Dictionary<string, object>();

            if (_pathInfoDic.ContainsKey("IsContainer"))
                _pathInfoDic["IsContainer"] = true;

            retResult.Add("ItemType", typeof(Microsoft.Win32.RegistryKey));

            return retResult;
#endif
        }

        protected Dictionary<string, object> GenericPathHandling(string path, string relativeTo)
        {
			string rootedPath = null;

			// ---
            // Current Location
            // ---

            PathInfo currentLocation = base.SessionState.Path.CurrentLocation;
            string currentLocationPath = currentLocation.Path;
            PSDriveInfo currentLocationDrive = currentLocation.Drive;
            

            // ---
            // Validity
            // ---

            bool isPathValid = false;
            try
            {
                isPathValid = base.SessionState.Path.IsValid(path);
            }
            catch
            {
                // IsValid throws if provider not found
            }


            // ---
            // Provider
            // ---

            bool isProviderQualified = base.SessionState.Path.IsProviderQualified(path);

            string pathWithoutProviderQualifier = null;
            string providerName = null;
            bool providerExists = false;
            ProviderInfo providerObject = null;
			string providerQualifiedPath = null;

            if (isProviderQualified)
            {
                int indexOfProviderQualifierSeparator = path.IndexOf("::", StringComparison.CurrentCulture);
                if (indexOfProviderQualifierSeparator == -1)
                {
                    // this shouldn't happen
                    throw new FormatException("Internal failure: IsProviderQualified(__PATH__)");
                }

                pathWithoutProviderQualifier = path.Substring(indexOfProviderQualifierSeparator + 2);
                providerName = path.Substring(0, indexOfProviderQualifierSeparator);
                IEnumerable<ProviderInfo> psProvidersList = base.SessionState.Provider.GetAll();

                Dictionary<string, ProviderInfo> availablePSProviders = new Dictionary<string, ProviderInfo>();
                foreach (ProviderInfo psProvider in psProvidersList)
                {
                    availablePSProviders.Add(psProvider.Name, psProvider);
                }

                foreach (string pName in availablePSProviders.Keys)
                {
                    if (string.Compare(pName, providerName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        providerExists = true;
                        providerObject = availablePSProviders[pName];
                        providerName = providerObject.Name;
                        break;
                    }
                }
            }
            else
            {
                // figure out provider name later when analyzing driveName
                pathWithoutProviderQualifier = path;
            }


            // ---
            // Drive
            // ---

            string driveName = null;
            PSDriveInfo driveObject = null;
            bool driveExists = false;
            bool isDriveQualified = base.SessionState.Path.IsPSAbsolute(pathWithoutProviderQualifier, out driveName);
            string pathWithoutRoot = null;
			
            if (isDriveQualified && !string.IsNullOrEmpty(driveName))
            {
                Collection<PSDriveInfo> allPSDrivesList = base.SessionState.Drive.GetAll();
                Dictionary<string, PSDriveInfo> allPSDrives = new Dictionary<string, PSDriveInfo>();
                foreach (PSDriveInfo psDrive in allPSDrivesList)
                {
                    allPSDrives.Add(psDrive.Name, psDrive);
                }

                foreach (string psDriveName in allPSDrives.Keys)
                {
                    if (string.Compare(psDriveName, driveName, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        driveExists = true;
                        driveObject = allPSDrives[psDriveName];
                        driveName = psDriveName;

                        if (!isProviderQualified)
                        {
                            providerObject = driveObject.Provider;
                            providerExists = true;
                            providerName = providerObject.Name;
                        }

                        break;
                    }
                }

                // E:\ means root of E:
                // E: means current location of E:
                pathWithoutRoot = pathWithoutProviderQualifier.Substring(driveName.Length + 1);

                if (driveExists && string.IsNullOrEmpty(pathWithoutRoot) && !string.IsNullOrEmpty(driveObject.CurrentLocation))
                    pathWithoutRoot = driveObject.CurrentLocation;

                rootedPath = base.SessionState.Path.Combine(driveName + ":", pathWithoutRoot);
            }
            else
            {
                // this shouldn't happen
                if (string.IsNullOrEmpty(driveName))
                    throw new FormatException("Internal failure: IsPSAbsolute(__PATH__, __DRIVE_NAME__)");

                driveObject = currentLocationDrive;
                driveName = driveObject.Name;
                driveExists = true;

                if (!isProviderQualified)
                {
                    providerObject = driveObject.Provider;
                    providerExists = true;
                    providerName = providerObject.Name;
                }

                foreach (char pathSepChar in _pathSeparatorChars)
                {
                    if (pathWithoutProviderQualifier.StartsWith(pathSepChar.ToString()))
                    {
                        rootedPath = driveName + ":\\";
                        pathWithoutRoot = "\\";
                        break;
                    }
                }

                if (string.IsNullOrEmpty(rootedPath))
                {
                    rootedPath = base.SessionState.Path.Combine(currentLocationPath, pathWithoutProviderQualifier);
                    pathWithoutRoot = rootedPath.Substring(driveName.Length + 1);
                }
            }

            // trim off trailing path separator
            // except if path is a drive

            bool isContainer = false;
            bool isLeaf = false;

            if (rootedPath.Length == (driveName.Length + 2))
            {
                isContainer = true;
            }
            else
            {
                foreach (char pathSepChar in _pathSeparatorChars)
                {
                    if (rootedPath.EndsWith(pathSepChar.ToString()))
                    {
                        rootedPath = rootedPath.Substring(0, rootedPath.Length - 1);
                        pathWithoutRoot = pathWithoutRoot.Substring(0, pathWithoutRoot.Length - 1);
                        isContainer = true;
                        break;
                    }
                }

                foreach (char pathSepChar in _pathSeparatorChars)
                {
                    if (rootedPath.EndsWith(pathSepChar.ToString()))
                        throw new FormatException("double path separators");
                }
            }

            if (!string.IsNullOrEmpty(providerName))
                providerQualifiedPath = providerName + "::" + rootedPath;
            else
                providerQualifiedPath = null;


            // ---
            // Absolute path
            // ---
            
            string absolutePath = null;
            try
            {
                // quirky ps behavior!
                // NormalizeRelativePath needs to be provider qualified to work reliably.
                // GetUnresolvedProviderPathFromPSPath is opposite. It won't normalize a relative path if it is provider qualified.

                if (providerQualifiedPath != null)
                    absolutePath = base.SessionState.Path.NormalizeRelativePath(providerQualifiedPath, string.Empty);
                else
                    absolutePath = base.SessionState.Path.NormalizeRelativePath(rootedPath, string.Empty);
            }
            catch
            {
                try
                {
                    // path does not exist?
                    absolutePath = base.SessionState.Path.GetUnresolvedProviderPathFromPSPath(rootedPath);
                }
                catch
                {
                    // cannot find drive?
                    try
                    {
                        if (string.IsNullOrEmpty(pathWithoutRoot))
                        {
                            absolutePath = rootedPath;
                        }
                        else
                        {
                            absolutePath = base.SessionState.Path.GetUnresolvedProviderPathFromPSPath(pathWithoutRoot);

                            if (absolutePath.Contains(":"))
                                absolutePath = driveName + absolutePath.Substring(absolutePath.IndexOf(":"));
                        }
                    }
                    catch
                    {
                        // unable to resolve absolute path
                    }
                }
            }


            // ---
            // Provider qualified path
            // ---

            if (!string.IsNullOrEmpty(providerQualifiedPath) && !string.IsNullOrEmpty(absolutePath))
                providerQualifiedPath = providerName + "::" + absolutePath;


            // ---
            // Relative path
            // ---
            bool isRelativePath = false;
            if (string.Compare(path, driveName + ":", StringComparison.OrdinalIgnoreCase) == 0)
                isRelativePath = true;
            else
                isRelativePath = string.Compare(absolutePath, rootedPath, StringComparison.OrdinalIgnoreCase) == 0 ? false : true;

            
            // ---
            // Parent/child
            // ---

            string childPath;
            string parentPath;
            string parentChildTarget;
            try
            {
                parentChildTarget = string.IsNullOrEmpty(absolutePath) ? rootedPath : absolutePath;
                childPath = base.SessionState.Path.ParseChildName(parentChildTarget);
                if (parentChildTarget.Length <= (driveName.Length + 2))
                    parentPath = string.Empty;
                else
                    parentPath = base.SessionState.Path.ParseParent(parentChildTarget, string.Empty);
            }
            catch
            {
                // drive not found?
                if (string.IsNullOrEmpty(pathWithoutRoot))
                {
                    childPath = driveName + ":\\";
                    parentPath = string.Empty;
                }
                else
                {
                    parentChildTarget = base.SessionState.Path.GetUnresolvedProviderPathFromPSPath(pathWithoutRoot);
                    childPath = base.SessionState.Path.ParseChildName(parentChildTarget);
                    if (parentChildTarget.Length <= childPath.Length)
                    {
                        childPath = driveName + ":\\";
                        parentPath = string.Empty;
                    }
                    else
                    {
                        parentPath = base.SessionState.Path.ParseParent(parentChildTarget, string.Empty);
                        if (parentPath.Contains(":"))
                            parentPath = driveName + parentPath.Substring(parentPath.IndexOf(":"));
                    }
                }
            }


            // ---
            // Exists?
            // ---

            bool itemExists = false;
            if (providerExists && driveExists)
            {
                string testPath;

                if (!string.IsNullOrEmpty(absolutePath))
                    testPath = absolutePath;
                else
                    testPath = providerQualifiedPath;

                itemExists = base.InvokeProvider.Item.Exists(testPath);
                if (itemExists)
                {
                    isContainer = base.InvokeProvider.Item.IsContainer(testPath);
                    isLeaf = !isContainer;
                }
            }


            // ---
            // Relative path
            // ---
            string relativePath = null;
            try
            {
                relativePath = base.SessionState.Path.NormalizeRelativePath(absolutePath, relativeTo);
            }
            catch
            {
                // no absolutepath?
                // wildcards?
                // nodrive?
                relativePath = absolutePath;
            }


            // ---
            // Wildcards
            // ---
            bool hasWildcard = false;
            if (rootedPath.Contains("*") || rootedPath.Contains("?"))
                hasWildcard = true;


            // ---
            // return
            // ---
            return new Dictionary<string, object>()
            {
                { "Path", path },
                { "IsValid", isPathValid },
                { "IsLiteralPath", !hasWildcard },
                { "UnqualifiedPath", pathWithoutRoot },

                { "IsProviderQualified", isProviderQualified },
                { "ProviderName", providerName },
                { "ProviderExists", providerExists },
                { "Provider", providerObject },
                { "ProviderQualifedPath", providerQualifiedPath },

                { "IsDriveQualified", isDriveQualified },
                { "DriveName", driveName },
                { "Drive", driveObject },
                { "DriveExists", driveExists },
                { "DriveQualifiedPath", rootedPath },

                { "IsRelativePath", isRelativePath },
                { "IsAbsolutePath", !isRelativePath },
                { "AbsolutePath", absolutePath },
                { "RelativePath", relativePath },

                { "IsContainer", isContainer },
                { "IsLeaf", isLeaf },
                { "Exists", itemExists },

                { "ParentPath", parentPath },
                { "ChildPath", childPath }
            };
        }
    }
}
