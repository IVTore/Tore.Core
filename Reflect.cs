/*————————————————————————————————————————————————————————————————————————————
    ———————————————————————————————————————————————
    |  Reflect.cs: Reflection utility functions.  |
    ———————————————————————————————————————————————

© Copyright 2020 İhsan Volkan Töre.

Author              : IVT.  (İhsan Volkan Töre)
Version             : 202303191158
License             : MIT.

History             :
202303191158: IVT   : Reflection moved to here from Sys.cs
————————————————————————————————————————————————————————————————————————————*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json.Linq;

using static Tore.Core.Sys;

namespace Tore.Core {

    /**———————————————————————————————————————————————————————————————————————————
        CLASS:  Reflect [Static]                                        <summary>
        USAGE:                                                          <br/>
                Contains a library of static utility methods treated as <br/>
                global functions which is used for managing:            <para/>
                Reflection                                              <br/>
                Type juggling                                           <br/>
                Attributes                                              </summary>
    ————————————————————————————————————————————————————————————————————————————*/
    public static class Reflect {

        #region Reflection utility functions.

        /**———————————————————————————————————————————————————————————————————————————
          FUNC: Attr [static]                                               <summary>
          TASK:                                                             <br/>
                Fetches an attribute of a member.                           <para/>
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
                Fetches an attribute of a member.                           <para/>
          ARGS:                                                             <br/>
                T           : Type(Class)   :   Type of attribute.          <br/>
                memInfo     : MemberInfo    :   Member information.         <para/>
          RETV:             : Attribute     :   The attribute or null if
                                                not found.                  </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static Attribute Attr<T>(MemberInfo memInfo) where T : Attribute {
            Chk(memInfo, nameof(memInfo));
            return memInfo.GetCustomAttribute(typeof(T), true);
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
                ignoreMissing: bool : If value or subvalues is Stl and 
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
                if (val is Stl lst)                     // If Stl.
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
                ignoreMissing: bool : If value or subvalues is Stl and      
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
                Finds all Types that are subclass of a base type and        <br/>
                defined in any assembly referring to the base assembly.     <para/>
          ARGS:                                                             <br/>
                baseAsm : AssemblyName : This assembly must be referred.    <br/>
                baseType: Type         : Types should be subclass of this.  <para/>
          RETV:                                                             <br/>
                        : List of Type : List of types that are subclass    
                          of base type and defined in any assembly referring
                          to assembly with name baseAsm.                    </summary>
        ————————————————————————————————————————————————————————————————————————————*/
        public static List<Type> DiscoverTypes<T>(AssemblyName baseAsm = null) {
            Assembly[] asmArr = AppDomain.CurrentDomain.GetAssemblies();
            AssemblyName[] refs;
            Type[] tArr;
            List<Type> tLst = new List<Type>();
            Type baseType = typeof(T);


            Chk(baseType, "<T>");
            if (baseAsm == null)
                baseAsm = baseType.Assembly.GetName();

            foreach (var asm in asmArr) {                // Scan assemblies:
                refs = asm.GetReferencedAssemblies();   // Get referenced.
                if (!refs.Contains(baseAsm))            // If this is not referenced,
                    continue;                           // ignore.
                tArr = asm.GetTypes();                  // Get types in assembly.
                if (tArr == null)                       // If no types in it,
                    continue;                           // ignore.
                foreach (var typ in tArr) {              // Scan types:
                    if (typ.IsSubclassOf(baseType))     // If baseType descendant,
                        tLst.Add(typ);                  // Add to list.
                }
            }
            return tLst;
        }

        #endregion

    }   // End static class Reflect.

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
}
