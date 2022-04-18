using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;
using static Tore.Core.Sys;

namespace Tore.Core {

    /**——————————————————————————————————————————————————————————————————————————— 
        CLASS:  Json                                                    <summary>
        USAGE:                                                          <br/>
                Json deserialization and serialization routines.        <br/>
                It also addresses cascading Stl conversion problems.    </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Json {

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Deserialize [static]                                        <summary>
          TASK:                                                             <br/>
                Deserializes an object from Json with cascading Stl support.<para/>
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
                res = JsonConvert.DeserializeObject<T>(val, Stl.stlJsonSettings);
            } catch (Exception e) {
                Exc("E_JSON_DESERIALIZE", typeof(T).Name, e);
                throw;
            }
            return res;
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
