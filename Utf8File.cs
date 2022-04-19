using System;
using System.IO;
using System.Text;

using static Tore.Core.Sys;

namespace Tore.Core {

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Utf8File [Static]                                       <summary>
        USAGE:                                                          <br/>
                Contains utility methods to load and save UTF8 files.   </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Utf8File {
        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Load [static]                                               <summary>
          TASK:                                                             <br/>
                Reads an Utf8 encoded file to a string.                     <para/>
          ARGS:                                                             <br/>
                filespec    : string : filename with path.                  <para/>
          RETV:                                                             <br/>
                            : string : Utf8 encoded content of file.        </summary>
       ————————————————————————————————————————————————————————————————————————————*/
        public static string Load(string fileSpec) {
            Chk(fileSpec, nameof(fileSpec));
            try {
                return File.ReadAllText(fileSpec, Encoding.UTF8);
            } catch (Exception e) {
                Exc("E_UTF8_FILE_LOAD", fileSpec, e);
                throw;
            }
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Save [static].                                              <summary>
          TASK:                                                             <br/>
                Writes an Utf8 string to a file.                            <para/>
          ARGS:                                                             <br/>
                fileSpec    : string : filename with path.                  <br/>
                text        : string : Utf8 text.                           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void Save(string fileSpec, string text) {
            Chk(fileSpec, nameof(fileSpec));
            try {
                File.WriteAllText(fileSpec, text, Encoding.UTF8);
            } catch (Exception e) {
                Exc("E_UTF8_FILE_SAVE", fileSpec, e);
                throw;
            }
        }


    }
}
