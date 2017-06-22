using System;
using System.Reflection;

namespace AutoQueryable.Helpers
{
    public static class ConvertHelper
    {
        public static T Convert<T>(string value)
        {
            Type type = typeof(T);
            return Convert(value, type);
        }

        public static dynamic Convert(string value, Type type)
        {
            if (value.Equals("null"))
            {
                return null;
            }

            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.GetTypeInfo().IsEnum)
                return Enum.Parse(type, value, ignoreCase:true);

            if (Equals(type.GetTypeInfo(), typeof(Guid).GetTypeInfo()))
                return Guid.Parse(value);

            return System.Convert.ChangeType(value, type);
        }
    }
}
