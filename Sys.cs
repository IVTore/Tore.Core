/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————
    |   Sys : C# Utility methods library         |
    ——————————————————————————————————————————————

© Copyright 2020 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202006131333
License             : MIT.

History             :
202003101700: IVT   : Revision.
202006131333: IVT   : Removal of unnecessary usings.
————————————————————————————————————————————————————————————————————————————*/

using System;
using System.Reflection;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Tore.Core {

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Sys [Static]                                            <summary>
        USAGE:                                                          <br/>
                Contains a library of static utility methods treated as <br/>
                global functions which is used for managing:            <para/>
                Exceptions                                              <br/>
                Reflection                                              <br/>
                Type juggling                                           <br/>
                Attributes                                              <br/>
                Simple File Load Save                                   <br/>
                Time,                                                   <br/>
                Date,                                                   <br/>
                
                and many others.                                        <para/>
                The best way of using them is by adding:                <br/>
                using static Tore.Core.Sys;                             <br/>
                to the source file.                                     </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Sys {

        /*————————————————————————————————————————————————————————————————————————————
          Handy constants.
        ————————————————————————————————————————————————————————————————————————————*/
        /**———————————————————————————————————————————————————————————————————————————
          CONST: CR                                                         <summary>
          USE  : Constant string for carriage return character 0x0D (\r)    </summary> 
        ————————————————————————————————————————————————————————————————————————————*/
        public const string CR = "\r";
        /**———————————————————————————————————————————————————————————————————————————
          CONST: LF                                                         <summary>
          USE  : Constant string for line feed character 0x0A (\n)          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public const string LF = "\n";
        /**———————————————————————————————————————————————————————————————————————————
          CONST: CRLF                                                       <summary>
          USE  : Constant string for carriage return line feed sequence.   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public const string CRLF = CR + LF;

        /**———————————————————————————————————————————————————————————————————————————
          PROP: isDebug [static] : bool.                                    <summary>
          TASK: GET: Returns if program is a DEBUG compilation.             </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool isDebug {
            get {
                bool d = false;
        #if DEBUG
                d = true;
        #endif
                return d;
            }
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: HexToInt [static]                                           <summary>
          TASK: Returns integer from hex string.                            <para/>
          ARGS: hex : String        : hexadecimal number.                   <para/>
          RETV:     : int           : integer value.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static int HexToInt(string hex) {
            return int.Parse(hex, NumberStyles.HexNumber);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: IntToHex [static]                                           <summary>
          TASK: Returns hex string from integer.                            <para/>
          ARGS:                                                             <br/>
                i       : int       : integer value.                        <br/>
                digits  : int       : Number of hex digits.                 <para/>
          RETV:         : String    : Formatted hexadecimal string.         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string IntToHex(int i, int digits) {
            return i.ToString("x" + digits.ToString());
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: HexStrToByteArr [static]                                    <summary>
          TASK: Converts a hex string to byte array.                        <para/>
          ARGS: hex : string    : Hex string.                               <para/>
          RETV:     : byte[]    : Byte array.                               <para/>
          INFO:                                                             <br/>
                *   Hex digit letters should be <b> CAPITAL </b>.           <br/>
                *   Throws exception if hex string is invalid.              <br/>
                *   'A'- 0xA => 65 - 10 = 55.                               </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static byte[] HexStrToByteArr(string hex) {
            int i,      // iterator.
                p,      // position.
                l;      // length.
            byte[] a;   // array.

            int n2i(int pos) {
                char n = hex[pos];
                if ((n >= '0') && (n <= '9'))
                    return (n - '0');
                if ((n >= 'A') && (n <= 'F'))
                    return (n - 55);
                Exc("E_INV_NIBBLE", $"hex[{pos}] = {n}");
                return 0;
            }

            Chk(hex, "hex");
            l = hex.Length;
            if ((l % 2) != 0)
                Exc("E_INV_ARG", "hex");
            l /= 2;
            a = new byte[l];
            for(i = 0; i < l; i++) {
                p = i * 2;
                a[i] = (byte)((n2i(p) << 4) | n2i(p + 1));
            }
            return a;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ByteArrToHexStr [static]                                    <summary>
          TASK: Converts a byte array to capital hex string.                <para/>
          ARGS: arr : byte[]    : Byte array.                               <para/>
          RETV:     : string    : Hex string.                               <para/>
          INFO: 'A'- 0xA => 65 - 10 = 55.                                   <para/>
          WARN: Throws exception if byte array is invalid.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string ByteArrToHexStr(byte[] arr) {
            int i,              // iterator.
                l,              // length.
                n;              // nibble value.
            StringBuilder s;    // String builder.

            Chk(arr, "arr");
            l = arr.Length;
            s = new StringBuilder(l * 2);
            for(i = 0; i < l; i++) {
                n = (arr[i] & 0xF0) >> 4;
                s.Append((char)((n < 0xA) ? (n + '0') : (n + 55)));
                n = (arr[i] & 0x0F);
                s.Append((char)((n < 0xA) ? (n + '0') : (n + 55)));
            }
            return s.ToString();
        }

        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————————
            |   Exception Subsystem  |
            ——————————————————————————
            A Practical Exception Subsystem.
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          TYPE: ExcInterceptorDelegate [delegate]                           <summary>
          TASK: Exception Interceptor Type.                                 </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public delegate void ExcInterceptorDelegate(Exception e, Stl dta); 

        /**———————————————————————————————————————————————————————————————————————————
          PROP: excInterceptor                                              <summary>
          USE: Exception Interceptor property, for logging etc.             </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static ExcInterceptorDelegate excInterceptor { get; set; } = null;

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: HasExcData [static]                                          <summary>
          TASK:                                                             <br/>
                If an exception is generated or processed through           <br/>
                <b>Exc</b>, it Has extra data and this will return true.    <para/>
1         ARGS:                                                             <br/>
                e   : Exception : Any Exception.                            <para/>
          RETV:                                                             <br/>
                    : bool      : True if exception Has extra data.         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool HasExcData(Exception? e) {
            return (e != null) && (e.Data.Contains("dta"));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: GetExcDta [static]                                          <summary>
          TASK:                                                             <br/>
                If an exception is generated or processed through           <br/>
                <b>Exc</b>, it Has extra data and this will return it.      <para/>
1         ARGS:                                                             <br/>
                e   : Exception : Any Exception.                            <para/>
          RETV:                                                             <br/>
                    : Stl       : Extra exception data or null              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Stl GetExcDta(Exception? e) {
            if (!HasExcData(e))
                return null;
            return (Stl)(e.Data["dta"]);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Exc [static]                                                <summary>
          TASK: Raises and/or logs an exception with extra control.         <para/>
          ARGS:                                                             <br/>
                tag : String    : Message selector.                         <br/>
                inf : String    : Extra information.                        <br/>
                e   : Exception : A thrown Exception if called in *catch*.  <para/>
          RETV: Look info.                                                  <para/>
          INFO: * Algorithm:                                                <br/>
                Finds the caller from stack frame.                          <br/>
                If e is null builds an exception.                           <br/>
                Collects exception data into exception object.              <br/>
                If an exception interceptor delegate is defined:
                    Interceptor function is called.                         <br/>
                Writes exception info via dbg().                            <br/>
                If e was null at entry
                    *throws* the exception built.                           <br/>
                else
                    returns null (must be *re-thrown* if needed).           <br/>
                                                                            <br/>
                * Collected Data: at e.Data["dta"] as a Stl*.               <br/>
                    "Exc" = Class name of exception.                        <br/>
                    "msg" = Exception message.                              <br/>
                    "tag" = Exception tag.                                  <br/>
                    "inf" = Exception info.                                 <br/>
                    "loc" = Call location (Class and method).               <para/>
                *Stl is a string associated object list class. 
                 Info about Stl can be found at Stl.cs.                     </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object Exc(string tag = "E_NO_TAG", 
                                 string inf = "", 
                                 Exception e = null) {
            Exception cex;
            Stl dta;
            MethodBase met;

            cex = e ?? new ToreCoreException(tag);      // If no exception make one.
            if (HasExcData(cex))                         // If exception already processed
                return null;                            // return.
            dta = new Stl(){                            // Collect data.
                {"exc", cex.GetType().FullName},
                {"msg", cex.Message},
                {"tag", tag},
                {"inf", inf}
            };
            met = new StackTrace(2)?.GetFrame(0)?.GetMethod();
            if (met != null) { 
                if (met.Name.Equals("chk") || met.Name.StartsWith("<"))
                    met = new StackTrace(3)?.GetFrame(0)?.GetMethod();
            }
            if (met != null)
                dta.Add("loc", $"{met.DeclaringType?.Name}.{met.Name}");
            excInterceptor?.Invoke(cex, dta);                   // if assigned, invoke.
            cex.Data.Add("dta", (object)dta);
            if (isDebug)
                ExcDbg(dta);
            if (e == null)                              // If we made the exception
                throw cex;                              // throw it
            return null;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ExcDbg [static]                                             <summary>
          TASK: Outputs exception info via dbg() method                     <para/>
          ARGS: ed  : Stl       : Exception data in Stl form.               <para/>
          INFO: The ed Stl can be found in exception.Data["dta"].           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void ExcDbg(Stl ed) {
            List<string> dia,
                        inl;
            string inf = (string)ed.Obj("inf");

            dia = new List<string>();
            dia.Add("_LINE_");
            dia.Add("[EXC]: " + (string)ed.Obj("exc"));
            dia.Add("[MSG]: " + (string)ed.Obj("msg"));
            dia.Add("[TAG]: " + (string)ed.Obj("tag"));
            if (inf.IsNullOrWhiteSpace()) {
                dia.Add("[INF]: - .");
            } else {
                inl = new List<string>(inf.Split('\n'));
                dia.Add("[INF]: " + inl[0].Trim());
                inl.RemoveAt(0);
                if (inl != null) {
                    foreach(string s in inl)
                        dia.Add("       " + s.Trim());
                }
            }
            dia.Add("[LOC]: " + (string)ed.Obj("loc"));
            dia.Add("_LINE_");
            Dbg(dia);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Chk [static]                                            <summary>
          TASK:                                                         <br/>
                Checks argument and raises exception if it is null 
                or empty.                                               <para/>
          ARGS:                                                         <br/>
                arg : object        : Argument to check validity.       <br/>
                inf : string        : Exception info if arg invalid.    <br/>
                tag : string        : Exception tag if arg invalid.
                    :DEF: "E_INV_ARG".                                  <para/>
          INFO:                                                         <br/>
                In case of strings, white spaces are not welcome.       <br/>
                In case of Guids, empty Guid's are not welcome.         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void Chk(object arg, string inf,
                string tag = "E_INV_ARG") {

            if  ((arg == null) ||
                ((arg is string) && (String.IsNullOrWhiteSpace((string)arg))) ||
                ((arg is Guid) && (((Guid)arg).Equals(Guid.Empty))))
                Exc(tag, inf);
        }

        private const string dbgLine = "————————————————————————————————" +
                                       "————————————————————————————————";
        /**———————————————————————————————————————————————————————————————————————————
          TYPE: DbgInterceptorDelegate [delegate]                           <summary>
          TASK: Logging Interceptor delegate Type for dbg() outputs.        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public delegate void DbgInterceptorDelegate(string s);

        /**———————————————————————————————————————————————————————————————————————————
          PROP: dbgInterceptor                                              <summary>
          USE : Logging Interceptor delegate property for dbg() outputs.    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static DbgInterceptorDelegate dbgInterceptor { get; set; } = null;

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Dbg [static]                                                <summary>
          TASK: Writes output to debug console and trace log.               <para/>
          ARGS: msg : String[]  : Message collection.                       <para/>
          INFO: Uses Debug.Write.  
                Has a log interceptor delegate which may be useful in case. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void Dbg(params string[] msg) {
            StringBuilder b;
            string s;

            b = new StringBuilder();
            if (msg != null) {
                foreach(string m in msg) {
                    switch(m) {
                    case "_LINE_":
                    b.AppendLine(dbgLine);
                    break;
                    default:
                    b.AppendLine(m);
                    break;
                    }
                }
            }
            s = b.ToString();
            Debug.Write(s);
            dbgInterceptor?.Invoke(s);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Dbg [static]                                                <summary>
          TASK: Writes output to debug console and to log if linked.        <para/>
          ARGS: lst : List of String  : Message collection.                 <para/>
          INFO: Uses Console.Write if in container, otherwise Debug.Write.  
                Has a log interceptor delegate which may be useful in case. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void Dbg(List<string> lst) {
            if (lst == null)
                return;
            Dbg(lst.ToArray());
        }

        

        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————————————————————
            |   Reflection utility functions     |
            ——————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: appName [static].                                           <summary>
          TASK: Returns application name.                                   <para/>
          RETV:     : string : Application name.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string appName() {
            return Assembly.GetEntryAssembly().GetName().Name;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: appPath [static].                                           <summary>
          TASK: Returns application path.                                   <para/>
          RETV:     : string : Application path.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string appPath() {
            return AppDomain.CurrentDomain.BaseDirectory;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: attr [static]                                               <summary>
          TASK: Fetches an attribute of a member.                           <para/>
          ARGS:                                                             <br/>
                attrType    : Type          :   Type of attribute.          <br/>
                memInfo     : MemberInfo    :   Member information.         <para/>
          RETV:             : Attribute     :   The attribute or null if
                                                not found.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Attribute attr(Type attrType, MemberInfo memInfo) {
            Chk(attrType, "attrType");
            Chk(memInfo, "memInfo");
            return memInfo.GetCustomAttribute(attrType, true);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: attr [static]                                               <summary>
          TASK: Fetches an attribute of a member.                           <para/>
          ARGS:                                                             <br/>
                T           : Type(Class)   :   Type of attribute.          <br/>
                memInfo     : MemberInfo    :   Member information.         <para/>
          RETV:             : Attribute     :   The attribute or null if
                                                not found.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Attribute attr<T>(MemberInfo memInfo) where T : Attribute {
            return memInfo.GetCustomAttribute(typeof(T), true);
        }


        /**———————————————————————————————————————————————————————————————————————————
          FUNC: hasAttr [static]                                            <summary>
          TASK: Checks if a member Has an attribute.                        <para/>
          ARGS:                                                             <br/>
                T           : Type (Class)  : Type of attribute.            <br/>
                memInfo     : MemberInfo    : Member information.           <para/>
          RETV:             : bool          : True if attribute exists 
                                              else false.                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool hasAttr<T>(MemberInfo memInfo) {
            return (attr(typeof(T), memInfo) != null);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: nameInAttr [static]                                         <summary>
          TASK: Fetches name in a NameMetadata attribute of any member.     <para/>
          ARGS:                                                             <br/>
                T           : Type (Class)  : Type of attribute.            <br/>
                memInfo     : MemberInfo    : Member information.           <para/>
          RETV:             : String        : If attribute exists, 
                                              name in attribute else null.  <para/>
          INFO:                                                             <br/>
                Attribute class must be descendant of NameMetadata class.
                Otherwise the return value will be null.                    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string nameInAttr<T>(MemberInfo memInfo) {
            Attribute a;

            a = attr(typeof(T), memInfo);
            return (a is NameMetadata nmd) ? nmd.name : null;
        }

        private static Type fetchType(object src, string name) {
            if (src == null)
                Exc("E_TYPE_SRC_NULL", name);
            return (src is Type t) ? t : src.GetType();
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: instanceProp [static]                                       <summary>
          TASK: Fetches Public Property info from a class.                  <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                propNam : String        : Instance Property name.           <br/>
                inherit : bool          : Include base classes to the
                                          search of property.:DEF:true.     <para/>
          RETV:         : PropertyInfo  : If exists, the property info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo instanceProp(object propSrc,
                                                string propNam,
                                                bool inherit = true) {
            Chk(propNam, nameof(propNam));
            return fetchType(propSrc, nameof(propSrc)).GetProperty(
                    propNam,
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: staticProp [static]                                         <summary>
          TASK: Fetches Public static Property info from a class.           <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                propNam : String        : Static Property name.             <br/>
                inherit : bool          : Include base classes to the
                                          search of property.:DEF:true.     <para/>
          RETV:         : PropertyInfo  : If exists, the property info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo staticProp(object propSrc,
                                                string propNam,
                                                bool inherit = true) {
            Chk(propNam, nameof(propNam));
            return fetchType(propSrc, nameof(propSrc)).GetProperty(
                    propNam,
                    BindingFlags.Public |
                    BindingFlags.Static |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: staticField [static]                                        <summary>
          TASK: Fetches Public Static Field info from a class.              <para/>
          ARGS:                                                             <br/>
                fldSrc  : Object        : Type or Instance.                 <br/>
                fldNam  : String        : Static Field name.                <br/>
                inherit : bool          : Include base classes to the
                                          search of field. :DEF:true.       <para/>
          RETV:         : fieldInfo     : If exists, the field info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static FieldInfo staticField(object fldSrc,
                                            string fldNam,
                                            bool inherit = true) {
            Chk(fldNam, nameof(fldNam));
            return fetchType(fldSrc, nameof(fldSrc)).GetField(
                fldNam,
                BindingFlags.Public |
                BindingFlags.Static |
                (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: instanceMethod [static]                                     <summary>
          TASK: Fetches Public Method info from a class.                    <para/>
          ARGS:                                                             <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Instance method name.             <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:         : MethodInfo    : If exists, the method info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static MethodInfo instanceMethod(object metSrc,
                                                string metNam,
                                                bool inherit = true) {
            Chk(metNam, nameof(metNam));
            return fetchType(metSrc, nameof(metSrc)).GetMethod(
                metNam,
                BindingFlags.Public |
                BindingFlags.Instance |
                (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: staticMethod [static]                                       <summary>
          TASK: Fetches Public Static Method info from a class.             <para/>
          ARGS:                                                             <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Static Method name.               <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:         : MethodInfo    : If exists, the method info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static MethodInfo staticMethod(object metSrc,
                                              string metNam,
                                              bool inherit = true) {
            Chk(metNam, nameof(metNam));
            return fetchType(metSrc, nameof(metSrc)).GetMethod(
                metNam,
                BindingFlags.Public |
                BindingFlags.Static |
                (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**——————————————————————————————————————————————————————————————————————————
          FUNC: instanceDelegate [static]                                   <summary>
          TASK: Finds Public Instance Method of a class                     <br/>
                and returns it as a delegate of Type T.                     <para/>
          ARGS:                                                             <br/>
                T       : Type          : Delegate type.                    <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Instance method name.             <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:         : T             : If method exists, the delegate,
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T instanceDelegate<T>(object metSrc,
                                            string metNam,
                                            bool inherit = true)
                                            where T : Delegate {
            return instanceMethod(metSrc, metNam, inherit)?.CreateDelegate<T>();
        }

        /**——————————————————————————————————————————————————————————————————————————
          FUNC: staticDelegate [static]                                     <summary>
          TASK: Finds Public Static Method of a class                       <br/>
                and returns it as a delegate of Type T.                     <para/>
          ARGS:                                                             <br/>
                T       : Type          : Delegate type.                    <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Static method name.               <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:         : T             : If method exists, the delegate,
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T staticDelegate<T>(object metSrc, string metNam, bool inherit = true)
                                          where T : Delegate {
            return staticMethod(metSrc, metNam, inherit)?.CreateDelegate<T>();
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: makeObject [static]                                         <summary>
          TASK: Builds an object of a given type.                           <para/>
          ARGS:                                                             <br/>
                template: object            : Object or Type of object.     <br/>
                args    : params object[]   : Constructor parameters if any.<para/>
          RETV:         : object            : Object constructed.           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object makeObject(object template, params object[] args) {
            return Activator.CreateInstance(fetchType(template, nameof(template)), args);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: setInstProp [static]                                        <summary>
          TASK: Sets an instance property on target instance to a value.    <para/>
          ARGS:                                                             <br/>
                propNam : string    : Property name.                        <br/>
                tar     : object    : Instance.                             <br/>
                val     : object    : Value to set to target's property.    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void setInstProp(string propNam, object tar, object val) {
            PropertyInfo inf;

            if ((tar == null) || (tar is Type))
                Exc("E_INV_INSTANCE", "tar");
            inf = instanceProp(tar, propNam, true);
            Chk(inf, tar.GetType().Name + "." + propNam, "E_PROP_INV");
            inf.SetValue(tar, setType(val, inf.PropertyType));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: setInstProp [static]                                        <summary>
          TASK: Sets an instance property on target instance to a value.    <para/>
          ARGS:                                                             <br/>
                inf     : PropertyInfo  : Property info.                    <br/>
                tar     : object        : Instance.                         <br/>
                val     : object        : Value to set to target property.  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void setInstProp(PropertyInfo inf, object tar, object val) {
            Type tty;

            if ((tar == null) || (tar is Type))
                Exc("E_INV_INSTANCE", "tar");
            Chk(inf, "inf");
            tty = tar.GetType();
            if (inf.DeclaringType != tty) {
                Exc("E_PROP_INV_CLASS",
                    inf.DeclaringType.Name + "!=" + tty.Name);
            }
            inf.SetValue(tar, setType(val, inf.PropertyType));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: setStatProp [static]                                        <summary>
          TASK: Sets a static property on target class to a value.          <para/>
          ARGS:                                                             <br/>
                propNam : string    : Property name.                        <br/>
                tar     : object    : Class Type or Instance.               <br/>
                val     : object    : Value to set to target's property.    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void setStatProp(string propNam, object tar, object val) {
            PropertyInfo inf;

            inf = staticProp(tar, propNam, true);
            if (inf == null) 
                Exc("E_INV_PROP", fetchType(tar, nameof(tar)).Name + "." + propNam);
            inf.SetValue(null, setType(val, inf.PropertyType));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: setInstPropType [static]                                    <summary>
          TASK: Checks type of a value and if it is not compatible to
                property type on target object class, tries to convert it.  <para/>
          ARGS:                                                             <br/>
                propNam : string    : Property name in object.              <br/>
                tar     : object    : Target object type or object.         <br/>
                val     : object    : Value to convert to property type.    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object setInstPropType(string propNam, object tar, object val) {
            PropertyInfo inf;
 
            inf = instanceProp(tar, propNam, true);
            if (inf == null)
                Exc("E_INV_PROP", fetchType(tar, nameof(tar)).Name + "." + propNam);
            return setType(val, inf.PropertyType);
        }

        // Used in setType...
        private static readonly Type NULLABLE = typeof(Nullable<>);
        
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: setType [static]                                            <summary>
          TASK: Checks type of a value and if required, tries to change it. <para/>
          ARGS:                                                             <br/>
                val : object    : Value to check and if required convert.   <br/>
                typ : Type      : Expected value type.                      <br/>
                ignoreMissing: bool : If value or subvalues is Stl and 
                                      will be assigned to a sub object,
                                      true    : ignores missing properties
                                      false   : raises exception.           <para/>
          RETV:     : object        : Value of Type typ if possible.        <para/>
          WARN: Throws E_TYPE_CONV on failure.                              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object setType(object val,
                                     Type typ,
                                     bool ignoreMissing = false) {

            bool n = ((val == null) || (val is DBNull));
            Type v = n ? null : val.GetType();
            
            if ((typ.IsGenericType)
            && (typ.GetGenericTypeDefinition() == NULLABLE)) {
                if (n)
                    return null;
                typ = Nullable.GetUnderlyingType(typ);
            }
            if (n) {                                    // If null,
                val = (typ.IsValueType) ?               // If value
                        makeObject(typ) :               // make a default,
                        null;                           // else null.
                return val;
            }
            if (typ == v)                               // Fast check.
                return val;
            if (typ.IsAssignableFrom(v))                // Is it?
                return val;
            try {                                       // Try to convert.
                if (val is JToken tok)                  // If coming from json, 
                    return tok.ToObject(typ);           // call json converter.
                if (typ == typeof(Guid)) {              // If guid,
                    if (val is string str)              // string support only.
                        return Guid.Parse(str);
                }
                if (val is Stl lst)                     // If Stl.
                    return lst.ToObj(typ, ignoreMissing);
                return Convert.ChangeType(val, typ);    // Otherwise...
            } catch(Exception e) {
                Exc("E_TYPE_CONV",
                    $"{val?.GetType()?.Name} => {typ?.Name}",
                    e);
                throw;
            }
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: setType [static]                                            <summary>
          TASK: Checks type of a value and if required, tries to change it. <para/>
          ARGS:                                                             <br/>
                T   : Type (Class)  : Expected value type.                  <br/>
                val : object        : Value to check, if required convert.  <br/>
                ignoreMissing: bool : If value or subvalues is Stl and      
                                      will be assigned to a sub object,
                                      true    : ignores missing properties
                                      false   : raises exception.           <para/>
          RETV:     : object        : Value of Type T if possible.          <para/>
          WARN: Throws E_TYPE_CONV on failure.                              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T setType<T>(object val, bool ignoreMissing = false) {
            return (T)setType(val, typeof(T), ignoreMissing);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: discoverTypes [static]                                      <summary>
          TASK: Finds all Types that are subclass of a base type and        
                defined in any assembly referring to the base assembly.     <para/>
          ARGS:                                                             <br/>
                baseAsm : AssemblyName : This assembly must be referred.    <br/>
                baseType: Type         : Types should be subclass of this.  <para/>
          RETV:         : List of Type : List of types that are subclass
                          of base type and defined in any assembly referring
                          to assembly with name baseAsm.                    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static List<Type> discoverTypes(AssemblyName baseAsm, Type baseType) {
            Assembly[] aArr = AppDomain.CurrentDomain.GetAssemblies();
            AssemblyName[] refs;
            Type[] tArr;
            List<Type> tLst = new List<Type>();

            Chk(baseAsm, "baseAsm");
            Chk(baseType, "baseType");
            foreach(var asm in aArr) {                  // Scan assemblies:
                refs = asm.GetReferencedAssemblies();   // Get referenced.
                if (!refs.Contains(baseAsm))            // If this is not referenced,
                    continue;                           // ignore.
                tArr = asm.GetTypes();                  // Get types in assembly.
                if (tArr == null)                       // If no types in it,
                    continue;                           // ignore.
                foreach(var typ in tArr) {              // Scan types:
                    if (typ.IsSubclassOf(baseType))     // If baseType descendant,
                        tLst.Add(typ);                  // Add to list.
                }
            }
            return tLst;
        }

        /*————————————————————————————————————————————————————————————————————————————
            ————————————————————————
            |  UTC Time functions  |
            ————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        private static CultureInfo timeProvider = CultureInfo.InvariantCulture;

        /**———————————————————————————————————————————————————————————————————————————
          FUNC:   strUtcNow [static]                                        <summary>
          TASK:   Returns UTC current time as string.                       <para/>
          RETV:           : string : UTC time string : "yyyyMMddHHmmssfff". </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string strUtcNow() {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", timeProvider);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC:   strToDateTime [static]                                    <summary>
          TASK:   Returns string formatted time as DateTime.                <para/>
          ARGS:   time    : string    : Time string: "yyyyMMddHHmmssfff".   <para/>
          RETV:           : DateTime  : DateTime translation of time string.</summary> 
        ————————————————————————————————————————————————————————————————————————————*/
        public static DateTime strToDateTime(string time) {
            return DateTime.ParseExact(time, "yyyyMMddHHmmssfff", timeProvider);
        }
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: seconds [static]                                            <summary>
          TASK: Returns long number of seconds since unix epoch + offset.   <para/>
          ARGS: offset  : long  : Offset seconds to Add : DEF : 0.          <para/>
          RETV:         : long  : Seconds since unix epoch + offset.        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static long seconds(long offset = 0) {
            DateTimeOffset dto = new DateTimeOffset(DateTime.UtcNow);
            return (dto.ToUnixTimeSeconds() + offset);
        }

        /*————————————————————————————————————————————————————————————————————————————
            —————————————————————————————————————
            |   UTF8 File utility functions     |
            —————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: loadUtf8File [static]                                       <summary>
          TASK: Reads an Utf8 encoded file to a string.                     <para/>
          ARGS: filespec    : string : filename with path.                  <para/>
          RETV:             : string : Utf8 encoded content of file.        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string loadUtf8File(string fileSpec) {
            Chk(fileSpec, "fileSpec");
            try {
                return File.ReadAllText(fileSpec, Encoding.UTF8);
            } catch(Exception e) {
                Exc("E_SYS_LOAD_FILE", fileSpec, e);
                throw;
            }
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: saveUtf8File [static].                                      <summary>
          TASK: Writes an Utf8 string to a file.                            <para/>
          ARGS:                                                             <br/>
                fileSpec    : string : filename with path.                  <br/>
                text        : string : Utf8 text.                           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void saveUtf8File(string fileSpec, string text) {
            Chk(fileSpec, "fileSpec");
            try {
                File.WriteAllText(fileSpec, text, Encoding.UTF8);
            } catch(Exception e) {
                Exc("E_SYS_SAVE_FILE", fileSpec, e);
                throw;
            }
        }

    }   // End static class Sys.


    /**———————————————————————————————————————————————————————————————————————————
                                                                        <summary>
      CLASS :   NameMetadata [attribute].                               <para/>
      USAGE :   Used as an anchestor class 
                for attributes with a name.                             </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public class NameMetadata:System.Attribute {
        /**———————————————————————————————————————————————————————————————————————————
          PROP: name: string;                                               <summary>
          GET : Returns the string kept in the attribute.                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public string name { get; }
        /**——————————————————————————————————————————————————————————————————————————
          CTOR: NameMetaData                                            <summary>
          TASK: Constructs a NameMetaData attribute.                    <para/>
          ARGS: aName : string : Name to keep in the attribute.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public NameMetadata(string aName) {
            name = aName;
        }

    }   // End NameMetadata Attribute class.

    /**———————————————————————————————————————————————————————————————————————————
                                                                        <summary>
      CLASS :   NameListMetadata [attribute].                           <para/>
      USAGE :   Used as an anchestor class 
                for attributes with a name list.                        </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public class NameListMetadata:System.Attribute {
        /**——————————————————————————————————————————————————————————————————————————
          CTOR: NameMetaData                                            <summary>
          TASK: Constructs a NameMetaData attribute.                    <para/>
          ARGS: names : string[] : Names to keep in the attribute.      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public NameListMetadata(params string[] names) {
            nameList = new List<string>();
            if (names != null) {
                foreach(string n in names)
                    Sys.addUnique(nameList, n);
            }
        }
        /**———————————————————————————————————————————————————————————————————————————
          PROP: nameList: List of string;                                   <summary>
          GET : Returns the strings kept in the attribute.                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public List<string> nameList { get; }

    }   // End NameListMetadata Attribute class.

    /**———————————————————————————————————————————————————————————————————————————
                                                                        <summary>
      CLASS :   ToreCoreException.                                      <para/>
      USAGE :   Tore.Core Exception class to distinguish.               </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public class ToreCoreException: Exception {
        /**<inheritdoc/>*/
        public ToreCoreException():base() { }
        /**<inheritdoc/>*/
        public ToreCoreException(string message): base(message) { }
        /**<inheritdoc/>*/
        public ToreCoreException(string message, Exception inner): base(message, inner) { }
    }

}   // End namespace.