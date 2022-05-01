using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tore.Core.Sys;

namespace Tore.Core {


    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Extensions [Static]                                     <summary>
        USAGE:                                                          <br/>
                Contains static utility extension methods for           <br/>
                string, char, ICollection, List of T.                   </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Extensions {

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
                removes all whitespaces from str.                           <para/>
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
                Removes chars in remove from string.                        <para/>
          ARGS:                                                             <br/>
                str     : string :  String to remove characters from.       <br/>
                remove  : string :  String containing characters to 
                                    remove from Key.                        <para/>
          WARN:                                                             <br/>
                Throws E_INV_ARG if string is null.                         </summary>
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
