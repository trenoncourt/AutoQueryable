using System;
using System.Globalization;
using System.Reflection;

namespace AutoQueryable.Helpers
{
    public static class ConvertHelper
    {
        public static T Convert<T>(string value)
        {
            var type = typeof(T);
            return Convert(value, type);
        }

        public static dynamic Convert(string value, Type type, IFormatProvider formatProvider = null)
        {
            if (value.Equals("null"))
            {
                return null;
            }

            type = Nullable.GetUnderlyingType(type) ?? type;

            if (type.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(type, value, ignoreCase:true);
            }

            if (Equals(type.GetTypeInfo(), typeof(Guid).GetTypeInfo()))
            {
                return Guid.Parse(value);
            }

            if (Equals(type.GetTypeInfo(), typeof(DateTime).GetTypeInfo()))
            {
                if (formatProvider != null)
                {
                    return DateTime.ParseExact(value.Replace(" ", "+"), formatProvider.GetFormat(typeof(DateTime)).ToString(), formatProvider);
                }
                return DateTime.Parse(value.Replace(" ", "+"), new DateTimeFormatInfo(), DateTimeStyles.RoundtripKind);
            }

            return System.Convert.ChangeType(value, type);
        }
    }
}
