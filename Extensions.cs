using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using static Tore.Core.Sys;

namespace Tore.Core {


    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Extensions [Static]                                     <summary>
        USAGE:                                                          <br/>
                Contains static utility extension methods for           <br/>
                Exception, string, char, ICollection, List of T.        </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Extensions {

        #region Exception extensions.
        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————————
            |  Exception extensions  |
            ——————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: HasInfo [static, extension].                                <summary>
          TASK:                                                             <br/>
            If an exception is generated or processed through               <br/>
            <b>Exc</b>, it has extra info and this will return true.        <para/>
          ARGS:                                                             <br/>
            e   : Exception : Any Exception.                                <para/>
          RETV:                                                             <br/>
                : bool      : True if exception is not null and has info.   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool HasInfo(this Exception e) {
            return (e != null) && (e.Data.Contains("dta"));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Info [static, extension]                                    <summary>
          TASK:                                                             <br/>
                If an exception is generated or processed through           <br/>
                <b> Exc </b>, it has extra data and this will return it.    <para/>
          ARGS:                                                             <br/>
                e   : Exception : Any Exception.                            <para/>
          RETV:                                                             <br/>
                    : StrLst    : Extra exception info or null              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static StrLst Info(this Exception e) {
           return e.HasInfo() ? (StrLst)(e.Data["dta"]) : null;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: InfoToConsole [static, extension]                           <summary>
          TASK: Outputs exception info to console.                          <para/>
          ARGS: e  : Exception : Exception.                                 <para/>
          INFO: If e is null or has no info this does nothing.              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void InfoToConsole(this Exception e) {
            if (!e.HasInfo())
                return;
            Console.Write(e.InfoToPrettyString());
        }

        private static string seperator = new string('—', 60);

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: InfoToPrettyString [static, extension]                      <summary>
          TASK: Outputs exception info to console.                          <para/>
          ARGS: e  : Exception  : Exception.                                <para/>
          RETV:    : string     : A printable string made of info or        <br/>
                                  "Exception is null." if e is null or      <br/>
                                  "Exception has no info." if no info.      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string InfoToPrettyString(this Exception e) {
            bool f;
            string k,
                   v;
            string[] l;
            StrLst d;
            StringBuilder s;

            if (e == null)
                return "Exception is null.";
            if (!e.Data.Contains("dta"))
                return "Exception has no info.";
            s = new();
            d = e.Info();
            s.AppendLine(seperator);
            foreach (var kv in d) {
                k = kv.Key.ToUpper(CultureInfo.InvariantCulture);
                v = (string)kv.Value;
                s.Append('[');
                s.Append(k);
                s.Append("]: ");
                if (v.IsNullOrWhiteSpace())
                    v = "- .";
                if (!v.Contains('\n')) {
                    s.AppendLine(v);
                    continue;
                }
                l = v.Split('\n');
                f = false;
                foreach (var str in l) {
                    s.Append(f ? "       " : "");
                    s.AppendLine(str.Trim());
                    f = true;
                }
            }
            s.AppendLine(seperator);
            return s.ToString();
        }


        #endregion

        #region String extensions.
        /*————————————————————————————————————————————————————————————————————————————
            ———————————————————————
            |  String extensions  |
            ———————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: IsNullOrWhiteSpace [static, extension]                      <summary>
          TASK:                                                             <br/>
                Shorthand for String.IsNullOrWhiteSpace.                    <para/>
          ARGS:                                                             <br/>
                str : string : Source string to check.                      <para/>
          RETV:                                                             <br/>
                : bool   : True if string is null or made of whitespaces.   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool IsNullOrWhiteSpace(this string str) {
            return string.IsNullOrWhiteSpace(str);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: IsIdentifier [static, extension]                        <summary>
          TASK:                                                         <br/>
                Checks if an identifier name has a valid syntax.        <para/>
          ARGS:                                                         <br/>
                s   : string    : identifier candidate stri             <para/>
          RETV:     : boolean   : true if valid else false.             <para/>
          INFO:                                                         <br/>
                *   This checks for unicode identifiers for runtime,    <br/>
                    not ASCII only.                                     <br/>
                *   The C# keywords are intentionally not checked.      <br/>
                *   @ as first character is not supported.              <br/>
                *   Optimized for speed.                                </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool IsIdentifier(this string str) {
            int i;
            int l;

            if (str == null)
                return false;
            l = str.Length;
            if (l == 0)
                return false;
            if (!str[0].IsIdentifierBegin())
                return false;
            for (i = 1; i < l; i++) {
                if (!str[i].IsIdentifierInner())
                    return false;
            }
            return true;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: RemoveWhiteSpaces [static, extension]                       <summary>
          TASK:                                                             <br/>
                Removes all whitespaces from str.                           <para/>
          ARGS:                                                             <br/>
                str : string    : Source string to strip whitespaces.       <para/>
          RETV:                                                             <br/>
                    : string    : String stripped of white spaces.          <para/>
          WARN:                                                             <br/>
                Throws E_INV_ARG if string is null.                         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string RemoveWhiteSpaces(this string str) {
            int len;
            int tar = 0;
            char[] src;
            
            if (str == null)
                Exc("E_INV_ARG", "str");
            len = str.Length;
            src = str.ToCharArray();
            for (int i = 0; i < len; i++) {
                var ch = src[i];
                if (char.IsWhiteSpace(ch))
                    continue;
                src[tar++] = ch;
            }
            return new string(src, 0, tar);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: WhiteSpacesToSpace [static, extension]                      <summary>
          TASK:                                                             <br/>
                Converts all single or consequtive whitespaces              <br/>
                to single space in string str.                              <para/>
          ARGS:                                                             <br/>
                str : string    : String to strip multi whitespaces.        <para/>
          RETV:                                                             <br/>
                    : string    : String stripped of multi white spaces.    <para/>
          INFO:                                                             <br/>
                White space characters other than space will be converted   <br/>
                to space. Modified from the solution of Felipe Machado.     <para/>
          WARN:                                                             <br/>
                Throws E_INV_ARG if string is null.                         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string WhiteSpacesToSpace(this string str) {
            int len;
            char[] src;
            int dstIx = 0;
            bool inWsp = false;

            if (str == null)
                Exc("E_INV_ARG", "str");
            len = str.Length;
            src = str.ToCharArray();
            for (int i = 0; i < len; i++) {
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

        /**——————————————————————————————————————————————————————————————————————————
          FUNC: RemoveChars [static, extension].                            <summary>
          TASK:                                                             <br/>
                Removes chars in remove string from string str.             <para/>
          ARGS:                                                             <br/>
                str     : string :  String to remove characters from.       <br/>
                remove  : string :  String containing characters to remove. <para/>
          WARN:                                                             <br/>
                Throws E_INV_ARG if string str is null.                     </summary>
       ————————————————————————————————————————————————————————————————————————————*/
        public static string RemoveChars(this string str, string remove) {
            StringBuilder ret;

            if (remove.IsNullOrWhiteSpace())
                return str;
            if (str == null)
                Exc("E_INV_ARG", "str");
            ret = new(str);
            foreach (char chr in remove)
                ret.Replace(chr.ToString(), "");
            return ret.ToString();
        }
        #endregion

        #region Char extensions.
        /*————————————————————————————————————————————————————————————————————————————
            —————————————————————
            |  Char extensions  |
            —————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        // In the identifier methods below, Char.IsLetter() costs a lot.
        // So the highly probable characters are checked before
        // Char.IsLetter() is called.
        // identifierBegin does not consider @ as valid.

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: IsIdentifierBegin [static, extension]                   <summary>
          TASK:                                                         <br/>
                Checks if a character can be at the beginning of an     <br/>
                identifier.                                             <para/>
          ARGS:                                                         <br/>
                chr : char    : identifier begin candidate character.   <para/>
          RETV:                                                         <br/>
                    : boolean : true if valid else false.               <para/>
          INFO:                                                         <br/>
                *   This accepts unicode identifier letters.            <br/>
                *   @ as beginning character is not supported.          <br/>
                *   Optimized for speed.                                </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool IsIdentifierBegin(this char chr) {
            return (chr >= 'a' && chr <= 'z') ||
                   (chr >= 'A' && chr <= 'Z') ||
                    chr == '_' ||
                    Char.IsLetter(chr);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: IsIdentifierInner [static, extension]                   <summary>
          TASK:                                                         <br/>
                Checks if a character can be inside of an identifier.   <para/>
          ARGS:                                                         <br/>
                chr : char    : inner identifier candidate character.   <para/>
          RETV:                                                         <br/>
                    : boolean : true if valid else false.               <para/>
          INFO:                                                         <br/>
                *   This accepts unicode identifier letters and digits. <br/>
                *   Optimized for speed.                                </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool IsIdentifierInner(this char chr) {
            return (chr >= 'a' && chr <= 'z') ||
                   (chr >= 'A' && chr <= 'Z') ||
                   (chr >= '0' && chr <= '9') ||
                    chr == '_' ||
                    Char.IsLetter(chr);
        }
        #endregion

        #region List and ICollection extensions.
        /*————————————————————————————————————————————————————————————————————————————
            —————————————————————————————————————
            |  List and ICollection extensions  |
            —————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: AddUnique [static, extension]                               <summary>
          TASK:                                                             <br/>
                If item of Type T does not exist in List of T list,         <br/>
                it is added. Index of item is returned.                     <para/>
          ARGS:                                                             <br/>
                list : List of T : List to Add.                             <br/>
                item : T         : Item to Add for.                         <para/>
          RETV:                                                             <br/>
                     : int       : Index of item in list.                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static int AddUnique<T>(this List<T> list, T item) {
            int r;                                  // Return value.

            Chk(list, nameof(list));
            r = list.IndexOf(item);               // Search object in list.
            if (r == -1) {                      // If not found
                list.Add(item);                   // Add the object
                r = list.Count - 1;              // and get the index.
            }
            return (r);                          // return the index.
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: CopyToList [static, extension ]                             <summary>
          TASK:                                                             <br/>
                Copies a collection of type T to a list of type T.          <para/>
          ARGS:                                                             <br/>
                collection   : ICollection of T  : Source Collection.       <para/>
          RETV:                                                             <br/>
                             : List of T : Copy of collection as List of T. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static List<T> CopyToList<T>(this ICollection<T> collection) {
            List<T> list;
            list = new List<T>();
            if (collection != null) {
                foreach (T x in collection)
                    list.Add(x);
            }
            return list;
        }

        #endregion
    }
}
