using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Tore.Core.Sys;

namespace Tore.Core {

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Xor [Static]                                            <summary>
        USAGE:                                                          <br/>
                Contains a library of static utility methods            <br/>
                for simple xor encryption and decryption.               </summary>
    ————————————————————————————————————————————————————————————————————————————*/

    public static class Xor {
        /**————————————————————————————————————————————————————————————————————————————
          FUNC: cryptByteArrays [static]                                <summary>
          TASK:                                                         <br/>
                XOR's two byte arrays.                                  <br/>
                If xorKey is shorter, it is repeated over encDec.       <para/>
          ARGS:                                                         <br/>
                encDec  : byte[]    : Byte array to encrypt or decrypt. <br/>   
                xorKey  : byte[]    : Byte array used as key.           <para/>
          RETV:                                                         <br/>
                        : byte[]    : XOR'ed byte array.                <para/>
          WARN:                                                         <br/>
                Throws exception if byte arrays are null or             <br/>
                xorKey length = 0.                                      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static byte[] cryptByteArrays(byte[] encDec, byte[] xorKey) {
            int idx,
                eln,
                kln;
            byte[] res;

            chk(encDec, nameof(encDec));
            chk(xorKey, nameof(xorKey));
            eln = encDec.Length;
            kln = xorKey.Length;
            if (kln == 0)
                exc("E_INV_ARG", "xorKey.Length = 0");
            res = new byte[eln];
            for (idx = 0; idx < eln; idx++)
                res[idx] = (byte)(encDec[idx] ^ xorKey[idx % kln]);
            return res;
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: cryptStrings [static]                                       <summary>
          TASK:                                                             <br/>
                Encodes UTF8 strings to byte arrays, XOR's them,            <br/>
                returns XOR'ed byte array.                                  <br/>
                if xorKey is shorter, it is repeated over encDec.           <para/>
          ARGS:                                                             <br/>
                encDec  : string    : String to encrypt or decrypt.         <br/>
                xorKey  : string    : String used as key.                   <para/>
          RETV:                                                             <br/>
                        : byte[]    : XOR'ed byte array.                    <para/>
          WARN:                                                             <br/>
                Throws exception if strings are null or empty               </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static byte[] cryptStrings(string encDec, string xorKey) {
            byte[] enc;
            byte[] key;

            chk(encDec, nameof(encDec));
            chk(xorKey, nameof(xorKey));
            enc = Encoding.UTF8.GetBytes(encDec);
            key = Encoding.UTF8.GetBytes(xorKey);
            return cryptByteArrays(enc, key);
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: decrypt [static]                                            <summary>
          TASK:                                                             <br/>
                Decodes an hex string into an UTF8 string, using two keys.  <br/>
                if xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                hex     : string    : Hex data to decode                    <br/>
                encKey  : string    : Primary   encryption key.             <br/>
                xorKey  : string    : Secondary encryption key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          RETV:                                                             <br/>
                        : string    : Decoded string.                       <para/>
          WARN:                                                             <br/>
                Throws exception if hex, encKey or xorKey are null or empty.</summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string decrypt(string hex, string encKey, string xorKey, string strip = "-"){
            byte[] decBuf;
            byte[] keyArr = prepDualKey(encKey, xorKey, strip);

            chk(hex, nameof(hex));
            decBuf = hexStrToByteArr(hex);
            decBuf = cryptByteArrays(decBuf, keyArr);
            return Encoding.UTF8.GetString(decBuf);
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: encrypt [static]                                            <summary>
          TASK:                                                             <br/>
                Encodes an UTF8 string into an hex string, using two keys.  <br/>
                if xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                data    : string    : Data to encode                        <br/>
                encKey  : string    : Primary   encryption key.             <br/>
                xorKey  : string    : Secondary encryption key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          RETV:                                                             <br/>
                        : string    : Encoded string.                       <para/>
          WARN:                                                             <br/>
            Throws exception if data, encKey or xorKey are null or empty.   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string encrypt(string data,
                                     string encKey, 
                                     string xorKey, 
                                     string strip = "-") {
            byte[] encBuf;
            byte[] keyArr = prepDualKey(encKey, xorKey, strip);
            
            encBuf = Encoding.UTF8.GetBytes(data);
            encBuf = cryptByteArrays(encBuf, keyArr);
            return Encoding.UTF8.GetString(encBuf);
        }

        /// <summary>
        /// Checks and prepares dual keys.
        /// </summary>
        private static byte[] prepDualKey(string encKey, string xorKey, string strip){
            chk(encKey, nameof(encKey));
            chk(xorKey, nameof(xorKey));
            encKey.removeChars(strip);
            xorKey.removeChars(strip);
            return cryptStrings(encKey, xorKey);
        }

        #region Crypto File routines.
        /*————————————————————————————————————————————————————————————————————————————
            ————————————————————————————————————
            |   Crypto File routines           |
            ————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        private const string CF_HDR = "TORE:CF01:";

        /**———————————————————————————————————————————————————————————————————————————
          FUNC:  isCryptoFile [static]                                      <summary>
          TASK:                                                             <br/>
                 Checks if given file contains Crypto File header.          <para/>
          ARGS:                                                             <br/>
                 file : string  : filename                                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool isCryptoFile(string file) {
            char[] buf = new char[CF_HDR.Length];

            using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                reader.Read(buf, 0, buf.Length);
            return CF_HDR.Equals(buf.ToString());
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: decryptFromFile [static]                                    <summary>
          TASK:                                                             <br/>
                Loads and decrypts contents of an encrypted file into       <br/>
                an UTF8 string, using two keys.                             <br/>
                if xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                file    : string    : File specification.                   <br/>
                encKey  : string    : Primary   encryption key.             <br/>
                xorKey  : string    : Secondary encryption key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          RETV:                                                             <br/>
                        : string    : Decoded string from file.             <para/>
          WARN:                                                             <br/>
                Throws exception if anything is null or empty except strip. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string decryptFromFile(string file, 
                                             string encKey, 
                                             string xorKey, 
                                             string strip = "-") {
            string buf;

            chk(file, nameof(file));
            buf = loadUtf8File(file);
            if (!buf.StartsWith(CF_HDR))
                exc("E_INV_ENC", file);
            return decrypt(buf.Substring(CF_HDR.Length), encKey, xorKey, strip);
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: encryptToFile [static]                                      <summary>
          TASK:                                                             <br/>
                Encrypts contents of an UTF8 string, using two keys, then   <br/>
                writes it into a file.                                      <br/>
                if xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                data    : string    : String to encrypt.                    <br/>
                file    : string    : File specification.                   <br/>
                encKey  : string    : Primary   encryption key.             <br/>
                xorKey  : string    : Secondary encryption key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          WARN:                                                             <br/>
                Throws exception if anything is null or empty except strip. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void encryptToFile(string data,
                                         string file,
                                         string encKey,
                                         string xorKey,
                                         string strip = "-") {
            chk(file, nameof(file));
            data = encrypt(data, encKey, xorKey, strip);
            saveUtf8File(file, CF_HDR + data);
        }

        #endregion
    }
}
