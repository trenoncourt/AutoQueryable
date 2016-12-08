using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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
            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.GetTypeInfo().IsEnum)
                return Enum.Parse(type, value);

            return System.Convert.ChangeType(value, type);
        }
    }
}
