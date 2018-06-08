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
    [Cmdlet(VerbsCommon.Get, "SpecialFolder", 
        HelpUri = "http://docs.lizoc.com/powerextend/get-specialfolder"
    )]
    [OutputType(typeof(PSObject[]))]
    public class GetSpecialFolderCommand : Cmdlet
    {
        // Properties
        private string[] _name = { "*" };

        // Constants
        private readonly string[] SpecialFolderEnumNames = 
        {
            "AddNewPrograms", "AdminTools", "AppUpdates", 
            "CDBurning", "ChangeRemovePrograms", "CommonAdminTools", "CommonOEMLinks", "CommonPrograms", "CommonStartMenu", "CommonStartup", "CommonTemplates", "ComputerFolder", "ConflictFolder", "ConnectionsFolder", "Contacts", "ControlPanelFolder", "Cookies", 
            "Desktop", "Documents", "Downloads", 
            "Favorites", "Fonts", 
            "Games", "GameTasks", 
            "History", 
            "InternetCache", "InternetFolder", 
            "Links", "LocalAppData", "LocalAppDataLow", "LocalizedResourcesDir", 
            "Music", 
            "NetHood", "NetworkFolder", 
            "OriginalImages", 
            "PhotoAlbums", "Pictures", "Playlists", "PrintersFolder", "PrintHood", "Profile", "ProgramData", "ProgramFiles", "ProgramFilesX64", "ProgramFilesX86", "ProgramFilesCommon", "ProgramFilesCommonX64", "ProgramFilesCommonX86", "Programs", "Public", "PublicDesktop", "PublicDocuments", "PublicDownloads", "PublicGameTasks", "PublicMusic", "PublicPictures", "PublicVideos", 
            "QuickLaunch", 
            "Recent", "RecycleBinFolder", "ResourceDir", "RoamingAppData", 
            "SampleMusic", "SamplePictures", "SamplePlaylists", "SampleVideos", "SavedGames", "SavedSearches", "SearchCSC", "SearchMapi", "SearchHome", "SendTo", "SidebarDefaultParts", "SidebarParts", "StartMenu", "Startup", "SyncManagerFolder", "SyncResultsFolder", "SyncSetupFolder", "System", "SystemX86", 
            "Templates", "TreeProperties", 
            "UserProfiles", "UsersFiles", 
            "Videos", 
            "Windows"
        };

        private readonly Dictionary<string, string> SpecialFolderAltNames = new Dictionary<string, string>() 
        {
            // Powershell env:*
            { "windir", "Windows" },
            { "CommonProgramFiles(x86)", "ProgramFilesCommonX86" },
            { "CommonProgramFilesX86", "ProgramFilesCommonX86" },
            { "CommonProgramFiles", "ProgramFilesCommon" },
            { "LocalApplicationData", "LocalAppData" },
            { "ProgramFiles(x86)", "ProgramFilesX86" },
            { "AppData", "RoamingAppData" },
            { "UserProfile", "Profile" },

            // Windows built-in renamed: <legacyName>, <newName>
            { "MyDocuments", "Documents" },
            { "MyMusic", "Music" },
            { "MyVideos", "Videos" },
            { "MyPictures", "Pictures" },
            { "NetworkShortcuts", "NetHood" },
            { "PrinterShortcuts", "Videos" },
            { "CommonDesktopDirectory", "PublicDesktop" },
            { "CommonDocuments", "PublicDocuments" },
            { "CommonPictures", "PublicPictures" },
            { "CommonVideos", "PublicVideos" },
            { "Resources", "ResourceDir" },

            // Extended mapping for consistency
            // Stuff Microsoft kind of forgot...
            { "CommonDownloads", "PublicDownloads" },
            { "CommonGameTasks", "PublicGameTasks" },
            { "PublicAdminTools", "CommonAdminTools" },
            { "PublicOEMLinks", "CommonOEMLinks" },
            { "PublicPrograms", "CommonPrograms" },
            { "PublicStartMenu", "CommonStartMenu" },
            { "PublicTemplates", "CommonTemplates" }
        };

        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true)]
        public string[] Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected override void ProcessRecord()
        {
            // name support wildcards.
            // the default is { "*" }, which will match and return all special folders defined in SpecialFolderEnumNames
            // ps will take care of validating against empty collections and empty/null string

            // loop through each name specified
            foreach (string nameItem in _name)
            {
                // create wildcard pattern for the name item
                WildcardPattern nameItemPattern = new WildcardPattern(
                    nameItem, 
                    WildcardOptions.IgnoreCase | WildcardOptions.Compiled | WildcardOptions.CultureInvariant
                );

                // holds things that match nameItem
                // first elem is the enum name that will be parsed, second is specified if it comes from an alias
                List<string[]> foundMatch = new List<string[]>();

                // deal with alias first
                foreach (string altName in SpecialFolderAltNames.Keys)
                {
                    if (nameItemPattern.IsMatch(altName))
                        foundMatch.Add(new string[] { SpecialFolderAltNames[altName], altName });
                }

                // try match with each enum name
                foreach (string folderName in SpecialFolderEnumNames)
                {
                    if (nameItemPattern.IsMatch(folderName))
                        foundMatch.Add(new string[] { folderName, null });
                }

                // loop through each enum name specified in SpecialFolderEnumNames
                foreach (string[] matchItem in foundMatch)
                {
                    string enumName = matchItem[0];
                    string altName = matchItem[1];
                    bool isAltName = (altName != null);
                    string displayName = isAltName ? altName : enumName;

                    SpecialFolder specialFolder;
                    if (Enum.TryParse(enumName, true, out specialFolder))
                    {
                        PSObject responseObj = new PSObject();
                        responseObj.Members.Add(new PSNoteProperty("Name", displayName));
                        responseObj.Members.Add(new PSNoteProperty("Path", 
                            WindowsUtility.GetSpecialFolder(specialFolder)
                        ));
                        if (isAltName)
                        {
                            responseObj.Members.Add(new PSNoteProperty("IsAlias", true));
                            responseObj.Members.Add(new PSNoteProperty("Alias", enumName));
                        }

                        base.WriteObject(responseObj);
                    }
                    else
                    {
                        // this shouldn't happen. Keep here for debugging purposes.
                        throw new InvalidOperationException(string.Format(RS.InvalidEnumName, enumName));
                    }
                }
            }
        }
    }
}
