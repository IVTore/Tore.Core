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
    
    Sys: 
    
        * Methods of reflection is moved to Reflect.cs into Reflect static class.
        * ExcInterceptorDelegate type renamed as ExceptionInterceptorDelegate.
        * sys.excInterceptor property is renamed as sys.exceptionInterceptor.
        * exception debug output is removed, and routed to Log subsystem.
        * public ExcDbg method modified into private ExcLog.
        * Debug output subsystem is removed.
        * Log output subsystem is added.
        * LogDelegate type is added.
        * LogDelegate sys.logMethod property is added.
        * boolean sys.logToConsole property is added and it defaults to true.
            When an additional logger is connected to logMethod, if that 
            logger outputs to console, setting logToConsole to false will avoid
            console logging duplication.

    Com:
        
        * Com is removed from Tore.Core. 
        * It is now in Tore.Http package as Tore.Http.Client class.
    

Deprecated Versions are available on Nuget as :
  - Deprecated Tore.Core v5.0.0  for net5.0 .
  - Deprecated Tore.Core v6.0.0+ for net6.0 .
  - Deprecated Tore.Core v7.0.0  for net6.0 (Modularized).

Dependencies: <br/>
net7.0
Newtonsoft.Json 13+


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

## Sys.cs :
Defines the static class Sys containing a library of utility methods treated as global functions which is used for managing:
  - Output by Dbg().
  - Exceptions by Exc().
  - Reflection.
  - Type juggling.
  - Attributes. 

The best way of using them is by adding: 
```C#
using static Tore.Core.Sys;
```                            
to the source file.    

Since the nuget package is Release version, to see the debug and exception messages add 
code similar to the ones below :

```C#
        // At some initializer method:

        Sys.dbgInterceptor = DebugInterceptor;
        Sys.excInterceptor = ExceptionInterceptor;

        /**———————————————————————————————————————————————————————————————————————————
          FUNC:  ExceptionInterceptor [static]                              <summary>
          TASK:                                                             <br/>
                 Method to show exception data under certain conditions.    <para/>
          ARGS:                                                             <br/>
                 e      : Exception : The exception intercepted.            <br/>
                 dta    : Stl       : Exception data after processing.      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void ExceptionInterceptor(Exception e, Stl dta) {
            if (isDebug)
                return;
            ExcDbg(dta);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC:  DebugInterceptor [static]                                  <summary>
          TASK:                                                             <br/>
                 Method to route Dbg output.                                <para/>
          ARGS:                                                             <br/>
                 s      : string : Dbg output.                              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void DebugInterceptor(string s) {
            Debug.Write(s);
            
            // Or if preferred :

            // Console.Write(s); 
        }
```

## Utc.cs :
Contains static utility methods for DateTime conversions. 
String, Seconds, Milliseconds return <b>UtcNow</b> values.
CultureInfo is <b>CultureInfo.InvariantCulture.</b>       

## Utf8File.cs :
Contains static utility methods to load and save UTF8 files.

## Xor.cs :
Contains static utility methods for simple xor encryption and decryption.

