Console
-------
[ ] Console.Banner
[ ] Console.Color
[ ] Console.Cursor
[ ] Console.Theme

Security
--------
[x] Crypto.Hash
 [x] Merged with [String.Hash]
[x] String.Hash
 [x] `Get-HashCode`
[ ] Security.Access
[ ] Security.RunAs

[OK] Debug
----------
[x] Debug.ErrorRecord
 [x] `New-ErrorRecord`
[x] Debug.Exception
 [x] Use the `GetBaseException()` method that comes with the `Exception` class.

Time
----
[ ] Time.Stopwatch
[ ] Time.Unix

String
------
[x] String.BinaryEncode
 [x] `ConvertFrom-Bytes`
 [x] `ConvertTo-Bytes`
[ ] String.Compress
[ ] String.Concat
[ ] String.Metric
[ ] String.Random
[ ] String.Sort
[ ] String.Substring
[ ] String.Token

Imaging
-------
[ ] DiskImage.ISO

File
----
[ ] File.Attribute
[ ] File.Compare
[ ] File.Encoding
[ ] File.Merge

Globalization
-------------
[ ] Globalization.Info

Data
----
[ ] Ini.Data
[ ] Xml.Dsl

UI
--
[ ] IPC.Input
[ ] UI.KeyPress
[ ] UI.Prompt

Maths
-----
[x] Math.Checksum
 [x] Deprecated.
[x] Math.Number
 [x] Use `[Standard.MathUtility]::DecimalToBase([int], [int])`
 [x] Use `[Standard.MathUtility]::BaseToDecimal([string], [int])`
[x] Math.Random
 [x] Use `[Standard.RandomUtility]::GetEntropy([byte[]])`
[x] Math.UnitConversion
 [x] `Convert-DataUnit`
[ ] Version.General
 [ ] Type extension [version].Add([version])

.NET
----
[ ] NetFx.Code
[ ] NetFx.Gac
[ ] NetFx.Generics
[ ] NetFx.Type
[ ] NetFx.Version

Path
----
[ ] Path.Container
[x] Path.File
 [x] Merged with `Path.PSPath`
[x] Path.Literal
 [x] Merged with `Path.PSPath`
[x] Path.PSPath
 [x] `Assert-Path`
 [x] `Get-PathInfo`

Powershell
----------
[ ] Command.Async
[ ] Command.Retry
[ ] Item.Join
[ ] Item.Name
[ ] Item.Property
[ ] Item.Size
[ ] Item.Temp
[o] PS.CallerPreference
 [x] `Test-CallerPreference`
 [ ] `Use-CallerPreference`
[ ] PS.Dependency
[x] PS.Drive
 [x] `Test-PSDrive`
 [x] `Test-PSProvider`
[x] PS.DynamicParameter
 [x] `New-DynamicParameter`
[ ] PS.ModuleManifest
[ ] PS.Session
[ ] PS.Token

Web
---
[ ] Web.General
 [x] `Test-WebConnection`

Windows
-------
[x] Windows.Mui
 [x] `Expand-MUIString`
[x] Windows.Login
 [x] `Lock-Computer`
[ ] Windows.Notification
[ ] Windows.Process
 [ ] `Test-Process`
[ ] Windows.ProductInfo
[ ] Windows.Registry
[ ] Windows.ScheduledTasks
[x] Windows.Service
 [x] `Test-Service`
 [ ] `Uninstall-Service`
[ ] Windows.Shell
[ ] Windows.Shortcut
[x] Windows.SpecialFolder
 [x] `Get-SpecialFolder`