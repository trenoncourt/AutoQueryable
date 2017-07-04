using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Aliases;
using AutoQueryable.Managers;
using AutoQueryable.Models;

namespace AutoQueryable.Helpers
{
    public static class QueryableHelper
    {
        public static dynamic GetAutoQuery<TEntity>(string queryString, Type entityType, IQueryable<TEntity> query, AutoQueryableProfile profile) where TEntity : class
        {
            if (string.IsNullOrEmpty(queryString))
            {
                IEnumerable<string> columns = SelectHelper.GetSelectableColumns(profile?.UnselectableProperties, entityType);
                return query.Select(SelectHelper.GetSelector<TEntity>(string.Join(",", columns.ToArray())));
            }
            string[] queryStringParts = queryString.Replace("?", "").Split('&');

            IList<Criteria> criterias = CriteriaManager.GetCriterias(entityType, queryStringParts).ToList();
            IList<Clause> clauses = ClauseManager.GetClauses(queryStringParts).ToList();

            Clause wrapWithClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.WrapWith);
            var countAllRows = false;
            IEnumerable<WrapperPartType> wrapperParts = null;
            if (wrapWithClause != null)
            {
                wrapperParts = WrapperManager.GetWrapperParts(wrapWithClause.Value.Split(',')).ToList();
                countAllRows = wrapperParts.Contains(WrapperPartType.TotalCount);
            }

            dynamic result = QueryBuilder.Build(query, entityType, clauses, criterias, profile?.UnselectableProperties,countAllRows);
            if (wrapWithClause == null)
            {
                return result.Item1;
            }

            return WrapperManager.GetWrappedResult(wrapperParts, result, clauses, queryString);
        }
    }
}