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
//    public static class QueryableHelper
//    {
//        public static dynamic GetAutoQuery<TEntity>(AutoQueryableContext context) where TEntity : class
//        {
//            // If query string empty select default entity properties.
//            if (string.IsNullOrEmpty(context.QueryString))
//            {
//                IEnumerable<SelectColumn> selectColumns = EntityColumnHelper.GetSelectableColumns(context.Profile, context.EntityType);
//                if (context.Profile.UseBaseType)
//                {
//                    return context.Query.Select(SelectHelper.GetSelector<TEntity, TEntity>(selectColumns, context.Profile));
//                }
//                return context.Query.Select(SelectHelper.GetSelector<TEntity, object>(selectColumns, context.Profile));
//            }
//            
//            // Get criteria & clauses from choosen provider (AQ, OData, ...)
//            string[] queryStringParts = Uri.UnescapeDataString(queryString).GetParts();
//            ICriteriaProvider criteriaProvider = ProviderFactory.GetCriteriaProvider(context.Profile.ProviderType);
//            IList<Criteria> criterias = context.Profile.IsClauseAllowed(ClauseType.Filter) ? criteriaProvider.GetCriterias(context.EntityType, queryStringParts, context.Profile).ToList() : null;
//            IClauseProvider clauseProvider = ProviderFactory.GetClauseProvider(context.Profile.ProviderType);
//            AllClauses clauses = clauseProvider.GetClauses(queryStringParts, context);
//
//            var countAllRows = false;
//            IEnumerable<WrapperPartType> wrapperParts = null;
//            if (clauses.WrapWith != null)
//            {
//                IWrapperProvider wrapperProvider = ProviderFactory.GetWrapperProvider();
//                wrapperParts = wrapperProvider.GetWrapperParts(clauses.WrapWith.Value.Split(','), context.Profile).ToList();
//                countAllRows = wrapperParts.Contains(WrapperPartType.TotalCount);
//            }
//
//            QueryResult queryResult = QueryBuilder.Build(query, context.EntityType, clauses, criterias, context.Profile, countAllRows);
//            if (clauses.WrapWith == null || !wrapperParts.Any())
//            {
//                return queryResult.Result;
//            }
//
//            return DefaultWrapperProvider.GetWrappedResult(wrapperParts, queryResult, clauses, queryString);
//        }
//    }
}