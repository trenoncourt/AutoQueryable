using System;
using System.Reflection;
using System.Reflection.Emit;

namespace AutoQueryable.Extensions
{
    public static class TypeBuilderExtension
    {
        public static void AddProperty(this TypeBuilder typeBuilder, PropertyInfo propertyInfo)
        {
            const MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName;

            FieldBuilder field = typeBuilder.DefineField("_" + propertyInfo.Name, propertyInfo.PropertyType, FieldAttributes.Private);
            
            PropertyBuilder property = typeBuilder.DefineProperty(propertyInfo.Name,propertyInfo.Attributes, propertyInfo.PropertyType, new Type[] {});

            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod($"get_{propertyInfo.Name}", getSetAttr, propertyInfo.PropertyType, Type.EmptyTypes);
     
            ILGenerator getIl = getMethodBuilder.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, field);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod($"set_{propertyInfo.Name}", getSetAttr, null, new[] { propertyInfo.PropertyType });
            ILGenerator setIl = setMethodBuilder.GetILGenerator();

            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, field);
            setIl.Emit(OpCodes.Ret);

            property.SetGetMethod(getMethodBuilder);
            property.SetSetMethod(setMethodBuilder);
        }
    }
}