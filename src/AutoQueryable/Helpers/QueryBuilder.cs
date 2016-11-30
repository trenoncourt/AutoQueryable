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

            List<Column> columns = SelectHelper.GetSelectableColumns(selectClause, unselectableProperties, entityType).ToList();

            if (criterias.Any())
            {
                query = query.Where(CreateWherePredicate<T>(criterias));
            }

            var queryProjection = query.Select(SelectHelper.GetSelector<T>(string.Join(",", columns.Select(c => c.PropertyName))));

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

            if (firstClause != null)
            {
                return queryProjection.FirstOrDefault();
            }
            if (lastClause != null)
            {
                return queryProjection.LastOrDefault();
            }
            return queryProjection;
        }

        private static Expression<Func<T, bool>> CreateWherePredicate<T>(IList<Criteria> criterias)
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
                    var tt = Convert.ChangeType(d, propertyInfo.PropertyType);
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

            return Expression.Lambda<Func<T, bool>>(whereExpression, entity);
        }
    }
}