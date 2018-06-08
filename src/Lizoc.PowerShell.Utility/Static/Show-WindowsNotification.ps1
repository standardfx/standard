function Show-WindowsNotification
{
    <#
        .SYNOPSIS
            Invokes a system notification.
            
        .DESCRIPTION
            Use this command to show a Windows toast notification.
            
            The Windows toast notification is only available in Windows 8 and up.

        .PARAMETER Style
            The style is used for specifying the toast appearance. The appended number represents how many lines are displayed:
            
            - 01: A single line wrapped around 3 lines
            - 02: 2 lines. The second line may wrap to the third line.
            - 03: 2 lines. The first line may wrap to the second line.
            - 04: 3 lines are displayed without wrapping. 
            
            If you do not specify this parameter, the most appropriate style will be chosen automatically.
            
        .EXAMPLE
            Show-WindowsNotification -Message @('Hello world', 'This is a test', 'from Powershell')
            
            DESCRIPTION
            -----------
            Shows a toast notification in Windows 8 and up. In Windows 10, this notification may not display if the 'Quiet Hour' option is activated under the 'Action Center' feature
            
        .LINK
            https://msdn.microsoft.com/en-us/library/windows/apps/windows.ui.notifications.toasttemplatetype.aspx
    #>
    
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true)]
        [string[]]$Message,
        
        [Parameter(Mandatory = $false)]
        [string]$IconPath,
        
        [Parameter(Mandatory = $false)]
        [ValidateSet('Silent', 'Default', 'IM', 'Mail', 'Reminder', 'SMS', 
            'Alarm', 'Alarm2', 'Alarm3', 'Alarm4', 'Alarm5', 'Alarm6', 'Alarm7', 'Alarm8', 'Alarm9', 'Alarm10',
            'Call', 'Call2', 'Call3', 'Call4', 'Call5', 'Call6', 'Call7', 'Call8', 'Call9', 'Call10')]
        [Alias('Sound')]
        [string]$SoundSchema = 'Default',

        [Parameter(Mandatory = $false)]
        [ValidateSet('ToastImageAndText01', 'ToastImageAndText02', 'ToastImageAndText03', 'ToastImageAndText04', 
            'ToastText01', 'ToastText02', 'ToastText03', 'ToastText04')]
        [Alias('Style')]
        [string]$StyleSchema,
        
        [Parameter(Mandatory = $false)]
        [string]$AppID,
        
        [Parameter(Mandatory = $false)]
        [PSCredential]$Credential
    )
    
    Begin
    {
        function TestAppId
        {
            $appCount = Get-StartApps | where { $_.AppId -eq $AppID } | measure | select -expand Count
            ($appCount -gt 0)
        }
        
        # returns true if running as a different user
        function TestRunningImpersonation
        {
            $processId = ([System.Diagnostics.Process]::GetCurrentProcess()).Id
            $parentProcessId = (Get-CimInstance Win32_Process -Filter "ProcessId = $processId").ParentProcessId

            if ($parentProcessId) { $false }
            else { $true }
        }
        
        $procValue = @()
    }
    
    Process
    {
        $procValue += $Message    
    }   
     
    End
    {        
        # runas handling
        if ((TestRunningImpersonation))
        {
            if ($PSBoundParameters.ContainsKey('Credential'))
            {
                $toastJob = Start-Job -Credential $Credential -ScriptBlock {
                    Param($BoundParameters)
                    
                    Show-WindowsNotification @BoundParameters
                } -ArgumentList ($MyInvocation.BoundParameters | where -FilterScript { $_.Key -ne 'Credential' })
                
                break
            }
            else 
            {
                $errRec = New-ErrorRecord -Message $msg.ToastRequireCredential -Exception InvalidOperationException -ErrorID ToastRequireCredential -ErrorCategory InvalidOperation
                $PSCmdlet.ThrowTerminatingError($errRec)
            }
        }
        
        # break down the lines
        $showLine = @()
        for ($i = 0; $i -lt [Math]::Min($procValue.Count, 3); $i++)
        {
            $showLine += $procValue[$i].Replace([Environment]::NewLine, '')
        }
        
        # auto choose a style depending on whether there is an icon specified and length of text
        if ($PSBoundParameters.ContainsKey('IconPath') -and (-not $PSBoundParameters.ContainsKey('StyleSchema')))
        {
            if ($showLine.Count -le 1) 
            { 
                $StyleSchema = 'ToastImageAndText01' 
            }
            ElseIf ($showLine.Count -eq 2) 
            {
                if ($showLine[0].Length -gt $showLine[1].Length) 
                { 
                    $StyleSchema = 'ToastImageAndText02' 
                }
                else 
                { 
                    $StyleSchema = 'ToastImageAndText03' 
                }
            }
            elseif ($showLine.Count -eq 3) 
            { 
                $StyleSchema = 'ToastImageAndText04' 
            }
        }
        elseif ((-not $PSBoundParameters.ContainsKey('IconPath')) -and (-not $PSBoundParameters.ContainsKey('StyleSchema')))
        {
            if ($showLine.Count -le 1) 
            { 
                $StyleSchema = 'ToastText01' 
            }
            elseif ($showLine.Count -eq 2) 
            {
                if ($showLine[0].Length -gt $showLine[1].Length) 
                { 
                    $StyleSchema = 'ToastText02' 
                }
                else 
                { 
                    $StyleSchema = 'ToastText03' 
                }
            }
            elseif ($showLine.Count -eq 3) 
            { 
                $StyleSchema = 'ToastText04' 
            }
        }
        
        # path check
        if ($PSBoundParameters.ContainsKey('IconPath'))
        {
            $absIconPath = ConvertTo-Path $IconPath -ToLiteral
            if (-not $absIconPath) { break }
            Assert-Path $absIconPath -ItemType File -ErrorVariable assertPathError
            if ($assertPathError) { break }
        }
        
        # choose appid
        if ($PSBoundParameters.ContainsKey('AppID'))
        {
            if (-not (TestAppId)) 
            { 
                $errRec = New-ErrorRecord -Message ($msg.BadShortcutIdentifier -f $AppID) -Exception ItemNotFoundException -ErrorID ShortcutIdentifierNotFound -ErrorCategory ObjectNotFound
                $PSCmdlet.ThrowTerminatingError($errRec)
            }
        }
        else
        {
            $AppID = Get-StartApps | where { $_.AppID -like '*powershell.exe' } | select -expand AppID | select -First 1
            if (-not $AppID)
            {
                $errRec = New-ErrorRecord -Message $msg.PowershellShortcutNotFound -Exception ItemNotFoundException -ErrorID PowershellShortcutNotFound -ErrorCategory ObjectNotFound
                $PSCmdlet.ThrowTerminatingError($errRec)
            }
        }
        
        [Windows.UI.Notifications.ToastNotificationManager, Windows.UI.Notifications, ContentType = WindowsRuntime] | Out-Null
        try
        {
            [Xml]$toastTmpl = ([Windows.UI.Notifications.ToastNotificationManager]::GetTemplateContent([Windows.UI.Notifications.ToastTemplateType]::$StyleSchema)).GetXml()
        }
        catch
        {
            # ToastNotificationManager loading didn't work...
            break
        }

        $textElements = $toastTmpl.GetElementsByTagName('text')
        foreach ($textElem in $textElements)
        {
            if ($textElem.id -eq 1)     { $textElem.AppendChild($toastTmpl.CreateTextNode($showLine[0])) | Out-Null }
            elseif ($textElem.id -eq 2) { $textElem.AppendChild($toastTmpl.CreateTextNode($showLine[1])) | Out-Null }
            elseif ($textElem.id -eq 3) { $textElem.AppendChild($toastTmpl.CreateTextNode($showLine[2])) | Out-Null }
        }   
        
        if ($StyleSchema -like '*Image*')
        {
            $imgElements = $toastTmpl.GetElementsByTagName('image')    
            $imgElements[0].src = "file:///$absIconPath"        
        }
        
        $toastNode = $toastTmpl.SelectSingleNode('/toast')
        if ($SoundSchema -eq 'Silent') 
        { 
            $audioElement = $toastTmpl.CreateElement('audio')
            $audioElement.SetAttribute('silent', 'true')
            $toastNode.AppendChild($audioElement) | Out-Null
        }     
        else
        {
            if ($SoundSchema -ne 'Default')
            {
                $soundEvent = $SoundSchema
                if (($soundEvent -like 'Alarm*') -or ($soundEvent -like 'Call*'))
                {
                    $toastNode.SetAttribute('duration', 'long')
                    $soundEvent = "Looping.$soundEvent"
                }

                $audioElement = $toastTmpl.CreateElement('audio')
                $audioElement.SetAttribute('src', "ms-winsoundevent:Notification.$soundEvent")
                    
                if ($soundEvent -like 'Looping.*')
                {
                    $audioElement.SetAttribute('loop', 'true')
                }
                    
                $toastNode.AppendChild($audioElement) | Out-Null
            }
        }
        
        $toastXml = New-Object -TypeName 'Windows.Data.Xml.Dom.XmlDocument'
        $toastXml.LoadXml($toastTmpl.OuterXml)
            
        $toaster = [Windows.UI.Notifications.ToastNotificationManager]::CreateToastNotifier($AppId)
        $toaster.Show($toastXml)
    }
}
