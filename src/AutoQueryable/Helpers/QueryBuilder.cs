using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoQueryable.Helpers
{
    public static class QueryBuilder
    {

        public static dynamic Build<T>(IQueryable<T> query, IEntityType entityType, IRelationalEntityTypeAnnotations table, IList<Clause> clauses, IList<Criteria> criterias, string[] unselectableProperties) where T : class
        {
            Clause selectClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Select);
            Clause topClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Top);
            Clause skipClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Skip);
            Clause firstClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.First);
            Clause lastClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Last);
            Clause orderByClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.OrderBy);
            Clause orderByDescClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.OrderByDesc);
            Clause includeClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Include);

            List<Column> selectColumns = SelectHelper.GetSelectableColumns(includeClause, selectClause, unselectableProperties, entityType).ToList();
            IEnumerable<Column> orderColumns = OrderByHelper.GetOrderByColumns(orderByClause, unselectableProperties, entityType);
            IEnumerable<Column> orderDescColumns = OrderByHelper.GetOrderByColumns(orderByDescClause, unselectableProperties, entityType);

            if (criterias.Any())
            {
                query = query.Where(criterias);
            }

            if (orderColumns != null)
            {
                query = query.OrderBy(orderColumns);
            }
            else if (orderDescColumns != null)
            {
                query = query.OrderByDesc(orderDescColumns);
            }
            
            var queryProjection = query.Select(SelectHelper.GetSelector<T>(string.Join(",", selectColumns.Select(c => c.PropertyName))));

            if (skipClause != null)
            {
                int skip;
                int.TryParse(skipClause.Value, out skip);
                queryProjection = queryProjection.Skip(skip);
            }
            if (topClause != null)
            {
                int take;
                int.TryParse(topClause.Value, out take);
                queryProjection = queryProjection.Take(take);
            }
            else if (firstClause != null)
            {
                return queryProjection.FirstOrDefault();
            }
            else if (lastClause != null)
            {
                return queryProjection.LastOrDefault();
            }
            return queryProjection;
        }

        private static IQueryable<T> Where<T>(this IQueryable<T> source, IList<Criteria> criterias)
        {
            ParameterExpression entity = Expression.Parameter(typeof(T), "x");
            Expression whereExpression = null;
            foreach (var c in criterias)
            {
                PropertyInfo propertyInfo = typeof(T)
                    .GetProperty(c.Column, BindingFlags.Public | BindingFlags.Instance);

                MemberExpression memberExpression = Expression.Property(entity, propertyInfo);
                Expression orExpression = null;
                foreach (var d in c.Values)
                {
                    var tt = ConvertHelper.Convert(d, propertyInfo.PropertyType);
                    ConstantExpression val = Expression.Constant(tt, propertyInfo.PropertyType);
                    Expression newExpression = c.ConditionType.ToBinaryExpression(memberExpression, val);
                    if (orExpression == null)
                        orExpression = newExpression;
                    else
                        orExpression = Expression.OrElse(orExpression, newExpression);
                }
                if (whereExpression == null)
                    whereExpression = orExpression;
                else
                    whereExpression = Expression.AndAlso(whereExpression, orExpression);
            }

            return source.Where(Expression.Lambda<Func<T, bool>>(whereExpression, entity));
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