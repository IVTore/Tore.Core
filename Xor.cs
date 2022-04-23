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
          FUNC: CryptByteArrays [static]                                <summary>
          TASK:                                                         <br/>
                XOR's two byte arrays.                                  <br/>
                If xorKey is shorter, it is repeated over encDec.       <para/>
          ARGS:                                                         <br/>
                encDec  : byte[]    : Byte array to Encrypt or Decrypt. <br/>   
                xorKey  : byte[]    : Byte array used as Key.           <para/>
          RETV:                                                         <br/>
                        : byte[]    : XOR'ed byte array.                <para/>
          WARN:                                                         <br/>
                Throws exception if byte arrays are null or             <br/>
                xorKey length = 0.                                      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static byte[] CryptByteArrays(byte[] encDec, byte[] xorKey) {
            int idx,
                eln,
                kln;
            byte[] res;

            Chk(encDec, nameof(encDec));
            Chk(xorKey, nameof(xorKey));
            eln = encDec.Length;
            kln = xorKey.Length;
            if (kln == 0)
                Exc("E_INV_ARG", "xorKey.Length = 0");
            res = new byte[eln];
            for (idx = 0; idx < eln; idx++)
                res[idx] = (byte)(encDec[idx] ^ xorKey[idx % kln]);
            return res;
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: CryptStrings [static]                                       <summary>
          TASK:                                                             <br/>
                Encodes UTF8 strings to byte arrays, XOR's them,            <br/>
                returns XOR'ed byte array.                                  <br/>
                if xorKey is shorter, it is repeated over encDec.           <para/>
          ARGS:                                                             <br/>
                encDec  : string    : String to Encrypt or Decrypt.         <br/>
                xorKey  : string    : String used as Key.                   <para/>
          RETV:                                                             <br/>
                        : byte[]    : XOR'ed byte array.                    <para/>
          WARN:                                                             <br/>
                Throws exception if strings are null or empty               </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static byte[] CryptStrings(string encDec, string xorKey) {
            byte[] enc;
            byte[] key;

            Chk(encDec, nameof(encDec));
            Chk(xorKey, nameof(xorKey));
            enc = Encoding.UTF8.GetBytes(encDec);
            key = Encoding.UTF8.GetBytes(xorKey);
            return CryptByteArrays(enc, key);
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: Decrypt [static]                                            <summary>
          TASK:                                                             <br/>
                Decodes an hex string into an UTF8 string, using two keys.  <br/>
                if xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                hex     : string    : Hex data to decode                    <br/>
                encKey  : string    : Primary   encryption Key.             <br/>
                xorKey  : string    : Secondary encryption Key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          RETV:                                                             <br/>
                        : string    : Decoded string.                       <para/>
          WARN:                                                             <br/>
                Throws exception if hex, encKey or xorKey are null or empty.</summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string Decrypt(string hex, string encKey, string xorKey, string strip = "-"){
            byte[] decBuf;
            byte[] keyArr = PrepDualKey(encKey, xorKey, strip);

            Chk(hex, nameof(hex));
            decBuf = Hex.ToByteArray(hex);
            decBuf = CryptByteArrays(decBuf, keyArr);
            return Encoding.UTF8.GetString(decBuf);
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: Encrypt [static]                                            <summary>
          TASK:                                                             <br/>
                Encodes an UTF8 string into an hex string, using two keys.  <br/>
                if xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                data    : string    : Data to encode                        <br/>
                encKey  : string    : Primary   encryption Key.             <br/>
                xorKey  : string    : Secondary encryption Key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          RETV:                                                             <br/>
                        : string    : Encoded string.                       <para/>
          WARN:                                                             <br/>
            Throws exception if data, encKey or xorKey are null or empty.   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string Encrypt(string data,
                                     string encKey, 
                                     string xorKey, 
                                     string strip = "-") {
            byte[] encBuf;
            byte[] keyArr = PrepDualKey(encKey, xorKey, strip);
            
            encBuf = Encoding.UTF8.GetBytes(data);
            encBuf = CryptByteArrays(encBuf, keyArr);
            return Encoding.UTF8.GetString(encBuf);
        }

        /// <summary>
        /// Checks and prepares dual keys.
        /// </summary>
        private static byte[] PrepDualKey(string encKey, string xorKey, string strip){
            Chk(encKey, nameof(encKey));
            Chk(xorKey, nameof(xorKey));
            encKey.RemoveChars(strip);
            xorKey.RemoveChars(strip);
            return CryptStrings(encKey, xorKey);
        }

        #region Crypto File routines.
        /*————————————————————————————————————————————————————————————————————————————
            ————————————————————————————————————
            |   Crypto File routines           |
            ————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/
        private const string CF_HDR = "TORE:CF01:";

        /**———————————————————————————————————————————————————————————————————————————
          FUNC:  IsCryptoFile [static]                                      <summary>
          TASK:                                                             <br/>
                 Checks if given file contains Crypto File header.          <para/>
          ARGS:                                                             <br/>
                 file : string  : filename                                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool IsCryptoFile(string file) {
            char[] buf = new char[CF_HDR.Length];

            using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
                reader.Read(buf, 0, buf.Length);
            return CF_HDR.Equals(buf.ToString());
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: DecryptFromFile [static]                                    <summary>
          TASK:                                                             <br/>
                Loads and decrypts contents of an encrypted file into       <br/>
                an UTF8 string, using two keys.                             <br/>
                if xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                file    : string    : File specification.                   <br/>
                encKey  : string    : Primary   encryption Key.             <br/>
                xorKey  : string    : Secondary encryption Key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          RETV:                                                             <br/>
                        : string    : Decoded string from file.             <para/>
          WARN:                                                             <br/>
                Throws exception if anything is null or empty except strip. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string DecryptFromFile(string file, 
                                             string encKey, 
                                             string xorKey, 
                                             string strip = "-") {
            string buf;

            Chk(file, nameof(file));
            buf = Utf8File.Load(file);
            if (!buf.StartsWith(CF_HDR))
                Exc("E_INV_ENC", file);
            return Decrypt(buf.Substring(CF_HDR.Length), encKey, xorKey, strip);
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: EncryptToFile [static]                                      <summary>
          TASK:                                                             <br/>
                Encrypts contents of an UTF8 string, using two keys, then   <br/>
                writes it into a file.                                      <br/>
                if xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                data    : string    : String to Encrypt.                    <br/>
                file    : string    : File specification.                   <br/>
                encKey  : string    : Primary   encryption Key.             <br/>
                xorKey  : string    : Secondary encryption Key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          WARN:                                                             <br/>
                Throws exception if anything is null or empty except strip. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void EncryptToFile(string data,
                                         string file,
                                         string encKey,
                                         string xorKey,
                                         string strip = "-") {
            Chk(file, nameof(file));
            data = Encrypt(data, encKey, xorKey, strip);
            Utf8File.Save(file, CF_HDR + data);
        }

        #endregion
    }
}
