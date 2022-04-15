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
          FUNC: deserialize [static]                                        <summary>
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
        public static T deserialize<T>(string val) {
            //JsonSerializerSettings ser;
            T res;

            if (val.isNullOrWhiteSpace())
                return default;
            chk(typeof(T), "T", "E_JSON_DESERIALIZE");
            /*
            ser = (typeof(T).IsAssignableTo(typeof(Stl))) ?
                        Stl.stlJsonSettings :
                        null;
            */
            try {
                res = JsonConvert.DeserializeObject<T>(val, Stl.stlJsonSettings);
            } catch (Exception e) {
                exc("E_JSON_DESERIALIZE", typeof(T).Name, e);
                throw;
            }
            return res;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: serialize [static]                                          <summary>
          TASK:                                                             <br/>
                Serializes an object to Json.                               <para/>
          ARGS:                                                             <br/>
                obj : object        : Object to convert to Json.            <para/>
          RETV:                                                             <br/>
                    : string        : Json string.                          <para/>
          WARN:                                                             <br/>
                Throws E_JSON_SERIALIZE on failure.                         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string serialize(object obj) {
            string res;

            try {
                res = JsonConvert.SerializeObject(obj, Formatting.Indented);
            } catch (Exception e) {
                exc("E_JSON_SERIALIZE", obj.GetType().Name, e);
                throw;
            }
            return res;
        }
    }
}
