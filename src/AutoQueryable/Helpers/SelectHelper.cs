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
    internal static class RuntimeTypeBuilder
    {
        private static readonly ModuleBuilder moduleBuilder;
        private static readonly IDictionary<string, Type> builtTypes;

        static RuntimeTypeBuilder()
        {
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AutoQueryableDynamicAssembly"), AssemblyBuilderAccess.Run);
            moduleBuilder = dynamicAssembly.DefineDynamicModule("AutoQueryableDynamicAssemblyModule");

            builtTypes = new Dictionary<string, Type>();
        }

        internal static Type GetRuntimeType<TEntity>(IDictionary<string, object> fields)
        {
            var typeKey = GetTypeKey<TEntity>(fields);
            if (!builtTypes.ContainsKey(typeKey))
            {
                lock (moduleBuilder)
                {
                    builtTypes[typeKey] = GetRuntimeType(typeKey, fields);
                }
            }

            return builtTypes[typeKey];
        }
        private static Type GetRuntimeType(string typeName, IEnumerable<KeyValuePair<string, object>> properties)
        {
            var typeBuilder = moduleBuilder.DefineType(typeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Serializable);
            foreach (var property in properties)
            {
                if (property.Value is PropertyInfo)
                {
                    typeBuilder.AddProperty(property.Key, property.Value as PropertyInfo);
                }
                else
                {
                    typeBuilder.AddProperty(property.Key, property.Value as Type);

                }
            }

            return typeBuilder.CreateTypeInfo().AsType();
        }

        private static string GetTypeKey<TEntity>(IEnumerable<KeyValuePair<string, object>> fields)
        {

            var fieldsKey = fields.Aggregate(string.Empty, (current, field) =>
            {
                if (field.Value is PropertyInfo)
                {
                    current = current + (field.Key + "#" + (field.Value as PropertyInfo).Name + "#");
                }
                else
                {
                    current = current + (field.Key + "#" + (field.Value as Type).FullName + "#");
                }
                return current;
            });
            return typeof(TEntity).FullName + fieldsKey;
        }
    }
    public static class SelectHelper
    {

        public static Expression<Func<TEntity, object>> GetSelector<TEntity>(string columns)
        {
            Dictionary<string, Expression> memberExpressions = new Dictionary<string, Expression>();

            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "p");
            var properties = new Dictionary<string, object>();
            foreach (string column in columns.Split(','))
            {
                var cols = column.Split('.');
                var ex = GetMemberExpression(parameter, cols, 0);
                if (ex == null)
                {
                    continue;
                }
                string propName = column.Replace(".", "");
                memberExpressions.Add(propName, ex);
                if (ex is MemberExpression)
                {
                    properties.Add(propName, (ex as MemberExpression).Member as PropertyInfo);
                }
                if (ex is MethodCallExpression)
                {
                    var callExpression = (ex as MethodCallExpression);
                    properties.Add(propName, ex.Type);
                }
            }

            var dynamicType = RuntimeTypeBuilder.GetRuntimeType<TEntity>(properties);

            var ctor = Expression.New(dynamicType);

            var memberAssignments = dynamicType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p =>
                {
                    return Expression.Bind(p, memberExpressions.Single(me => me.Key == p.Name).Value);
                });

            var memberInit = Expression.MemberInit(ctor, memberAssignments);
            return Expression.Lambda<Func<TEntity, object>>(memberInit, parameter);

        }
        private static Expression GetMemberExpression(Expression parent, string[] properties, int index)
        {
            if (index < properties.Length)
            {
                var member = properties[index];

                // check if it's IEnumerable like 
                var isCollection = parent.Type.GetInterfaces().Any(x => x.Name == "IEnumerable");
                if (isCollection && parent.Type != typeof(string))
                {
                    // input eg: Product.SalesOrderDetail (type IList<SalesOrderDetail>), output: type SalesOrderDetail
                    var enumerableType = parent.Type.GetGenericArguments().SingleOrDefault();

                    // declare parameter for the lambda expression of SalesOrderDetail.Select(x => x.LineTotal)
                    var param = Expression.Parameter(enumerableType, "x");

                    // Recurse to build the inside of the lambda, so x => x.LineTotal. 
                    var lambdaBody = GetMemberExpression(param, properties, index);

                    // Lambda is of type Func<Order, int> in the case of x => x.LineTotal
                    var funcType = typeof(Func<,>).MakeGenericType(enumerableType, lambdaBody.Type); 

                    var lambda = Expression.Lambda(funcType, lambdaBody, param);

            
                    var selectMethod = (from m in typeof(Enumerable).GetMethods()
                                        where m.Name == "Select"
                                        && m.IsGenericMethod
                                        let parameters = m.GetParameters()
                                        where parameters.Length == 2
                                        && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
                                        select m).Single().MakeGenericMethod(enumerableType, lambdaBody.Type);

                    // Do SalesOrderDetail.Select(x => x.LineTotal)
                    var invokeSelect = Expression.Call(null, selectMethod, parent, lambda);

                    return invokeSelect;

                }
                else
                {
                    // Simply access a property like ProductId
                    var propertyInfo = parent.Type.GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        return null;
                    }
                    var newParent = Expression.PropertyOrField(parent, member);

                    // Recurse
                    return GetMemberExpression(newParent, properties, ++index);

                }

            }
            else
            {
                // Return the final expression once we're done recursing.
                return parent;
            }

        }

        public static IEnumerable<string> GetSelectableColumns(Clause selectClause, string[] unselectableProperties, Type entityType)
        {
            if (selectClause == null)
            {
                return GetSelectableColumns(unselectableProperties, entityType);
            }
            IEnumerable<string> columns = selectClause.Value.Split(',');

            if (unselectableProperties != null)
            {
                columns = columns.Where(c => !unselectableProperties.Contains(c, StringComparer.OrdinalIgnoreCase));
            }
            return columns;
        }
        public static IEnumerable<string> GetSelectableColumns(string[] unselectableProperties, Type entityType)
        {
            IEnumerable<string> columns = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p =>
                (p.PropertyType.GetTypeInfo().IsGenericType && p.PropertyType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
                || (!p.PropertyType.GetTypeInfo().IsClass && !p.PropertyType.GetTypeInfo().IsGenericType)
                || p.PropertyType.GetTypeInfo().IsArray
                || p.PropertyType == typeof(string)
                )
                .Select(p => p.Name);
            if (unselectableProperties != null)
            {
                columns = columns.Where(c => !unselectableProperties.Contains(c, StringComparer.OrdinalIgnoreCase));
            }
            return columns.ToList();
        }
    }
}