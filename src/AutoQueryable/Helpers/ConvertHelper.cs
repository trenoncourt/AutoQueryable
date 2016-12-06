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
            if (typeof(T).GetTypeInfo().IsEnum)
                return (T)Enum.Parse(typeof(T), value);

            return (T)System.Convert.ChangeType(value, typeof(T));
        }

        public static dynamic Convert(string value, Type type)
        {
            if (type.GetTypeInfo().IsEnum)
                return Enum.Parse(type, value);

            return System.Convert.ChangeType(value, type);
        }
    }
}
