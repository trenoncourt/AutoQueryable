using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Providers;
using AutoQueryable.Models;
using AutoQueryable.Providers;
using AutoQueryable.Providers.Default;

namespace AutoQueryable.Helpers
{
    public static class QueryableHelper
    {
        public static dynamic GetAutoQuery<TEntity>(string queryString, Type entityType, IQueryable<TEntity> query, AutoQueryableProfile profile) where TEntity : class
        {
            if (string.IsNullOrEmpty(queryString))
            {
                if ((profile?.UnselectableProperties == null || !profile.UnselectableProperties.Any()) && (profile?.SelectableProperties == null || !profile.SelectableProperties.Any()))
                {
                    // There is no query string & no selectable/unselectable properties, so return directly the query.
                    return query;
                }
                // Get all columns without unselectable properties
                IEnumerable<string> columns = SelectHelper.GetSelectableColumns(profile, entityType);
                queryString = $"select={string.Join(",", columns.ToArray())}"; 
            }
            string[] queryStringParts = queryString.Replace("?", "").Split('&');

            ICriteriaProvider criteriaProvider = ProviderFactory.GetCriteriaProvider();
            IList<Criteria> criterias = profile.IsClauseAllowed(ClauseType.Filter) ? criteriaProvider.GetCriterias(entityType, queryStringParts, profile).ToList() : null;

            IClauseProvider clauseProvider = ProviderFactory.GetClauseProvider();
            Clauses clauses = clauseProvider.GetClauses(queryStringParts, profile);

            var countAllRows = false;
            IEnumerable<WrapperPartType> wrapperParts = null;
            if (clauses.WrapWith != null)
            {
                IWrapperProvider wrapperProvider = ProviderFactory.GetWrapperProvider();
                wrapperParts = wrapperProvider.GetWrapperParts(clauses.WrapWith.Value.Split(','), profile).ToList();
                countAllRows = wrapperParts.Contains(WrapperPartType.TotalCount);
            }

            QueryResult queryResult = QueryBuilder.Build(query, entityType, clauses, criterias, profile, countAllRows);
            if (clauses.WrapWith == null || !wrapperParts.Any())
            {
                return queryResult.Result;
            }

            return DefaultWrapperProvider.GetWrappedResult(wrapperParts, queryResult, clauses, queryString);
        }
    }
}