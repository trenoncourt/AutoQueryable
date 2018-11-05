using System;
using System.Reflection;
using System.Reflection.Emit;

namespace AutoQueryable.Extensions
{
    public static class TypeBuilderExtension
    {
        private static void AddProperty(this TypeBuilder typeBuilder, string propName, PropertyAttributes attributes, Type propertyType) {
            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

            var field = typeBuilder.DefineField("_" + propName, propertyType, FieldAttributes.Private);

            var property = typeBuilder.DefineProperty(propName, attributes, propertyType, new Type[] { });

            var getMethodBuilder = typeBuilder.DefineMethod($"get_{propName}", getSetAttr, propertyType, Type.EmptyTypes);

            var getIl = getMethodBuilder.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, field);
            getIl.Emit(OpCodes.Ret);

            var setMethodBuilder = typeBuilder.DefineMethod($"set_{propName}", getSetAttr, null, new[] { propertyType });
            var setIl = setMethodBuilder.GetILGenerator();

            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, field);
            setIl.Emit(OpCodes.Ret);

            property.SetGetMethod(getMethodBuilder);
            property.SetSetMethod(setMethodBuilder);
        }

        public static void AddProperty(this TypeBuilder typeBuilder, string propName, PropertyInfo propertyInfo)
        {
            typeBuilder.AddProperty(propName, propertyInfo.Attributes, propertyInfo.PropertyType); 
        }

        public static void AddProperty(this TypeBuilder typeBuilder, string propName, Type propertyType)
        {

            typeBuilder.AddProperty(propName, PropertyAttributes.None, propertyType); 
        }
    }
}