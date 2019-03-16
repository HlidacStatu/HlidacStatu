﻿using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Postal")]
[assembly: AssemblyDescription("Create emails from ASP.NET MVC views")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Andrew Davey")]
[assembly: AssemblyProduct("Postal")]
[assembly: AssemblyCopyright("Copyright © Andrew Davey 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("5279c130-0531-4cb5-9fea-39406a93443b")]

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
[assembly: AssemblyVersion("1.0.0.0")]
[assembly: AssemblyFileVersion("1.0.0.0")]

#if !ASPNET5
[assembly: CLSCompliant(true)]
#endif

#if TESTABLE
[assembly: InternalsVisibleTo("Postal.Tests")]
#endif