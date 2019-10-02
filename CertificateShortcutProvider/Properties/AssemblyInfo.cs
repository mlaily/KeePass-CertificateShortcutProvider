using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Certificate Shortcut Provider")]
[assembly: AssemblyDescription("Provide a way to open the KeePass key-chain with an X509 certificate.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Melvyn Laïly")]
[assembly: AssemblyProduct("KeePass Plugin")]
[assembly: AssemblyCopyright("Copyright © Melvyn Laïly 2019")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("3a7a0ecd-58e1-4fa2-97d3-0ddc6ce32724")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion(Version.Number)]
[assembly: AssemblyFileVersion(Version.Number)]

public static class Version
{
    public const string Number = "1.2.0.0";
}
