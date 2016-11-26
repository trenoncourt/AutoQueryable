using System;
using System.Linq;
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

            //PropertyBuilder property = typeBuilder.DefineProperty(propertyInfo.Name, PropertyAttributes.None, propertyInfo.PropertyType, new[] { propertyInfo.PropertyType });
            PropertyBuilder property = typeBuilder.DefineProperty(propertyInfo.Name,propertyInfo.Attributes, propertyInfo.PropertyType, new Type[] {/* propertyInfo.PropertyType */ });

            MethodBuilder getMethodBuilder = typeBuilder.DefineMethod($"get_{propertyInfo.Name}", getSetAttr, propertyInfo.PropertyType, Type.EmptyTypes);
            //foreach (CustomAttributeData getMethodCustomAttribute in propertyInfo.GetMethod.CustomAttributes)
            //{
            //    getMethodBuilder.SetCustomAttribute(new CustomAttributeBuilder(getMethodCustomAttribute.Constructor, getMethodCustomAttribute.ConstructorArguments.Select(t => t.Value).ToArray()));
            //}
            ILGenerator getIl = getMethodBuilder.GetILGenerator();
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, field);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setMethodBuilder = typeBuilder.DefineMethod($"set_{propertyInfo.Name}", getSetAttr, null, new[] { propertyInfo.PropertyType });
            ILGenerator setIl = setMethodBuilder.GetILGenerator();
            //foreach (CustomAttributeData setMethodCustomAttribute in propertyInfo.SetMethod.CustomAttributes)
            //{
            //    setMethodBuilder.SetCustomAttribute(new CustomAttributeBuilder(setMethodCustomAttribute.Constructor, setMethodCustomAttribute.ConstructorArguments.Select(t => t.Value).ToArray()));
            //}

            setIl.Emit(OpCodes.Ldarg_0);
            setIl.Emit(OpCodes.Ldarg_1);
            setIl.Emit(OpCodes.Stfld, field);
            setIl.Emit(OpCodes.Ret);

            property.SetGetMethod(getMethodBuilder);
            property.SetSetMethod(setMethodBuilder);
        }
    }
}