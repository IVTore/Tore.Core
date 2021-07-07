/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————
    |   Stl : String associated object list      |
    ——————————————————————————————————————————————

© Copyright 2020 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202003101300. 
License				: MIT.

History             :
202101261231: IVT   : - Removed unnecessary mapped enc/dec.
202003101300: IVT   : + toLstKvpStr() + mapped enc/dec.
202002011604: IVT   : toObj behaviour changed. Added toObj<T>.
201909051400: IVT   : First Draft.
————————————————————————————————————————————————————————————————————————————*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using static Tore.Core.Sys;
using Kvp   =       System.Collections.Generic.KeyValuePair<string, object>;
using Kvs   =       System.Collections.Generic.KeyValuePair<string, string>;
using IDso  =       System.Collections.Generic.IDictionary<string, object>;

namespace Tore.Core {

/**———————————————————————————————————————————————————————————————————————————
                                                                    <summary>
  CLASS :   StlIgnore [property attribute].                         <para/>
  USAGE :   This attribute excludes  property from 
            toObj and byObj operations.                             </summary>
————————————————————————————————————————————————————————————————————————————*/
[System.AttributeUsage( 
        System.AttributeTargets.Property,  
        AllowMultiple = false
)]  

public class StlIgnore:  System.Attribute { }


/**——————————————————————————————————————————————————————————————————————————— 
    CLASS:  Stl                                                     <summary>
    TASKS:                                                          <br/>
            A string associated object list class with tricks.      <para/>
    USAGE:                                                          <br/>
            Many key associated object containers need:             <br/>
            1) Numerically indexed access to keys and objects       <br/>
            2) Ordering.                                            <br/>
                                                                    <br/>
                                                                    <br/>
            * Keys can not be null empty or whitespace.             <br/>
            * Lists are public in this class intentionally.         <br/>
            * Stl also acts as a bridge for, 
            json, 
            objects (public properties), 
            static classes (static fields),
            IDictionary string key, object value [Alias: IDso] and
            List KeyValuePair string,string      [Alias: Kvs].      <br/>
            Has Enumerator and Nested converter support.            </summary>
————————————————————————————————————————————————————————————————————————————*/
[Serializable]
[JsonConverter(typeof(NestedStlConverter))]     // For web api conversions.

public class Stl    :   IEnumerable,
                        IEnumerable<Kvp>,
                        IDso {

public  List <string>   kl;         // Keys     list.
public  List <object>   ol;         // Objects  list.
private bool            un,         // Keys should be unique        if true.
                        id,         // Keys should be identifier    if true.
                        wr;         // Overwrite Object with same key.

public static   JsonConverter[]         stlJsonConvArr {get; }= 
                    new JsonConverter[]{new NestedStlConverter()};
public static   JsonSerializerSettings  stlJsonSrlzSet {get; } = 
                    new JsonSerializerSettings(){Converters = stlJsonConvArr};
            
/**——————————————————————————————————————————————————————————————————————————
  CTOR: Stl                                                     <summary>
  TASK: Constructs a string associated object list.             <para/>
  ARGS:                                                         <br/>
        aUnique     : bool: true if list keys should be unique.
                            :DEF: true.                         <br/>
        aIdentifier : bool: true if keys should be identifier.
                            :DEF: true.                         <br/>
        aOverwrite  : bool: true if object is overwritable
                            with the same key.                  <br/>
                            This works only if keys are unique.
                            :DEF: true.                         </summary>
————————————————————————————————————————————————————————————————————————————*/
public          Stl(    bool aUnique        = true, 
                        bool aIdentifier    = true, 
                        bool aOverwrite     = true) {

    initialize(aUnique, aIdentifier, aOverwrite);
}

/**——————————————————————————————————————————————————————————————————————————
  CTOR: Stl                                                     <summary>
  TASK: Constructs a string associated object list.             <para/>
  INFO:                                                         <br/>
        Parameterless version:                                  <br/>
        Keys must be always unique identifiers.                 <br/>
        Objects can be overwritten if added with same key.      </summary>
————————————————————————————————————————————————————————————————————————————*/
public          Stl(){
    initialize(true, true, true);
}

/**——————————————————————————————————————————————————————————————————————————
  CTOR: Stl                                                         <summary>
  TASK: Constructs a string associated object list.                 <para/>
  ARGS: o   : object :                                              <br/>
        *   If string, it is assumed to be json and Stl is
            loaded via json conversion.                             <br/>
        *   If Type, the <b>public static fields</b> of the type 
            will be used to load the Stl.                           <br/>
        *   If Stl, it will be cloned [look info].                  <br/>
        *   If any other object the <b>public properties</b> of 
            object will be used to load the Stl.                    <para/>
  INFO:                                                             <br/>
        Keys must be always unique identifiers.                     <br/>
        Objects can be overwritten if added with same key.          <br/>
        These may be different for Stl cloning case                 <br/>
        since it copies the properties of original Stl.             </summary>
————————————————————————————————————————————————————————————————————————————*/
public          Stl(object o){
    initialize();
    if (o is string){
        byJson((string)o, false);
        return;
    }
    if (o is Type){
        byStatic((Type)o, false);
        return;
    }
    if (o is Stl){
        clone((Stl)o);
        return;
    }
    byObj(o, false);
}

/**——————————————————————————————————————————————————————————————————————————
  CTOR: Stl                                                     <summary>
  TASK: Constructs a string associated object list.             <para/>
  ARGS: akv : object[]  : array: key, value, key, value, ...    <para/>
  INFO:                                                         <br/>
        Keys must be always unique identifiers.                 <br/>
        Objects can be overwritten if added with same key.      <br/>
        akv is not a key value array, it is a linear array 
        of keys and values following each other.                </summary>
————————————————————————————————————————————————————————————————————————————*/
public          Stl(params object[] akv){
int i,
    l;

    initialize();
    chk(akv, "akv");
    l = akv.Length;
    if (akv.Length == 0)
        exc("E_INV_ARG","akv");
    if (l % 2 != 0)
        exc("E_ARG_COUNT","akv");
    for(i = 0; i < l; i += 2){
        if (!(akv[i] is string))
            exc("E_INV_KEY", "akv["+i.ToString()+"]");
        add((string)akv[i], akv[i+1]);
    }
}


/*———————————————————————————————————————————————————————————————————————————
  DTOR: ~Stl
  TASK: Destroys a string associated object list.
————————————————————————————————————————————————————————————————————————————*/
~Stl(){

    if (kl == null)
        return;
    clear();
    kl = null;
    ol = null;
}
/*————————————————————————————————————————————————————————————————————————————
  FUNC: initialize [private]
  TASK: Initializes a new Stl. Helps constructors.
————————————————————————————————————————————————————————————————————————————*/
private void    initialize( bool aUnique        = true, 
                            bool aIdentifier    = true, 
                            bool aOverwrite     = true) {

    un = aUnique;
    id = aIdentifier;
    wr = aOverwrite;
    kl = new List<string>();
    ol = new List<object>();
}

/*————————————————————————————————————————————————————————————————————————————
    ————————————————————————
    | Manipulator methods  |
    ————————————————————————
————————————————————————————————————————————————————————————————————————————*/

/**———————————————————————————————————————————————————————————————————————————
  FUNC: clear                                                       <summary>
  TASK: Shallow deletation of all entries (no element destruction). </summary>
————————————————————————————————————————————————————————————————————————————*/
public  void    clear() {
    kl.Clear();
    ol.Clear();
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: clone                                                       <summary>
  TASK: Clones a Stl into this Stl.                                 </summary>
————————————————————————————————————————————————————————————————————————————*/
public  Stl    clone(Stl src) {    
    clear();
    un = src.un;
    id = src.id;
    wr = src.wr;
    append(src);
    return this;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: append                                                      <summary>
  TASK: Shallow transfer of all entries.                            </summary>
————————————————————————————————————————————————————————————————————————————*/
public  void    append(Stl src) {
int i;
    if (src == null)
        return;
    for(i = 0; i < src.count; i++)
        add(src.kl[i], src.ol[i]);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: add                                                         <summary>
  TASK: Adds a key and associated object to the list.               <para/>
  ARGS:                                                             <br/>
        aKey: String : Key to add.                                  <br/>
        aObj: Object : Object to add.          :DEF: null.          <br/>
        pUni: Bool   : Unique pair (addPair).  :DEF: false.         <br/>
        aIdx: int    : Insert index, 
                       if aIdx out of bounds, inserts to end (push).
                       Otherwise inserts to aIdx. :DEF: -1 (push).  <para/>
  RETV:     : int    : Index of pair.                               <para/>
  INFO: This is a working logical madness.                          </summary>
————————————————————————————————————————————————————————————————————————————*/
public int      add(string  aKey, 
                    object  aObj = null,
                    bool    pUni = false, 
                    int     aIdx = -1   ) {
int i;
    if (String.IsNullOrWhiteSpace(aKey))
        exc("E_INV_ARG", "aKey");
    if (id  && (!identifier(aKey)))             // Check if Identifier.
        exc("E_INV_IDENT", $"aKey = {aKey}");
    i = (pUni) ? idxPair(aKey, aObj):idx(aKey); // Search (look: pUni).
    if (pUni && (i > -1))                       // If we have this 
        return(i);                              // return index.
    if((!un) || (i == -1)){                     // If not unique or not found
        if((aIdx < 0) || (aIdx >= kl.Count))    // If insert to end
            aIdx = kl.Count;                    // set index to append.
        kl.Insert(aIdx, aKey);                  // insert key to index
        ol.Insert(aIdx, aObj);                  // insert object to index
        return(aIdx);                           // Return index.
    }
    if(!wr)                                     // If no overwrite
        exc("E_STL_NO_OVR", aKey);              // Error.
    ol[i] = aObj;                               // Overwrite object
    return i;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: addPair                                                     <summary>
  TASK: Adds a key and associated object pair to the list.          <para/>
  ARGS:                                                             <br/>
        aKey: String : Key to add.                                  <br/>
        aObj: Object : Object to add.          :DEF: null.          <br/>
        pUni: Bool   : Unique pair (addPair).  :DEF: false.         <para/>
  RETV:     : int    : Index of pair.                               <para/>
  INFO: By default it adds unique pairs.                            </summary>
————————————————————————————————————————————————————————————————————————————*/  
public int      addPair(string  aKey, 
                        object  aObj = null,
                        bool    pUni = true) {
    return add(aKey, aObj, pUni);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: delIdx                                                      <summary>
  TASK: Deletes pair at the index.                                  <para/>
  ARGS: index   : int : pair index.                                 </summary>
————————————————————————————————————————————————————————————————————————————*/
public void     delIdx(int index) {
    if((index > -1) && (index < kl.Count)){
        ol[index] = null;
        kl.RemoveAt(index);
        ol.RemoveAt(index);
    }
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: del                                                         <summary>
  TASK: Deletes first occurence of a key and 
        associated object from the list.                            <para/>
  ARGS: aKey    : String : Key.                                     <para/>
  RETV          : bool   : true if key found and pair deleted.      </summary>
————————————————————————————————————————————————————————————————————————————*/
public bool     del(string aKey){
int i = idx(aKey);

    if (i > -1)
        delIdx(i);
    return (i > -1);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: delPair                                                     <summary>
  TASK: Deletes first occurence of a key, object pair.              <para/>
  ARGS:                                                             <br/>
        aKey: String : Key.                                         <br/>
        aObj: Object : Object.                                      <para/>
  RETV:     : bool   : true if key found and pair deleted.          <para/>
  INFO: Key and object must match for erasure.                      </summary>
  ————————————————————————————————————————————————————————————————————————————*/
public bool     delPair(string aKey, object aObj) {
int i = idxPair(aKey, aObj);

    if (i > -1)
        delIdx(i);
    return (i > -1);
}


/*————————————————————————————————————————————————————————————————————————————
    ————————————————————————
    |   Search methods     |
    ————————————————————————
————————————————————————————————————————————————————————————————————————————*/
/**———————————————————————————————————————————————————————————————————————————
  FUNC: idx                                                         <summary>
  TASK: Finds first occurence of a key from the beginning index.    <para/>
  ARGS:                                                             <br/>
        aKey        : String : Key to search for.                   <br/>
        fromIndex   : int    : Index to start the search.:DEF: 0.   <para/>
  RETV:             : int    : The key index else -1.               </summary>
————————————————————————————————————————————————————————————————————————————*/
public  int     idx(string aKey, int fromIndex = 0) {
    if (String.IsNullOrWhiteSpace(aKey))
        return -1;
    return(kl.IndexOf(aKey, fromIndex));
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: idxObj                                                      <summary>
  TASK: Finds first occurence of an object from the beginning index.<para/>
  ARGS:                                                             <br/>
        aObj        : Object : Object to search for. :DEF: "".      <br/>
        fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
  RETV:             : int    : The object index else -1.            </summary>
————————————————————————————————————————————————————————————————————————————*/
public  int     idxObj(object aObj, int fromIndex = 0) {
    return(ol.IndexOf(aObj, fromIndex));                     
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: idxPair                                                     <summary>
  TASK: Finds first occurence of a key, object pair 
        from the start index.                                       <para/>
  ARGS:                                                             <br/>
        aKey        : String : Key of pair to search for.           <br/>
        aObj        : Object : Object of pair to search for.        <br/>
        fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
  RETV:             : int    : The index found, else -1.            </summary>
————————————————————————————————————————————————————————————————————————————*/
public int      idxPair(string  aKey,
                        object  aObj = null, 
                        int     fromIndex=0) {
int j;

    if (String.IsNullOrWhiteSpace(aKey))
        return -1;
    while(true) {                           
        j = kl.IndexOf(aKey, fromIndex);    // Search key.  
        if(j == -1)                         // If key not found
            return(-1);                     // return not found.
        if(ol[j] == aObj)                   // If object at key matches
            return(j);                      // return index.
        fromIndex = j + 1;                  // Change search start.
    }                                       // Loop
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: has                                                         <summary>
  TASK: Looks if a key exists from the beginning index.             <para/>
  ARGS:                                                             <br/>
        aKey        : String : Key to search for.                   <br/>
        fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
  RETV:             : bool   : true if found, else false.           </summary>
————————————————————————————————————————————————————————————————————————————*/
public  bool     has(string aKey, int fromIndex = 0) {
    if (String.IsNullOrWhiteSpace(aKey))
        return false;
    return(kl.IndexOf(aKey, fromIndex) > -1);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: hasObj                                                      <summary>
  TASK: Looks if an object exists from the beginning index.         <para/>
  ARGS:                                                             <br/>
        aObj        : Object : Object to search for.                  <br/>
        fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
RETV:               : bool   : true if found, else false.           </summary>
————————————————————————————————————————————————————————————————————————————*/
public  bool     hasObj(object aObj, int fromIndex = 0) {
    return(ol.IndexOf(aObj, fromIndex) > -1);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: hasPair                                                     <summary>
  TASK: Looks if a key object pair exists from the beginning index. <para/>
  ARGS:                                                             <br/>
        aKey        : String : Key of pair to search for.           <br/>
        aObj        : Object : Object of pair to search for.        <br/>
        fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
RETV:               : bool   : true if found, else false.           </summary>
————————————————————————————————————————————————————————————————————————————*/
public  bool     hasPair(string aKey, object aObj, int fromIndex = 0) {
    return(idxPair(aKey, aObj, fromIndex)>-1);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: obj                                                         <summary>
  TASK: Returns first object with the key given from the
        search index.                                               <para/>
  ARGS:                                                             <br/>
        aKey      : String : Key to search for.                     <br/>
        fromIndex : int    : Index to start the search. :DEF: 0.    <para/>
  RETV:           : object : If key found returns object else null. </summary>
————————————————————————————————————————————————————————————————————————————*/
public object   obj(string aKey, int fromIndex = 0) {
int i;
    if (String.IsNullOrWhiteSpace(aKey))
        return null;
    i = kl.IndexOf(aKey, fromIndex);
    return  (i < 0) ? null : ol[i];
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: val.                                                        <summary>
  TASK: Checks and gets a value corresponding to a key from Stl.    <para/>
  ARGS:                                                             <br/>
        T     : Type   : Value type.                                <br/>
        key   : string : Key of value.                              <br/>
        empty : bool   : Null or empty allowed.    :DEF: false.     <br/>
        name  : string : Name of Stl.              :DEF: "".        <para/>
  RETV:       : T      : Value or default(T).                       <para/>
  INFO:                                                             <br/>
        Algorithm:                                                  <br/>
        If  key empty                           raises exception.   <br/>
        If  key does not exist in stl                               <br/>
            if empty is true (allowed)          returns default.    <br/>
            else                                raises exception.   <br/>
        If  value type incompatible with type T raises exception.   <br/>
        If  value is null or empty                                  <br/>
            If empty is false (allowed)         returns default.    <br/>
            else                                raises exception.   <br/>
        returns value.                                              <br/>
                                                                    <br/>
        name supplies information for exceptions.                   </summary>
————————————————————————————————————————————————————————————————————————————*/
public T        val<T>(string key, bool empty = false, string name = ""){
object  v = null;
Type    t = typeof(T);
int     i;
string  s;

    chk(key, "key");
    s = (name != "")? $"{name}[\"{key}\"] ": key;
    i = idx(key);
    if (i == -1){ 
        if (empty)
            return default;
        exc("E_STL_VAL", s + " undefined");
    }
    v = setType<T>(ol[i]);
    if (!empty)
        chk(v, s+" empty." , "E_STL_VAL");
    return (T)v;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: key                                                         <summary>
  TASK: Returns key associated to an object from the search index.  <para/>
  ARGS:                                                             <br/>
        aObj        : Object : Object to search for.                <br/>
        fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
  RETV:             : string : The key found else null.             </summary>
————————————————————————————————————————————————————————————————————————————*/
public string   key(object aObj, int fromIndex = 0) {
int i = idxObj(aObj, fromIndex);

     return( (i < 0) ? null : kl[i]);
}

/**———————————————————————————————————————————————————————————————————————————
  PROP: count: int;                                             <summary>
  GET : Returns number of entries in list.                      </summary>
————————————————————————————————————————————————————————————————————————————*/
public int count => kl.Count;

/**———————————————————————————————————————————————————————————————————————————
  PROP: capacity: int;                                          <summary>
  GET : Returns current entry capacity of list.                 <br/>
  SET : Sets entry capacity of list.                            </summary>
————————————————————————————————————————————————————————————————————————————*/
public int capacity {
    get {
        return kl.Capacity;
    }   
    set {   
        try {
            kl.Capacity = value;
            ol.Capacity = value;
        } catch (Exception e) {
            exc("E_STL_CAP", "", e);
        }
    }   
}

/*————————————————————————————————————————————————————————————————————————————
    —————————————————————————————————————————
    |   Conversion methods.                 |
    —————————————————————————————————————————
    Recommended use: When Stl is with unique keys and overwrite is true.
————————————————————————————————————————————————————————————————————————————*/

/**———————————————————————————————————————————————————————————————————————————
  FUNC: byJson.                                                     <summary>
  TASK: Converts a json string and loads it to Stl.                 <para/>
  ARGS:                                                             <br/>
        json : string   : Json string to convert to Stl.            <br/>
        init : bool     : If true clears stl before conversion.
                          :DEF: true.                               <para/>
  RETV:      : Stl      : Stl itself.                               <para/>
  INFO:                                                             <br/>
        Uses Newtonsoft json library.                               <br/>
        Json must contain an object not a primitive or array.       <br/>
        All sub-objects which are non-primitive and non array       <br/>
        are also converted to sub - Stl's.                          </summary>
————————————————————————————————————————————————————————————————————————————*/
public Stl      byJson(string json, bool init = true){
    if (init){
        clear();
        un = true;
        id = true;
        wr = true;
    }
    JsonConvert.PopulateObject(json, this, stlJsonSrlzSet);
    return this;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: toJson.                                                     <summary>
  TASK: Converts an Stl to json string.                             <para/>
  INFO:                                                             <br/>
        Uses Newtonsoft json library.                               <br/>
        General json conversion rules apply.                        <br/>
        Circular references must be avoided.                        </summary>
————————————————————————————————————————————————————————————————————————————*/
public string   toJson(){
     return JsonConvert.SerializeObject(    
                this, 
                Formatting.Indented
            );
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: toStatic.                                                   <summary>
  TASK: Sets <b>public static fields</b> of a class from Stl.       <para/>
  ARGS:                                                             <br/>
        t             : Type : Class type.                          <br/>
        ignoreMissing : bool : When true : Do not raise exception 
                               if a field is missing in Stl. 
                               :DEF:false.                          <para/>
  INFO: Public static fields of type t are sought in Stl.           </summary>
————————————————————————————————————————————————————————————————————————————*/
public void     toStatic(   Type    t, 
                            bool    ignoreMissing   = false){
FieldInfo[]     l;
object          v;
    
    chk(t, "t");
    l = t.GetFields(BindingFlags.Public | BindingFlags.Static);
    if (l == null)
        return;
    foreach(FieldInfo f in l) { 
        if (TryGetValue(f.Name, out v)){
            if (f.FieldType != v.GetType())
                v = setType(v, f.FieldType, ignoreMissing);
            f.SetValue(null, v);
        } else {
            if (!ignoreMissing)
                exc("E_STL_TO_STATIC",$"{t.Name}:{f.Name}?");
        }
    }
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: byStatic.                                                   <summary>
  TASK: Loads Stl from public static fields of type.                <para/>
  ARGS: t   : Type      : Class type.                               <para/>
  RETV:     : Stl       : Stl itself.                               </summary>
————————————————————————————————————————————————————————————————————————————*/
public Stl      byStatic(Type t, bool init = true) {
FieldInfo[] l;
bool        i;
                
    chk(t, "t");
    if (init){
        clear();
        un = true;
        id = true;      // Identifier only.
        wr = true;
    }
    i   = id;           // Store identifier restriction status.
    id  = false;        // Optimization: field names are already identifiers.
    l = t.GetFields(BindingFlags.Public |   // Get public static Fieldinfo.
                    BindingFlags.Static);   // Never returns null.
    foreach(FieldInfo f in l)               // For each field,
        add(f.Name, f.GetValue(null));      // Add name and value to Stl.
    id = i;             // Restore identifier restriction status.
    return this;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: toObj.                                                      <summary>
  TASK: Makes an object of class T and fills corresponding properties 
        of it from Stl.                                             <para/>
  ARGS:                                                             <br/>
        T             : Class : Target object class.                <br/>
        ignoreMissing : bool  : When true : Do not raise exception 
                                if a property is missing in Stl. 
                                :DEF:false.                         <para/>
  INFO: Properties of class T are sought in Keys of Stl.            </summary>
————————————————————————————————————————————————————————————————————————————*/
public T                toObj<T>(bool ignoreMissing = false) where T: new(){
    return (T)toObj(makeObject(typeof(T)), ignoreMissing);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: toObj.                                                      <summary>
  TASK: Makes an object of class type typ and fills corresponding 
        properties of it from Stl.                                  <para/>
  ARGS:                                                             <br/>
        type          : Type  : Target object class type.           <br/>
        ignoreMissing : bool  : When true : Do not raise exception 
                                if a property is missing in Stl. 
                                :DEF:false.                         <para/>
  INFO: Properties of class T are sought in Keys of Stl.            </summary>
————————————————————————————————————————————————————————————————————————————*/
public object               toObj(Type typ, bool ignoreMissing = false){
    return toObj(makeObject(typ), ignoreMissing);
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: toObj.                                                      <summary>
  TASK: Fills corresponding properties of an object from Stl.       <para/>
  ARGS:                                                             <br/>
        o             : object  : Target object.                    <br/>
        ignoreMissing : bool    : When true : Do not raise exception 
                                  if a property is missing in Stl. 
                                  :DEF:false.                       <para/>
  INFO: Properties of class T are sought in Keys of Stl.            </summary>
————————————————————————————————————————————————————————————————————————————*/
public object               toObj(object o, bool ignoreMissing = false){
PropertyInfo[]  l;
Type            t;
object          v;
int             i;
                
    
    chk(o, "o");
    t = o.GetType();
    if (t == typeof(Stl))
        ignoreMissing = true;
    l = t.GetProperties(BindingFlags.Public|
                        BindingFlags.Instance|
                        BindingFlags.FlattenHierarchy);
    if (l == null)
        return o;
    foreach(PropertyInfo p in l){
        if (hasAttr<StlIgnore>(p))
            continue;
        i = idx(p.Name);
        if (i == -1){
            if (ignoreMissing)
                continue;
            exc("E_PROP_MISSING", t.Name+"."+p.Name);
        }
        v = ol[i];
        p.SetValue(o, setType(v, p.PropertyType));
    }
    return o;
}


/**———————————————————————————————————————————————————————————————————————————
  FUNC: byObj.                                                      <summary>
  TASK: Fills Stl from an objects public properties.                <para/>
  ARGS:                                                             <br/>
        o   : object : Source object.                               <br/>
        init: bool   : Clears Stl before copy if true. :DEF:true.   <para/>
  RETV:     : Stl    : Stl itself.                                  <para/>
  INFO:                                                             <br/>
        Shallow copy.                                               <br/>
        Multiple object properties can be merged in to an Stl
        by setting init = false, but either the property names
        should be unique or Stl must accept duplicate keys.
        like myStl = new Stl(false, true, true)                     </summary>
————————————————————————————————————————————————————————————————————————————*/
public Stl              byObj(object o, bool init = true){
PropertyInfo[]  a;
Type            t;
bool            i;

    chk(o, "o");
    if (init){
        clear();
        un = true;
        id = true; 
        wr = true;
    }
    i   = id;           // Store identifier restriction status.
    id  = false;        // Optimization: Incoming keys are identifiers.
    t = o.GetType();
    a = t.GetProperties(BindingFlags.Public|        // never returns null.
                        BindingFlags.Instance|
                        BindingFlags.FlattenHierarchy); 
    foreach(PropertyInfo p in a){
        if (hasAttr<StlIgnore>(p))
            continue;
        add(p.Name, p.GetValue(o));
    }
    id = i;             // Restore identifier restriction status.
    return this;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: toDST.                                                      <summary>
  TASK: Tries to make a Dictionary of string, T  out of Stl.        <para/>
  ARGS: T   : object class type for objects.                        <para/>
  INFO: Shallow copy. Stl objects must be compatible with type T.   </summary>
————————————————————————————————————————————————————————————————————————————*/
public Dictionary<string, T>    toDST<T>(){
int                     i,
                        l;
T                       t;
Dictionary<string, T>   d;

    d = new Dictionary<string, T>();
    l = count;
    for(i = 0; i < l; i++){
        if (ol[i] is T) {
            t = (T)ol[i]; 
        } else {
            t = setType<T>(ol[i]);
        }
        d.Add(kl[i], t);
    }
    return d;
}

/**———————————————————————————————————————————————————————————————————————————
  FUNC: toLstKvpStr.                                                <summary>
  TASK: Makes a List of KeyValuePair of string, string and fills it 
        with the keys and values from Stl.                          <para/> 
  INFO:                                                             <br/>
        Used at Http request form data construction in c#.          <br/>
        Values are obtained by ToString();                          <br/>
        Kvs is an alias to KeyValuePair of string, string.          </summary>
————————————————————————————————————————————————————————————————————————————*/
public List<Kvs> toLstKvpStr(){
int         i,
            c;
List<Kvs>   r;
    
    c = count;
    r = new List<Kvs>();
    for(i = 0; i < c; i++) 
        r.Add(new Kvs(kl[i], ol[i].ToString()));
    return r;
}

/*————————————————————————————————————————————————————————————————————————————
    —————————————————————————————————————————
    |   IDictionary interface.              |
    —————————————————————————————————————————
    Recommended use: When Stl is with unique keys and overwrite is true.
————————————————————————————————————————————————————————————————————————————*/

public void Add(string key, object value)   => add(key, value);

public bool Remove(string key)              => del(key);

public bool Remove(Kvp item)                => delPair(item.Key, item.Value);

public void Add(Kvp item)                   => add(item.Key, item.Value);

public void Clear()                         => clear();

public bool ContainsKey(string key)         => has(key);

public bool Contains(Kvp item)              => hasPair(item.Key, item.Value);

public int  Count                           => count;

public bool IsReadOnly                      => false;

public ICollection<string>  Keys            => copyToList<string>(kl);

public ICollection<object>  Values          => copyToList<object>(ol);

public IEnumerator<Kvp>     GetEnumerator() => new StlEnumeratorKVP(this);

IEnumerator     IEnumerable.GetEnumerator() => kl.GetEnumerator();

public bool     TryGetValue(string key, out object value) {
int     i;
bool    b;

    i = idx(key);
    b = (i > -1);
    value = b ? ol[i] : null;
    return(b); 
}

public void     CopyTo(Kvp[] array, int arrayIndex) {
int i,
    j = 0,
    c;
        
    c = count;
    for(i = arrayIndex; i < c; i++) 
        array[j] = new Kvp(kl[i], ol[i]);
}

public object   this[string key] {
    get => obj(key, 0); 
    set {
    int i;
        i = idx(key);
        if (i < 0){ 
            add(key, value);
            return;
        } 
        if (!wr){
            exc("E_STL_NO_OVR", key);   
        }
        ol[i] = value;
    }
}

}// end class Stl


public class        StlEnumeratorKVP: IEnumerator<Kvp> {

private Stl         lst;
private int         idx;
public  Kvp         Current => new Kvp(lst.kl[idx], lst.ol[idx]);

object              IEnumerator.Current => Current;

public              StlEnumeratorKVP(Stl stl) {
    lst = stl;
    idx = -1;
}

public void             Reset(){
    idx = -1;   
}

public void             Dispose(){ 
    Dispose(true);
}
   
protected virtual void  Dispose(bool disposing){
     
}

public bool             MoveNext() {
    if ((idx + 1) == lst.count)
        return false;
    ++idx;
    return true;
}
}   // end class StlEnumeratorKVP.

/*————————————————————————————————————————————————————————————————————————————
  CLASS :   NestedStlConverter.
  USAGE :   Used in jsonDeserializer, for supporting nested Stl.
  
    This replaces all untyped sub-list elements of a Stl which can be 
    IDictionary<string,object> lists with Stl. 
————————————————————————————————————————————————————————————————————————————*/
public class        NestedStlConverter : 
                    CustomCreationConverter<IDso>{

public override IDso    Create(Type objectType){
    return new Stl();
}

public override bool    CanConvert( Type objectType){
    return objectType == typeof(object) || base.CanConvert(objectType);
}

public override object  ReadJson(   JsonReader      reader, 
                                    Type            objectType, 
                                    object          existingValue, 
                                    JsonSerializer  serializer){
    if (reader.TokenType == JsonToken.StartObject
        || reader.TokenType == JsonToken.Null)
        return base.ReadJson(reader, objectType, existingValue, serializer);
    return serializer.Deserialize(reader);
}

}   // End class NestedStlConverter.

}
