# Tore.Core
Core utilities library for c#.

## Sys.cs :
Defines the static class Sys containing a library of utility methods treated as global functions which is used for managing:           
Exceptions / Strings / Simple encryption / Reflection / Type juggling / Attributes / Simple File Load Save / Simple Encrypted File Load Save/ Time, Date, and many others.                                       
The best way of using them is by adding:               
using static Tore.Core.Sys;                            
to the source file.    

## Stl.cs :
Defines the class Stl which is a string associated object list (key - value) class with tricks.     
                                                       
Stl provides:                                          
1) Numerically indexed access to keys and objects      
2) Ordering.                                           
3) Translation forward and backward to various formats.
                                                       
* Keys can not be null empty or whitespace.            
* Lists are public in this class intentionally.        
* Stl also acts as a bridge for,

   - Json, 
   - Objects (public properties), 
   - Static classes (static fields),
   - IDictionary string key, object value [Alias: IDso] and
   - List KeyValuePair string,string      [Alias: Kvs].     
 
Has Enumerator and Nested conversion support.           

## Com.cs :
Defines the class Com which manages Http client requests and responses.

Com gathers sub components required for a proper request and the response to it into an instance.
Since client requests and responses differ dramatically, Com objects give both standard and micro managed request types and response handling.                            
For simple standard http requests use STATIC functions Com.send(...), Com.sendAsync(...), Com.Talk<T>(...) and Com.TalkAsync<T>(...) .
Otherwise, create a Com object and manipulate the request via 
   - The Com properties, like: content, accept, mediaType.
   - or Com req the HttpRequestMessage property, directly. 
then use INSTANCE send() and sendAsync() routines.                               
IMPORTANT:                                              
content, accept and mediaType properties are transferred to request just before sending it.          
Tricky assignments must be done via                     
   - req.Content,                                        
   - req.Content.Headers.Accept,                         
   - req.Content.Headers.ContentType.MediaType properties, 
When req.content is non null, Com content property is ignored. 
Respective accept, mediaType of Com instance properties must be <b>empty</b>.           
Please read the comments on the code at least once for using this class efficiently.                           
