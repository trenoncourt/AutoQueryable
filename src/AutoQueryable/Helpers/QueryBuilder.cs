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
    public interface IQueryBuilder
    {
        IQueryable<dynamic> Build<T>(IQueryable<T> query, ICollection<Criteria> criterias, IAutoQueryableProfile profile) where T : class;
    }
    public class QueryBuilder : IQueryBuilder
    {
        private readonly IClauseValueManager _clauseValueManager;
        private readonly ICriteriaFilterManager _criteriaFilterManager;


        public QueryBuilder(IClauseValueManager clauseValueManager, ICriteriaFilterManager criteriaFilterManager)
        {
            _clauseValueManager = clauseValueManager;
            _criteriaFilterManager = criteriaFilterManager;
        }
        public IQueryable<dynamic> Build<T>(IQueryable<T> query, ICollection<Criteria> criterias, IAutoQueryableProfile profile) where T : class
        {
            if (criterias != null && criterias.Any())
            {
                query = _addCriterias(query, criterias);
            }
            query = _addOrderBy(query, _clauseValueManager.OrderBy, profile);
            IQueryable<dynamic> queryProjection;
           
            if (!_clauseValueManager.Select.Any() && profile?.UnselectableProperties == null && profile?.SelectableProperties == null)
            {
                queryProjection = query;
                
            } else
            {
                queryProjection = profile.UseBaseType ? 
                    query.Select(SelectHelper.GetSelector<T, T>(_clauseValueManager.Select, profile)) : query.Select(SelectHelper.GetSelector<T, object>(_clauseValueManager.Select, profile));
            }
            if (_clauseValueManager.Skip.HasValue)
            {
                if (profile?.MaxToSkip != null && _clauseValueManager.Skip > profile.MaxToSkip)
                {
                    _clauseValueManager.Skip = profile.MaxToSkip.Value;
                }

                if (profile != null && profile.UseBaseType)
                {
                    queryProjection = queryProjection.Skip(_clauseValueManager.Skip.Value);
                }
                else
                {
                    queryProjection = queryProjection.Skip(_clauseValueManager.Skip.Value);
                }
            }
            if (_clauseValueManager.Top.HasValue)
            {
                if (profile?.MaxToTake != null && _clauseValueManager.Top > profile.MaxToTake)
                {
                    _clauseValueManager.Top = profile.MaxToTake.Value;
                }

                if (profile != null && profile.UseBaseType)
                {
                    queryProjection = queryProjection.Take(_clauseValueManager.Top.Value);
                }
                else
                {
                    queryProjection = queryProjection.Take(_clauseValueManager.Top.Value);
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

                var filter = _criteriaFilterManager.GetTypeFilter(childProperty.PropertyType, criteria.Filter.Alias);
                if (filter != null)
                {
                    var newExpression = filter.Filter(memberExpression, val);
                    orExpression = orExpression == null ? newExpression : Expression.OrElse(orExpression, newExpression);
                }
            }
            return orExpression;
        }

        private IQueryable<T> _addCriterias<T>(IQueryable<T> source, IEnumerable<Criteria> criterias) where T : class
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
        // TODO: Handle '-' to orderbydesc (in order that they are in the orderby string)
        private IQueryable<T> _addOrderBy<T>(IQueryable<T> source, Dictionary<string, bool> orderClause, IAutoQueryableProfile profile) where T : class
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