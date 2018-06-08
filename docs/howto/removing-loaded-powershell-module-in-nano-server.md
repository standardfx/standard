Removing a loaded PowerShell module in Nano Server
==================================================
I hit an issue today trying to delete files from Nano Server. These are dll files belonging to a PowerShell module I 
loaded for testing.


The Problem
-----------
My gateway device is a x86 computer running Nano Server. 

```powershell
$PSVersionTable

Name                           Value
----                           -----
PSVersion                      5.1.14368.1000
PSEdition                      Core
PSCompatibleVersions           {1.0, 2.0, 3.0, 4.0...}
SerializationVersion           1.1.0.1
PSRemotingProtocolVersion      2.3
WSManStackVersion              3.0
CLRVersion
BuildVersion                   10.0.14368.1000
```

After creating a PowerShell module in C#, I wanted to test it on my Nano Server:

```powershell
$cs = New-PSSession -ComputerName mynanoserver -Credential (Get-Credential)
copy C:\repo\SuperPS\bin\SuperPS\* 'C:\Program Files\WindowsPowerShell\Modules\SuperPS\' -Recurse -ToSession $cs
$cs | Enter-PSSession

# in the session
cd 'C:\Program Files\WindowsPowerShell\Modules\SuperPS\'
ipmo .\SuperPS.dll
Test-MyAwesomeCSCmdlet
Remove-Module SuperPS
del .\SuperPS.dll
del : Cannot remove item C:\Program Files\WindowsPowerShell\Modules\SuperPS\SuperPS.dll: Access
to the path 'C:\Program Files\WindowsPowerShell\Modules\SuperPS\SuperPS.dll' is denied.
    + CategoryInfo          : PermissionDenied: (C:\Program F...uperPS.dll:FileInfo) [Remove-Item], Unau
   thorizedAccessException
    + FullyQualifiedErrorId : RemoveFileSystemItemUnAuthorizedAccess,Microsoft.PowerShell.Commands.RemoveItemCom
   mand
```

Oh dear. I tried renaming the file, exit and re-enter the remote session, close and recreate a new session, etc. No dice! I was 
about to restart my remote computer when inspiration hits! Suppose each time I create a session on Nano Server is like opening a 
new Powershell console, all I have to do is close all the consoles opened.


The Fix
-------
Turns out you don't actually need to restart the remote computer to fix this mess. Just kill any dangling `wsmprovhost` processes:

```powershell
# lets sign in
$cs = New-PSSession -ComputerName mynanoserver -Credential (Get-Credential)
$cs | Enter-PSSession

# let's see the orphaned remoting sessions
Get-Process -Name wsmprovhost -IncludeUserName | select Handle, UserName, StartTime
$p = Get-Process -Name wsmprovhost -IncludeUserName
# Kill all except the most recent 1
$p[1].Kill()

# now you can delete the old locked files
rd 'C:\Program Files\WindowsPowerShell\Modules\SuperPS\' -Recurse -Force

# the most recent 1 is the one currently alive.
# you will disconnect if you kill it.
# close the current session properly like this:
Exit-PSSession
$cs | Remove-PSSession
```


Moral of the story
------------------
Don't just exit the remote session and close your Powershell console. The process `wsmprovhost` is still living on the server, which eats up resource and whatnot.

```powershell
$cs = New-PSSession -ComputerName mynanoserver -Credential (Get-Credential)
$cs | Enter-PSSession
# after you have done your work, 
# in the remote session:
Exit-PSSession
# remove the session on client and server
# this will close wsmprovhost on server
$cs | Remove-PSSession
```

Now you can close your client Powershell console.

Good luck!
