# Tore.Core
Core utilities and extensions library for C# By İ. Volkan Töre.

Please give a STAR (it's free!) if you find this library useful.

I want to know if this library is used by real people.

Nuget reports many downloads but probably they are bots.


Language: C#.

Nuget package: [Tore.Core](https://www.nuget.org/packages/Tore.Core/)

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)

Dependencies: <br/>
.NET 8 LTS.<br/>
Newtonsoft.Json 13.0.3+

Changes in v8.0.3:

    Upgraded to .NET 8 LTS.
    
    Last .NET 7 version was v.8.0.2.

Changes in v8.0.2: 

    StrLst.cs:
        Corrected hasPair method name into HasPair.

Changes in v8.0.1: 

    Minor corrections on documentation.
    Sys.cs:
        Corrected Sys.Attr<T> method to return type T instead of Attribute.
    Json.cs:
        Added Json.Populate(object obj, string json) method.

Changes in v8.0.0: 

    #### WARNING : 
    Tore.Core has undergone radical changes in version 8.<br/>
    It is not compatible to previous versions! <br/>

    Deprecated Versions are available on Nuget as :
    - Deprecated Tore.Core v5.0.0  for net5.0 .
    - Deprecated Tore.Core v6.0.0+ for net6.0 .
    - Deprecated Tore.Core v7.0.0  for net6.0 (Modularized).
    
    Upgraded to .net 7.0.
    
    Sys.cs: 
    
        * Dbg(),isDebug removed.
        * Exception debug output is removed, and routed to console.
        * ExcInterceptorDelegate type renamed as ExceptionInterceptorDelegate.
        * excInterceptor property is renamed as exceptionInterceptor.
        * Added bool exceptionInfoToConsole property.
        * public HasExcData moved into Extensions.cs as Exception.HasInfo();
        * public GetExcData moved into Extensions.cs as Exception.Info();
        * public ExcDbg     moved into Extensions.cs as Exception.InfoToConsole();

    Com.cs:
        
        * Com is renamed into Client, removed from Tore.Core. 
        * It is now in Tore.Http namespace 
            packaged as Tore.Http.Client.

    Extensions.cs:

        * Added Exception extensions for exception info built by Sys.Exc:
            bool   Exception.HasInfo().
            StrLst Exception.Info().
            void   Exception.InfoToConsole().
            string Exception.InfoToPrettyString().

    Stl.cs: (StrLst.cs).

        * Renamed as StrLst.cs and StrLst class:
            For avoiding confusion with C++ STL library.
          New name is not also StringList or StrList 
            For avoiding confusion with List<string>.
        * Corrected add method.
            When StrLst allows only unique keys, 
            It is not allowed to add unique key, value pairs.
            add()     with uniquePair true and
            addPair() with uniquePair true will throw an exception 
            with 'E_STL_UNIQUE' tag in such cases.
        * Upgraded Append() by using List:AddRange methods. 


## ConfigFile.cs :
Contains static utility methods for simple encrypted configuration file support. 
Configurations are loaded to and saved from <b> public static fields </b> of a class.

## Extensions.cs :
Contains static utility extension methods for string, char, ICollection, List of T.

## Hex.cs :
Contains static utility methods for Hex string conversions.

## Json.cs :
Contains static utility methods for Json conversions.

## StrLst.cs :
Defines the class StrLst which is a string associated object list (key - value) class with tricks.

StrLst provides:
1) Numerically indexed access to keys and objects.
2) Ordering.
3) Translation forward and backward to various formats.
4) Duplicate key support.

* Keys can not be null empty or whitespace.            
* Lists are public in this class intentionally.        
* StrLst also acts as a bridge for,

   - Json, 
   - Objects (public properties), 
   - Static classes (public static fields),
   - IDictionary <string, object> and
   - List KeyValuePair <string, string>.     
 
Has Enumerator and Nested conversion support.           

## Sys.cs :
Defines the static class Sys containing a library of utility methods treated as global functions which is used for managing:
  - Exceptions by Exc().
  - Parameter checking by Chk().
  - Several application information routines.
  - Attributes, reflection and type juggling.  

The best way of using them is by adding: 
```C#
using static Tore.Core.Sys;
```
to the source for filewide or by adding:

```C#
global using static Tore.Core.Sys;
```
to the usings.cs file for projectwide access.

## Utc.cs :
Contains static utility methods for DateTime conversions. 
String, Seconds, Milliseconds return <b>UtcNow</b> values.

CultureInfo is <b>CultureInfo.InvariantCulture.</b>       

## Utf8File.cs :
Contains static utility methods to load and save UTF8 files.

## Xor.cs :
Contains static utility methods for simple xor encryption and decryption.

