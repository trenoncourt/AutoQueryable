using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace AutoQueryable.Helpers
{
    public static class SelectHelper
    {
        public static Expression<Func<TEntity, object>> GetSelector<TEntity>(string columns)
        {
            Type entityType = typeof(TEntity);
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AutoQueryableDynamicAssembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("AutoQueryableDynamicAssemblyModule");
            TypeBuilder dynamicTypeBuilder = dynamicModule.DefineType("AutoQueryableDynamicType", TypeAttributes.Public);
            foreach (string column in columns.Split(','))
            {
                PropertyInfo property = entityType.GetProperty(column, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null)
                {
                    continue;
                }
                dynamicTypeBuilder.AddProperty(property);
            }

            Type dynamicType = dynamicTypeBuilder.CreateTypeInfo().AsType();

            var ctor = Expression.New(dynamicType);

            ParameterExpression parameter = Expression.Parameter(entityType, "p");

            var memberAssignments = dynamicType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p =>
            {
                PropertyInfo propertyInfo = entityType.GetProperty(p.Name, BindingFlags.Public | BindingFlags.Instance);
                MemberExpression memberExpression = Expression.Property(parameter, propertyInfo);
                return Expression.Bind(p, memberExpression);
            });

            var memberInit = Expression.MemberInit(ctor, memberAssignments);
            return Expression.Lambda<Func<TEntity, object>>(memberInit, parameter);

        }

        public static IEnumerable<Column> GetSelectableColumns(Clause includeClause, Clause selectClause, string[] unselectableProperties, IEntityType entityType)
        {
            IEnumerable<IProperty> properties = entityType.GetProperties();
            IEnumerable<INavigation> navigations = new List<INavigation>();
            if (unselectableProperties != null)
            {
                properties = properties.Where(c => !unselectableProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase));
            }
            if (selectClause != null)
            {
                string[] columns = selectClause.Value.Split(',');
                properties = properties.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase));
            }
            if (includeClause != null)
            {
                string[] columns = includeClause.Value.Split(',');
                navigations = entityType.GetNavigations().Where(n => columns.Contains(n.Name, StringComparer.OrdinalIgnoreCase));
            }

            return properties.Select(p => p.Name).Concat(navigations.Select(n => n.Name)).Select(v => new Column
            {
                PropertyName = v
            });
        }

        public static IEnumerable<Column> GetSelectableColumns(Clause selectClause, string[] unselectableProperties, Type entityType)
        {
            IEnumerable<PropertyInfo> properties = entityType.GetProperties();
            if (unselectableProperties != null)
            {
                properties = properties.Where(c => !unselectableProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase));
            }
            if (selectClause != null)
            {
                string[] columns = selectClause.Value.Split(',');
                properties = properties.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase));
            }

            return properties.Select(p => p.Name).Select(v => new Column
            {
                PropertyName = v
            });
        }
    }
}