# Tore.Core
Core utilities and extensions library for C# By İ. Volkan Töre.

Language: C#.

Nuget package: [Tore.Core](https://www.nuget.org/packages/Tore.Core/)

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

#### WARNING : 
Tore.Core v7.0.0+ has undergone radical changes.<br/>
It is not compatible to previous versions! <br/>

Deprecated Versions are available on Nuget as :
  - Deprecated Tore.Core v5.0.0  for net5.0 .
  - Deprecated Tore.Core v6.0.0+ for net6.0 .

Dependencies: <br/>
net6.0
Newtonsoft.Json 13+

## Com.cs :
Defines the class Com which manages Http client requests and responses.

Com gathers sub components required for a proper request and its response into an instance.
Since client requests and responses differ dramatically, 
Com objects give both standard and micro managed request types and response handling.
It maintains and uses a single static HttpClient object as recommended by Microsoft.

For simple standard http requests use STATIC functions 

```C#
   Com.Send(...);
   Com.SendAsync(...);
   Com.Talk<T>(...);
   Com.TalkAsync<T>(...);
```
Otherwise, create a Com object and manipulate the request via

   - The Com instance properties, like: content, accept, mediaType.
   - The Com.request, the HttpRequestMessage property, directly.

Then use INSTANCE send() or sendAsync() routines. 
  
IMPORTANT:
Instance content, accept and mediaType properties are transferred to request just before sending it.
Tricky assignments must be done via;
   - request.Content,
   - request.Content.Headers.Accept,
   - request.Content.Headers.ContentType.MediaType properties,
  
When request.content is non null, content property of instance is ignored.
If accept or mediaType is set through request.Content.Headers
respective Com accept and mediaType properties must be empty.  

Please read the comments on the code at least once for using this class efficiently.

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

