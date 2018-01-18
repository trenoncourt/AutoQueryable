using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Helpers;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Providers;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using AutoQueryable.Providers;
using AutoQueryable.Providers.Default;

namespace AutoQueryable.Helpers
{
    public static class QueryableHelper
    {
        public static dynamic GetAutoQuery<TEntity>(string queryString, Type entityType, IQueryable<TEntity> query, AutoQueryableProfile profile) where TEntity : class
        {
            // If query string empty select default entity properties.
            if (string.IsNullOrEmpty(queryString))
            {
                IColumnProvider columnProvider = ProviderFactory.GetColumnProvider();
                IEnumerable<SelectColumn> selectColumns = EntityColumnHelper.GetSelectableColumns(profile, entityType);
                if (profile.UseBaseType)
                {
                    return query.Select(SelectHelper.GetSelector<TEntity, TEntity>(selectColumns, profile));
                }
                return query.Select(SelectHelper.GetSelector<TEntity, object>(selectColumns, profile));
            }
            
            // Get criteria & clauses from choosen provider (AQ, OData, ...)
            string[] queryStringParts = Uri.UnescapeDataString(queryString).GetParts();
            ICriteriaProvider criteriaProvider = ProviderFactory.GetCriteriaProvider(profile?.ProviderType);
            IList<Criteria> criterias = profile.IsClauseAllowed(ClauseType.Filter) ? criteriaProvider.GetCriterias(entityType, queryStringParts, profile).ToList() : null;
            IClauseProvider clauseProvider = ProviderFactory.GetClauseProvider(profile?.ProviderType);
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