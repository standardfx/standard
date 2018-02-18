Deprecated
==========
- shared
- packOptions
  - owners

To investigate
==============
- entryPoint
- embedInteropTypes
- preprocess
- packOptions
  - summary
- runtimeOptions
  - applyPatches
  - framework
  - framework
    - version
    - name
- buildOptions
  - additionalArguments
  - languageVersion
  - delaySign
  - compilerName
  - publicSign
- dependencies
  - target 
  - include 
  - exclude 
  - supressParent

Metadata
========
- assemblyName
- assemblyVersion
  - [MSBuild]VersionPrefix
- assemblyVersionSuffix
  - [MSBuild]VersionSuffix
- authors[]
- company
- assemblyTitle
- assemblyDescription
  - [MSBuild]Description
- copyright
- [Optional] userSecretsId
- [Optional] rootNamespace
- [Optional] targetOS[]
  - [MSBuild]RuntimeIdentifiers
- [Object] frameworks -> (keys)
  - [MSBuild]TargetFrameworks
- [Object] dependencies
  - [MSBuild]DotNetCliToolReference -> (? type = 'build-tool')
    - Include -> (key)name
    - Version -> version
  - [MSBuild]NetStandardImplicitPackageVersion -> (? (key)name = 'NETStandard.Library' & type = 'package')
    - #text -> version
  - [MSBuild]RuntimeFrameworkVersion -> (? (key)name = 'Microsoft.NETCore.App' & type = 'package')
    - #text -> version
  - [MSBuild.ItemGroup]ProjectReference -> (? type = 'project')
    - [Optional] Path -> path
  - [MSBuild.ItemGroup]PackageReference -> (? type = 'package', 'dev-package')
    - Include -> (key)name
    - Version -> version
    - [Choice: release, debug] configuration
- [Object] scripts
  - preCompile[] -> Target.Exec
  - postCompile[] -> Target.Exec
  - prePublish[] -> Target.Exec
  - postPublish[] -> Target.Exec
- [Object][Optional] vm
  - [Optional][Bool] serverGarbageCollection
  - [Optional][Bool] concurrentGarbageCollection
  - [Optional][Bool] retainVMGarbageCollection
  - [Optional] threadPoolMinThreads
  - [Optional] threadPoolMaxThreads
- [Object] compiler
  - [Optional][Choice: Library, Exe, WinExe][Default: Library] outputType
  - [Optional] noWarn[]
  - [Optional] constants[]
  - [Optional][Bool][Default: false] treatWarningsAsErrors
  - [Optional][Bool][Default: false] preserveCompilationContext
  - [Optional][Bool][Default: false] xmlDoc
  - [Optional][Bool][Default: false] allowUnsafe
  - [Optional][Choice: full,portable][Default: portable] debugSymbolsType
  - [Optional][Bool][Default: false] optimize
  - [Optional] strongNameKey
    - AssemblyOriginatorKeyFile -> path
    - SignAssembly -> true
    - PublicSign -> true

    <PackageId>{{ $assemblyName | xmltext }}</PackageId>
    <PackageTags>{{ ($package.output.nuget.tags -join ';') | xmltext }}</PackageTags>
    <PackageReleaseNotes>{{ $package.output.nuget.releaseNotesUrl | xmltext }}</PackageReleaseNotes>
    <PackageIconUrl>{{ $package.output.nuget.iconUrl | xmltext }}</PackageIconUrl>
    <PackageProjectUrl>{{ $package.output.nuget.projectUrl | xmltext }}</PackageProjectUrl>
    <PackageLicenseUrl>{{ $package.output.nuget.licenseUrl | xmltext }}</PackageLicenseUrl>
    <PackageRequireLicenseAcceptance>{{ $package.output.nuget.requireLicenseAcceptance | bool }}</PackageRequireLicenseAcceptance>
    <RepositoryType>git</RepositoryType>
    <RepositoryUrl>{{ $package.output.nuget.sourceCodeUrl | xmltext }}</RepositoryUrl>




author = ['author']
#userSecretsId = ''


dependencies {
  'NETStandard.Library' {
    version = ''
  }
  'Microsoft.NETCore.App' {
    version = ''
  }

  'xxx' {
    version = ''

    # debug|release|* (default is *)
    #configuration = 'debug'
 
    # package | dev-package | build-tool
    # gac | dev-gac | 
    type = 'package'
  }

  'yyy' {
    type = 'project'
    #makeFileExtension = ${compiler.makeFileExtension}
    #hintPath = '../yy/yy.'${yyy.makeFilExtension}
  }
}

# [!] Reference runtime id available on ms
#targetOS = []

# [!] All optional
scripts {
  preCompile = []
  postCompile = []
  prePublish = []
  postPublish = []
}

# [!] All optional
vm {
  serverGarbageCollection = true
  concurrentGarbageCollection = true
  retainVMGarbageCollection = true
  threadPoolMinThreads = 4
  threadPoolMaxThreads = 25
}

compiler {
  # [i] Choose: Library, Exe, WinExe
  #outputType = 'Library'

  #noWarn = ['CS0168', 'CS0219']

  #constants = ['TRACE']
  #treatWarningsAsErrors = true
  #preserveCompilationContext = true
  #xmlDoc = false
  #allowUnsafe = false
  debugSymbolsType = 'portable'
  #optimize = false
  #strongNameKey {
  #  path = ${credDir}'/testsign.snk'
  #}

  # [i] all optional. support all other fields under 'compiler'
  # The following is a recommended config
  configuration {
    release {
      xmlDoc = true
      constants = ['TRACE']
      preserveCompilationContext = false
      strongNameKey {
        path = ${credDir}'/testsign.snk'
      }
    }
    debug {
      xmlDoc = false
      constants = ['TRACE', 'DEBUG']
    }
  }
}

package.output.nuget {
  tags = []
  sourceCodeUrl = 'http://www.github.com/...'
  projectUrl = 'http://...'
  licenseUrl = ${nugetPack.projectUrl}'/license'
  releaseNotesUrl = ${nugetPack.projectUrl}'/releasenotes'
  iconUrl = ${nugetPack.projectUrl}'/icon.png'
  #requireLicenseAcceptance = false
}

frameworks {
  'xxx' {
    # fully support all under (root).compiler
    # in addition, the following: noStdLib, cpuArchitecture (processorArchitecture), platform, applicationIcon, targetCompactFramework
    compiler {}

    # does not support 'NETStandard.Library' or 'Microsoft.NETCore.App' (silently ignored)
    # does not support type=build-tool, but support type=gac/dev-gac
    dependencies {}  
  }
}
