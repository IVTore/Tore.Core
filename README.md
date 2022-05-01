# Tore.Core
Core utilities and extensions library for C# By İ. Volkan Töre.

Language: C#.

Nuget package: [Tore.Core](https://www.nuget.org/packages/Tore.Core/)

<b>WARNING</b>: <br/>
Tore.Core v7.0.0+ has undergone radical changes.<br/>
It is not compatible to previous versions! <br/>
<br/>

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
 ```
——————————————————————————————————————————————————————————————————
 FUNC: Load [static]                                              
 TASK:                                                            
       Loads and decrypts contents of an encrypted file into      
       public static fields of a class, using two keys.  
       If file is not encrypted, it is not decrypted.             
       If xorKey is shorter, it is repeated over encKey.          
 ARGS:                                                            
       type    : Type      : Class with public static fields.     
       file    : string    : File specification.                  
       encKey  : string    : Primary   encryption Key.            
       xorKey  : string    : Secondary encryption Key.            
       strip   : string    : Characters to remove from keys.      
 WARN:                                                            
       Throws exception if anything is null or empty except strip.
——————————————————————————————————————————————————————————————————
```
 
```
——————————————————————————————————————————————————————————————————
 FUNC: Save [static]                                              
 TASK:                                                            
       Encrypts and saves public static fields of a class
       into a file, using two keys.                               
       If keys are empty file is not encrypted.                   
       If xorKey is shorter, it is repeated over encKey.          
 ARGS:                                                            
       type    : Type      : Class with public static fields.     
       file    : string    : File specification.                  
       encKey  : string    : Primary   encryption Key.            
       xorKey  : string    : Secondary encryption Key.            
       strip   : string    : Characters to remove from keys.      
 WARN:                                                            
       Throws exception if anything is null or empty except strip.
——————————————————————————————————————————————————————————————————
```

## Extensions.cs
Contains static utility extension methods for
string, char, ICollection, List of T.


```C#
——————————————————————————————————————————————————————————————————
FUNC: IsNullOrWhiteSpace [static, extension]                   
TASK:                                                          
      Shorthand for String.IsNullOrWhiteSpace.                 
ARGS:                                                          
      str : string : Source string to check.                   
RETV:                                                          
          : bool   : True if string is null or only whitespaces.

——————————————————————————————————————————————————————————————————
```

```C#
——————————————————————————————————————————————————————————————————
FUNC: IsIdentifier [static, extension]                    
TASK:                                                     
      Checks if an identifier name has a valid syntax.    
ARGS:                                                     
      s   : string    : identifier candidate stri         
RETV:     : boolean   : true if valid else false.         
INFO:                                                     
      *   This checks for unicode identifiers for runtime,
          not ASCII only.                                 
      *   The C# keywords are intentionally not checked.  
      *   @ as first character is not supported.          
      *   Optimized for speed.                            
——————————————————————————————————————————————————————————————————
```

```C#
——————————————————————————————————————————————————————————————————
FUNC: RemoveWhiteSpaces [static, extension]                  
TASK:                                                        
      removes all whitespaces from str.                      
ARGS:                                                        
      str : string    : Source string to strip whitespaces.  
RETV:                                                        
          : string    : String stripped of white spaces.     
WARN:                                                        
      Throws E_INV_ARG if string is null.                    
——————————————————————————————————————————————————————————————————
```

```C#
——————————————————————————————————————————————————————————————————
FUNC: WhiteSpacesToSpace [static, extension]                    
TASK:                                                           
      Converts all single or consequtive whitespaces            
      to single space in string str.                            
ARGS:                                                           
      str : string    : String to strip multi whitespaces.      
RETV:                                                           
          : string    : String stripped of multi white spaces.  
INFO:                                                           
      White space characters other than space will be converted 
      to space. Modified from the solution of Felipe Machado.   
WARN:                                                           
      Throws E_INV_ARG if string is null.                       
——————————————————————————————————————————————————————————————————
```

```C#
——————————————————————————————————————————————————————————————————
FUNC: RemoveChars [static, extension].                       
TASK:                                                        
      Removes chars in remove from string.                   
ARGS:                                                        
      str     : string :  String to remove characters from.  
      remove  : string :  String containing characters to 
                          remove from Key.                   
WARN:                                                        
      Throws E_INV_ARG if string is null.                    
——————————————————————————————————————————————————————————————————
```

```C#
——————————————————————————————————————————————————————————————————
——————————————————————————————————————————————————————————————————
```

```C#
——————————————————————————————————————————————————————————————————
——————————————————————————————————————————————————————————————————
```
```C#
——————————————————————————————————————————————————————————————————
——————————————————————————————————————————————————————————————————
```

## Sys.cs :
Defines the static class Sys containing a library of utility methods treated as global functions which is used for managing:
  - Output by dbg().
  - Exceptions by exc().
  - Reflection.
  - Type juggling.
  - Attributes. 
  - Simple File Load Save. 
  - Time, Date.
  
and many others.

The best way of using them is by adding: 
```C#
using static Tore.Core.Sys;
```                            
to the source file.    

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

                           
