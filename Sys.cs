/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————
    |   Sys : C# Utility methods library         |
    ——————————————————————————————————————————————

© Copyright 2020 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202303191158
License             : MIT.

History             :
202303191158: IVT   : Reflection moved to Reflect.cs.
202003101700: IVT   : Revision.
202006131333: IVT   : Removal of unnecessary usings.
————————————————————————————————————————————————————————————————————————————*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

using Microsoft.Extensions.Logging;

using Newtonsoft.Json.Linq;

namespace Tore.Core {

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Sys [Static]                                            <summary>
        USAGE:                                                          <br/>
                Contains a library of static utility methods treated as <br/>
                global functions which is used for managing:            <para/>
                Exceptions,                                             <br/>
                Logging,                                                <br/>
                Application.                                            <para/>
                The best way of using them is by adding:                <br/>
                using static Tore.Core.Sys;                             <br/>
                to the source file.                                     </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Sys {


        #region Handy constants.

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

        #region Exception Subsystem.
        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————————
            |  Exception Subsystem.  |
            ——————————————————————————
            A Practical Exception Subsystem.
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          TYPE: ExceptionInterceptorDelegate [delegate]                     <summary>
          TASK: Exception Interceptor Type.                                 </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public delegate void ExceptionInterceptorDelegate(Exception e, StrLst dta);

        /**———————————————————————————————————————————————————————————————————————————
          PROP: exceptionInterceptor: ExceptionInterceptorDelegate [static].<summary>
          GET : Gets Exception Interceptor method.                          <br/>
          SET : Sets Exception Interceptor method.                          <br/>
          INFO: Used for logging monitoring etc.                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static ExceptionInterceptorDelegate exceptionInterceptor { get; set; } = null;

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: HasExcData [static]                                          <summary>
          TASK:                                                             <br/>
                If an exception is generated or processed through           <br/>
                <b>Exc</b>, it Has extra data and this will return true.    <para/>
1         ARGS:                                                             <br/>
                e   : Exception : Any Exception.                            <para/>
          RETV:                                                             <br/>
                    : bool      : True if exception has extra data.         </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool HasExcData(Exception e) {
            return (e != null) && (e.Data.Contains("dta"));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: GetExcData [static]                                         <summary>
          TASK:                                                             <br/>
                If an exception is generated or processed through           <br/>
                <b> Exc </b>, it Has extra data and this will return it.    <para/>
          ARGS:                                                             <br/>
                e   : Exception : Any Exception.                            <para/>
          RETV:                                                             <br/>
                    : StrLst       : Extra exception data or null              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static StrLst GetExcData(Exception e) {
            if (!HasExcData(e))
                return null;
            return (StrLst)(e.Data["dta"]);
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
                * Collected Data: at e.Data["dta"] as a StrLst*.            <br/>
                    "Exc" = Class name of exception.                        <br/>
                    "msg" = Exception message.                              <br/>
                    "tag" = Exception tag.                                  <br/>
                    "inf" = Exception info.                                 <br/>
                    "loc" = Call location (Class and method).               <para/>
                *StrLst is a string associated object list class. 
                 Info about StrLst can be found at StrLst.cs.               </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Exception Exc(string tag = "E_NO_TAG", 
                                    string inf = "", 
                                    Exception e = null) {
            Exception cex;
            StrLst dta;
            MethodBase met;

            cex = e ?? new ToreCoreException(tag);      // If no exception make one.
            if (HasExcData(cex))                        // If exception already processed
                return null;                            // return.
            dta = new StrLst(){                            // Collect data.
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
            exceptionInterceptor?.Invoke(cex, dta);     // if assigned, invoke.
            cex.Data.Add("dta", (object)dta);
            if (logger == null)                         // if no logger.
                ExcLog(dta);                            // log exception to console.
            if (e == null)                              // If we made the exception
                throw cex;                              // throw it
            return cex;
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: ExcLog [static]                                             <summary>
          TASK: Outputs exception info to console.                          <para/>
          ARGS: ed  : StrLst       : Exception data in StrLst form.         <para/>
          INFO: The ed StrLst can be found in exception.Data["dta"].        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        private static void ExcLog(StrLst ed) {
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

        #region Log Subsystem.
        /*————————————————————————————————————————————————————————————————————————————
            ————————————————————
            |  Log Subsystem.  |
            ————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          PROP: logger : ILogger [Static]                                   <summary>
          GET : Gets logger object.                                         <br/>
          SET : Sets logger object.                                         <br/>
          TASK: Logger object for systemwide log support.                   <br/>
          INFO: Assign this first...                                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static ILogger logger;






        
        #endregion

        #region Application utility functions.
        /*————————————————————————————————————————————————————————————————————————————
            ———————————————————————————————————————
            |   Application utility functions.    |
            ———————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

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

    }

    #region ToreCoreException class.
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
    #endregion

}