/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————
    |  Com : Network Http Communication object   |
    ——————————————————————————————————————————————

© Copyright 2020 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202003201700 
License				: MIT.

History             :
202003101700: IVT   : Complete rewrite. Now in Tore.Core namespace.
————————————————————————————————————————————————————————————————————————————*/
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

using static Tore.Core.Sys;


namespace Tore.Core {

/**——————————————————————————————————————————————————————————————————————————— 
    CLASS:  Com.                                                    <summary>
    TASKS:  Manages Http client requests and responses.             <para/>
    USAGE:                                                          <br/>
            Since client requests and responses differ dramatically,
            Com objects give both standard and micro managed request
            types and response handling.                            <para/>
            For standard http requests use STATIC functions         <br/>
                Com.send(...) and Com.sendAsync(...)                <para/>
            Otherwise, create a Com object and manipulate the
            request via req, request headers via headers 
            (maps to req.headers), etc. then use INSTANCE send() 
            and sendAsync() routines.                               <para/>
            IMPORTANT:                                              <br/>
            content, accept and mediaType properties are 
            transferred to request just before sending it.          <para/>
            Tricky assignments must be done via                     <br/>
                req.Content,                                        <br/>
                req.Content.Headers.Accept,                         <br/>
                req.Content.Headers.ContentType.MediaType           <br/>
            properties, And Respective content, accept, mediaType of 
            Com instance properties must be <b>empty</b>.           <para/>
            Please read the comments on the code at least once for 
            using this class efficiently.                           </summary>
————————————————————————————————————————————————————————————————————————————*/
public class  Com {

/*———————————————————————————————————————————————————————————————————————————— 
  PROP: HttpClient : client [private]
  WARN:
    Microsoft recommends usage of a single HttpClient for all requests.
    HttpClient is thread safe and manages all requests and responses to
    the socket level. So beware.
    It is private here because of many web articles and examples abusing 
    it, like setting default headers etc. 
    No copy paste and cargo cult programming allowed...
————————————————————————————————————————————————————————————————————————————*/
private static HttpClient   client      {get; set;} = new HttpClient();

/**———————————————————————————————————————————————————————————————————————————
  PROP: req.                                                    <summary>
  GET : Returns HttpRequestMessage object.                      <br/>
  INFO: For cases requiring direct manipulation.                </summary>
————————————————————————————————————————————————————————————————————————————*/
public HttpRequestMessage   req         {get; private set;}
/**———————————————————————————————————————————————————————————————————————————
  PROP: res.                                                    <summary>
  GET : Returns HttpResponseMessage object.                     <para/>
  INFO: For cases requiring direct manipulation.                </summary>
————————————————————————————————————————————————————————————————————————————*/
public HttpResponseMessage  res         {get; private set;}
/**———————————————————————————————————————————————————————————————————————————
  PROP: headers.                                                <summary>
  GET : Returns HttpRequestHeaders object.                      <br/>
  INFO: For cases requiring direct manipulation.                </summary>
————————————————————————————————————————————————————————————————————————————*/
public HttpRequestHeaders   headers     {get => req.Headers;}   
/**———————————————————————————————————————————————————————————————————————————
  PROP: method.                                                 <summary>
  GET : Returns current request method.                         <br/>
  SET : Sets request method.                                    </summary>
————————————————————————————————————————————————————————————————————————————*/
public HttpMethod           method      {   // Request method  GET, POST etc.
    get => req.Method;
    set => req.Method = value;
}
/**———————————————————————————————————————————————————————————————————————————
  PROP: url                                                     <summary>
  GET : Returns request url as string.                          <br/>
  SET : Sets request url by string                              </summary>
————————————————————————————————————————————————————————————————————————————*/
public string   url         {           // Request url.
    get => req.RequestUri.ToString();
    set => req.RequestUri = new Uri(value);
}
/**———————————————————————————————————————————————————————————————————————————
  PROP: encoding.                                                   <summary>
  GET : Returns request encoding to set during request.             <br/>
  SET : Sets request encoding to set during request.                </summary>
————————————————————————————————————————————————————————————————————————————*/
public Encoding encoding    {get; set;} 
/**———————————————————————————————————————————————————————————————————————————
  PROP: accept.                                                     <summary>
  GET : Returns accept mime type to set during request.             <br/>
  SET : Sets accept mime type to set during request.                <para/>
  INFO: Must be left null when direct manipulation required.        </summary>
————————————————————————————————————————————————————————————————————————————*/
public string   accept      {get; set;} // Req. accept  MIME.
/**———————————————————————————————————————————————————————————————————————————
  PROP: mediaType.                                                  <summary>
  GET : Returns content mime type to set during request.            <br/>
  SET : Sets content mime type to set during request.               <para/>
  INFO: Must be left null when direct manipulation required.        </summary>
————————————————————————————————————————————————————————————————————————————*/
public string   mediaType   {get; set;} // Req. content MIME.
/**———————————————————————————————————————————————————————————————————————————
  PROP: content  .                                                  <summary>
  GET : Returns content to set during request.                      <br/>
  SET : Sets content to set during request.                         <para/>
  INFO: Must be left null when direct manipulation required.        </summary>
————————————————————————————————————————————————————————————————————————————*/
public object   content     {get; set;} // Req. content.
/**———————————————————————————————————————————————————————————————————————————
  PROP: isForm.                                                     <summary>
  GET : Returns if content must be form url encoded.                <br/>
  SET : Sets if content must be form url encoded.                   <para/>
  INFO: Irrelevant when content is directly manipulated.            </summary>
————————————————————————————————————————————————————————————————————————————*/
public bool     isForm      {get; set;} 
/**———————————————————————————————————————————————————————————————————————————
  PROP: queryList.                                                  <summary>
  GET : Returns a clone of query list.                              </summary>
————————————————————————————————————————————————————————————————————————————*/
public Stl      queryList    => qList == null ? null: new Stl().clone(qList);

private Stl     qList      {get; set;} // info for query.

/**———————————————————————————————————————————————————————————————————————————
  PROP: resString.                                              <summary>
  GET : Returns response as string.                             </summary>
————————————————————————————————————————————————————————————————————————————*/
public string   resString   {           // Response as string.
    get => (res == null)    ? 
                ""          :
                res .Content
                    .ReadAsStringAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult(); 
}
/**———————————————————————————————————————————————————————————————————————————
  PROP: resByteArray.                                           <summary>
  GET : Returns response as byte[].                             </summary>
————————————————————————————————————————————————————————————————————————————*/
public byte[]   resByteArray{          // Response as byte array.
    get => (res == null)    ? 
                null        :
                res .Content
                    .ReadAsByteArrayAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult(); 
}

/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————
    |   Static utility methods   |
    ——————————————————————————————
————————————————————————————————————————————————————————————————————————————*/
/**———————————————————————————————————————————————————————————————————————————
  FUNC: send [static]                                               <summary>
  TASK: Static method to prepare and send a request, blocking 
        until a response or an exception.[Synchronous].             <br/>
        This is a convenience routine for standard requests which
        does not need header manipulations etc.                     <para/>
  ARGS:                                                             <br/>
    url       : string     : Target url.                            <br/>
    content   : object     : Object holding the content.            <br/>
    query     : Stl        : Query list.  :DEF: null.               <br/>
    method    : HttpMethod : Http method. :DEF: null -> POST.       <br/>
    mediaType : string     : Media type.  :DEF: "application/json". <br/>
    encoding  : Encoding   : Encoding.    :DEF: null -> UTF8.       <br/>
    bool      : isForm     : If true content will be FormUrlEncoded.
                                          :DEF: false.              <para/>
  RETV:         : Com       : Com object with response.             <para/>
  WARN: Rethrows all exceptions coming from HttpClient sendAsync.   </summary>
————————————————————————————————————————————————————————————————————————————*/
public static Com   send(   string      url, 
                            object      content,
                            Stl         query       = null,
                            HttpMethod  method      = null, 
                            string      mediaType   = "application/json",
                            Encoding    encoding    = null,
                            bool        isForm      = false){

Com c = makeCom(url, content, query, method, mediaType, encoding, isForm);
    return c.send();
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: sendAsync [static]                                          <summary>
  TASK: Static method to prepare and send a request without blocking 
        [Asynchronous].                                             <br/>
        Does not need header manipulations etc.                     <para/>
  ARGS:                                                             <br/>
    url       : string     : Target url.                            <br/>
    content   : object     : Object holding the content.            <br/>
    query     : Stl        : Query list.  :DEF: null.               <br/>
    method    : HttpMethod : Http method. :DEF: null -> POST.       <br/>
    mediaType : string     : Media type.  :DEF: "application/json". <br/>
    encoding  : Encoding   : Encoding.    :DEF: null -> UTF8.       <br/>
    bool      : isForm     : If true content will be FormUrlEncoded.
                                          :DEF: false.              <para/>
  RETV:       : Com        : Com object with response.              <para/>
  WARN: Rethrows all exceptions coming from HttpClient sendAsync.   </summary>
————————————————————————————————————————————————————————————————————————————*/
public static async Task<Com>   sendAsync(  
                                string      url, 
                                object      content,
                                Stl         query       = null,
                                HttpMethod  method      = null, 
                                string      mediaType   = "application/json",
                                Encoding    encoding    = null,
                                bool        isForm      = false){

Com c = makeCom(url, content, query, method, mediaType, encoding, isForm);
    return await c.sendAsync();
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: talk [static]                                           <summary>
  TASK: POST content as an Utf-8 json request                       <br/>
        and return response as an instance of T [Synchronous].      <br/>
        This is a convenience routine for standard requests which 
        does not need header manipulations etc.                     <para/>
  ARGS:                                                             <br/>
    url         : string    : Target url.                           <br/>
    content     : object    : Object holding the content.           <br/>
    query       : Stl       : Query list.       :DEF: null          <para/>
  RETV:         : T         : Json decoded response object.         <para/>
  WARN: Rethrows all exceptions coming from HttpClient sendAsync.   <br/>
        Throws E_INV_ARG  if url or content is null.                <br/>
        Throws E_COM_FAIL if not successful.                        <br/>
        Throws E_COM_INV_OBJ if response object invalid             </summary>
————————————————————————————————————————————————————————————————————————————*/
public static T     talk<T>(string  url, 
                            object  content,
                            Stl     query = null){

string  jsn;

    chk(url,     "url");
    chk(content, "content");
    jsn = JsonConvert.SerializeObject(content, Formatting.Indented);
    return Com.send(url, jsn, query).resObjByJson<T>();
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: talkAsync [static]                                          <summary>
  TASK: POST content as an Utf-8 json request                       <br/>
        and return response as an instance of T [Asynchronous].     <br/>
        This is a convenience routine for standard requests which 
        does not need header manipulations etc.                     <para/>
  ARGS:                                                             <br/>
    url         : string    : Target url.                           <br/>
    content     : object    : Object holding the content.           <br/>
    query       : Stl       : Query list.       :DEF: null          <para/>
  RETV:         : T         : Json decoded response object.         <para/>
  WARN: Rethrows all exceptions coming from HttpClient sendAsync.   <br/>
        Throws E_INV_ARG  if url or content is null.                <br/>
        Throws E_COM_FAIL if not successful.                        <br/>
        Throws E_COM_INV_OBJ if response object invalid             </summary>
————————————————————————————————————————————————————————————————————————————*/
public static async Task<T>     talkAsync<T>(   string  url, 
                                                object  content,
                                                Stl     query = null){
Com     com;
string  jsn;

    chk(url,     "url");
    chk(content, "content");
    jsn = JsonConvert.SerializeObject(content, Formatting.Indented);
    com = await Com.sendAsync(url, jsn, query);
    return com.resObjByJson<T>();
}

// Com builder for static utilities.
private static Com makeCom( string      url, 
                            object      content,
                            Stl         query       = null,
                            HttpMethod  method      = null, 
                            string      mediaType   = "application/json",
                            Encoding    encoding    = null,
                            bool        isForm      = false){
Com c = new Com(){
        url         = url,
        content     = content,
        mediaType   = mediaType,
        isForm      = isForm,
        encoding    = encoding  ?? Encoding.UTF8,
        method      = method    ?? HttpMethod.Post
    };
    if (query != null)
        c.addQuery(query);
    return c;
}

/*————————————————————————————————————————————————————————————————————————————
    ————————————————————————
    |   Instance methods   |
    ————————————————————————
————————————————————————————————————————————————————————————————————————————*/

/**———————————————————————————————————————————————————————————————————————————
  CTOR: Com.                                                    <summary>
  TASK: Constructs a Com object.                                <para/>
  INFO:                                                         <br/>
        Sets req to a new HttpRequestMessage.                   <br/>
        Sets method to POST.                                    <br/>
        Sets encoding to UTF8.                                  <br/>
        Sets mediaType (content type) to "application/json".    </summary>                                  
————————————————————————————————————————————————————————————————————————————*/
public                      Com(){
    req         = new HttpRequestMessage();
    method      = HttpMethod.Post;
    encoding    = Encoding.UTF8;
    mediaType   = "application/json";
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: send                                                        <summary>
  TASK: Sends request in a blocking fashion. [Synchronous].         <br/>
  RETV:     : Com : this object.                                    </summary>
————————————————————————————————————————————————————————————————————————————*/
public Com                  send(){
    sendReqAsync(false).GetAwaiter().GetResult();
    return this;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: sendAsync.                                                  <summary>
  TASK: Sends request asynchronously.                               <br/>
  RETV:     : Com : this object.                                    </summary>
————————————————————————————————————————————————————————————————————————————*/
public async Task<Com>      sendAsync(){
    await sendReqAsync(true);
    return this;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: checkResult.                                                <summary>
  TASK: Raises exception if request not successfull.                <para/>
  RETV:     : Com : this object.                                    </summary>
————————————————————————————————————————————————————————————————————————————*/
public void                     checkResult(){
    if (!res.IsSuccessStatusCode){ 
        exc("E_COM_FAIL", 
            $"Url        : {url}\n"+
            $"Status Code: {res.StatusCode}\n"+
            $"Reason     : {res.ReasonPhrase}");
    }
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: resObjByJson                                                <summary>
  TASK: Checks and deserializes response json into Type T.          <para/>
  ARGS: T   : Type : Type of response expected.                     <para/>
  RETV:     : T    : Response object.                               <para/>
  WARN: Throws E_COM_FAIL if request was not successful.            <br/>
        Throws E_COM_INV_OBJ if response object invalid             </summary>
————————————————————————————————————————————————————————————————————————————*/
public T                        resObjByJson<T>(){
T   obj;
var ser = (typeof(T) == typeof(Stl)) ? Stl.stlJsonSrlzSet : null;

    checkResult();
    try { 
        obj = JsonConvert.DeserializeObject<T>(resString, ser);
    } catch (Exception e) {
        exc("E_COM_INV_OBJ", 
            $"Url        : {url}\n"+
            $"Class      : {typeof(T).Name}",
            e);
        throw;
    }
    return obj;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: addQuery                                                    <summary>
  TASK: Collects key value pairs to prepare a request uri query 
        information. Key duplication allowed.                       <para/>
  ARGS:                                                             <br/>
        query : Stl : List containing keys and values.              <para/>
  WARN: Throws all exceptions coming from Stl.                      </summary>
————————————————————————————————————————————————————————————————————————————*/
public  void    addQuery(Stl query){
    if (qList == null)
        qList = new Stl(false, false, false);
    qList.append(query);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: addQuery                                                    <summary>
  TASK: Collects key value pairs to prepare a request uri query 
        information. Key duplication allowed.                       <para/>
  ARGS:                                                             <br/>
        key     : string : A query key.                             <br/>
        value   : string : A value corresponding to the key.        <para/>
  WARN: Throws all exceptions coming from Stl.                      </summary>
————————————————————————————————————————————————————————————————————————————*/
public void     addQuery(string key, string value){
    if (qList == null)
        qList = new Stl(false, false, false);
    qList.Add(key, value);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: clearQuery.                                                 <summary>
  TASK: Clears query list.                                          <para/>
  INFO: Throws all exceptions coming from Stl.                      </summary>
————————————————————————————————————————————————————————————————————————————*/
public void     clearQuery(){
    if (qList != null)
        qList.clear();
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: buildQuery                                                  <summary>
  TASK: Converts the key value pairs in query list to a get query.  <para/>
  INFO: qList is a Stl, addQuery methods can be used to populate it.</summary>
————————————————————————————————————————————————————————————————————————————*/
public string   buildQuery(){
StringBuilder   b;
int             i,
                l;

    if (qList == null)
        return "";
    l = qList.count;
    if (l == 0)
        return "";
    b = new StringBuilder();
    for(i = 0; i < l; i++){
        if (i > 0)
            b.Append('&');
        b.AppendFormat(
            "{0}={1}", 
            Uri.EscapeDataString(qList.kl[i]), 
            Uri.EscapeDataString((string)qList.ol[i]));
    }
    return b.ToString();
}

/*———————————————————————————————————————————————————————————————————————————
  FUNC: requestSetup [private]
  TASK: Sets up a request right before sending.
  INFO: 
        Algorithm :
        Call contentSetup

        If  accept is defined
            adds it to request headers.
        If  mediaType is defined
            adds it to request headers.

  WARN: This routine is called by sendReqAsync().
        IT WORKS AT EVERY REQUEST.
        If accept, mediaType (content type) or content is
        very special, they should be assigned via req property and
        corresponding properties (accept or mediaType or content)
        should be left empty.       
————————————————————————————————————————————————————————————————————————————*/
private void                requestSetup(){
    contentSetup();
    if (qList != null)
        url += "?"+buildQuery();
    if (!snoWhite(accept))
        req.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
    if (!snoWhite(mediaType))
        req.Content.Headers.ContentType.MediaType = mediaType;
}

/*———————————————————————————————————————————————————————————————————————————
  FUNC: contentSetup [private]
  TASK: Sets up a request right before sending.
  INFO: 
        Algorithm :
        If  req.content is not null (some special request)
            return, ignoring everything.
        If  content is null (req.content is also null here)
            set req.content to "";
            return;
        If  content is byte array
            request content is loaded with it as byte array content.
        If  content is string
            request content is loaded with it as string content
            with encoding and mediaType settings.
        If  content is not Stl (String associated object list)
            content object is converted to Stl. 
            That means public properties are loaded into Stl as
            key string and value objects.
        If  isForm is flagged
            Stl is converted to List<KeyValuePair<string, string>>
            request content is loaded with it as FormUrlEncodedContent.
        Otherwise,
            Stl is converted to json string.
            request content is loaded with it as string content
            with encoding and mediaType settings.

  WARN: This routine is called by sendReqAsync().
        IT WORKS AT EVERY REQUEST.
        If you set req.content manually,
        leave the Com object content property empty.
————————————————————————————————————————————————————————————————————————————*/
private void contentSetup(){
Stl     stl = null;

    if (req.Content != null)
        return;
    if (content == null){ 
        req.Content = new StringContent("");
        return;
    }
    if (content is byte[]){
        req.Content = new ByteArrayContent((byte[])content);
        return;
    }
    if (content is string){
        req.Content = new StringContent((string) content, encoding, mediaType);
        return;
    }
    stl = (content is Stl) ? (Stl)content : new Stl(content);
    if (isForm){
        req.Content = new FormUrlEncodedContent(stl.toLstKvpStr());
        return;
    }
    req.Content = new StringContent(stl.toJson(), encoding, mediaType);
}


/*———————————————————————————————————————————————————————————————————————————
  FUNC: sendReqAsync [private].
  TASK: Chooses completion context and sends request asynchronously.
  ARGS: onCaptured : bool : If true,    await lands on calling context.
                            If false,   await lands on any context available.
  INFO: For synchronous calls onCapture should be false.
  WARN: Rethrows all exceptions coming from HttpClient sendAsync.
————————————————————————————————————————————————————————————————————————————*/
private async Task          sendReqAsync(bool onCaptured){
string  exs;
    try { 
        requestSetup();
        res = await client
                    .SendAsync(req)
                    .ConfigureAwait(onCaptured);
    } catch (Exception e) {
        if (e.InnerException != null)
            e = e.InnerException;
        exs = (onCaptured) ? "E_SEND_ASYNC" : "E_SEND_SYNC";
        exc(exs, $"url: {url}", e);
        throw;
    }
}

}   //  End class Com.
}   //  End namespace.