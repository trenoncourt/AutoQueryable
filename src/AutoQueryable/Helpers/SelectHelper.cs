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
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AutoQueryableDynamicAssembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("AutoQueryableDynamicAssemblyModule");
            TypeBuilder dynamicTypeBuilder = dynamicModule.DefineType("AutoQueryableDynamicType", TypeAttributes.Public);
            ICollection<MemberExpression> memberExpressions = new List<MemberExpression>();

            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "p");
            foreach (string column in columns.Split(','))
            {
                MemberExpression ex = GetMemberExpression<TEntity>(column, parameter);
                if (ex == null)
                {
                    continue;
                }
                memberExpressions.Add(ex);
                dynamicTypeBuilder.AddProperty(ex.Member as PropertyInfo);
            }

            Type dynamicType = dynamicTypeBuilder.CreateTypeInfo().AsType();

            var ctor = Expression.New(dynamicType);

            var memberAssignments = dynamicType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p =>
                {
                    var t = Expression.Bind(p, memberExpressions.Single(me => me.Member.Name == p.Name));

                    return Expression.Bind(p, memberExpressions.Single(me => me.Member.Name == p.Name));
                });

            var memberInit = Expression.MemberInit(ctor, memberAssignments);
            return Expression.Lambda<Func<TEntity, object>>(memberInit, parameter);

        }

        private static MemberExpression GetMemberExpression<TEntity>(string column, ParameterExpression parameter)
        {
            string[] properties = column.Split('.');
            Type type = typeof(TEntity);
            MemberExpression memberExpression = null;
            foreach (string property in properties)
            {
                PropertyInfo propertyInfo = type.GetProperty(property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    return null;
                }
                if (memberExpression == null)
                {
                    memberExpression = Expression.Property(parameter, propertyInfo);
                }
                else
                {
                    memberExpression = Expression.Property(memberExpression, propertyInfo);
                }
                type = propertyInfo.PropertyType;
            }
            return memberExpression;
        }

        public static IEnumerable<Column> GetSelectableColumns(Clause includeClause, Clause selectClause, string[] unselectableProperties, IEntityType entityType)
        {
            //IEnumerable<IProperty> properties = entityType.GetProperties();
            IEnumerable<Column> columns = selectClause.Value.Split(',').Select(c => new Column
            {
                PropertyName = c
            });
            IEnumerable<INavigation> navigations = new List<INavigation>();
            if (unselectableProperties != null)
            {
                columns = columns.Where(c => !unselectableProperties.Contains(c.PropertyName, StringComparer.OrdinalIgnoreCase));
            }
            //if (selectClause != null)
            //{
            //    string[] columns = selectClause.Value.Split(',');
            //    properties = properties.Where(p => columns.Contains(p.Name, StringComparer.OrdinalIgnoreCase));
            //}
            if (includeClause != null)
            {
                string[] includeColumns = includeClause.Value.Split(',');
                navigations = entityType.GetNavigations().Where(n => includeColumns.Contains(n.Name, StringComparer.OrdinalIgnoreCase));
            }

            return columns.Select(p => p.PropertyName).Concat(navigations.Select(n => n.Name)).Select(v => new Column
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