using System.Globalization;
using System.Text;


using static Tore.Core.Sys;

namespace Tore.Core {
    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Hex [Static]                                            <summary>
        USAGE:                                                          <br/>
            Contains static utility methods for Hex string conversions. </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Hex {

        #region Hex string conversions.
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ToInt [static]                                              <summary>
          TASK:                                                             <br/>
                Returns integer from hex string.                            <para/>
          ARGS:                                                             <br/>
                hex : String        : hexadecimal number.                   <para/>
          RETV:                                                             <br/>
                    : int           : integer value.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static int ToInt(string hex) {
            return int.Parse(hex, NumberStyles.HexNumber);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: FromInt [static]                                            <summary>
          TASK:                                                             <br/>
                Returns hex string from integer.                            <para/>
          ARGS:                                                             <br/>
                i       : int       : integer value.                        <br/>
                digits  : int       : Number of hex digits.                 <para/>
          RETV:                                                             <br/>
                        : String    : Formatted hexadecimal string.         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string FromInt(int i, int digits) {
            return i.ToString("x" + digits.ToString());
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ToByteArray [static]                                        <summary>
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
        public static byte[] ToByteArray(string hex) {
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
            for (i = 0; i < l; i++) {
                p = i * 2;
                a[i] = (byte)((n2i(p) << 4) | n2i(p + 1));
            }
            return a;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: FromByteArr [static]                                    <summary>
          TASK:                                                             <br/>
                Converts a byte array to <b> CAPITAL </b> hex string.       <para/>
          ARGS:                                                             <br/>
                arr : byte[]    : Byte array.                               <para/>
          RETV:                                                             <br/>
                    : string    : Hex string.                               <para/>
          INFO:                                                             <br/>
                'A'- 0xA => 65 - 10 = 55.                                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string FromByteArr(byte[] arr) {
            int i,              // iterator.
                l,              // length.
                n;              // nibble value.
            StringBuilder s;    // String builder.

            Chk(arr, nameof(arr));
            l = arr.Length;
            s = new StringBuilder(l * 2);
            for (i = 0; i < l; i++) {
                n = (arr[i] & 0xF0) >> 4;
                s.Append((char)((n < 0xA) ? (n + '0') : (n + 55)));
                n = (arr[i] & 0x0F);
                s.Append((char)((n < 0xA) ? (n + '0') : (n + 55)));
            }
            return s.ToString();
        }
        #endregion
    }
}
