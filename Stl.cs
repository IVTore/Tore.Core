/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————
    |   Stl : String associated object list      |
    ——————————————————————————————————————————————

© Copyright 2020 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202003101300. 
Licenses            : MIT.

History             :
202101261231: IVT   : - Removed unnecessary mapped enc/dec.
202003101300: IVT   : + ToListOfKeyValuePairsOfString() + mapped enc/dec.
202002011604: IVT   : ToObj behaviour changed. Added ToObj<T>.
201909051400: IVT   : First Draft.
————————————————————————————————————————————————————————————————————————————*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using static Tore.Core.Sys;
using Kvp = System.Collections.Generic.KeyValuePair<string, object>;
using Kvs = System.Collections.Generic.KeyValuePair<string, string>;
using IDso = System.Collections.Generic.IDictionary<string, object>;

namespace Tore.Core {
    /**———————————————————————————————————————————————————————————————————————————
                                                                        <summary>
      CLASS :   StlIgnore [property attribute].                         <para/>
      USAGE :                                                           <br/>
                This attribute excludes                                 <br/>
                <b> Instance properties </b> from ToObj and ByObj,      <br/>
                <b> Static fields </b> from ToStatic and ByStatic,      <br/>
                operations of Stl.                                      </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    [System.AttributeUsage(
        System.AttributeTargets.Property|
        System.AttributeTargets.Field, 
        AllowMultiple = false)]
    
    public class StlIgnore: System.Attribute { }

    /**——————————————————————————————————————————————————————————————————————————— 
        CLASS:  Stl                                                     <summary>
        USAGE:                                                          <br/>
                A string associated object list class with tricks.      <br/>
                                                                        <br/>
                Stl provides:                                           <br/>
                1) Numerically indexed access to keys and objects       <br/>
                2) Ordering.                                            <br/>
                3) Translation forward and backward to various formats. <br/>
                4) Duplicate Key support.                               <br/>
                                                                        <br/>
                * Keys can not be null empty or whitespace.             <br/>
                * Lists are public in this class intentionally.         <br/>
                * Stl also acts as a bridge for,                        <br/>
                json,                                                   <br/>
                objects <b> (public properties with get and set) </b>,  <br/>
                static classes <b> (public static fields) </b>,         <br/>
                IDictionary string Key, object value [Alias: IDso] and  <br/>
                List KeyValuePair string,string      [Alias: Kvs].      <br/>
                Has Enumerator and Nested converter support.            </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    [Serializable]
    [JsonConverter(typeof(NestedStlConverter))]     // For web api conversions.

    public class Stl:IEnumerable, IEnumerable<Kvp>, IDso {

        private bool un,                    // Keys should be unique        if true.
                     id,                    // Keys should be identifier    if true.
                     wr;                    // Overwrite Object with same Key.

        /**<summary>Used for json conversion while Stl's are nested.</summary>*/
        public static JsonConverter[] stlJsonConverter { get; } =
                        new JsonConverter[] { new NestedStlConverter() };

        /**<summary>Used for setting json conversion for nested Stl's.</summary>*/
        public static JsonSerializerSettings stlJsonSettings { get; } =
                        new JsonSerializerSettings() { Converters = stlJsonConverter };


