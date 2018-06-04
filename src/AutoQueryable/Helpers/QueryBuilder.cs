using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoMapper.QueryableExtensions;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.Clauses;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using AutoQueryable.Models.Constants;

namespace AutoQueryable.Helpers
{
    public class QueryBuilder
    {
        private readonly IClauseMapManager _clauseMapManager;

        public QueryBuilder(IClauseMapManager clauseMapManager)
        {
            _clauseMapManager = clauseMapManager;
        }
        public IQueryable<TAs> Build<TEntity, TAs>(IQueryable<TEntity> query, ICollection<Clause> clauses, ICollection<Criteria> criterias, IAutoQueryableProfile profile) where TEntity : class where TAs : class
        {
            // TODO: Handle '-' to orderbydesc (in order that they are in the orderby string)
            var orderColumns = OrderByHelper.GetOrderByColumns(profile, _clauseMapManager.GetClause(ClauseType.OrderBy), typeof(TEntity));

            if (criterias != null && criterias.Any())
            {
                query = query.Where(criterias);
            }
            if (orderColumns != null)
            {
                query = query.OrderBy(orderColumns);
            }

            IQueryable<TAs> queryProjection;
            var selectClause = _clauseMapManager.GetClause(ClauseType.Select);
            if (selectClause != null || profile?.UnselectableProperties != null || profile?.SelectableProperties != null)
            {
                queryProjection = query.Select(SelectHelper.GetSelector<TEntity, TAs>(selectClause, profile));
            }else
            {
                queryProjection = query.ProjectTo<TAs>();
            }
            var skipClause = _clauseMapManager.GetClause(ClauseType.Skip);
            if (skipClause != null)
            {
                int.TryParse(skipClause.GetValue<string>(), out var skip);
                if (profile?.MaxToSkip != null && skip > profile.MaxToSkip)
                {
                    skip = profile.MaxToSkip.Value;
                }

                if (profile != null && profile.UseBaseType)
                {
                    queryProjection = queryProjection.Skip(skip);
                }
                else
                {
                    queryProjection = queryProjection.Skip(skip);
                }
            }
            var topClause = _clauseMapManager.GetClause(ClauseType.Top);
            if (topClause != null)
            {
                int.TryParse(topClause.GetValue<string>(), out var take);
                if (profile?.MaxToTake != null && take > profile.MaxToTake)
                {
                    take = profile.MaxToTake.Value;
                }

                if (profile != null && profile.UseBaseType)
                {
                    queryProjection = queryProjection.Take(take);
                }
                else
                {
                    queryProjection = queryProjection.Take(take);
                }
            }
            else if (profile?.MaxToTake != null)
            {
                queryProjection = queryProjection.Take(profile.MaxToTake.Value);
            }

            if (profile?.MaxToTake != null)
            {
                queryProjection = queryProjection.Take(profile.MaxToTake.Value);
            }
            return queryProjection;
        }
        

        private Expression MakeLambda(Expression parameter, Expression predicate)
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

        private Expression BuildWhereSubQueryExpression(Expression parameter, Expression childParameter, Type childType, Expression predicate)
        {
            var anyMethod = typeof(Enumerable).GetMethods().Single(m => m.Name == "Any" && m.GetParameters().Length == 2);
            anyMethod = anyMethod.MakeGenericMethod(childType);
            var lambdaPredicate = MakeLambda(childParameter, predicate);
            return Expression.Call(anyMethod, parameter, lambdaPredicate);
        }

        private Expression BuildWhereExpression(Expression parameter, Criteria criteria, params string[] properties)
        {
            Type childType = null;

            if (properties.Length > 1)
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
                var predicate = BuildWhereExpression(childParameter, criteria, innerProperties);
                if (isCollection)
                {
                    //build subquery
                    predicate = BuildWhereSubQueryExpression(parameter, childParameter, childType, predicate);
                }

                return predicate;
            }
            //build final predicate
            var childProperty = parameter.Type.GetProperty(properties[0]);
            var memberExpression = Expression.Property(parameter, childProperty);
            Expression orExpression = null;
            foreach (var value in criteria.Values)
            {
                var convertedValue = ConvertHelper.Convert(value, childProperty.PropertyType, criteria.Filter.FormatProvider);
                ConstantExpression val = Expression.Constant(convertedValue, childProperty.PropertyType);

                var filter = CriteriaFilterManager.GetTypeFilter(childProperty.PropertyType, criteria);
                if (filter != null)
                {
                    var newExpression = filter.Filter(memberExpression, val);
                    orExpression = orExpression == null ? newExpression : Expression.OrElse(orExpression, newExpression);
                }
            }
            return orExpression;
        }

        private IQueryable<T> Where<T>(IEnumerable<Criteria> criterias)
        {
            var parentEntity = Expression.Parameter(typeof(T), "x");
            Expression whereExpression = null;
            foreach (var c in criterias)
            {
                var expression = BuildWhereExpression(parentEntity, c, c.ColumnPath.ToArray());

                whereExpression = whereExpression == null ? expression : Expression.AndAlso(whereExpression, expression);
            }

            return source.Where(Expression.Lambda<Func<T, bool>>(whereExpression, parentEntity));
        }

        private IQueryable<T> OrderBy<T>(IEnumerable<Column> columns)
        {
            foreach (var column in columns)
            {
                source = source.Call(QueryableMethods.OrderBy, column.PropertyName);
            }
            return source;
        }

        private IQueryable<T> OrderByDesc<T>(IEnumerable<Column> columns)
        {
            foreach (var column in columns)
            {
                source = source.Call(QueryableMethods.OrderByDescending, column.PropertyName);
            }
            return source;
        }
    }
}