using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Models;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using AutoQueryable.Models.Constants;

namespace AutoQueryable.Helpers
{
    public static class QueryBuilder
    {
        public static IQueryable<dynamic> TotalCountQuery { get; private set; }

        public static IQueryable<dynamic> Build<T>(IClauseValueManager clauseValueManager, ICriteriaFilterManager criteriaFilterManager, IQueryable<T> query, ICollection<Criteria> criterias, IAutoQueryableProfile profile) where T : class
        {
            if (criterias != null && criterias.Any())
            {
                query = _addCriterias(criteriaFilterManager, query, criterias);
            }
            query = _addOrderBy(query, clauseValueManager.OrderBy, profile);

            TotalCountQuery = query;
            if(clauseValueManager.First)
            {
                query = query.Take(1);
            }else{
                query = _handlePaging(clauseValueManager, query, profile);
            }
            //if (profile?.MaxToTake != null)
            //{
            //    queryProjection = profile.UseBaseType ? ((IQueryable<T>)queryProjection).Take(profile.MaxToTake.Value) : queryProjection.Take(profile.MaxToTake.Value);
            //}

            IQueryable<dynamic> queryProjection = query;
            if (clauseValueManager.Select.Any() || profile?.UnselectableProperties != null || profile?.SelectableProperties != null)
            {
                if (profile != null)
                {
                    if (profile.ToListBeforeSelect)
                    {
                        query = query.ToList().AsQueryable();
                    }
                    queryProjection = profile.UseBaseType ?
                        query.Select(SelectHelper.GetSelector<T, T>(clauseValueManager.Select, profile)) : query.Select(SelectHelper.GetSelector<T, object>(clauseValueManager.Select, profile));
                }
            }

            return queryProjection;
        }

        private static IQueryable<T> _handlePaging<T>(IClauseValueManager clauseValueManager, IQueryable<T> query, IAutoQueryableProfile profile) where T : class
        {
            if (clauseValueManager.Skip.HasValue)
            {
                if (profile?.MaxToSkip != null && clauseValueManager.Skip > profile.MaxToSkip)
                {
                    clauseValueManager.Skip = profile.MaxToSkip.Value;
                }
                query = query.Skip(clauseValueManager.Skip.Value);
            }
            // Top or DefaultToTake = 0 => return ALL values
            if (clauseValueManager.Top.HasValue)
            {
                if (clauseValueManager.Top != 0 && (profile == null || profile.DefaultToTake != 0))
                {
                    if (profile?.MaxToTake != null && clauseValueManager.Top > profile.MaxToTake)
                    {
                        clauseValueManager.Top = profile.MaxToTake.Value;
                    }
                    query = query.Take(clauseValueManager.Top.Value);
                }
            }
            else if (profile?.MaxToTake != null)
            {
                query = query.Take(profile.MaxToTake.Value);
            }
            else
            {
                if (profile != null && profile.DefaultToTake != 0)
                {
                    query = query.Take(profile.DefaultToTake);
                }

            }

            return query;
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

        private static Expression BuildWhereExpression(ICriteriaFilterManager criteriaFilterManager, Expression parameter, Criteria criteria, params string[] properties)
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
                var predicate = BuildWhereExpression(criteriaFilterManager, childParameter, criteria, innerProperties);
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

                var filter = criteriaFilterManager.GetTypeFilter(childProperty.PropertyType, criteria.Filter.Alias);
                if (filter != null)
                {
                    var newExpression = filter.Filter(memberExpression, val);
                    orExpression = orExpression == null ? newExpression : Expression.OrElse(orExpression, newExpression);
                }
            }
            return orExpression;
        }

        private static IQueryable<T> _addCriterias<T>(ICriteriaFilterManager criteriaFilterManager, IQueryable<T> source, IEnumerable<Criteria> criterias) where T : class
        {
            var parentEntity = Expression.Parameter(typeof(T), "x");
            Expression whereExpression = null;
            foreach (var c in criterias)
            {
                var expression = BuildWhereExpression(criteriaFilterManager, parentEntity, c, c.ColumnPath.ToArray());

                whereExpression = whereExpression == null ? expression : Expression.AndAlso(whereExpression, expression);
            }

            return source.Where(Expression.Lambda<Func<T, bool>>(whereExpression, parentEntity));
        }
        // TODO: Handle '-' to orderbydesc (in order that they are in the orderby string)
        private static IQueryable<T> _addOrderBy<T>(IQueryable<T> source, Dictionary<string, bool> orderClause, IAutoQueryableProfile profile) where T : class
        {
            if (orderClause.Count == 0)
            {
                return source;
            }
            IEnumerable<PropertyInfo> properties = typeof(T).GetProperties();
            if (profile?.SortableProperties != null)
            {
                properties = properties.Where(c => profile.SortableProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase));
            }
            if (profile?.UnSortableProperties != null)
            {
                properties = properties.Where(c => !profile.UnSortableProperties.Contains(c.Name, StringComparer.OrdinalIgnoreCase));
            }

            properties = properties.Where(p => orderClause.Keys.Contains(p.Name, StringComparer.OrdinalIgnoreCase));

            var columns = properties.Select(v => new Column
            {
                PropertyName = v.Name
            });
            foreach (var column in columns)
            {
                var desc = orderClause.FirstOrDefault(o => string.Equals(o.Key, column.PropertyName, StringComparison.OrdinalIgnoreCase)).Value;
                if(desc){
                    source = source.Call(QueryableMethods.OrderByDescending, column.PropertyName);
                }else
                {
                    source = source.Call(QueryableMethods.OrderBy, column.PropertyName);
                }
            }
            return source;
        }

        //private IQueryable<T> OrderByDesc<T>(IQueryable<T> source, IEnumerable<Column> columns)
        //{
        //    foreach (var column in columns)
        //    {
        //        source = source.Call(QueryableMethods.OrderByDescending, column.PropertyName);
        //    }
        //    return source;
        //}
    }
}