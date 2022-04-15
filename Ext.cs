using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Tore.Core.Sys;

namespace Tore.Core {


    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Ext [Static]                                            <summary>
        USAGE:                                                          <br/>
                Contains a library of static extension methods for      <br/>
                string, char.                                           </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Ext {

        #region String extensions.
        /*————————————————————————————————————————————————————————————————————————————
            ———————————————————————
            |  String extensions  |
            ———————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: isNullOrWhiteSpace [static extension]                       <summary>
          TASK:                                                             <br/>
                Shorthand for String.IsNullOrWhiteSpace.                    <para/>
          ARGS:                                                             <br/>
                str : string : Source string to check.                      <para/>
          RETV:                                                             <br/>
                : bool   : True if string is null or made of whitespaces.   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool isNullOrWhiteSpace(this string str) {
            return string.IsNullOrWhiteSpace(str);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: isIdentifier [static]                                     <summary>
          TASK:                                                         <br/>
                Checks if an identifier name has a valid syntax.        <para/>
          ARGS:                                                         <br/>
                s   : string : identifier candidate stri         <para/>
          RETV:     : boolean       : true if valid else false.         <para/>
          INFO:                                                         <br/>
                *   This checks for unicode identifiers for runtime,    <br/>
                    not ASCII only.                                     <br/>
                *   The C# keywords are intentionally not checked.      <br/>
                *   @ as first character is not supported.              <br/>
                *   Optimized for speed.                                </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool isIdentifier(this string str) {
            int i;
            int l;

            if (str == null)
                return false;
            l = str.Length;
            if (l == 0)
                return false;
            if (!str[0].isIdentifierBegin())
                return false;
            for (i = 1; i < l; i++) {
                if (!str[i].isIdentifierInner())
                    return false;
            }
            return true;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: removeWhiteSpaces [static extension]                        <summary>
          TASK:                                                             <br/>
                removes all whitespaces from str.                           <para/>
          ARGS:                                                             <br/>
                str : string    : Source string to strip whitespaces.       <para/>
          RETV:                                                             <br/>
                    : string    : String stripped of white spaces.          <para/>
          WARN:                                                             <br/>
                Throws E_INV_ARG if string is null.                         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string removeWhiteSpaces(this string str) {
            int len;
            int tar = 0;
            char[] src;
            
            if (str == null)
                exc("E_INV_ARG", "str");
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
          FUNC: whiteSpacesToSpace [static extension]                       <summary>
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
        public static string whiteSpacesToSpace(this string str) {
            int len;
            char[] src;
            int dstIx = 0;
            bool inWsp = false;

            if (str == null)
                exc("E_INV_ARG", "str");
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
          FUNC: removeChars [static extension].                             <summary>
          TASK:                                                             <br/>
                Removes chars in remove from string.                        <para/>
          ARGS:                                                             <br/>
                str     : string :  String to remove characters from.       <br/>
                remove  : string :  String containing characters to 
                                    remove from key.                        <para/>
          WARN:                                                             <br/>
                Throws E_INV_ARG if string is null.                         </summary>
       ————————————————————————————————————————————————————————————————————————————*/
        public static string removeChars(this string str, string remove) {
            StringBuilder ret;

            if (remove.isNullOrWhiteSpace())
                return str;
            if (str == null)
                exc("E_INV_ARG", "str");
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
          FUNC: isIdentifierBegin [static extension]                    <summary>
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
        public static bool isIdentifierBegin(this char chr) {
            return (chr >= 'a' && chr <= 'z') ||
                   (chr >= 'A' && chr <= 'Z') ||
                    chr == '_' ||
                    Char.IsLetter(chr);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: isIdentifierInner [static extension]                    <summary>
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
        public static bool isIdentifierInner(this char chr) {
            return (chr >= 'a' && chr <= 'z') ||
                   (chr >= 'A' && chr <= 'Z') ||
                   (chr >= '0' && chr <= '9') ||
                    chr == '_' ||
                    Char.IsLetter(chr);
        }
        #endregion
    }
}
