using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Helpers
{
    public class EntityColumnHelper
    {
        public static IEnumerable<SelectColumn> GetSelectableColumns(AutoQueryableProfile profile, Type entityType, SelectInclusingType selectInclusingType = SelectInclusingType.IncludeBaseProperties)
        {
            IEnumerable<SelectColumn> columns = null;
            bool isCollection = entityType.IsEnumerable();
            var type = entityType;
            if (isCollection)
            {
                type = entityType.GetGenericArguments().FirstOrDefault();
            } 
            // Get all properties without navigation properties.
            if (selectInclusingType == SelectInclusingType.IncludeBaseProperties)
            {
                columns = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p =>
                    (p.PropertyType.GetTypeInfo().IsGenericType && p.PropertyType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
                    || (!p.PropertyType.GetTypeInfo().IsClass && !p.PropertyType.GetTypeInfo().IsGenericType)
                    || p.PropertyType.GetTypeInfo().IsArray
                    || p.PropertyType == typeof(string)
                    )
                    .Select(p => new SelectColumn
                    {
                        Key = p.Name,
                        Name = p.Name,
                        Type = p.PropertyType
                    });
            }
            // Get all properties.
            else if (selectInclusingType == SelectInclusingType.IncludeAllProperties)
            {
                columns = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Select(p => new SelectColumn
                    {
                        Key = p.Name,
                        Name = p.Name,
                        Type = p.PropertyType
                    });
            }

            // Remove non selectable properties.
            if (profile?.SelectableProperties != null)
            {
                columns = columns?.Where(c => profile.SelectableProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase));
            }
            
            // Remove unselectable properties.
            if (profile?.UnselectableProperties != null)
            {
                columns = columns?.Where(c => !profile.UnselectableProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase));
            }
            
            return columns?.ToList();
        }
    }
}