        /**———————————————————————————————————————————————————————————————————————————
          VAR : keyLst: List of string;                                     <summary>
          USE :                                                             <br/>
                Keeps the list of keys.                                     <para/>
          INFO:                                                             <br/>
                Public by design.                                           <br/>
                Never Add or remove items directly.                         <br/>
                Use Add(), AddPair, Delete(), DeletePair()                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public List<string> keyLst;

        /**———————————————————————————————————————————————————————————————————————————
          VAR : objLst: List of objects;                                    <summary>
          USE :                                                             <br/>
                Keeps the list of values (objects).                         <para/>
          INFO:                                                             <br/>
                Public by design.                                           <br/>
                Never Add or remove items directly.                         <br/>
                Use Add(), AddPair, delObj(), DeletePair()                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public List<object> objLst;         // Objects  list.

        #region Constructor and initializer methods.
        /*————————————————————————————————————————————————————————————————————————————
            ————————————————————————————————————————
            | Constructor and initializer methods. |
            ————————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ByArgs [static]                                             <summary>
          TASK:                                                             <br/>
                Converts an object array to Stl appropriately.              <para/>
          ARGS:                                                             <br/>
                args    : object[]  : Arguments (may be Key - values).      <para/>
          RETV:                                                             <br/>
                        : Stl       : Arguments as Stl.                     <para/>
          INFO:                                                             <br/>
                If args length = 0 null is returned.                        <br/>
                If args length = 1 then if args[0] is                       <br/>
                   * either an Stl, and it is returned.                     <br/>
                   * or an object, then converted to an Stl and returned.   <br/>
                Otherwise Stl constructor with object[] is called.          <br/>
                For more info refer to Stl.                                 </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Stl ByArgs(object[] args) {
            if (args.Length == 0)
                return null;
            if (args.Length == 1) {
                if (args[0] is Stl stl)
                    return stl;
                return new Stl(args[0]);
            }
            return new Stl(args);
        }

        /**——————————————————————————————————————————————————————————————————————————
          CTOR: Stl                                                     <summary>
          TASK:                                                         <br/>
                Constructs a string associated object list.             <para/>
          ARGS:                                                         <br/>
                unique     : bool: true if list keys should be unique.
                                    :DEF: true.                         <br/>
                identifier : bool: true if keys should be identifier.
                                    :DEF: true.                         <br/>
                overwrite  : bool: true if object is overwritable
                                    with the same Key.                  <br/>
                                    This works only if keys are unique.
                                    :DEF: true.                         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public Stl(bool unique = true, bool identifier = true, bool overwrite = true) {
            Init(unique, identifier, overwrite);
        }

        /**——————————————————————————————————————————————————————————————————————————
          CTOR: Stl                                                     <summary>
          TASK:                                                         <br/>
                Constructs a string associated object list.             <para/>
          INFO:                                                         <br/>
                Parameterless version:                                  <br/>
                Keys must be always unique identifiers.                 <br/>
                Objects can be overwritten if added with same Key.      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public Stl() {
            Init(true, true, true);
        }

        /**——————————————————————————————————————————————————————————————————————————
          CTOR: Stl                                                         <summary>
          TASK:                                                             <br/>
                Constructs a string associated object list.                 <para/>
          ARGS:                                                             <br/>
                o   : object :                                              <br/>
                *   If string, it is assumed to be json and Stl is
                    loaded via json conversion.                             <br/>
                *   If Type, the <b>public static fields</b> of the type 
                    will be used to load the Stl.                           <br/>
                *   If Stl, it will be cloned [look info].                  <br/>
                *   If any other object the <b>public properties</b> of
                    object will be used to load the Stl.                    <para/>
          INFO:                                                             <br/>
                Keys must be always unique identifiers.                     <br/>
                Objects can be overwritten if added with same Key.          <br/>
                These may be different for Stl cloning case
                since it copies the properties of the original Stl.         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public Stl(object o) {
            if (o is Stl lst) {
                Init(lst.un, lst.id, lst.wr);
                Append(lst);
                return;
            }
            Init();
            if (o is string str) {
                ByJson(str, false);
                return;
            }
            if (o is Type typ) {
                ByStatic(typ, false);
                return;
            }
            ByObj(o, false);
        }

        /**——————————————————————————————————————————————————————————————————————————
          CTOR: Stl                                                     <summary>
          TASK:                                                         <br/>
                Constructs a string associated object list.             <para/>
          ARGS:                                                         <br/>
                arrKeyVal : object[]  : array: key, val, key, val, ...  <para/>
          INFO:                                                         <br/>
                Keys must be always unique identifiers.                 <br/>
                Objects can be overwritten if added with same Key.      <br/>
                arrKeyVal is not a Key value array, it is a linear      <br/>
                array of keys and values following each other.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public Stl(params object[] arrKeyVal) {
            int i,
                l;

            Init();
            Chk(arrKeyVal, nameof(arrKeyVal));
            l = arrKeyVal.Length;
            if (arrKeyVal.Length == 0)
                Exc("E_INV_ARG", "akv");
            if (l % 2 != 0)
                Exc("E_ARG_COUNT", "akv");
            for(i = 0; i < l; i += 2) {
                if (arrKeyVal[i] is not string str)
                    Exc("E_INV_KEY", "akv[" + i.ToString() + "]");
                else
                    Add(str, arrKeyVal[i + 1]);
            }
        }

        /**——————————————————————————————————————————————————————————————————————————
          DTOR: ~Stl                                                        <summary>
          TASK: Destroys a string associated object list.                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        ~Stl() {
            if (keyLst == null)
                return;
            Clear();
            keyLst = null;
            objLst = null;
        }

        /*————————————————————————————————————————————————————————————————————————————
          FUNC: Init [private]
          TASK: Initializes a new Stl. Helps constructors.
        ————————————————————————————————————————————————————————————————————————————*/
        private void Init(bool unique = true, bool identifier = true, bool overwrite = true) {
            un = unique;
            id = identifier;
            wr = overwrite;
            keyLst = new List<string>();
            objLst = new List<object>();
        }
        #endregion

