using System;
using System.Collections;
using System.Reflection;

namespace AutoQueryable.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Check if a type is not a string implements IEnumerable.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <returns>True if the type is not a string and implements IEnumerable, False otherwise</returns>
        public static bool IsEnumerableButNotString(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type) && type != typeof(string);
        }

        /// <summary>
        /// Check if a type own a property by a propertyname.
        /// </summary>
        /// <param name="type">Type to check</param>
        /// <param name="propertyName">Name of the property (case is ignored)</param>
        /// <returns>True if the type own the property, False otherwise</returns>
        public static bool PropertyExist(this Type type, string propertyName)
        {
            PropertyInfo propertyInfo = type.GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            return propertyInfo != null;
        }
    }
}