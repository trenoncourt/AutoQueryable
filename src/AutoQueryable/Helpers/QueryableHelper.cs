using System;
using System.Collections.Generic;
using System.Linq;
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
                if (profile?.UnselectableProperties == null || !profile.UnselectableProperties.Any())
                {
                    // There is no query string & no unselectable properties, so return directly the query.
                    return query;
                }
                // Get all columns without unselectable properties
                IEnumerable<string> columns = SelectHelper.GetSelectableColumns(profile?.UnselectableProperties, entityType);
                queryString = $"select={string.Join(",", columns.ToArray())}"; 
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

            QueryResult queryResult = QueryBuilder.Build(query, entityType, clauses, criterias, profile?.UnselectableProperties, countAllRows);
            if (wrapWithClause == null)
            {
                return queryResult.Result;
            }

            return WrapperManager.GetWrappedResult(wrapperParts, queryResult, clauses, queryString);
        }
    }
}