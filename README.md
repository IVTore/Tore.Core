# Tore.Core
Core utilities and extensions library for C# By İ. Volkan Töre.

Language: C#.

Nuget package: [Tore.Core](https://www.nuget.org/packages/Tore.Core/)

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

#### WARNING : 
Tore.Core has undergone radical changes.<br/>
It is not compatible to previous versions! <br/>

Changes in v7.0.1: 
    
    Upgraded to .net 7.0.
    
    Sys.cs: 
    
        * Debug output subsystem is removed.
        * static readonly Sys.isDebug property removed.
        * Methods of reflection are moved to Reflect static class in Reflect.cs.
        * ExcInterceptorDelegate type renamed as ExceptionInterceptorDelegate.
        * Sys.excInterceptor property is renamed as Sys.exceptionInterceptor.
        * exception debug output is removed, and routed to Log subsystem.
        * public ExcDbg method modified into private ExcLog.
        * Log output subsystem is added.
        * Static ILogger Sys.logger property is added.
        * Static Sys.Log method and overrides added.
        

    Com.cs:
        
        * Com is renamed into Client, removed from Tore.Core. 
        * It is now in Tore.Http namespace 
            packaged as Tore.Http.Client.

    Stl.cs:

        * Renamed as StrLst:
          For avoiding confusion with C++ STL library.
          New name is not StrList to avoid confusion with List<string>.
    

Deprecated Versions are available on Nuget as :
  - Deprecated Tore.Core v5.0.0  for net5.0 .
  - Deprecated Tore.Core v6.0.0+ for net6.0 .
  - Deprecated Tore.Core v7.0.0  for net6.0 (Modularized).

Dependencies: <br/>
net7.0
Newtonsoft.Json 13+

## Sys.cs :
Defines the static class Sys containing a library of utility methods treated as global functions which is used for managing:
  - Logging by Log().
  - Exceptions by Exc().
  - Parameter checking by Chk().
  - Several application information routines.
  

The best way of using them is by adding: 
```C#
using static Tore.Core.Sys;
```                            
to the source file.    

For logging, assign your logger to sys.logger.

```C#
        // At some initializer method:

        Sys.logger = myLogger;
```

## ConfigFile.cs :
Contains static utility methods for simple encrypted configuration file support. 
Configurations are loaded to and saved from <b> public static fields </b> of a class.

## Extensions.cs :
Contains static utility extension methods for string, char, ICollection, List of T.

## Hex.cs :
Contains static utility methods for Hex string conversions.

## Json.cs :
Contains static utility methods for Json conversions.

## Stl.cs :
Defines the class Stl which is a string associated object list (key - value) class with tricks.     
                                                       
Stl provides:                                          
1) Numerically indexed access to keys and objects.      
2) Ordering.                                           
3) Translation forward and backward to various formats.
4) Duplicate key support.

* Keys can not be null empty or whitespace.            
* Lists are public in this class intentionally.        
* Stl also acts as a bridge for,

   - Json, 
   - Objects (public properties), 
   - Static classes (public static fields),
   - IDictionary string key, object value [Alias: IDso] and
   - List KeyValuePair string,string      [Alias: Kvs].     
 
Has Enumerator and Nested conversion support.           
Note : Stl is neither suitable nor built for millions of entries.


## Utc.cs :
Contains static utility methods for DateTime conversions. 
String, Seconds, Milliseconds return <b>UtcNow</b> values.
CultureInfo is <b>CultureInfo.InvariantCulture.</b>       

## Utf8File.cs :
Contains static utility methods to load and save UTF8 files.

## Xor.cs :
Contains static utility methods for simple xor encryption and decryption.