        #region Manipulator methods.
        /*————————————————————————————————————————————————————————————————————————————
            ————————————————————————
            | Manipulator methods  |
            ————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
           FUNC: Clear                                                       <summary>
           TASK:                                                             <br/>
                 Shallow deletation of all entries (no element destruction). <br/>
                 Also part of IDictionary interface.                         </summary>
         ————————————————————————————————————————————————————————————————————————————*/
        public void Clear() {
            keyLst.Clear();
            objLst.Clear();
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Clone                                                       <summary>
          TASK: Clones this Stl into a new Stl.                             </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public Stl Clone() {
            Stl clone = new (un, id, wr);
            clone.Append(this);
            return clone;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Append                                                      <summary>
          TASK: Shallow transfer of all entries.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public void Append(Stl src) {
            int i,
                l = Count;
            List<string> sKey;
            List<object> sObj;

            if (src == null)
                return;
            sKey = src.keyLst;
            sObj = src.objLst;
            for(i = 0; i < l; i++)
                Add(sKey[i], sObj[i], -1);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Add                                                         <summary>
          TASK:                                                             <br/>
                Adds a Key and associated object to the list.               <para/>
          ARGS:                                                             <br/>
                aKey        : String : Key to Add.                          <br/>
                aObj        : Object : Object to Add. :DEF: null.           <br/>
                aIdx        : int    : Insert index,                        <br/>
                            * if aIdx out of bounds, inserts to end (push). <br/>
                            * Otherwise inserts to aIdx. :DEF: -1 (push).   <br/>
                uniquePair  : Bool   : Unique pair (AddPair).  :DEF: false. <para/>
          RETV:                                                             <br/>
                            : int    : Index of pair.                       <para/>
          INFO:                                                             <br/>
                This is a working logical madness.                          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int Add(string aKey, object aObj = null, int aIdx = -1, bool uniquePair = false) {
            int i;

            if (aKey.IsNullOrWhiteSpace())
                Exc("E_INV_ARG", nameof(aKey));
            if (id && (!aKey.IsIdentifier()))               // Check if Identifier.
                Exc("E_INV_IDENT", $"aKey = {aKey}");
            i = (uniquePair) ? 
                    IndexPair(aKey, aObj) : Index(aKey);    // Search (look: uniquePair).
            if (uniquePair && (i > -1))                     // If we have this 
                return (i);                                 // return index.
            if ((!un) || (i == -1)) {                       // If not unique or not found
                if ((aIdx < 0) || (aIdx >= keyLst.Count))   // If insert to end
                    aIdx = keyLst.Count;                    // set index to Append.
                keyLst.Insert(aIdx, aKey);                  // insert Key to index
                objLst.Insert(aIdx, aObj);                  // insert object to index
                return (aIdx);                              // Return index.
            }
            if (!wr)                                        // If no overwrite
                Exc("E_STL_NO_OVR", aKey);                  // Error.
            objLst[i] = aObj;                               // Overwrite object
            return i;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: AddPair                                                     <summary>
          TASK:                                                             <br/>
                Adds a Key and associated object pair to the list.          <para/>
          ARGS:                                                             <br/>
                aKey        : String : Key to Add.                          <br/>
                aObj        : Object : Object to Add.          :DEF: null.  <br/>
                uniquePair  : Bool   : Unique pair (AddPair).  :DEF: true.  <para/>
          RETV:                                                             <br/>
                : int    : Index of pair.                                   <para/>
          INFO:                                                             <br/>
                By default it adds unique pairs.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int AddPair(string aKey, object aObj = null, bool uniquePair = true) {
            return Add(aKey, aObj, -1, uniquePair);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: DeleteIndex                                                 <summary>
          TASK: Deletes pair at the index.                                  <para/>
          ARGS: index   : int : pair index.                                 </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public void DeleteIndex(int index) {
            if ((index > -1) && (index < keyLst.Count)) {
                objLst[index] = null;
                keyLst.RemoveAt(index);
                objLst.RemoveAt(index);
            }
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Delete                                                      <summary>
          TASK:                                                             <br/>
                Deletes first occurence of a Key and associated object.     <para/>
          ARGS:                                                             <br/>
                aKey    : String : Key.                                     <para/>
          RETV:                                                             <br/>
                        : bool   : true if Key found and pair deleted.      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool Delete(string aKey) {
            int i = Index(aKey);

            if (i > -1)
                DeleteIndex(i);
            return (i > -1);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: DeletePair                                                  <summary>
          TASK:                                                             <br/>
                Deletes first occurence of a Key, object pair.              <para/>
          ARGS:                                                             <br/>
                aKey: String : Key.                                         <br/>
                aObj: Object : Object.                                      <para/>
          RETV:                                                             <br/>
                    : bool   : true if Key found and pair deleted.          <para/>
          INFO:                                                             <br/>
                Key and object must match for erasure.                      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool DeletePair(string aKey, object aObj) {
            int i = IndexPair(aKey, aObj);

            if (i > -1)
                DeleteIndex(i);
            return (i > -1);
        }
        #endregion

        #region Search methods.
        /*————————————————————————————————————————————————————————————————————————————
            ————————————————————————
            |   Search methods.    |
            ————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Index                                                       <summary>
          TASK:                                                             <br/>
                Finds first occurence of a Key from the beginning index.    <para/>
          ARGS:                                                             <br/>
                aKey        : String : Key to search for.                   <br/>
                fromIndex   : int    : Index to start the search.:DEF: 0.   <para/>
          RETV:                                                             <br/>
                            : int    : The Key index else -1.               </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int Index(string aKey, int fromIndex = 0) {
            if (aKey.IsNullOrWhiteSpace())
                return -1;
            return (keyLst.IndexOf(aKey, fromIndex));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: IndexObj                                                    <summary>
          TASK:                                                             <br/>
                Finds first occurence of an object from the index.          <para/>
          ARGS:                                                             <br/>
                aObj        : Object : Object to search for. :DEF: "".      <br/>
                fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
          RETV:                                                             <br/>
                            : int    : The object index else -1.            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int IndexObj(object aObj, int fromIndex = 0) {
            return (objLst.IndexOf(aObj, fromIndex));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: IndexPair                                                   <summary>
          TASK:                                                             <br/>
                Finds first occurence of a Key, object pair from the index. <para/>
          ARGS:                                                             <br/>
                aKey        : String : Key of pair to search for.           <br/>
                aObj        : Object : Object of pair to search for.        <br/>
                fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
          RETV:                                                             <br/>
                            : int    : The index found, else -1.            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int IndexPair(string aKey, object aObj = null, int fromIndex = 0) {
            int j;

            if (aKey.IsNullOrWhiteSpace())
                return -1;
            while(true) {
                j = keyLst.IndexOf(aKey, fromIndex);    // Search Key.  
                if (j == -1)                            // If Key not found
                    return (-1);                        // return not found.
                if (objLst[j] == aObj)                  // If object at Key matches
                    return (j);                         // return index.
                fromIndex = j + 1;                      // Change search start.
            }                                           // Loop
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Has                                                         <summary>
          TASK:                                                             <br/>
                Looks if a Key exists from the beginning index.             <para/>
          ARGS:                                                             <br/>
                aKey        : String : Key to search for.                   <br/>
                fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
          RETV:                                                             <br/>
                            : bool   : true if found, else false.           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool Has(string aKey, int fromIndex = 0) {
            if (aKey.IsNullOrWhiteSpace())
                return false;
            return (keyLst.IndexOf(aKey, fromIndex) > -1);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: HasObj                                                      <summary>
          TASK:                                                             <br/>
                Looks if an object exists from the beginning index.         <para/>
          ARGS:                                                             <br/>
                aObj        : Object : Object to search for.                <br/>
                fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
          RETV:                                                             <br/>
                            : bool   : true if found, else false.           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool HasObj(object aObj, int fromIndex = 0) {
            return (objLst.IndexOf(aObj, fromIndex) > -1);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: hasPair                                                     <summary>
          TASK:                                                             <br/>
                Looks if a Key object pair exists from the beginning index. <para/>
          ARGS:                                                             <br/>
                aKey        : String : Key of pair to search for.           <br/>
                aObj        : Object : Object of pair to search for.        <br/>
                fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
          RETV:                                                             <br/>
                            : bool   : true if found, else false.           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool hasPair(string aKey, object aObj, int fromIndex = 0) {
            return (IndexPair(aKey, aObj, fromIndex) > -1);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Obj                                                         <summary>
          TASK: Returns first object with the Key given from the
                search index.                                               <para/>
          ARGS:                                                             <br/>
                aKey      : String : Key to search for.                     <br/>
                fromIndex : int    : Index to start the search. :DEF: 0.    <para/>
          RETV:                                                             <br/>
                          : object : If Key found returns object else null. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public object Obj(string aKey, int fromIndex = 0) {
            int i;

            if (aKey.IsNullOrWhiteSpace())
                return null;
            i = keyLst.IndexOf(aKey, fromIndex);
            return (i < 0) ? null : objLst[i];
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Key                                                         <summary>
          TASK:                                                             <br/>
                Returns Key associated to an object from the search index.  <para/>
          ARGS:                                                             <br/>
                aObj        : Object : Object to search for.                <br/>
                fromIndex   : int    : Index to start the search. :DEF: 0.  <para/>
          RETV:                                                             <br/>
                            : string : The Key found else null.             </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public string Key(object aObj, int fromIndex = 0) {
            int i = IndexObj(aObj, fromIndex);

            return ((i < 0) ? null : keyLst[i]);
        }
        #endregion

        #region Properties.
        /*————————————————————————————————————————————————————————————————————————————
            ————————————————————
            |   Properties.    |
            ————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        /**———————————————————————————————————————————————————————————————————————————
          PROP: Count                                                       <summary>
          GET : Returns number of entries in list.                          <br/>
                Also part of IDictionary interface.                         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int Count => keyLst.Count;

        /**———————————————————————————————————————————————————————————————————————————
          PROP: Capacity: int;                                          <summary>
          GET : Returns current entry Capacity of list.                 <br/>
          SET : Sets entry Capacity of list.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public int Capacity {
            get {
                return keyLst.Capacity;
            }

            set {
                try {
                    keyLst.Capacity = value;
                    objLst.Capacity = value;
                } catch(Exception e) {
                    Exc("E_STL_CAP", "", e);
                }
            }
        }
        #endregion

        #region Conversion methods.
        /*————————————————————————————————————————————————————————————————————————————
            —————————————————————————————————————————
            |   Conversion methods.                 |
            —————————————————————————————————————————
            Recommended use: When Stl is with unique keys and overwrite is true.
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ByJson.                                                     <summary>
          TASK:                                                             <br/>
                Converts a json string and loads it to Stl.                 <para/>
          ARGS:                                                             <br/>
                json : string   : Json string to convert to Stl.            <br/>
                Init : bool     : If true clears stl before conversion.
                                  :DEF: true.                               <para/>
          RETV:                                                             <br/>
                     : Stl      : Stl itself.                               <para/>
          INFO:                                                             <br/>
                Uses Newtonsoft json library.                               <br/>
                Json must contain an object not a primitive or array.       <br/>
                All sub-objects which are non-primitive and non array       <br/>
                are also converted to sub - Stl's.                          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public Stl ByJson(string json, bool init = true) {
            if (init) {
                Clear();
                un = true;
                id = true;
                wr = true;
            }
            JsonConvert.PopulateObject(json, this, stlJsonSettings);
            return this;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ToJson.                                                     <summary>
          TASK:                                                             <br/>
                Converts an Stl to json string.                             <para/>
          INFO:                                                             <br/>
                Uses Newtonsoft json library.                               <br/>
                General json conversion rules apply.                        <br/>
                Circular references must be avoided.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public string ToJson() {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ToStatic.                                                   <summary>
          TASK:                                                             <br/>
                Sets <b>public static fields</b> of a class from Stl.       <para/>
          ARGS:                                                             <br/>
                type          : Type : Class type.                          <br/>
                ignoreMissing : bool : If true, ignores missing fields else
                                       raises exception.:DEF:false.         <para/>
          INFO:                                                             <br/>
                Public static fields of type type are sought in Stl.        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public void ToStatic(Type type, bool ignoreMissing = false) {
            FieldInfo[] fldInfArr;
            object val;

            Chk(type, nameof(type));
            fldInfArr = StaticFields(type);
            foreach(FieldInfo fld in fldInfArr) {
                if (hasAttr<StlIgnore>(fld))
                    continue;
                if (TryGetValue(fld.Name, out val)) {
                    if (fld.FieldType != val.GetType())
                        val = SetType(val, fld.FieldType, ignoreMissing);
                    fld.SetValue(null, val);
                } else {
                    if (!ignoreMissing)
                        Exc("E_STL_TO_STATIC", $"{type.Name}:{fld.Name}?");
                }
            }
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ByStatic.                                                   <summary>
          TASK:                                                             <br/>
                Loads Stl from <b> public static fields </b> of type.       <para/>
          ARGS:                                                             <br/>
                type    : Type  : Class type.                               <br/>
                init    : bool  : Clears Stl before copy if true. :DEF:true.<para/>
          RETV:                                                             <br/>
                        : Stl   : Stl itself.                               </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public Stl ByStatic(Type type, bool init = true) {
            FieldInfo[] fldLst;
            bool irs;

            Chk(type, nameof(type));
            if (init) {
                Clear();
                un = true;
                id = true;      // Identifier only.
                wr = true;
            }
            irs = id;           // Store identifier restriction status.
            id = false;         // Optimization: field names are already identifiers.
            fldLst = StaticFields(type);            // Get static field info list.
            foreach(FieldInfo fld in fldLst){       // For each field,
                if (hasAttr<StlIgnore>(fld))        // Check if ignored.
                    continue;
                Add(fld.Name, fld.GetValue(null));  // Add name and value to Stl.
            }
            id = irs;           // Restore identifier restriction status.
            return this;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ToObj.                                                      <summary>
          TASK:                                                             <br/>
                Makes an object of class T and fills corresponding properties 
                of it from Stl.                                             <para/>
          ARGS:                                                             <br/>
                T             : Class : Target object class.                <br/>
                ignoreMissing : bool  : When true : Do not raise exception 
                                        if a property is missing in Stl. 
                                        :DEF:false.                         <para/>
          INFO:                                                             <br/>
                Properties of class T are sought in Keys of Stl.            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public T ToObj<T>(bool ignoreMissing = false) where T : new() {
            return (T)ToObj(MakeObject(typeof(T)), ignoreMissing);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ToObj.                                                      <summary>
          TASK:                                                             <br/>
                Makes an object of class type typ and fills corresponding 
                properties of it from Stl.                                  <para/>
          ARGS:                                                             <br/>
                type          : Type  : Target object class type.           <br/>
                ignoreMissing : bool  : When true : Do not raise exception 
                                        if a property is missing in Stl. 
                                        :DEF:false.                         <para/>
          INFO:                                                             <br/>
                Properties of class T are sought in Keys of Stl.            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public object ToObj(Type typ, bool ignoreMissing = false) {
            return ToObj(MakeObject(typ), ignoreMissing);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ToObj.                                                      <summary>
          TASK:                                                             <br/>
                Fills corresponding properties of an object from Stl.       <para/>
          ARGS:                                                             <br/>
                target        : object  : Target object.                    <br/>
                ignoreMissing : bool    : When true : Do not raise exception 
                                          if a property is missing in Stl. 
                                          :DEF:false.                       <para/>
          INFO:                                                             <br/>
                Properties of class T are sought in Keys of Stl.            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public object ToObj(object target, bool ignoreMissing = false) {
            PropertyInfo[] pLst;
            Type type;
            object v;
            int i;
            
            Chk(target, nameof(target));
            type = target.GetType();
            if (type == typeof(Stl))
                ignoreMissing = true;
            pLst = InstanceProps(type);
            foreach(PropertyInfo p in pLst) {
                if (hasAttr<StlIgnore>(p))
                    continue;
                i = Index(p.Name);
                if (i == -1) {
                    if (ignoreMissing)
                        continue;
                    Exc("E_PROP_MISSING", type.Name + "." + p.Name);
                }
                v = objLst[i];
                p.SetValue(target, SetType(v, p.PropertyType, ignoreMissing));
            }
            return target;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ByObj.                                                      <summary>
          TASK:                                                             <br/>
                Fills Stl from an objects public properties.                <para/>
          ARGS:                                                             <br/>
                source  : object : Source object.                           <br/>
                init    : bool   : Clears Stl before copy if true.:DEF:true.<para/>
          RETV:                                                             <br/>
                        : Stl    : Stl itself.                              <para/>
          INFO:                                                             <br/>
                Shallow copy.                                               <br/>
                Multiple object properties can be merged in to an Stl
                by setting init = false, but either the property names
                should be unique or Stl must accept duplicate keys.
                like myStl = new Stl(false, true, true)                     </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public Stl ByObj(object source, bool init = true) {
            PropertyInfo[] pArr;
            Type type;
            bool irs;

            Chk(source, nameof(source));
            if (init) {
                Clear();
                un = true;
                id = true;
                wr = true;
            }
            irs = id;               // Store identifier restriction status.
            id = false;             // Optimization: Incoming keys are identifiers.
            type = source.GetType();
            pArr = InstanceProps(type);    // never returns null.
            foreach(PropertyInfo p in pArr) {
                if (hasAttr<StlIgnore>(p))
                    continue;
                Add(p.Name, p.GetValue(source));
            }
            id = irs;             // Restore identifier restriction status.
            return this;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ToDictionaryOfStringTypeT.                                  <summary>
          TASK:                                                             <br/>
                Tries to make a <b>Dictionary&lt;string, T&gt;</b> from Stl.<para/>
          ARGS:                                                             <br/>
                T             : object class type for objects.              <br/>
                ignoreMissing : Ignore missing sub properties.:DEF: false.  <para/>
          INFO:                                                             <br/>
                Objects in Stl must be compatible with type T.              <br/>
                If complicated sub property objects are in sub Stl's and    <br/>
                their properties may be incomplete, better call with        <br/>
                ignoreMissing set to true                                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public Dictionary<string, T> ToDictionaryOfStringTypeT<T>(bool ignoreMissing = false) {
            int i;
            int l;
            T t;
            Dictionary<string, T> d;

            d = new Dictionary<string, T>();
            l = Count;
            for(i = 0; i < l; i++) {
                if (objLst[i] is T obj) {
                    t = obj;
                } else {
                    t = SetType<T>(objLst[i], ignoreMissing);
                }
                d.Add(keyLst[i], t);
            }
            return d;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ToListOfKeyValuePairsOfString.                              <summary>
          TASK:                                                             <br/>
                Makes a List of KeyValuePair of string, string and fills it <br/>
                with the keys and values from Stl.                          <para/> 
          INFO:                                                             <br/>
                Used at Http request form data construction in c#.          <br/>
                Values are obtained by ToString();                          <br/>
                Kvs is an alias to KeyValuePair of string, string.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public List<Kvs> ToListOfKeyValuePairsOfString() {
            int i;
            int c;
            List<Kvs> r;

            c = Count;
            r = new List<Kvs>();
            for(i = 0; i < c; i++)
                r.Add(new Kvs(keyLst[i], objLst[i].ToString()));
            return r;
        }
        #endregion

        #region IDictionary interface.
        /*————————————————————————————————————————————————————————————————————————————
            —————————————————————————————————————————
            |   IDictionary interface.              |
            —————————————————————————————————————————
            Recommended use: When Stl is with unique keys.
        ————————————————————————————————————————————————————————————————————————————*/
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Add                                                         <summary>
          TASK:                                                             <br/>
                Adds a Key-value pair to Stl.                               <para/>
          ARGS:                                                             <br/>
                Key  : string : Key to Add.                                 <br/>
                value: object : Object to Add.          :DEF: null.         <para/>
          INFO:                                                             <br/>
                IDictionary interface. Better use Add().                    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public void Add(string key, object value) => Add(key, value, -1);

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Remove                                                      <summary>
          TASK:                                                             <br/>
                Deletes first occurence of a Key and                        <br/>
                associated object from the list.                            <para/>
          ARGS:                                                             <br/>
                Key     : String : Key.                                     <para/>
          RETV:                                                             <br/>
                        : bool   : true if Key found and pair deleted.      <para/>
          INFO:                                                             <br/>
                IDictionary interface. Better use Delete().                 </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool Remove(string key) => Delete(key);

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Remove                                                      <summary>
          TASK:                                                             <br/>
                Deletes first occurence of a Key-value pair.                <para/>
          ARGS:                                                             <br/>
                item: KeyValuePair of string,object: Pair to remove.        <para/>
          RETV:                                                             <br/>
                    : bool   : true if pair found and deleted.              <para/>
          INFO:                                                             <br/>
                IDictionary interface. Look DeletePair().                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool Remove(Kvp item) => DeletePair(item.Key, item.Value);

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Add                                                         <summary>
          TASK:                                                             <br/>
                Adds a Key-value pair to Stl.                               <br/>
          ARGS:                                                             <br/>
                KeyValuePair of string,object: item.                        <br/>
          INFO:                                                             <br/>
                IDictionary interface. Look Add(), AddPair().               </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public void Add(Kvp item) => Add(item.Key, item.Value);
        
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ContainsKey                                                 <summary>
          TASK: IDictionary interface. Better use Has().                    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool ContainsKey(string key) => Has(key);

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Contains                                                    <summary>
          TASK: IDictionary interface. Better use hasPair().                </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool Contains(Kvp item) => hasPair(item.Key, item.Value);
        
        /**———————————————————————————————————————————————————————————————————————————
          PROP: IsReadOnly                                                  <summary>
          GET : IDictionary interface. Always false.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public bool IsReadOnly => false;
        
        /**———————————————————————————————————————————————————————————————————————————
          PROP: Keys                                                        <summary>
          GET : Gets a shallow copy of Keys list.                           <br/>
          INFO: IDictionary interface.                                      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public ICollection<string> Keys => keyLst.CopyToList();
        
        /**———————————————————————————————————————————————————————————————————————————
          PROP: Values                                                      <summary>
          GET : Gets a shallow copy of values (objects) list.               <br/>
          INFO: IDictionary interface.                                      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public ICollection<object> Values => objLst.CopyToList();
        
        /**<inheritdoc/>*/
        public IEnumerator<Kvp> GetEnumerator() => new StlEnumeratorKVP(this);
        
        /**<inheritdoc/>*/
        IEnumerator IEnumerable.GetEnumerator() => keyLst.GetEnumerator();
        
        /**<inheritdoc/>*/
        public bool TryGetValue(string key, out object value) {
            int i;
            bool b;

            i = Index(key);
            b = (i > -1);
            value = b ? objLst[i] : null;
            return (b);
        }
        
        /**<inheritdoc/>*/
        public void CopyTo(Kvp[] array, int arrayIndex) {
            int i,
                j = 0,
                c;

            c = Count;
            for(i = arrayIndex; i < c; i++)
                array[j] = new Kvp(keyLst[i], objLst[i]);
        }
        
        /**<inheritdoc/>*/
        public object this[string key] {
            get => Obj(key, 0);
            set {
                int i;
                i = Index(key);
                if (i < 0) {
                    Add(key, value);
                    return;
                }
                if (!wr) {
                    Exc("E_STL_NO_OVR", key);
                }
                objLst[i] = value;
            }
        }
        #endregion

    }   // end class Stl

    /**———————————————————————————————————————————————————————————————————————————
      CLASS : StlEnumeratorKVP.                                         <summary>
      USAGE : Stl Enumeration Support for IDictionary interface.        </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public class StlEnumeratorKVP: IEnumerator<Kvp> {

        private Stl lst;
        private int idx;

        /**<inheritdoc/>*/
        public Kvp Current => new (lst.keyLst[idx], lst.objLst[idx]);

        object IEnumerator.Current => Current;
        /**<summary> Constructor for enumerator class. </summary>*/
        public StlEnumeratorKVP(Stl stl) {
            lst = stl;
            idx = -1;
        }

        /**<inheritdoc/>*/
        public void Reset() {
            idx = -1;
        }

        /**<inheritdoc/>*/
        public void Dispose() {
            Dispose(true);
        }

        /**<inheritdoc/>*/
        protected virtual void Dispose(bool disposing) { }

        /**<inheritdoc/>*/
        public bool MoveNext() {
            if ((idx + 1) == lst.Count)
                return false;
            ++idx;
            return true;
        }
    }   // end class StlEnumeratorKVP.

    /**———————————————————————————————————————————————————————————————————————————
      CLASS :   NestedStlConverter.                                     <summary>
      USAGE :                                                           <br/>
        In jsonDeserializer, for supporting nested Stl.                 <br/>
        This replaces all untyped sub-list elements that can be         <br/>
        IDictionary string,object lists with Stl.                       </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public class NestedStlConverter: CustomCreationConverter<IDso> {

        /**<inheritdoc/>*/
        public override IDso Create(Type objectType) {
            return new Stl();
        }
    
        /**<inheritdoc/>*/
        public override bool CanConvert(Type objectType) {
            return objectType == typeof(object) || base.CanConvert(objectType);
        }
        
        /**<inheritdoc/>*/
        public override object ReadJson(JsonReader reader, Type objectType,
                object existingValue, JsonSerializer serializer) {
            if (reader.TokenType == JsonToken.StartObject || 
                reader.TokenType == JsonToken.Null)
                return base.ReadJson(reader, objectType, existingValue, serializer);
            return serializer.Deserialize(reader);
        }
    }   // End class NestedStlConverter.
}
