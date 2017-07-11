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

            builtTypes = new Dictionary<string, Type>(); // Todo : Maybe use ConcurrentDictionary ?
        }

        internal static Type GetRuntimeType<TEntity>(IDictionary<string, object> fields)
        {
            string typeKey = GetTypeKey<TEntity>(fields);
            if (!builtTypes.ContainsKey(typeKey))
            {
                lock (moduleBuilder)
                {
                    builtTypes[typeKey] = GetRuntimeType(typeKey, fields);
                }
            }

            return builtTypes[typeKey];
        }

        internal static Type GetRuntimeType(string typeName, IEnumerable<KeyValuePair<string, object>> properties)
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
        public static Expression<Func<TEntity, object>> GetSelector<TEntity>(IEnumerable<SelectColumn> columns)
        {
            Dictionary<string, Expression> memberExpressions = new Dictionary<string, Expression>();

            ParameterExpression parameter = Expression.Parameter(typeof(TEntity), "p");
            var properties = new Dictionary<string, object>();
            foreach (SelectColumn column in columns)
            {
                var ex = GetMemberExpression<TEntity>(parameter, column);
                if (ex == null)
                {
                    continue;
                }
                memberExpressions.Add(column.Name, ex);
                if (ex is MemberExpression)
                {
                    properties.Add(column.Name, (ex as MemberExpression).Member as PropertyInfo);
                }
                if (ex is MethodCallExpression || ex is MemberInitExpression)
                {
                    properties.Add(column.Name, ex.Type);
                }
            }

            var dynamicType = RuntimeTypeBuilder.GetRuntimeType<TEntity>(properties);

            var ctor = Expression.New(dynamicType);

            IEnumerable<MemberAssignment> memberAssignments = dynamicType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p =>
                {
                    return Expression.Bind(p, memberExpressions.Single(me => me.Key == p.Name).Value);
                }).ToList();
            
            var memberInit = Expression.MemberInit(ctor, memberAssignments);
            return Expression.Lambda<Func<TEntity, object>>(memberInit, parameter);

        }

        private static Expression GetMemberExpression<TEntity>(Expression parent, SelectColumn column, bool isLambdaBody = false)
        {
            string member = column.Name;
            bool isCollection = parent.Type.GetInterfaces().Any(x => x.Name == "IEnumerable");
            if (column.HasSubColumn || isCollection)
            {
                // check if it's IEnumerable like 
                if (isCollection && parent.Type != typeof(string) && !isLambdaBody)
                {
                    // declare parameter for the lambda expression of SalesOrderDetail.Select(x => x.LineTotal)
                    ParameterExpression param = parent.CreateParameterFromGenericType();

                    // Recurse to build the inside of the lambda, so x => x.LineTotal. 
                    var lambdaBody = GetMemberExpression<TEntity>(param, column.ParentColumn, true);
                    return parent.CreateSelect(lambdaBody, param);
                }
                Expression newParent;
                // access to an object with childs
                if (isLambdaBody)
                {
                    newParent = parent;
                }
                else
                {
                    var propertyInfo = parent.Type.GetProperty(member,
                        BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo == null)
                    {
                        return null;
                    }
                    newParent = Expression.PropertyOrField(parent, member);
                }

                var properties = new Dictionary<string, object>();
                var expressions = new Dictionary<string, Expression>();
                foreach (SelectColumn subColumn in column.SubColumns)
                {
                    Expression ex = GetMemberExpression<TEntity>(newParent, subColumn);

                    if (ex is MemberExpression)
                    {
                        expressions.Add(subColumn.Name, ex);
                        properties.Add(subColumn.Name, (ex as MemberExpression).Member as PropertyInfo);
                    }
                    if (ex is MemberInitExpression)
                    {
                        expressions.Add(subColumn.Name, ex);
                        properties.Add(subColumn.Name, ex.Type);
                    }
                    if (ex is MethodCallExpression)
                    {
                        return ex;
                    }
                }
                    
                Type dynamicType = RuntimeTypeBuilder.GetRuntimeType<TEntity>(properties);
                NewExpression ctor = Expression.New(dynamicType);
                MemberInitExpression init = Expression.MemberInit(ctor, expressions.Select(p => Expression.Bind(dynamicType.GetProperty(p.Key), p.Value)));
                return init;
            }
            else
            {
                var propertyInfo = parent.Type.GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyInfo == null)
                {
                    return null;
                }
                var newParent = Expression.PropertyOrField(parent, member);
                // Return the final expression once we're done recursing.
                return newParent;
            }

        }

        //private static Expression GetMemberExpression(Expression parent, SelectColumn column, int index)
        //{
        //    if (index < properties.Length)
        //    {
        //        string member = properties[index];

        //        // check if it's IEnumerable like 
        //        bool isCollection = parent.Type.GetInterfaces().Any(x => x.Name == "IEnumerable");
        //        if (isCollection && parent.Type != typeof(string))
        //        {
        //            // input eg: Product.SalesOrderDetail (type IList<SalesOrderDetail>), output: type SalesOrderDetail
        //            var enumerableType = parent.Type.GetGenericArguments().SingleOrDefault();

        //            // declare parameter for the lambda expression of SalesOrderDetail.Select(x => x.LineTotal)
        //            var param = Expression.Parameter(enumerableType, "x");

        //            // Recurse to build the inside of the lambda, so x => x.LineTotal. 
        //            var lambdaBody = GetMemberExpression(param, properties, index);

        //            // Lambda is of type Func<Order, int> in the case of x => x.LineTotal
        //            var funcType = typeof(Func<,>).MakeGenericType(enumerableType, lambdaBody.Type);

        //            var lambda = Expression.Lambda(funcType, lambdaBody, param);


        //            var selectMethod = (from m in typeof(Enumerable).GetMethods()
        //                where m.Name == "Select"
        //                      && m.IsGenericMethod
        //                let parameters = m.GetParameters()
        //                where parameters.Length == 2
        //                      && parameters[1].ParameterType.GetGenericTypeDefinition() == typeof(Func<,>)
        //                select m).Single().MakeGenericMethod(enumerableType, lambdaBody.Type);

        //            // Do SalesOrderDetail.Select(x => x.LineTotal)
        //            var invokeSelect = Expression.Call(null, selectMethod, parent, lambda);

        //            return invokeSelect;

        //        }
        //        else
        //        {
        //            // Simply access a property like ProductId
        //            var propertyInfo = parent.Type.GetProperty(member, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        //            if (propertyInfo == null)
        //            {
        //                return null;
        //            }
        //            var newParent = Expression.PropertyOrField(parent, member);

        //            // Recurse
        //            return GetMemberExpression(newParent, properties, ++index);

        //        }

        //    }
        //    else
        //    {
        //        // Return the final expression once we're done recursing.
        //        return parent;
        //    }

        //}

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

        public static IEnumerable<SelectColumn> GetSelectableColumns(Clause selectClause, Type entityType)
        {
            if (selectClause == null)
            {
                // TODO unselectable properties.
                //return GetSelectableColumns(unselectableProperties, entityType);
                return new List<SelectColumn>();
            }
            ICollection<SelectColumn> allSelectColumns = new List<SelectColumn>();
            ICollection<SelectColumn> selectColumns = new List<SelectColumn>();
            IEnumerable<string[]> strings = selectClause.Value.Split(',').ToList().Select(d => d.Split('.'));
            foreach (string[] s in strings)
            {
                for (int i = 0; i < s.Length; i++)
                {
                    string key = string.Join(".", s.Take(i + 1)).ToLowerInvariant();
                    SelectColumn column = allSelectColumns.FirstOrDefault(all => all.Key == key);
                    if (column == null)
                    {
                        column = new SelectColumn
                        {
                            Key = key,
                            Name = s[i],
                            SubColumns = new List<SelectColumn>()
                        };
                        allSelectColumns.Add(column);
                        if (i == 0)
                        {
                            selectColumns.Add(column);
                        }
                        else
                        {
                            string parentKey = string.Join(".", s.Take(i)).ToLowerInvariant();
                            SelectColumn parentColumn = allSelectColumns.FirstOrDefault(all => all.Key == parentKey);
                            column.ParentColumn = parentColumn;
                            parentColumn.SubColumns.Add(column);
                        }
                    }
                }
            }

            return selectColumns;
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

        //private static Expression BuildWhereExpression(Expression parameter, params string[] properties)
        //{
        //    Type childType = null;

        //    if (properties.Count() > 1)
        //    {
        //        //build path
        //        parameter = Expression.Property(parameter, properties[0]);
        //        var isCollection = parameter.Type.GetInterfaces().Any(x => x.Name == "IEnumerable");
        //        //if it´s a collection we later need to use the predicate in the methodexpressioncall
        //        Expression childParameter;
        //        if (isCollection)
        //        {
        //            childType = parameter.Type.GetGenericArguments()[0];
        //            childParameter = Expression.Parameter(childType, childType.Name);
        //        }
        //        else
        //        {
        //            childParameter = parameter;
        //        }
        //        //skip current property and get navigation property expression recursivly
        //        var innerProperties = properties.Skip(1).ToArray();
        //        Expression predicate = BuildWhereExpression(childParameter, innerProperties);
        //        if (isCollection)
        //        {
        //            //build subquery
        //            predicate = BuildWhereSubQueryExpression(parameter, childParameter, childType, predicate);
        //        }

        //        return predicate;
        //    }
        //    //build final predicate
        //    var childProperty = parameter.Type.GetProperty(properties[0]);
        //    MemberExpression memberExpression = Expression.Property(parameter, childProperty);
        //    return memberExpression;
        //}
    }
}