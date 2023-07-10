/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————
    |  Json : C# Json Utility methods library    |
    ——————————————————————————————————————————————

© Copyright 2023 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202310071044 (v8.0.1).
License             : MIT.

History             :
202310071044: IVT   : Added Json.Populate.
202303191158: IVT   : Isolated from Tore.Core.Sys.cs
————————————————————————————————————————————————————————————————————————————*/

using System;

using Newtonsoft.Json;

using static Tore.Core.Sys;

namespace Tore.Core {

    /**——————————————————————————————————————————————————————————————————————————— 
        CLASS:  Json                                                    <summary>
        USAGE:                                                          <br/>
                Json deserialization and serialization routines.        <br/>
                It also addresses cascading StrLst conversion problems. </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Json {

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Deserialize [static]                                        <summary>
          TASK:                                                             <br/>
                Deserializes an object from Json with StrLst support.       <para/>
          ARGS:                                                             <br/>
                T   : Type (Class)  : Expected object type.                 <br/>
                val : string        : Json string.                          <para/>
          RETV:                                                             <br/>
                    : object        : Object of Type T if possible.*        <para/>
          INFO:                                                             <br/>
                * If val is null or empty, returns default.                 <para/>
          WARN:                                                             <br/>
                Throws E_JSON_DESERIALIZE on failure.                       </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T Deserialize<T>(string val) {
            T res;

            if (val.IsNullOrWhiteSpace())
                return default;
            Chk(typeof(T), "T", "E_JSON_DESERIALIZE");
            try {
                res = JsonConvert.DeserializeObject<T>(val, StrLst.strLstJsonSettings);
            } catch (Exception e) {
                Exc("E_JSON_DESERIALIZE", typeof(T).Name, e);
                throw;
            }
            return res;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Populate [static]                                           <summary>
          TASK:                                                             <br/>
                Populates an object from Json with StrLst support.          <para/>
          ARGS:                                                             <br/>
                obj : object        : Object to populate.                   <br/>
                val : string        : Json string.                          <para/>
          RETV:                                                             <br/>
                    : object        : obj populated.                        <para/>
          INFO:                                                             <br/>
                * If val is null or empty, returns obj as is.               <para/>
          WARN:                                                             <br/>
                Throws E_JSON_POPULATE on failure.                          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object Populate(object obj, string val) {
            Chk(obj, "obj", "E_JSON_POPULATE");
            if (val.IsNullOrWhiteSpace())
                return obj;
            try {
                JsonConvert.PopulateObject(val, obj, StrLst.strLstJsonSettings);
            } catch (Exception e) {
                Exc("E_JSON_POPULATE",obj.GetType().Name, e);
                throw;
            }
            return obj;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Serialize [static]                                          <summary>
          TASK:                                                             <br/>
                Serializes an object to Json.                               <para/>
          ARGS:                                                             <br/>
                obj : object        : Object to convert to Json.            <para/>
          RETV:                                                             <br/>
                    : string        : Json string.                          <para/>
          WARN:                                                             <br/>
                Throws E_JSON_SERIALIZE on failure.                         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string Serialize(object obj) {
            string res;

            try {
                res = JsonConvert.SerializeObject(obj, Formatting.Indented);
            } catch (Exception e) {
                Exc("E_JSON_SERIALIZE", obj?.GetType().Name, e);
                throw;
            }
            return res;
        }
    }
}
