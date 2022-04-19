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
          VAR : isDebug [static readonly]                                   <summary>
          TASK: True if program is a DEBUG compilation.                     </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public readonly static bool isDebug =
        #if DEBUG
            true;
#else
            false;
#endif


        #region Hex conversions.
        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————
            |  Hex conversions.  |
            ——————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: HexToInt [static]                                           <summary>
          TASK: 
                Returns integer from hex string.                            <para/>
          ARGS:                                                             <br/>
                hex : String        : hexadecimal number.                   <para/>
          RETV:                                                             <br/>
                    : int           : integer value.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static int HexToInt(string hex) {
            return int.Parse(hex, NumberStyles.HexNumber);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: IntToHex [static]                                           <summary>
          TASK:                                                             <br/>
                Returns hex string from integer.                            <para/>
          ARGS:                                                             <br/>
                i       : int       : integer value.                        <br/>
                digits  : int       : Number of hex digits.                 <para/>
          RETV:                                                             <br/>
                        : String    : Formatted hexadecimal string.         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string IntToHex(int i, int digits) {
            return i.ToString("x" + digits.ToString());
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: HexStrToByteArr [static]                                    <summary>
          TASK:                                                             <br/>
                Converts a <b> CAPITAL </b>hex string to byte array.        <para/>
          ARGS:                                                             <br/>
                hex : string    : Hex string.                               <para/>
          RETV:                                                             <br/>
                    : byte[]    : Byte array.                               <para/>
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

            Chk(hex, nameof(hex));
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
          TASK:                                                             <br/>
                Converts a byte array to <b> CAPITAL </b> hex string.       <para/>
          ARGS:                                                             <br/>
                arr : byte[]    : Byte array.                               <para/>
          RETV:                                                             <br/>
                    : string    : Hex string.                               <para/>
          INFO:                                                             <br/>
                'A'- 0xA => 65 - 10 = 55.                                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string ByteArrToHexStr(byte[] arr) {
            int i,              // iterator.
                l,              // length.
                n;              // nibble value.
            StringBuilder s;    // String builder.

            Chk(arr, nameof(arr));
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
        #endregion

        #region Exception Subsystem.
        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————————
            |  Exception Subsystem.  |
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
          GET : Gets Exception Interceptor method.                          <br/>
          SET : Sets Exception Interceptor method.                          <br/>
          INFO: Used for logging monitoring etc.                            </summary>
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
        public static bool HasExcData(Exception e) {
            return (e != null) && (e.Data.Contains("dta"));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: GetExcDta [static]                                          <summary>
          TASK:                                                             <br/>
                If an exception is generated or processed through           <br/>
                <b> Exc </b>, it Has extra data and this will return it.    <para/>
          ARGS:                                                             <br/>
                e   : Exception : Any Exception.                            <para/>
          RETV:                                                             <br/>
                    : Stl       : Extra exception data or null              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Stl GetExcDta(Exception e) {
            if (!HasExcData(e))
                return null;
            return (Stl)(e.Data["dta"]);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Exc [static]                                                <summary>
          TASK:                                                             <br/>
                Raises and/or logs an exception with extra control.         <para/>
          ARGS:                                                             <br/>
                tag : String    : Message selector.                         <br/>
                inf : String    : Extra information.                        <br/>
                e   : Exception : Exception to process if any. :DEF: null.  <para/>
          RETV:                                                             <br/>
                    : Exception : <b> e </b> If is was not null at entry.   <para/>
          INFO:                                                             <br/>
                * Algorithm:                                                <br/>
                Finds the caller from stack frame.                          <br/>
                If e is null builds an exception.                           <br/>
                Collects exception data into exception object.              <br/>
                If an exception interceptor delegate is defined:
                    Interceptor function is called.                         <br/>
                Writes exception info via dbg().                            <br/>
                If e was null at entry
                    <b> throws </b> the exception built.                    <br/>
                else
                    returns e (for syntactic sugar).                        <br/>
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
        public static Exception Exc(string tag = "E_NO_TAG", 
                                    string inf = "", 
                                    Exception e = null) {
            Exception cex;
            Stl dta;
            MethodBase met;

            cex = e ?? new ToreCoreException(tag);      // If no exception make one.
            if (HasExcData(cex))                        // If exception already processed
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
            excInterceptor?.Invoke(cex, dta);           // if assigned, invoke.
            cex.Data.Add("dta", (object)dta);
            if (isDebug)
                ExcDbg(dta);
            if (e == null)                              // If we made the exception
                throw cex;                              // throw it
            return cex;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ExcDbg [static]                                             <summary>
          TASK: Outputs exception info via dbg() method                     <para/>
          ARGS: ed  : Stl       : Exception data in Stl form.               <para/>
          INFO: The ed Stl can be found in exception.Data["dta"].           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void ExcDbg(Stl ed) {
            List<string> dia, inl;

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

        #endregion

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Chk [static]                                            <summary>
          TASK:                                                         <br/>
                Checks argument and raises exception if it is null 
                or empty.                                               <para/>
          ARGS:                                                         <br/>
                arg : object        : Argument to check validity.       <br/>
                inf : string        : Exception info if arg invalid.    <br/>
                tag : string        : Exception tag if arg invalid.
                                        :DEF: "E_INV_ARG".              <para/>
          INFO:                                                         <br/>
                In case of strings, white spaces are not welcome.       <br/>
                In case of Guids, empty Guid's are not welcome.         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void Chk(object arg, string inf, string tag = "E_INV_ARG") {
            if (arg == null ||
               (arg is string && ((string)arg).IsNullOrWhiteSpace()) ||
               (arg is Guid && ((Guid)arg).Equals(Guid.Empty)))
                Exc(tag, inf);
        }

        #region Debug Output Subsystem.
        /*————————————————————————————————————————————————————————————————————————————
            —————————————————————————————
            |  Debug Output Subsystem.  |
            —————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        private const string dbgLine = "————————————————————————————————" +
                                       "————————————————————————————————";
        /**———————————————————————————————————————————————————————————————————————————
          TYPE: DbgInterceptorDelegate [delegate]                           <summary>
          TASK: Logging Interceptor delegate Type for dbg() outputs.        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public delegate void DbgInterceptorDelegate(string s);

        /**———————————————————————————————————————————————————————————————————————————
          PROP: dbgInterceptor                                              <summary>
          GET : Gets debug interceptor method.                              <br/>
          SET : Sets debug interceptor method.                              <br/>
          INFO: Interceptor delegate property for dbg() outputs.            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static DbgInterceptorDelegate dbgInterceptor { get; set; } = null;

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Dbg [static]                                                <summary>
          TASK:                                                             <br/>
                Writes output to System.Diagnostics.Debug.Listeners.        <para/>
          ARGS:                                                             <br/>
                msg : object[]  : Message collection.                       <para/>
          INFO:                                                             <br/>
                Uses Debug.Write.                                           <br/>
                Has a log interceptor delegate which may be useful in case. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void Dbg(params object[] msg) {
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
          TASK:                                                             <br/>
                Writes output to System.Diagnostics.Debug.Listeners.        <para/>
          ARGS:                                                             <br/>
                lst : List of String  : Message collection.                 </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void Dbg(List<object> lst) {
            if (lst == null)
                return;
            Dbg(lst.ToArray());
        }
        #endregion

        #region Application utility functions.
        /*————————————————————————————————————————————————————————————————————————————
            ———————————————————————————————————————
            |   Application utility functions.    |
            ———————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ApplicationName [static].                                   <summary>
          TASK:                                                             <br/>
                Returns application name.                                   <para/>
          RETV:                                                             <br/>
                    : string : Application name.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string ApplicationName() {
            return Assembly.GetEntryAssembly().GetName().Name;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ApplicationPath [static].                                   <summary>
          TASK:                                                             <br/>
                Returns application path.                                   <para/>
          RETV:                                                             <br/>
                    : string : Application path.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string ApplicationPath() {
            return AppDomain.CurrentDomain.BaseDirectory;
        }
        #endregion

        #region Reflection utility functions.
        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————————————————————
            |   Reflection utility functions.    |
            ——————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Attr [static]                                               <summary>
          TASK:                                                             <br/>
                Fetches an attribute of a member.                           <para/>
          ARGS:                                                             <br/>
                attrType    : Type          :   Type of attribute.          <br/>
                memInfo     : MemberInfo    :   Member information.         <para/>
          RETV:             : Attribute     :   The attribute or null if
                                                not found.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Attribute Attr(Type attrType, MemberInfo memInfo) {
            Chk(attrType, "attrType");
            Chk(memInfo, "memInfo");
            return memInfo.GetCustomAttribute(attrType, true);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Attr [static]                                               <summary>
          TASK:                                                             <br/>
                Fetches an attribute of a member.                           <para/>
          ARGS:                                                             <br/>
                T           : Type(Class)   :   Type of attribute.          <br/>
                memInfo     : MemberInfo    :   Member information.         <para/>
          RETV:             : Attribute     :   The attribute or null if
                                                not found.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Attribute Attr<T>(MemberInfo memInfo) where T : Attribute {
            return memInfo.GetCustomAttribute(typeof(T), true);
        }


        /**———————————————————————————————————————————————————————————————————————————
          FUNC: hasAttr [static]                                            <summary>
          TASK:                                                             <br/>
                Checks if a member Has an attribute.                        <para/>
          ARGS:                                                             <br/>
                T           : Type (Class)  : Type of attribute.            <br/>
                memInfo     : MemberInfo    : Member information.           <para/>
          RETV:             : bool          : True if attribute exists 
                                              else false.                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool hasAttr<T>(MemberInfo memInfo) {
            return (Attr(typeof(T), memInfo) != null);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: NameInAttr [static]                                         <summary>
          TASK:                                                             <br/>
                Fetches name in a NameMetadata attribute of any member.     <para/>
          ARGS:                                                             <br/>
                T           : Type (Class)  : Type of attribute.            <br/>
                memInfo     : MemberInfo    : Member information.           <para/>
          RETV:             : String        : If attribute exists, 
                                              name in attribute else null.  <para/>
          INFO:                                                             <br/>
                Attribute class must be descendant of NameMetadata class.
                Otherwise the return value will be null.                    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string NameInAttr<T>(MemberInfo memInfo) {
            Attribute a;

            a = Attr(typeof(T), memInfo);
            return (a is NameMetadata nmd) ? nmd.name : null;
        }

        private static Type fetchType(object src, string name) {
            if (src == null)
                Exc("E_TYPE_SRC_NULL", name);
            return (src is Type t) ? t : src.GetType();
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: InstanceProp [static]                                       <summary>
          TASK:                                                             <br/>
                Fetches Public Property info from a class.                  <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                propNam : String        : Instance Property name.           <br/>
                inherit : bool          : Include base classes to the
                                          search of property.:DEF:true.     <para/>
          RETV:         : PropertyInfo  : If exists, the property info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo InstanceProp(object propSrc,
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
          FUNC: InstanceProps [static]                                      <summary>
          TASK:                                                             <br/>
                Fetches <b> all </b> Public Property infos from a class.    <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                inherit : bool          : Include inherited properties
                                          :DEF:true.                        <para/>
          RETV:                                                             <br/>
                : PropertyInfo[]: Property info array, never null.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo[] InstanceProps(object propSrc, bool inherit = true) {
            return fetchType(propSrc, nameof(propSrc)).GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: StaticProp [static]                                         <summary>
          TASK:                                                             <br/>
                Fetches Public static Property info from a class.           <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                propNam : String        : Static Property name.             <br/>
                inherit : bool          : Include base classes to the
                                          search of property.:DEF:true.     <para/>
          RETV:                                                             <br/>
                        : PropertyInfo  : If exists, the property info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo StaticProp(object propSrc,
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
          FUNC: StaticProps [static]                                        <summary>
          TASK:                                                             <br/>
                Fetches <b>all</b> Public static property infos of a class. <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                inherit : bool          : Include inherited properties.
                                          :DEF:true.                        <para/>
          RETV:                                                             <br/>
                : PropertyInfo[]: Property info array, never null.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo[] StaticProps(object propSrc, bool inherit = true) {
            return fetchType(propSrc, nameof(propSrc)).GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: StaticField [static]                                        <summary>
          TASK:                                                             <br/>
                Fetches Public Static Field info from a class.              <para/>
          ARGS:                                                             <br/>
                fieldSrc    : Object        : Type or Instance.             <br/>
                fieldNam    : String        : Static Field name.            <br/>
                inherit     : bool          : Include base classes to the
                                              search of field. :DEF:true.   <para/>
          RETV:                                                             <br/>
                        : fieldInfo     : If exists, the field info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static FieldInfo StaticField(object fieldSrc,
                                            string fieldNam,
                                            bool inherit = true) {
            Chk(fieldNam, nameof(fieldNam));
            return fetchType(fieldSrc, nameof(fieldSrc)).GetField(
                fieldNam,
                BindingFlags.Public |
                BindingFlags.Static |
                (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: StaticFields [static]                                       <summary>
          TASK:                                                             <br/>
                Fetches <b>all</b> Public static field infos of a class.    <para/>
          ARGS:                                                             <br/>
                fieldSrc : Object       : Type or Instance.                 <br/>
                inherit  : bool         : Include inherited fields.
                                          :DEF:true.                        <para/>
          RETV:                                                             <br/>
                         : FieldInfo[]  : Field info array, never null.     </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static FieldInfo[] StaticFields(object fieldSrc, bool inherit = true) {
            return fetchType(fieldSrc, nameof(fieldSrc)).GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: InstanceMethod [static]                                     <summary>
          TASK:                                                             <br/>
                Fetches Public Method info from a class.                    <para/>
          ARGS:                                                             <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Instance method name.             <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:                                                             <br/>
                        : MethodInfo    : If exists, the method info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static MethodInfo InstanceMethod(object metSrc,
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
          FUNC: StaticMethod [static]                                       <summary>
          TASK:                                                             <br/>
                Fetches Public Static Method info from a class.             <para/>
          ARGS:                                                             <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Static Method name.               <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:                                                             <br/>
                        : MethodInfo    : If exists, the method info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static MethodInfo StaticMethod(object metSrc,
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
          FUNC: InstanceDelegate [static]                                   <summary>
          TASK:                                                             <br/>
                Finds Public Instance Method of a class                     <br/>
                and returns it as a delegate of Type T.                     <para/>
          ARGS:                                                             <br/>
                T       : Type          : Delegate type.                    <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Instance method name.             <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:                                                             <br/>
                        : T             : If method exists, the delegate,
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T InstanceDelegate<T>(object metSrc,
                                            string metNam,
                                            bool inherit = true)
                                            where T : Delegate {
            return InstanceMethod(metSrc, metNam, inherit)?.CreateDelegate<T>();
        }

        /**——————————————————————————————————————————————————————————————————————————
          FUNC: StaticDelegate [static]                                     <summary>
          TASK:                                                             <br/>
                Finds Public Static Method of a class                       <br/>
                and returns it as a delegate of Type T.                     <para/>
          ARGS:                                                             <br/>
                T       : Type          : Delegate type.                    <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Static method name.               <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:                                                             <br/>
                        : T             : If method exists, the delegate,
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T StaticDelegate<T>(object metSrc, string metNam, bool inherit = true)
                                          where T : Delegate {
            return StaticMethod(metSrc, metNam, inherit)?.CreateDelegate<T>();
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: MakeObject [static]                                         <summary>
          TASK:                                                             <br/>
                Builds an object of a given type.                           <para/>
          ARGS:                                                             <br/>
                template: object            : Object or Type of object.     <br/>
                args    : params object[]   : Constructor parameters if any.<para/>
          RETV:                                                             <br/>
                        : object            : Object constructed.           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object MakeObject(object template, params object[] args) {
            return Activator.CreateInstance(fetchType(template, nameof(template)), args);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetInstanceProp [static]                                    <summary>
          TASK:                                                             <br/>
                Sets a property on target instance to a value.              <para/>
          ARGS:                                                             <br/>
                target  : object    : Instance.                             <br/>
                propNam : string    : Property name.                        <br/>
                val     : object    : Value to set to target's property.    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void SetInstanceProp(object target, string propNam, object val) {
            PropertyInfo inf;

            if ((target == null) || (target is Type))
                Exc("E_INV_INSTANCE", nameof(target));
            inf = InstanceProp(target, propNam, true);
            Chk(inf, target.GetType().Name + "." + propNam, "E_INV_PROP");
            inf.SetValue(target, SetType(val, inf.PropertyType));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetInstanceProp [static]                                    <summary>
          TASK:                                                             <br/>
                Sets a property on target instance to a value.              <para/>
          ARGS:                                                             <br/>
                info    : PropertyInfo  : Property info.                    <br/>
                target  : object        : Instance.                         <br/>
                val     : object        : Value to set to target property.  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void SetInstanceProp(PropertyInfo info, object target, object val) {
            Type tty;

            if ((target == null) || (target is Type))
                Exc("E_INV_INSTANCE", nameof(target));
            Chk(info, "inf");
            tty = target.GetType();
            if (info.DeclaringType != tty) {
                Exc("E_PROP_INV_CLASS",
                    info.DeclaringType.Name + "!=" + tty.Name);
            }
            info.SetValue(target, SetType(val, info.PropertyType));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetStaticProp [static]                                      <summary>
          TASK:                                                             <br/>
                Sets a static property on target class to a value.          <para/>
          ARGS:                                                             <br/>
                target  : object    : Class Type or Instance.               <br/>
                propNam : string    : Property name.                        <br/>
                val     : object    : Value to set to target's property.    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void SetStaticProp(object target, string propNam, object val) {
            PropertyInfo inf;

            inf = StaticProp(target, propNam, true);
            if (inf == null) 
                Exc("E_INV_PROP", fetchType(target, nameof(target)).Name + "." + propNam);
            inf.SetValue(null, SetType(val, inf.PropertyType));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetInstancePropType [static]                                <summary>
          TASK:                                                             <br/>
                Checks type of a value and if it is not compatible to       <br/>
                property type on target object class, tries to convert it.  <para/>
          ARGS:                                                             <br/>
                propNam : string    : Property name in object.              <br/>
                target  : object    : Target object type or object.         <br/>
                val     : object    : Value to convert to property type.    <para/>
          INFO:                                                             <br/>
                This method only returns a value at required type.          <br/>
                It does not set the property in the target object.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object SetInstancePropType(string propNam, object target, object val) {
            PropertyInfo inf;
 
            inf = InstanceProp(target, propNam, true);
            if (inf == null)
                Exc("E_INV_PROP", fetchType(target, nameof(target)).Name + "." + propNam);
            return SetType(val, inf.PropertyType);
        }

        // Used in SetType...
        private static readonly Type NULLABLE = typeof(Nullable<>);

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetType [static]                                            <summary>
          TASK:                                                             <br/>
                Checks type of a value and if required, tries to change it. <para/>
          ARGS:                                                             <br/>
                val : object    : Value to check and if required convert.   <br/>
                typ : Type      : Expected value type.                      <br/>
                ignoreMissing: bool : If value or subvalues is Stl and 
                                      will be assigned to a sub object,
                                      true    : ignores missing properties
                                      false   : raises exception.           <para/>
          RETV:                                                             <br/>
                    : object    : Value of Type typ if possible.            <para/>
          WARN:                                                             <br/>
                Throws E_TYPE_CONV on failure.                              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object SetType(object val,
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
                        MakeObject(typ) :               // make a default,
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
          FUNC: SetType [static]                                            <summary>
          TASK:                                                             <br/>
                Checks type of a value and if required, tries to change it. <para/>
          ARGS:                                                             <br/>
                T   : Type (Class)  : Expected value type.                  <br/>
                val : object        : Value to check, if required convert.  <br/>
                ignoreMissing: bool : If value or subvalues is Stl and      
                                      will be assigned to a sub object,
                                      true    : ignores missing properties
                                      false   : raises exception.           <para/>
          RETV:                                                             <br/>
                    : object        : Value of Type T if possible.          <para/>
          WARN:                                                             <br/>
                Throws E_TYPE_CONV on failure.                              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T SetType<T>(object val, bool ignoreMissing = false) {
            return (T)SetType(val, typeof(T), ignoreMissing);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: DiscoverTypes [static]                                      <summary>
          TASK:                                                             <br/>
                Finds all Types that are subclass of a base type and        <br/>
                defined in any assembly referring to the base assembly.     <para/>
          ARGS:                                                             <br/>
                baseAsm : AssemblyName : This assembly must be referred.    <br/>
                baseType: Type         : Types should be subclass of this.  <para/>
          RETV:                                                             <br/>
                        : List of Type : List of types that are subclass    
                          of base type and defined in any assembly referring
                          to assembly with name baseAsm.                    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static List<Type> DiscoverTypes(AssemblyName baseAsm, Type baseType) {
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

        #endregion

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
                    nameList.AddUnique( n);
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