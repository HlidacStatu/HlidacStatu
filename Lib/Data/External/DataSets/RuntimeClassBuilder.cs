using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

//based on https://www.c-sharpcorner.com/uploadfile/87b416/dynamically-create-a-class-at-runtime/

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public class RuntimeClassBuilder
    {
        AssemblyName asemblyName;
        Dictionary<string, Type> properties = null;
        public RuntimeClassBuilder(Dictionary<string, Type> properties)
            : this("class_" + Guid.NewGuid().ToString("N"), properties)
        {
        }

        public object GetPropertyValue(object instance, string propName)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException("propName");

            if (instance.GetType() != this.CreateType())
                throw new InvalidCastException($"Instance type {instance.GetType().FullName} different from {this.CreateType().FullName}");
            return this.CreateType().GetProperty(propName)?.GetValue(instance);

        }

        public void SetPropertyValue(object instance, string propName, object value)
        {
            if (instance == null)
                throw new ArgumentNullException("instance");
            if (string.IsNullOrEmpty(propName))
                throw new ArgumentNullException("propName");

            if (instance.GetType() != this.CreateType())
                throw new InvalidCastException($"Instance type {instance.GetType().FullName} different from {this.CreateType().FullName}");
            var propType = this.CreateType().GetProperty(propName).PropertyType;

            this.CreateType().GetProperty(propName)?.SetValue(instance, HlidacStatu.Util.ParseTools.ChangeType(value, propType));

        }

        public RuntimeClassBuilder(string className, Dictionary<string, Type> properties)
        {
            if (string.IsNullOrEmpty(className))
                throw new ArgumentNullException("classNames");
            if (properties == null)
                throw new ArgumentNullException("properties");
            if (properties.Count == 0)
                throw new ArgumentException("No properties", "properties");

            this.asemblyName = new AssemblyName(className);
            this.properties = properties;
        }
        public object CreateObject() // string[] PropertyNames,Type[]Types)
        {
            return Activator.CreateInstance(CreateType());
        }

        Type _createdType = null;
        public Type CreateType() // string[] PropertyNames,Type[]Types)
        {
            if (_createdType == null)
            {

                TypeBuilder DynamicClass = this.CreateClass();
                this.CreateConstructor(DynamicClass);

                foreach (var kv in properties)
                {
                    CreateProperty(DynamicClass, kv.Key, kv.Value);
                }

                _createdType = DynamicClass.CreateType();
            }
            return _createdType;
        }
        private TypeBuilder CreateClass()
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(this.asemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(this.asemblyName.FullName
                                , TypeAttributes.Public |
                                TypeAttributes.Class |
                                TypeAttributes.AutoClass |
                                TypeAttributes.AnsiClass |
                                TypeAttributes.BeforeFieldInit |
                                TypeAttributes.AutoLayout
                                , null);
            return typeBuilder;
        }
        private void CreateConstructor(TypeBuilder typeBuilder)
        {
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);
        }
        private void CreateProperty(TypeBuilder typeBuilder, string propertyName, Type propertyType)
        {
            FieldBuilder fieldBuilder = typeBuilder.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();

            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator setIl = setPropMthdBldr.GetILGenerator();
            Label modifyProperty = setIl.DefineLabel();
            Label exitSet = setIl.DefineLabel();

            setIl.MarkLabel(modifyProperty);
            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, fieldBuilder);

            setIl.Emit(OpCodes.Nop);
            setIl.MarkLabel(exitSet);
            setIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }
    }
}
