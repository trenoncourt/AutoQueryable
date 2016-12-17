using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using System.Linq;

namespace AutoQueryable.Helpers
{
    public static class SelectHelper
    {
        public static Expression<Func<TEntity, object>> GetSelector<TEntity>(string columns)
        {
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AutoQueryableDynamicAssembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("AutoQueryableDynamicAssemblyModule");
            TypeBuilder dynamicTypeBuilder = dynamicModule.DefineType("AutoQueryableDynamicType", TypeAttributes.Public);
            Dictionary<string, MemberExpression> memberExpressions = new Dictionary<string, MemberExpression>();

            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "p");
            foreach (string column in columns.Split(','))
            {
                MemberExpression ex = GetMemberExpression<TEntity>(column, parameter);
                if (ex == null)
                {
                    continue;
                }
                string propName = column.Replace(".", "");
                memberExpressions.Add(propName, ex);
                dynamicTypeBuilder.AddProperty(propName, ex.Member as PropertyInfo);
            }

            Type dynamicType = dynamicTypeBuilder.CreateTypeInfo().AsType();

            var ctor = Expression.New(dynamicType);

            var memberAssignments = dynamicType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p =>
                {
                    return Expression.Bind(p, memberExpressions.Single(me => me.Key == p.Name).Value);
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

        public static IEnumerable<string> GetSelectableColumns(Clause selectClause, string[] unselectableProperties, Type entityType)
        {
            IEnumerable<string> columns = selectClause.Value.Split(',');
            if (unselectableProperties != null)
            {
                columns = columns.Where(c => !unselectableProperties.Contains(c, StringComparer.OrdinalIgnoreCase));
            }
            return columns;
        }
    }
}