﻿/*————————————————————————————————————————————————————————————————————————————
    ——————————————————————————————————————————————
    |   Sys : C# Utility methods library         |
    ——————————————————————————————————————————————

© Copyright 2023 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202403131321: (v8.0.3).
License             : MIT.

History             :
202403131321: IVT   : Revision and upgrade to .NET 8 LTS.
202310071044: IVT   : Generic Attr<T>() method modified to return T.
202303191158: IVT   : 
    * Dbg(), isDebug etc. removed.
    * ExcInterceptorDelegate type renamed as ExceptionInterceptorDelegate.
    * excInterceptor property is renamed as exceptionInterceptor.
    * bool exceptionInfoToConsole property added.
    * exception debug output is removed, and routed to console.
    * public HasExcData moved into Extensions.cs as Exception.HasInfo();
    * public GetExcData moved into Extensions.cs as Exception.Info();
    * public ExcDbg     moved into Extensions.cs as Exception.InfoToConsole();
202003101700: IVT   : Revision.
202006131333: IVT   : Removal of unnecessary usings.
————————————————————————————————————————————————————————————————————————————*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json.Linq;

namespace Tore.Core {

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Sys [Static]                                            <summary>
        USAGE:                                                          <br/>
                Contains a library of static utility methods treated as <br/>
                global functions which is used for managing:            <para/>
                Exceptions            [ Exc() ],                        <br/>
                Parameter checking    [ Chk() ],                        <br/>
                Reflection, Attributes, Type juggling,                  <br/>
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

        #endregion

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
          PROP: exceptionInfoToConsole: bool [static].                      <summary>
          GET : Returns the value of property                               <br/>
          SET : Sets If exception data will be written to console.          <br/>
          INFO: Default is true.                                            </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool exceptionInfoToConsole { get; set; } = true;

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
                If exceptionInfoToConsole is true:
                    Writes exception info to console.                       <br/>
                If e was null at entry:
                    <b> throws </b> the exception built.                    <br/>
                else
                    returns e (for rethrowing etc.).                        <br/>
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

            tag ??= "E_NO_TAG";
            inf ??= "";
            cex = e ?? new ToreCoreException(tag);      // If no exception make one.
            if (cex.HasInfo())                          // If exception already processed
                return cex;                             // return.
            dta = new StrLst(){                         // Collect data.
                {"exc", cex.GetType().FullName},
                {"msg", cex.Message},
                {"tag", tag},
                {"inf", inf}
            };
            met = new StackTrace(2)?.GetFrame(0)?.GetMethod();
            if (met != null) { 
                if ( met.Name.Equals("chk") || met.Name.StartsWith("<") )
                    met = new StackTrace(3)?.GetFrame(0)?.GetMethod();
            }
            if (met != null)
                dta.Add("loc", $"{met.DeclaringType?.Name}.{met.Name}");
            exceptionInterceptor?.Invoke(cex, dta);     // if assigned, invoke.
            cex.Data.Add("dta", (object)dta);
            if (exceptionInfoToConsole)                 // if console output permitted.
                cex.InfoToConsole();                    // write exception info to console.
            if (e == null)                              // If we made the exception
                throw cex;                              // throw it
            return cex;
        }

        #endregion

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
        public static void Chk(object arg = null, string inf = null, string tag = "E_INV_ARG") {
            if (arg == null ||
               (arg is string str && str.IsNullOrWhiteSpace()) ||
               (arg is Guid guid && guid.Equals(Guid.Empty)))
                Exc(tag, inf);
        }

        #region Reflection utility functions.

        /*————————————————————————————————————————————————————————————————————————————
            ——————————————————————————————————————
            |   Reflection utility functions.    |
            ——————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Attr [static]                                               <summary>
          TASK:                                                             <br/>
                Fetches an attribute of a member. Non-generic version.      <para/>
          ARGS:                                                             <br/>
                attrType    : Type          :   Type of attribute.          <br/>
                memInfo     : MemberInfo    :   Member information.         <para/>
          RETV:             : Attribute     :   The attribute or null if
                                                not found.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Attribute Attr(Type attrType, MemberInfo memInfo) {
            Chk(attrType, nameof(attrType));
            Chk(memInfo, nameof(memInfo));
            return memInfo.GetCustomAttribute(attrType, true);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Attr [static]                                               <summary>
          TASK:                                                             <br/>
                Fetches an attribute of a member. Generic version.          <para/>
          ARGS:                                                             <br/>
                T           : Type(Class)   :   Type of attribute.          <br/>
                memInfo     : MemberInfo    :   Member information.         <para/>
          RETV:             : T             :   The attribute or null if
                                                not found.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T Attr<T>(MemberInfo memInfo) where T : Attribute {
            Chk(memInfo, nameof(memInfo));
            return (T)memInfo.GetCustomAttribute(typeof(T), true);
        }


        /**———————————————————————————————————————————————————————————————————————————
          FUNC: hasAttr [static]                                            <summary>
          TASK:                                                             <br/>
                Checks if a member Has an attribute.                        <para/>
          ARGS:                                                             <br/>
                T           : Type (Class)  : Type of attribute.            <br/>
                memInfo     : MemberInfo    : Member information.           <para/>
          RETV:             : bool          : True if attribute exists 
                                              else false.                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static bool hasAttr<T>(MemberInfo memInfo) {
            return (Attr(typeof(T), memInfo) != null);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: NameInAttr [static]                                         <summary>
          TASK:                                                             <br/>
                Fetches name in a NameMetadata attribute of any member.     <para/>
          ARGS:                                                             <br/>
                T           : Type (Class)  : Type of attribute.            <br/>
                memInfo     : MemberInfo    : Member information.           <para/>
          RETV:             : String        : If attribute exists, 
                                              name in attribute else null.  <para/>
          INFO:                                                             <br/>
                Attribute class must be descendant of NameMetadata class.
                Otherwise the return value will be null.                    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static string NameInAttr<T>(MemberInfo memInfo) {
            Attribute a;

            a = Attr(typeof(T), memInfo);
            return (a is NameMetadata nmd) ? nmd.name : null;
        }

        private static Type fetchType(object src, string name) {
            if (src == null)
                Exc("E_TYPE_SRC_NULL", name);
            return (src is Type t) ? t : src.GetType();
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: InstanceProp [static]                                       <summary>
          TASK:                                                             <br/>
                Fetches Public Property info from a class.                  <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                propNam : String        : Instance Property name.           <br/>
                inherit : bool          : Include base classes to the
                                          search of property.:DEF:true.     <para/>
          RETV:         : PropertyInfo  : If exists, the property info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo InstanceProp(object propSrc,
                                                string propNam,
                                                bool inherit = true) {
            Chk(propNam, nameof(propNam));
            return fetchType(propSrc, nameof(propSrc)).GetProperty(
                    propNam,
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: InstanceProps [static]                                      <summary>
          TASK:                                                             <br/>
                Fetches <b> all </b> Public Property infos from a class.    <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                inherit : bool          : Include inherited properties
                                          :DEF:true.                        <para/>
          RETV:                                                             <br/>
                : PropertyInfo[]: Property info array, never null.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo[] InstanceProps(object propSrc, bool inherit = true) {
            return fetchType(propSrc, nameof(propSrc)).GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Instance |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: StaticProp [static]                                         <summary>
          TASK:                                                             <br/>
                Fetches Public static Property info from a class.           <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                propNam : String        : Static Property name.             <br/>
                inherit : bool          : Include base classes to the
                                          search of property.:DEF:true.     <para/>
          RETV:                                                             <br/>
                        : PropertyInfo  : If exists, the property info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo StaticProp(object propSrc,
                                              string propNam,
                                              bool inherit = true) {
            Chk(propNam, nameof(propNam));
            return fetchType(propSrc, nameof(propSrc)).GetProperty(
                    propNam,
                    BindingFlags.Public |
                    BindingFlags.Static |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: StaticProps [static]                                        <summary>
          TASK:                                                             <br/>
                Fetches <b>all</b> Public static property infos of a class. <para/>
          ARGS:                                                             <br/>
                propSrc : Object        : Type or Instance.                 <br/>
                inherit : bool          : Include inherited properties.
                                          :DEF:true.                        <para/>
          RETV:                                                             <br/>
                : PropertyInfo[]: Property info array, never null.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static PropertyInfo[] StaticProps(object propSrc, bool inherit = true) {
            return fetchType(propSrc, nameof(propSrc)).GetProperties(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: StaticField [static]                                        <summary>
          TASK:                                                             <br/>
                Fetches Public Static Field info from a class.              <para/>
          ARGS:                                                             <br/>
                fieldSrc    : Object        : Type or Instance.             <br/>
                fieldNam    : String        : Static Field name.            <br/>
                inherit     : bool          : Include base classes to the
                                              search of field. :DEF:true.   <para/>
          RETV:                                                             <br/>
                        : fieldInfo     : If exists, the field info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static FieldInfo StaticField(object fieldSrc,
                                            string fieldNam,
                                            bool inherit = true) {
            Chk(fieldNam, nameof(fieldNam));
            return fetchType(fieldSrc, nameof(fieldSrc)).GetField(
                fieldNam,
                BindingFlags.Public |
                BindingFlags.Static |
                (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: StaticFields [static]                                       <summary>
          TASK:                                                             <br/>
                Fetches <b>all</b> Public static field infos of a class.    <para/>
          ARGS:                                                             <br/>
                fieldSrc : Object       : Type or Instance.                 <br/>
                inherit  : bool         : Include inherited fields.
                                          :DEF:true.                        <para/>
          RETV:                                                             <br/>
                         : FieldInfo[]  : Field info array, never null.     </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static FieldInfo[] StaticFields(object fieldSrc, bool inherit = true) {
            return fetchType(fieldSrc, nameof(fieldSrc)).GetFields(
                    BindingFlags.Public |
                    BindingFlags.Static |
                    (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: InstanceMethod [static]                                     <summary>
          TASK:                                                             <br/>
                Fetches Public Method info from a class.                    <para/>
          ARGS:                                                             <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Instance method name.             <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:                                                             <br/>
                        : MethodInfo    : If exists, the method info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static MethodInfo InstanceMethod(object metSrc,
                                                string metNam,
                                                bool inherit = true) {
            Chk(metNam, nameof(metNam));
            return fetchType(metSrc, nameof(metSrc)).GetMethod(
                metNam,
                BindingFlags.Public |
                BindingFlags.Instance |
                (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: StaticMethod [static]                                       <summary>
          TASK:                                                             <br/>
                Fetches Public Static Method info from a class.             <para/>
          ARGS:                                                             <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Static Method name.               <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:                                                             <br/>
                        : MethodInfo    : If exists, the method info, 
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static MethodInfo StaticMethod(object metSrc,
                                              string metNam,
                                              bool inherit = true) {
            Chk(metNam, nameof(metNam));
            return fetchType(metSrc, nameof(metSrc)).GetMethod(
                metNam,
                BindingFlags.Public |
                BindingFlags.Static |
                (inherit ? BindingFlags.FlattenHierarchy : 0)
            );
        }

        /**——————————————————————————————————————————————————————————————————————————
          FUNC: InstanceDelegate [static]                                   <summary>
          TASK:                                                             <br/>
                Finds Public Instance Method of a class                     <br/>
                and returns it as a delegate of Type T.                     <para/>
          ARGS:                                                             <br/>
                T       : Type          : Delegate type.                    <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Instance method name.             <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:                                                             <br/>
                        : T             : If method exists, the delegate,
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T InstanceDelegate<T>(object metSrc,
                                            string metNam,
                                            bool inherit = true)
                                            where T : Delegate {
            return InstanceMethod(metSrc, metNam, inherit)?.CreateDelegate<T>();
        }

        /**——————————————————————————————————————————————————————————————————————————
          FUNC: StaticDelegate [static]                                     <summary>
          TASK:                                                             <br/>
                Finds Public Static Method of a class                       <br/>
                and returns it as a delegate of Type T.                     <para/>
          ARGS:                                                             <br/>
                T       : Type          : Delegate type.                    <br/>
                metSrc  : Object        : Class Type or Instance.           <br/>
                metNam  : String        : Static method name.               <br/>
                inherit : bool          : Include base classes to the
                                          search of method. :DEF:true.      <para/>
          RETV:                                                             <br/>
                        : T             : If method exists, the delegate,
                                          else null.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T StaticDelegate<T>(object metSrc, string metNam, bool inherit = true)
                                          where T : Delegate {
            return StaticMethod(metSrc, metNam, inherit)?.CreateDelegate<T>();
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: MakeObject [static]                                         <summary>
          TASK:                                                             <br/>
                Builds an object of a given type.                           <para/>
          ARGS:                                                             <br/>
                template: object            : Object or Type of object.     <br/>
                args    : params object[]   : Constructor parameters if any.<para/>
          RETV:                                                             <br/>
                        : object            : Object constructed.           </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object MakeObject(object template, params object[] args) {
            return Activator.CreateInstance(fetchType(template, nameof(template)), args);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetInstanceProp [static]                                    <summary>
          TASK:                                                             <br/>
                Sets a property on target instance to a value.              <para/>
          ARGS:                                                             <br/>
                target  : object    : Instance.                             <br/>
                propNam : string    : Property name.                        <br/>
                val     : object    : Value to set to target's property.    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void SetInstanceProp(object target, string propNam, object val) {
            PropertyInfo inf;

            if ((target == null) || (target is Type))
                Exc("E_INV_INSTANCE", nameof(target));
            inf = InstanceProp(target, propNam, true);
            Chk(inf, target.GetType().Name + "." + propNam, "E_INV_PROP");
            inf.SetValue(target, SetType(val, inf.PropertyType));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetInstanceProp [static]                                    <summary>
          TASK:                                                             <br/>
                Sets a property on target instance to a value.              <para/>
          ARGS:                                                             <br/>
                info    : PropertyInfo  : Property info.                    <br/>
                target  : object        : Instance.                         <br/>
                val     : object        : Value to set to target property.  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void SetInstanceProp(PropertyInfo info, object target, object val) {
            Type tty;

            if ((target == null) || (target is Type))
                Exc("E_INV_INSTANCE", nameof(target));
            Chk(info, nameof(info));
            tty = target.GetType();
            if (info.DeclaringType != tty) {
                Exc("E_PROP_INV_CLASS",
                    info.DeclaringType.Name + "!=" + tty.Name);
            }
            info.SetValue(target, SetType(val, info.PropertyType));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetStaticProp [static]                                      <summary>
          TASK:                                                             <br/>
                Sets a static property on target class to a value.          <para/>
          ARGS:                                                             <br/>
                target  : object    : Class Type or Instance.               <br/>
                propNam : string    : Property name.                        <br/>
                val     : object    : Value to set to target's property.    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static void SetStaticProp(object target, string propNam, object val) {
            PropertyInfo inf;

            inf = StaticProp(target, propNam, true);
            if (inf == null)
                Exc("E_INV_PROP", fetchType(target, nameof(target)).Name + "." + propNam);
            inf.SetValue(null, SetType(val, inf.PropertyType));
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetInstancePropType [static]                                <summary>
          TASK:                                                             <br/>
                Checks type of a value and if it is not compatible to       <br/>
                property type on target object class, tries to convert it.  <para/>
          ARGS:                                                             <br/>
                propNam : string    : Property name in object.              <br/>
                target  : object    : Target object type or object.         <br/>
                val     : object    : Value to convert to property type.    <para/>
          INFO:                                                             <br/>
                This method only returns a value at required type.          <br/>
                It does not set the property in the target object.          </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object SetInstancePropType(string propNam, object target, object val) {
            PropertyInfo inf;

            inf = InstanceProp(target, propNam, true);
            if (inf == null)
                Exc("E_INV_PROP", fetchType(target, nameof(target)).Name + "." + propNam);
            return SetType(val, inf.PropertyType);
        }

        // Used in SetType...
        private static readonly Type NULLABLE = typeof(Nullable<>);

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetType [static]                                            <summary>
          TASK:                                                             <br/>
                Checks type of a value and if required, tries to change it. <para/>
          ARGS:                                                             <br/>
                val : object    : Value to check and if required convert.   <br/>
                typ : Type      : Expected value type.                      <br/>
                ignoreMissing: bool : If value or subvalues is StrLst and 
                                      will be assigned to a sub object,
                                      true    : ignores missing properties
                                      false   : raises exception.           <para/>
          RETV:                                                             <br/>
                    : object    : Value of Type typ if possible.            <para/>
          WARN:                                                             <br/>
                Throws E_TYPE_CONV on failure.                              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static object SetType(object val,
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
            if (n) {                                    // If null,
                val = (typ.IsValueType) ?               // If value
                        MakeObject(typ) :               // make a default,
                        null;                           // else null.
                return val;
            }
            if (typ == v)                               // Fast check.
                return val;
            if (typ.IsAssignableFrom(v))                // Is it?
                return val;
            try {                                       // Try to convert.
                if (val is JToken tok)                  // If coming from json, 
                    return tok.ToObject(typ);           // call json converter.
                if (typ == typeof(Guid)) {              // If guid,
                    if (val is string str)              // string support only.
                        return Guid.Parse(str);
                }
                if (val is StrLst lst)                  // If StrLst.
                    return lst.ToObj(typ, ignoreMissing);
                return Convert.ChangeType(val, typ);    // Otherwise...
            } catch (Exception e) {
                Exc("E_TYPE_CONV",
                    $"{val?.GetType()?.Name} => {typ?.Name}",
                    e);
                throw;
            }
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: SetType [static]                                            <summary>
          TASK:                                                             <br/>
                Checks type of a value and if required, tries to change it. <para/>
          ARGS:                                                             <br/>
                T   : Type (Class)  : Expected value type.                  <br/>
                val : object        : Value to check, if required convert.  <br/>
                ignoreMissing: bool : If value or subvalues is StrLst and      
                                      will be assigned to a sub object,
                                      true    : ignores missing properties
                                      false   : raises exception.           <para/>
          RETV:                                                             <br/>
                    : object        : Value of Type T if possible.          <para/>
          WARN:                                                             <br/>
                Throws E_TYPE_CONV on failure.                              </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static T SetType<T>(object val, bool ignoreMissing = false) {
            return (T)SetType(val, typeof(T), ignoreMissing);
        }

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: DiscoverTypes [static]                                      <summary>
          TASK:                                                             <br/>
                Finds all Types that are subclass of a type and             <br/>
                defined in any assembly referring to the base assembly.     <para/>
          ARGS:                                                             <br/>
            (T)     : Type         : Types should be subclass of this.      <br/>
            baseAsm : AssemblyName : This assembly must be referred.        <br/>
                                     if null assembly name of Type T.       <br/>
                                     :DEF: null                             <para/>
          RETV:                                                             <br/>
                    : List of Type : List of types that are subclass        <br/>
                      of Type T and defined in any assembly referring       <br/>
                      to assembly with name baseAsm.                        </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static List<Type> DiscoverTypes<T>(AssemblyName baseAsm = null) {
            Assembly[] asmArr = AppDomain.CurrentDomain.GetAssemblies();
            AssemblyName[] refs;
            Type[] tArr;
            List<Type> tLst = new List<Type>();
            Type baseType = typeof(T);


            Chk(baseType, "<T>");
            baseAsm ??= baseType.Assembly.GetName();

            foreach (var asm in asmArr) {               // Scan assemblies:
                refs = asm.GetReferencedAssemblies();   // Get referenced.
                if (!refs.Contains(baseAsm))            // If this is not referenced,
                    continue;                           // ignore.
                tArr = asm.GetTypes();                  // Get types in assembly.
                if (tArr == null)                       // If no types in it,
                    continue;                           // ignore.
                foreach (var typ in tArr) {             // Scan types:
                    if (typ.IsSubclassOf(baseType))     // If baseType descendant,
                        tLst.Add(typ);                  // Add to list.
                }
            }
            return tLst;
        }

        #endregion

        #region Application utility functions.
        /*————————————————————————————————————————————————————————————————————————————
            ———————————————————————————————————————
            |   Application utility functions.    |
            ———————————————————————————————————————
        ————————————————————————————————————————————————————————————————————————————*/

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

    #region Attribute base classes.
    /**———————————————————————————————————————————————————————————————————————————
                                                                        <summary>
      CLASS :   NameMetadata [attribute].                               <para/>
      USAGE :   Used as an anchestor class 
                for attributes with a name.                             </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public class NameMetadata: System.Attribute {
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
    public class NameListMetadata: System.Attribute {
        /**——————————————————————————————————————————————————————————————————————————
          CTOR: NameMetaData                                            <summary>
          TASK: Constructs a NameMetaData attribute.                    <para/>
          ARGS: names : string[] : Names to keep in the attribute.      </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public NameListMetadata(params string[] names) {
            nameList = new List<string>();
            if (names != null) {
                foreach (string n in names)
                    nameList.AddUnique(n);
            }
        }
        /**———————————————————————————————————————————————————————————————————————————
          PROP: nameList: List of string;                                   <summary>
          GET : Returns the strings kept in the attribute.                   </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public List<string> nameList { get; }

    }   // End NameListMetadata Attribute class.

    #endregion

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