using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Providers;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using AutoQueryable.Models.Constants;
using AutoQueryable.Providers;

namespace AutoQueryable.Helpers
{
    public static class QueryBuilder
    {
        public static QueryResult Build<T>(IQueryable<T> query, Type entityType, Clauses clauses, IList<Criteria> criterias, AutoQueryableProfile profile, bool countAllRows) where T : class
        {
            IColumnProvider columnProvider = ProviderFactory.GetColumnProvider(profile?.ProviderType);
            IEnumerable<SelectColumn> selectColumns = columnProvider.GetSelectableColumns(clauses, profile, entityType);
            
            IEnumerable<Column> orderColumns = OrderByHelper.GetOrderByColumns(profile, clauses.OrderBy, entityType);
            IEnumerable<Column> orderDescColumns = OrderByHelper.GetOrderByColumns(profile, clauses.OrderByDesc, entityType);

            if (criterias != null && criterias.Any())
            {
                query = query.Where(criterias);
            }
            var totalCount = 0;
            if (countAllRows)
            {
                totalCount = query.Count();
            }
            if (orderColumns != null)
            {
                query = query.OrderBy(orderColumns);
            }
            else if (orderDescColumns != null)
            {
                query = query.OrderByDesc(orderDescColumns);
            }

            IQueryable<object> queryProjection;
            if (clauses.Select == null && profile?.UnselectableProperties == null && profile?.SelectableProperties == null)
            {
                queryProjection = query;
            }
            else
            {
                if (profile.UseBaseType)
                {
                    queryProjection = query.Select(SelectHelper.GetSelector<T, T>(selectColumns, profile));
                }
                else
                {
                    queryProjection = query.Select(SelectHelper.GetSelector<T, object>(selectColumns, profile));
                }
            }

            if (clauses.Skip != null)
            {
                int.TryParse(clauses.Skip.Value, out int skip);
                if (profile?.MaxToSkip != null && skip > profile.MaxToSkip)
                {
                    skip = profile.MaxToSkip.Value;
                }
                queryProjection = queryProjection.Skip(skip);
            }
            if (clauses.Top != null)
            {
                int.TryParse(clauses.Top.Value, out int take);
                if (profile?.MaxToTake != null && take > profile?.MaxToTake)
                {
                    take = profile.MaxToTake.Value;
                }
                queryProjection = queryProjection.Take(take);
            }
            else if (clauses.First != null)
            {
                return new QueryResult { Result = queryProjection.FirstOrDefault(), TotalCount = totalCount };
            }
            else if (clauses.Last != null)
            {
                return new QueryResult { Result = queryProjection.LastOrDefault(), TotalCount = totalCount };
            }
            else if (profile?.MaxToTake != null)
            {
                queryProjection = queryProjection.Take(profile.MaxToTake.Value);
            }
            return new QueryResult { Result = queryProjection, TotalCount = totalCount };
        }

        private static Expression MakeLambda(Expression parameter, Expression predicate)
        {
            var resultParameterVisitor = new ParameterVisitor();
            resultParameterVisitor.Visit(parameter);
            var resultParameter = resultParameterVisitor.Parameter;
            return Expression.Lambda(predicate, (ParameterExpression)resultParameter);
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            public Expression Parameter
            {
                get;
                private set;
            }
            protected override Expression VisitParameter(ParameterExpression node)
            {
                Parameter = node;
                return node;
            }
        }

        private static Expression BuildWhereSubQueryExpression(Expression parameter, Expression childParameter, Type childType, Expression predicate)
        {
            var anyMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
            anyMethod = anyMethod.MakeGenericMethod(childType);
            var lambdaPredicate = MakeLambda(childParameter, predicate);
            return Expression.Call(anyMethod, parameter, lambdaPredicate);
        }

        private static Expression BuildWhereExpression(Expression parameter, ConditionType conditionType, dynamic[] values, params string[] properties)
        {
            Type childType = null;

            if (properties.Count() > 1)
            {
                //build path
                parameter = Expression.Property(parameter, properties[0]);
                var isCollection = parameter.Type.GetInterfaces().Any(x => x.Name == "IEnumerable");
                //if it´s a collection we later need to use the predicate in the methodexpressioncall
                Expression childParameter;
                if (isCollection)
                {
                    childType = parameter.Type.GetGenericArguments()[0];
                    childParameter = Expression.Parameter(childType, childType.Name);
                }
                else
                {
                    childParameter = parameter;
                }
                //skip current property and get navigation property expression recursivly
                var innerProperties = properties.Skip(1).ToArray();
                Expression predicate = BuildWhereExpression(childParameter, conditionType, values, innerProperties);
                if (isCollection)
                {
                    //build subquery
                    predicate = BuildWhereSubQueryExpression(parameter, childParameter, childType, predicate);
                }

                return predicate;
            }
            //build final predicate
            var childProperty = parameter.Type.GetProperty(properties[0]);
            MemberExpression memberExpression = Expression.Property(parameter, childProperty);
            Expression orExpression = null;
            foreach (var value in values)
            {
                var convertedValue = ConvertHelper.Convert(value, childProperty.PropertyType);
                ConstantExpression val = Expression.Constant(convertedValue, childProperty.PropertyType);
                Expression newExpression = conditionType.ToBinaryExpression(memberExpression, val);

                if (orExpression == null)
                    orExpression = newExpression;
                else
                    orExpression = Expression.OrElse(orExpression, newExpression);
            }
            return orExpression;
        }

        private static IQueryable<T> Where<T>(this IQueryable<T> source, IList<Criteria> criterias)
        {
            var parentEntity = Expression.Parameter(typeof(T), "x");
            Expression whereExpression = null;
            foreach (var c in criterias)
            {

                Expression expression = BuildWhereExpression(parentEntity, c.ConditionType, c.Values, c.ColumnPath.ToArray());
                if (whereExpression == null)
                    whereExpression = expression;
                else
                    whereExpression = Expression.AndAlso(whereExpression, expression); 
            }

            return source.Where(Expression.Lambda<Func<T, bool>>(whereExpression, parentEntity));
        }

        private static IQueryable<T> OrderBy<T>(this IQueryable<T> source, IEnumerable<Column> columns)
        {
            foreach (Column column in columns)
            {
                source = source.Call(QueryableMethods.OrderBy, column.PropertyName);
            }
            return source;
        }

        private static IQueryable<T> OrderByDesc<T>(this IQueryable<T> source, IEnumerable<Column> columns)
        {
            foreach (Column column in columns)
            {
                source = source.Call(QueryableMethods.OrderByDescending, column.PropertyName);
            }
            return source;
        }
    }
}