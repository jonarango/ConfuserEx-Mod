using System.Reflection;

[assembly: AssemblyProduct("Trinity")]
[assembly: AssemblyCompany("Ki")]
[assembly: AssemblyCopyright("Copyright (C) Ki 2014")]

#if DEBUG

[assembly: AssemblyConfiguration("Debug")]
#else

[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyVersion("1.0.0")]
[assembly: AssemblyFileVersion("1.0.0")]
[assembly: AssemblyInformationalVersion("v1")]