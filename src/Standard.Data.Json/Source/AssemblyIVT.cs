using System;
using System.Diagnostics;
using System.Security;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#if DEBUG || NETSTANDARD

[assembly: InternalsVisibleTo(Standard.Data.Json.JsonConvert.JSON_GENERATED_ASSEMBLY_NAME)]

#else

[assembly: InternalsVisibleTo(Standard.Data.Json.JsonConvert.JSON_GENERATED_ASSEMBLY_STRONG_NAME)]

#endif
