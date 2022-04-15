using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Tore.Core.Sys;

namespace Tore.Core {

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Cfg [Static]                                            <summary>
        USAGE:                                                          <br/>
                Contains a library of static utility methods            <br/>
                for simple encrypted configuration file support         <br/>
                with fallbacks.                                         <para/>
                Configurations are loaded to and saved from             <br/>
                a classes <b> public static fields </b>.                </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Cfg {

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: load [static]                                               <summary>
          TASK:                                                             <br/>
                Loads and decrypts contents of an encrypted file into       <br/>
                <b> public static fields </b> of a class, using two keys.   <br/>
                If file is not encrypted, it is not decrypted.              <br/>
                If xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                type    : Type      : Class with public static fields.      <br/>
                file    : string    : File specification.                   <br/>
                encKey  : string    : Primary   encryption key.             <br/>
                xorKey  : string    : Secondary encryption key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          WARN:                                                             <br/>
                Throws exception if anything is null or empty except strip. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void load(Type type,
                                string file,
                                string encKey,
                                string xorKey,
                                string strip = "-") {
            string json;

            chk(type, nameof(type), "E_CFG_INV_ARG");
            chk(file, nameof(file), "E_CFG_INV_ARG");
            if (!File.Exists(file))
                exc("E_CFG_FILE_NA", file);
            try {
                if (Xor.isCryptoFile(file))
                    json = Xor.decryptFromFile(file, encKey, xorKey, strip);
                else
                    json = loadUtf8File(file);
                new Stl(json).toStatic(type, true);
            } catch (Exception e) {
                exc("E_CFG_LOAD_FAIL", "", e);
                throw;
            }
        }

        /**————————————————————————————————————————————————————————————————————————————
          FUNC: save [static]                                               <summary>
          TASK:                                                             <br/>
                Encrypts and saves <b> public static fields </b> of a class <br/>
                into a file, using two keys.                                <br/>
                If keys are empty file is not encrypted.                    <br/>
                If xorKey is shorter, it is repeated over encKey.           <para/>
          ARGS:                                                             <br/>
                type    : Type      : Class with public static fields.      <br/>
                file    : string    : File specification.                   <br/>
                encKey  : string    : Primary   encryption key.             <br/>
                xorKey  : string    : Secondary encryption key.             <br/>
                strip   : string    : Characters to remove from keys.       <para/>
          WARN:                                                             <br/>
                Throws exception if anything is null or empty except strip. </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void save(Type type,
                                string file,
                                string encKey,
                                string xorKey,
                                string strip = "-") {
            string json;

            chk(type, nameof(type), "E_CFG_INV_ARG");
            chk(file, nameof(file), "E_CFG_INV_ARG");
            try {
                json = new Stl(type).toJson();
                if (encKey.isNullOrWhiteSpace() || xorKey.isNullOrWhiteSpace()) 
                    saveUtf8File(file, json);
                else
                    Xor.encryptToFile(json, file, encKey, xorKey, strip);
            } catch (Exception e) {
                exc("E_CFG_SAVE_FAIL", "", e);
                throw;
            }
        }
    }
}
