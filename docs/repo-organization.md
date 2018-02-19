Repo Organization
=================
Our repo relies heavily on templates and automation tools/scripts. This means a lot of content in the repo is generated for viewing only. With certain exceptions as explained below, you will either make changes to templates or the template data bindings.

Folder Structure
----------------
Our repos are constructed and maintained using [PowerBuild](https://buildcenter.github.io/powerbuild). For example, a C# repo will have a structure that resembles this:

```
README.md
icon.png
LICENSE.txt
THIRD-PARTY-LICENSE.txt
Build.cmd
build.sh
Docs/
    README.md
    ...
    conceptual/
        ...
    api/
        ...
Source/
    global.bsd
    BuildOrder.ini
    buildnum.ini
    myproject1
        project.bsd
        Embed/
            ...
        Properties
            StringData.csv
        Source/
            Class1.cs
            ...
        Templates/
            mytemplate.pstmpl
            ...
        Resources/
            ...
    myproject1.Tests
        project.bsd
        Embed/
            ...
        Properties
            StringData.csv
        Source/
            Class1.cs
            ...
        Templates/
            mytemplate.pstmpl
            ...
        Resources/
            ...
Tools/
    BuildScript/
        ...
    PowerBuild/
        ...
```



Important paths
---------------
The build process assumed a couple of paths when building .NET projects. You can change them by modifying
the file 'Tools/BuildScript/configs/path.bsd'. Remember to use forward slash `/`, not backslash `\`.

- E:/bin/dotnetcore/1.0.4
  - Location of .NETCore SDK 1.0.4
  - Variable name: `DotNetCoreDir`
- E:/Credentials/builder/EcmaPrivateKey.snk
  - Strong name key (private) for signing projects.
  - Variable name: `CredentialDir`
  - Note: You can modify the folder path, but the filename is hardcoded.
- E:/Credentials/builder/EcmaPublicKey.snk
  - Strong name key (public) for signing projects.
  - Variable name: `CredentialDir`
  - Note: You can modify the folder path, but the filename is hardcoded.
- E:/Credentials/builder/NugetPushApi.json
  - NuGet API key for publishing packages built.
  - Variable name: `CredentialDir`
  - Note: You can modify the folder path, but the filename is hardcoded.
- E:/pkgs/nuget
  - Will download (restore) packages required by projects here.
  - Variable name: `WorkingPkgDir`
- E:/pkgs/nuget-local
  - Will copy output packages to this folder (release/publish mode)
  - Variable name: `LocalPackageOutputDir`


*Last updated on 24 May, 2017*
