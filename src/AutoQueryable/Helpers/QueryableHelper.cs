using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Managers;
using AutoQueryable.Models;
using AutoQueryable.Models.Enums;

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
            Clauses clauses = ClauseManager.GetClauses(queryStringParts, profile);

            var countAllRows = false;
            IEnumerable<WrapperPartType> wrapperParts = null;
            if (clauses.WrapWith != null)
            {
                wrapperParts = WrapperManager.GetWrapperParts(clauses.WrapWith.Value.Split(',')).ToList();
                countAllRows = wrapperParts.Contains(WrapperPartType.TotalCount);
            }

            QueryResult queryResult = QueryBuilder.Build(query, entityType, clauses, criterias, profile, countAllRows);
            if (clauses.WrapWith == null)
            {
                return queryResult.Result;
            }

            return WrapperManager.GetWrappedResult(wrapperParts, queryResult, clauses, queryString);
        }
    }
}