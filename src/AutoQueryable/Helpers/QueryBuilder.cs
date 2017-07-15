using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using AutoQueryable.Models.Enums;

namespace AutoQueryable.Helpers
{
    public static class QueryBuilder
    {
        public static QueryResult Build<T>(IQueryable<T> query, Type entityType, IList<Clause> clauses, IList<Criteria> criterias, AutoQueryableProfile profile, bool countAllRows) where T : class
        {
            Clause selectClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Select);
            Clause topClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Top);
            Clause skipClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Skip);
            Clause firstClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.First);
            Clause lastClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Last);
            Clause orderByClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.OrderBy);
            Clause orderByDescClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.OrderByDesc);

            //List<string> selectColumns = SelectHelper.GetSelectableColumns(selectClause, unselectableProperties, entityType).ToList();
            IEnumerable<SelectColumn> selectColumns = SelectHelper.GetSelectableColumns(selectClause, profile?.UnselectableProperties, entityType);
            IEnumerable<Column> orderColumns = OrderByHelper.GetOrderByColumns(orderByClause, profile?.UnselectableProperties, entityType);
            IEnumerable<Column> orderDescColumns = OrderByHelper.GetOrderByColumns(orderByDescClause, profile?.UnselectableProperties, entityType);

            if (criterias.Any())
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
            if (selectClause == null && profile?.UnselectableProperties == null)
            {
                queryProjection = query;
            }
            else
            {
                queryProjection = query.Select(SelectHelper.GetSelector<T>(selectColumns));
            }

            if (skipClause != null)
            {
                int.TryParse(skipClause.Value, out int skip);
                queryProjection = queryProjection.Skip(skip);
            }
            if (topClause != null)
            {
                int.TryParse(topClause.Value, out int take);
                queryProjection = queryProjection.Take(take);
            }
            else if (firstClause != null)
            {
                return new QueryResult { Result = queryProjection.FirstOrDefault(), TotalCount = totalCount };
            }
            else if (lastClause != null)
            {
                return new QueryResult { Result = queryProjection.LastOrDefault(), TotalCount = totalCount };
            }
            return new QueryResult { Result = queryProjection, TotalCount = totalCount };
        }

        private static bool IsClauseAllowed(this AutoQueryableProfile profile, ClauseType clauseType)
        {
            bool isClauseAllowed = true;
            bool? isAllowed = profile?.AllowedClauses?.HasFlag(ClauseType.Skip);
            bool? isDisallowed = profile?.DisAllowedClauses?.HasFlag(ClauseType.Skip);

            if (isAllowed.HasValue && !isAllowed.Value)
            {
                isClauseAllowed = false;
            }

            if (isDisallowed.HasValue && isDisallowed.Value)
            {
                isClauseAllowed = false;
            }
            return isClauseAllowed;
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
            foreach (var d in values)
            {
                var tt = ConvertHelper.Convert(d, childProperty.PropertyType);
                ConstantExpression val = Expression.Constant(tt, childProperty.PropertyType);
                Expression newExpression = conditionType.ToBinaryExpression(memberExpression, val); //MakeLambda(parameter, conditionType.ToBinaryExpression(memberExpression, val));

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
                source = source.OrderBy(column.PropertyName);
            }
            return source;
        }

        private static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string sortProperty)
        {
            var type = typeof(T);
            var property = type.GetProperty(sortProperty);
            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            Expression lambda = Expression.Lambda(propertyAccess, parameter);
            var resultExp = Expression.Call(typeof(Queryable), "OrderBy", new[] { typeof(T), property.PropertyType }, source.Expression, lambda);

            return source.Provider.CreateQuery<T>(resultExp);
        }

        private static IQueryable<T> OrderByDesc<T>(this IQueryable<T> source, IEnumerable<Column> columns)
        {
            foreach (Column column in columns)
            {
                source = source.OrderByDesc(column.PropertyName);
            }
            return source;
        }

        private static IQueryable<T> OrderByDesc<T>(this IQueryable<T> source, string sortProperty)
        {
            var type = typeof(T);
            var property = type.GetProperty(sortProperty);
            var parameter = Expression.Parameter(type, "x");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            Expression lambda = Expression.Lambda(propertyAccess, parameter);
            var resultExp = Expression.Call(typeof(Queryable), "OrderByDescending", new[] { typeof(T), property.PropertyType }, source.Expression, lambda);

            return source.Provider.CreateQuery<T>(resultExp);
        }
    }
}