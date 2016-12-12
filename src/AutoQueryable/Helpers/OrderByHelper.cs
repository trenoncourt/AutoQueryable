using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoQueryable.Helpers
{
    public static class OrderByHelper
    {
        public static IEnumerable<Column> GetOrderByColumns(Clause orderClause, string[] unselectableProperties, IEntityType entityType)
        {
            IEnumerable<IProperty> properties = entityType.GetProperties();
            if (unselectableProperties != null)
            {
                properties = properties.Where(c => !unselectableProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase));
            }
            if (orderClause == null)
            {
                return null;
            }
            string[] columns = orderClause.Value.Split(',');
            properties = properties.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase));

            return properties.Select(v => new Column
            {
                PropertyName = v.Name
            });
        }

        public static IEnumerable<Column> GetOrderByColumns(Clause orderClause, string[] unselectableProperties, Type entityType)
        {
            IEnumerable<PropertyInfo> properties = entityType.GetProperties();
            if (unselectableProperties != null)
            {
                properties = properties.Where(c => !unselectableProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase));
            }
            if (orderClause == null)
            {
                return null;
            }
            string[] columns = orderClause.Value.Split(',');
            properties = properties.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase));

            return properties.Select(v => new Column
            {
                PropertyName = v.Name
            });
        }
    }
}