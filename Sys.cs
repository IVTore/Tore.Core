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
        USAGE:  Contains a library of static utility methods treated as 
                global functions which is used for managing:            <para/>
                Exceptions / Strings / Simple encryption / Reflection   <br/>
                Type juggling / Attributes / Simple File Load Save      <br/>
                Simple Encrypted File Load Save/ Time, Date,
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
          PROP: isDocker [static] : bool.                                   <summary>
          TASK: GET: Returns if program is running in a container.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool isDocker {
            get {
                return 
                    Environment
                    .GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
            }
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: h2i [static]                                                <summary>
          TASK: Returns integer from hex string.                            <para/>
          ARGS: hex : String        : hexadecimal number.                   <para/>
          RETV:     : int           : integer value.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static int h2i(string hex) {
            return int.Parse(hex, NumberStyles.HexNumber);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: i2h [static]                                                <summary>
          TASK: Returns hex string from integer.                            <para/>
          ARGS:                                                             <br/>
                i       : int       : integer value.                        <br/>
                digits  : int       : Number of hex digits.                 <para/>
          RETV:         : String    : Formatted hexadecimal string.         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string i2h(int i, int digits) {
            return i.ToString("x" + digits.ToString());
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: hexStrToByteArr [static]                                    <summary>
          TASK: Converts a hex string to byte array.                        <para/>
          ARGS: hex : string    : Hex string.                               <para/>
          RETV:     : byte[]    : Byte array.                               <para/>
          INFO:                                                             <br/>
                *   Hex digit letters should be capital.                    <br/>
                *   Throws exception if hex string is invalid.              <br/>
                *   'A'- 0xA => 65 - 10 = 55.                               </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static byte[] hexStrToByteArr(string hex) {
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
                exc("E_INV_NIBBLE", $"hex[{pos}] = {n}");
                return 0;
            }

            chk(hex, "hex");
            l = hex.Length;
            if ((l % 2) != 0)
                exc("E_INV_ARG", "hex");
            l /= 2;
            a = new byte[l];
            for(i = 0; i < l; i++) {
                p = i * 2;
                a[i] = (byte)((n2i(p) << 4) | n2i(p + 1));
            }
            return a;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: byteArrToHexStr [static]                                    <summary>
          TASK: Converts a byte array to capital hex string.                <para/>
          ARGS: arr : byte[]    : Byte array.                               <para/>
          RETV:     : string    : Hex string.                               <para/>
          INFO: 'A'- 0xA => 65 - 10 = 55.                                   <para/>
          WARN: Throws exception if byte array is invalid.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string byteArrToHexStr(byte[] arr) {
            int i,              // iterator.
                l,              // length.
                n;              // nibble value.
            StringBuilder s;    // String builder.

            chk(arr, "arr");
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

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: cryptByteArrays [static]                                <summary>
          TASK:                                                         <br/>
                XOR's two byte arrays, returns XOR'ed byte array.       <para/>
          ARGS:                                                         <br/>
                encDec  : byte[]    : Byte array to encrypt or decrypt. <br/>   
                xorKey  : byte[]    : Byte array used as key.           <para/>
          RETV:         : byte[]    : Result.                           <br/>
          INFO: xorKey is wrapped over encDec if xorKey is shorter.     <para/>
          WARN: Throws exception if byte arrays are null or             <br/>
                xorKey length = 0.                                      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static byte[] cryptByteArrays(byte[] encDec, byte[] xorKey) {
            int idx,
                eln,
                kln;
            byte[] res;

            chk(encDec, "encDec");
            chk(xorKey, "xorKey");
            eln = encDec.Length;
            kln = xorKey.Length;
            if (kln == 0)
                exc("E_INV_ARG", "xorKey.Length = 0");
            res = new byte[eln];
            for(idx = 0; idx < eln; idx++)
                res[idx] = (byte)(encDec[idx] ^ xorKey[idx % kln]);
            return res;
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: cryptStrings [static]                                       <summary>
          TASK:                                                             <br/>
                Encodes UTF8 strings to byte arrays, XOR's them,            <br/>
                returns XOR'ed byte array.                                  <br/>
                xorKey is wrapped over encDec if xorKey is shorter.         <para/>
          ARGS:                                                             <br/>
                encDec  : string    : String to encrypt or decrypt.         <br/>   
                xorKey  : string    : String used as key.                   <para/>
          RETV:         : byte[]    : XOR'ed result as byte array.          <para/>
          INFO: xorKey is wrapped over encDec if xorKey is shorter.         <para/>
          WARN: Throws exception if strings are null or empty               </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static byte[] cryptStrings(string encDec, string xorKey) {
            byte[] enc;
            byte[] key;

            chk(encDec, "encDec");
            chk(xorKey, "xorKey");
            enc = Encoding.UTF8.GetBytes(encDec);
            key = Encoding.UTF8.GetBytes(xorKey);
            return cryptByteArrays(enc, key);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: identifier [static]                                     <summary>
          TASK:                                                         <br/>
                Checks if an identifier name has a valid syntax.        <para/>
          ARGS:                                                         <br/>
                s   : string    : identifier candidate string.          <para/>
          RETV:     : boolean   : true if valid else false.             <para/>
          INFO:                                                         <br/>
                *   This checks for unicode identifiers for runtime,    <br/>
                    not ASCII only.                                     <br/>
                *   The C# keywords are intentionally not checked.      <br/>
                *   @ as first character is not supported.              <br/>
                *   Optimized for speed.                                </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool identifier(string s) {
            int i,
            l;

            if (s == null)
                return false;
            l = s.Length;
            if (l == 0)
                return false;
            if (!identifierBegin(s[0]))
                return false;
            for(i = 1; i < l; i++) {
                if (!identifierInner(s[i]))
                    return false;
            }
            return true;
        }

        // In the methods below, Char.IsLetter() costs a lot.
        // So the highly probable characters are checked before
        // Char.IsLetter() is called.
        // identifierBegin does not consider @ as valid.

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: identifierBegin [static]                                <summary>
          TASK: Checks if a character can be at the beginning of an 
                identifier.                                             <para/>
          ARGS: c   : char    : identifier begin candidate character.   <para/>
          RETV:     : boolean : true if valid else false.               <para/>
          INFO:                                                         <br/>
                *   This accepts unicode identifier letters.            <br/>
                *   @ as beginning character is not supported.          <br/>
                *   Optimized for speed.                                </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool identifierBegin(char c) {
            return (c >= 'a' && c <= 'z')
                || (c >= 'A' && c <= 'Z')
                || c == '_'
                || Char.IsLetter(c);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: identifierInner [static]                                <summary>
          TASK: Checks if a character can be inside of an identifier.   <para/>
          ARGS: c   : char    : inner identifier candidate character.   <para/>
          RETV:     : boolean : true if valid else false.               <para/>
          INFO:                                                         <br/>
                *   This accepts unicode identifier letters and digits. <br/>
                *   Optimized for speed.                                </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool identifierInner(char c) {
            return  (c >= 'a' && c <= 'z') ||
                    (c >= 'A' && c <= 'Z') ||
                    (c >= '0' && c <= '9') ||
                    c == '_' ||
                    Char.IsLetter(c);
        }

        /**———————————————————————————————————————————————————————————————————————————
            FUNC:   snoWhite [static]                              <summary>
            TASK:   Shorthand for String.IsNullOrWhiteSpace.       <para/>
            ARGS:   s   : string    : Source string to check.      <para/>
            RETV:       : bool      : True if string is null       
                                      or made of whitespaces.      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool snoWhite(string s) {
            return string.IsNullOrWhiteSpace(s);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: whiteSpacesToSpace [static]                                 <summary>
          TASK: Converts all single or consequtive whitespaces 
                to single space in str.                                     <para/>
          ARGS: s   : string    : Source string to strip multi whitespaces. <para/>
          RETV:     : string    : String stripped of multi white spaces.    <para/>
          INFO: White space characters other than space will be converted to space.
                Modified from solution of   Felipe Machado, thank you.      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string whiteSpacesToSpace(string str) {
            int len = str.Length;
            char[] src = str.ToCharArray();
            int dstIx = 0;
            bool inWsp = false;

            if (str == null)
                exc("E_INV_ARG", "str");
            for(int i = 0; i < len; i++) {
                var ch = src[i];
                if (char.IsWhiteSpace(ch)) {
                    if (!inWsp) {
                        src[dstIx++] = ' ';
                        inWsp = true;
                    }
                    continue;
                }
                inWsp = false;
                src[dstIx++] = ch;
            }
            return new string(src, 0, dstIx);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: rmWhiteSpaces [static]                                      <summary>
          TASK: removes all whitespaces from str.                           <para/>
          ARGS: s   : string    : Source string to strip whitespaces.       <para/>
          RETV:     : string    : String stripped of white spaces.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string rmWhiteSpaces(string str) {
            int len;
            char[] src;
            int dstIx = 0;

            if (str == null)
                exc("E_INV_ARG", "str");
            len = str.Length;
            src = str.ToCharArray();
            for(int i = 0; i < len; i++) {
                var ch = src[i];
                if (char.IsWhiteSpace(ch))
                    continue;
                src[dstIx++] = ch;
            }
            return new string(src, 0, dstIx);
        }
        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————————
            |   Exception Subsystem  |
            ——————————————————————————
            A Practical Exception Subsystem.
        ————————————————————————————————————————————————————————————————————————————*/
        /**———————————————————————————————————————————————————————————————————————————
          TYPE: ExcInt [delegate]                                           <summary>
          TASK: Exception Interceptor delegate Type                         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public delegate void ExcInt(Exception e);        // Type definition.
        /**———————————————————————————————————————————————————————————————————————————
          PROP: excInt                                                      <summary>
          USE: Exception Interceptor delegate property.                     </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static ExcInt excInt { get; set; } = null;

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: exc [static]                                                <summary>
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
                    "exc" = Class name of exception.                        <br/>
                    "msg" = Exception message.                              <br/>
                    "tag" = Exception tag.                                  <br/>
                    "inf" = Exception info.                                 <br/>
                    "loc" = Call location (Class and method).               <para/>
                *Stl is a string associated object list class. 
                 Info about Stl can be found at Stl.cs.                     </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object exc(string tag = "E_NO_TAG", string inf = "",
                Exception e = null) {

            Exception cex;
            Stl lst;
            MethodBase met;

            cex = e ?? new Exception(tag);              // If no exception make one.
            if (cex.Data.Contains("dta"))               // If exception already processed
                return null;                            // return.
            lst = new Stl(){                            // Collect data.
                {"exc", cex.GetType().FullName},
                {"msg", cex.Message},
                {"tag", tag},
                {"inf", inf}
            };
            met = new StackTrace(2)?.GetFrame(0)?.GetMethod();
            if (met?.Name == "chk")
                met = new StackTrace(3)?.GetFrame(0)?.GetMethod();
            if ((met != null) && (met.DeclaringType != null))
                lst.add("loc", $"{met.DeclaringType.Name}.{met.Name}");
            cex.Data.Add("dta", (object)lst);
            excDbg(lst);
            excInt?.Invoke(cex);                        // if assigned, invoke.
            if (e == null)                              // If we made the exception
                throw cex;                              // throw it
            return null;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: excDbg [static]                                             <summary>
          TASK: Outputs exception info via dbg() method                     <para/>
          ARGS: ed  : Stl       : Exception data in Stl form.               <para/>
          INFO: The ed Stl can be found in exception.Data["dta"].           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void excDbg(Stl ed) {
            List<string> dia,
                        inl;
            string inf = (string)ed.obj("inf");

            dia = new List<string>();
            dia.Add("_LINE_");
            dia.Add("[EXC]: " + (string)ed.obj("exc"));
            dia.Add("[MSG]: " + (string)ed.obj("msg"));
            dia.Add("[TAG]: " + (string)ed.obj("tag"));
            if (snoWhite(inf)) {
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
            dia.Add("[LOC]: " + (string)ed.obj("loc"));
            dia.Add("_LINE_");
            dbg(dia);
        }
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: chk [static]                                            <summary>
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
        public static void chk(object arg, string inf,
                string tag = "E_INV_ARG") {

            if  ((arg == null) ||
                ((arg is string) && (String.IsNullOrWhiteSpace((string)arg))) ||
                ((arg is Guid) && (((Guid)arg).Equals(Guid.Empty))))
                exc(tag, inf);
        }

        private const string dbgLine = "—————————————————————————————————" +
                                        "—————————————————————————————————";
        /**———————————————————————————————————————————————————————————————————————————
          TYPE: LogInt [delegate]                                           <summary>
          TASK: Logging Interceptor delegate Type for dbg() outputs.        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public delegate void LogInt(string s);              // Type definition.
        /**———————————————————————————————————————————————————————————————————————————
          PROP: logInt                                                      <summary>
          USE : Logging Interceptor delegate property for dbg() outputs.    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static LogInt logInt { get; set; } = null;   // Function pointer.

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: dbg [static]                                                <summary>
          TASK: Writes output to debug console and trace log.               <para/>
          ARGS: msg : String[]  : Message collection.                       <para/>
          INFO: Uses Console.Write if in container, otherwise Debug.Write.  
                Has a log interceptor delegate which may be useful in case. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void dbg(params string[] msg) {
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
            if (isDocker)
                Console.Write(s);
            else
                Debug.Write(s);
            logInt?.Invoke(s);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: dbg [static]                                                <summary>
          TASK: Writes output to debug console and to log if linked.        <para/>
          ARGS: lst : List of String  : Message collection.                 <para/>
          INFO: Uses Console.Write if in container, otherwise Debug.Write.  
                Has a log interceptor delegate which may be useful in case. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void dbg(List<string> lst) {
            if (lst == null)
                return;
            dbg(lst.ToArray());
        }

        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————————————
            |   List utility functions   |
            ——————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: addUnique [static]                                          <summary>
          TASK: If itm of Type T does not exist in List of T lst, it is added.
                Index of itm is returned.                                   <para/>
          ARGS:                                                             <br/>
                lst : List of T : List to add.                              <br/>
                itm : T         : Item to add for.                          <para/>
          RETV:     : int       : Index of itm in list.                     </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static int addUnique<T>(List<T> lst, T itm) {
            int r;                                  // Return value.

            chk(lst, "lst");
            r = lst.IndexOf(itm);               // Search object in list.
            if (r == -1) {                      // If not found
                lst.Add(itm);                   // add the object
                r = lst.Count - 1;              // and get the index.
            }
            return (r);                          // return the index.
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: copyToList [static]                                         <summary>
          TASK: Copies a collection of type T to a list of type T.          <para/>
          ARGS: c   : ICollection of T  : Source Collection.                <para/>
          RETV:     : List of T         : Copy of collection as List of T.  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static List<T> copyToList<T>(ICollection<T> c) {
            List<T> l;
            l = new List<T>();
            if (c != null) {
                foreach(T x in c)
                    l.Add(x);
            }
            return l;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: argStl [static]                                             <summary>
          TASK: Converts an object array to Stl appropriately.              <para/>
          ARGS: akv : object[]  : Arguments (may be key - values).          <para/>
          RETV:     : Stl       : Arguments as Stl.                         <para/>
          INFO:                                                             <br/>
                If  akv length = 0 null is returned.                        <br/>
                If  akv length = 1                                          <br/>
                  __ if akv[0] is an Stl, it is returned.                    <br/>
                  __ else akv[0] is assumed to be a data transfer object.    <br/>
                  __ It is converted to an Stl and returned.                 <br/>
                Otherwise Stl constructor with object[] is called.
                For more info refer to Stl.                                 </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Stl argStl(object[] akv) {
            if (akv.Length == 0)
                return null;
            if (akv.Length == 1) {
                if (akv[0] is Stl)
                    return (Stl)akv[0];
                return new Stl(akv[0]);
            }
            return new Stl(akv);
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
            chk(attrType, "attrType");
            chk(memInfo, "memInfo");
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
          TASK: Checks if a member has an attribute.                        <para/>
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
            return (a is NameMetadata) ? ((NameMetadata)a).name : null;
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
            Type t;

            chk(propSrc, "propSrc");
            chk(propNam, "propNam");
            t = (propSrc is Type) ? (Type)propSrc : propSrc.GetType();
            return t.GetProperty(
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
            Type t;

            chk(propSrc, "propSrc");
            chk(propNam, "propNam");
            t = (propSrc is Type) ? (Type)propSrc : propSrc.GetType();
            return t.GetProperty(
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
            Type t;

            chk(fldSrc, "fldSrc");
            chk(fldNam, "fldNam");
            t = (fldSrc is Type) ? (Type)fldSrc : fldSrc.GetType();
            return t.GetField(
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
            Type t;

            chk(metSrc, "metSrc");
            chk(metNam, "metNam");
            t = (metSrc is Type) ? (Type)metSrc : metSrc.GetType();
            return t.GetMethod(
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
            Type t;

            chk(metSrc, "metSrc");
            chk(metNam, "metNam");
            t = (metSrc is Type) ? (Type)metSrc : metSrc.GetType();
            return t.GetMethod(
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
            return instanceMethod(metSrc, metNam, inherit)?
                    .CreateDelegate<T>();
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
        public static T staticDelegate<T>(object metSrc,
                                                string metNam,
                                                bool inherit = true)
                                                where T : Delegate {
            return staticMethod(metSrc, metNam, inherit)?
                    .CreateDelegate<T>();
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
            Type t;

            chk(template, "template");
            t = (template is Type) ? (Type)template : template.GetType();
            return Activator.CreateInstance(t, args);
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
                exc("E_INV_INSTANCE", "tar");
            inf = instanceProp(tar, propNam, true);
            chk(inf, tar.GetType().Name + "." + propNam, "E_PROP_INV");
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
                exc("E_INV_INSTANCE", "tar");
            chk(inf, "inf");
            tty = tar.GetType();
            if (inf.DeclaringType != tty) {
                exc("E_PROP_INV_CLASS",
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
            string tnm;

            inf = staticProp(tar, propNam, true);
            if (inf == null) {
                tnm = (tar is Type) ? ((Type)tar).Name : tar.GetType().Name;
                exc("E_INV_PROP", tnm + "." + propNam);
            }
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
            string tnm;

            inf = instanceProp(tar, propNam, true);
            if (inf == null) {
                tnm = (tar is Type) ? ((Type)tar).Name : tar.GetType().Name;
                exc("E_INV_PROP", tnm + "." + propNam);
            }
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
            if (n) {                                     // If null,
                val = (typ.IsValueType) ?              // If value
                        makeObject(typ) :              // make a default,
                        null;                           // else null.
                return val;
            }
            if (typ == v)                               // Fast check.
                return val;
            if (typ.IsAssignableFrom(v))                // Is it?
                return val;
            try {                                       // Try to convert.
                if (val is JToken)                      // If coming from json, 
                    return ((JToken)val).ToObject(typ); // call json converter.
                if (typ == typeof(Guid)) {               // If guid,
                    if (val is string)                  // string support only.
                        return Guid.Parse((string)val);
                }
                if (val is Stl)                         // If Stl.
                    return ((Stl)val).toObj(typ, ignoreMissing);
                return Convert.ChangeType(val, typ);    // Otherwise...
            } catch(Exception e) {
                exc("E_TYPE_CONV",
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

            chk(baseAsm, "baseAsm");
            chk(baseType, "baseType");
            foreach(var asm in aArr) {                   // Scan assemblies:
                refs = asm.GetReferencedAssemblies();   // Get referenced.
                if (!refs.Contains(baseAsm))            // If this is not referenced,
                    continue;                           // ignore.
                tArr = asm.GetTypes();                  // Get types in assembly.
                if (tArr == null)                       // If no types in it,
                    continue;                           // ignore.
                foreach(var typ in tArr) {               // Scan types:
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
          ARGS: offset  : long  : Offset seconds to add : DEF : 0.          <para/>
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
            chk(fileSpec, "fileSpec");
            try {
                return File.ReadAllText(fileSpec, Encoding.UTF8);
            } catch(Exception e) {
                exc("E_SYS_LOAD_FILE", fileSpec, e);
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
            chk(fileSpec, "fileSpec");
            try {
                File.WriteAllText(fileSpec, text, Encoding.UTF8);
            } catch(Exception e) {
                exc("E_SYS_SAVE_FILE", fileSpec, e);
                throw;
            }
        }

        /*————————————————————————————————————————————————————————————————————————————
            —————————————————————————————————————————
            |   Crypto json File routines           |
            —————————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: loadCryptoJsonFile [static].                                <summary>
          TASK: Reads and decrypts an encrypted Utf8 json file into a Stl.  <para/>
          ARGS:                                                             <br/>
                fileSpec    : string : filename with path.                  <br/>
                encKey      : string : Encrypt  key builder.                <br/>
                xorKey      : string : Xor      key builder.                <br/>
                strip       : string : Any string to strip from keys.       <para/>
          RETV:             : Stl    : Decrypted file as Stl.               <para/>
          INFO: Encrypted files are marked with string "TORECJF:1:".        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Stl loadCryptoJsonFile(string fileSpec, string encKey,
                string xorKey, string strip = "-") {
            byte[] key;
            byte[] dec;
            string buf;

            chk(fileSpec, "fileSpec");
            chk(encKey, "encKey");
            chk(xorKey, "xorKey");
            buf = loadUtf8File(fileSpec);
            if (!buf.StartsWith("TORECJF:1:"))
                exc("E_INV_ENC", fileSpec);
            dec = hexStrToByteArr(buf.Substring(10));
            encKey = prepCryptoKey(encKey, strip);
            xorKey = prepCryptoKey(xorKey, strip);
            key = cryptStrings(encKey, xorKey);
            dec = cryptByteArrays(dec, key);
            buf = Encoding.UTF8.GetString(dec);
            return new Stl(buf);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: saveCryptoJsonFile [static].                                <summary>
          TASK: Encrypts and writes an Stl to an Utf8 json file.             <para/>
          ARGS:                                                             <br/>
                fileSpec    : string : filename with path.                  <br/>
                encKey      : string : Encrypt  key builder.                <br/>
                xorKey      : string : Xor      key builder.                <br/>
                strip       : string : Any string to strip from keys.       <para/>
          INFO: Encrypted files are marked with string "TORECJF:1:".        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void saveCryptoJsonFile(string fileSpec, Stl src,
                string encKey, string xorKey, string strip = "-") {
            byte[] key;
            byte[] enc;
            string buf;

            chk(fileSpec, "fileSpec");
            chk(src, "src");
            chk(encKey, "encKey");
            chk(xorKey, "xorKey");
            buf = src.toJson();
            encKey = prepCryptoKey(encKey, strip);
            xorKey = prepCryptoKey(xorKey, strip);
            key = cryptStrings(encKey, xorKey);
            enc = Encoding.UTF8.GetBytes(buf);
            enc = cryptByteArrays(enc, key);
            buf = byteArrToHexStr(enc);
            saveUtf8File(fileSpec, "TORECJF:1:" + buf);
        }

        /*————————————————————————————————————————————————————————————————————————————
            FUNC:   prepCryptoKey [static].
            TASK:   Prepares a crypto key string. 
                    Used in LoadCryptoJsonFile and SaveCryptoJsonFile.
            ARGS:   key         : string : Key builder.
                    strip       : string : Any string to strip from key.
                    exInf       : string : For exception info.
        ————————————————————————————————————————————————————————————————————————————*/
        private static string prepCryptoKey(string key, string strip) {
            return (snoWhite(strip)) ? key : key.Replace(strip, "");
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

}   // End namespace.