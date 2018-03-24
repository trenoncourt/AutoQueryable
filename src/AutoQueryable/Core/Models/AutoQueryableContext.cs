using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Providers;
using AutoQueryable.Helpers;
using AutoQueryable.Models;
using AutoQueryable.Providers;
using AutoQueryable.Providers.Default;

namespace AutoQueryable.Core.Models
{
    public abstract class AutoQueryableContext
    {
        private string[] _queryStringParts;
        
        public AutoQueryableProfile Profile { get; private set; }
        public string QueryString { get; private set; }
        public Type EntityType { get; private set; }
        public AllClauses Clauses { get; set; }
        
        public string[] QueryStringParts => _queryStringParts ?? (_queryStringParts = QueryString.Replace("?", "").Split('&'));

        public abstract dynamic GetAutoQuery();

        public static AutoQueryableContext Create<TEntity>(IQueryable<TEntity> query, string queryString,
            AutoQueryableProfile profile) where TEntity : class 
        {
            Type entityType = query.GetType().GenericTypeArguments[0];
            
            return new AutoQueryableContext<TEntity>
            {
                Query = query,
                QueryString = Uri.UnescapeDataString(queryString),
                EntityType = entityType,
                Profile = profile
            };
        }
    }

    public class AutoQueryableContext<TEntity> : AutoQueryableContext where TEntity : class 
    {
        public IQueryable<TEntity> Query { get; set; }
        
        public override dynamic GetAutoQuery()
        {
            // No query string, get only selectable columns
            if (string.IsNullOrEmpty(QueryString))
                return GetDefaultSelectableQuery();
            
            ICriteriaProvider criteriaProvider = ProviderFactory.GetCriteriaProvider(Profile.ProviderType);
            IList<Criteria> criterias = Profile.IsClauseAllowed(ClauseType.Filter) ? criteriaProvider.GetCriterias(EntityType, QueryStringParts, Profile).ToList() : null;
            IClauseProvider clauseProvider = ProviderFactory.GetClauseProvider(Profile.ProviderType);
            AllClauses clauses = clauseProvider.GetClauses(QueryStringParts, this);
            
            bool countAllRows = false;
            ICollection<WrapperPartType> wrapperParts = null;
            if (clauses.WrapWith != null)
            {
                IWrapperProvider wrapperProvider = ProviderFactory.GetWrapperProvider();
                wrapperParts = wrapperProvider.GetWrapperParts(clauses.WrapWith.Value.Split(','), Profile).ToList();
                countAllRows = wrapperParts.Contains(WrapperPartType.TotalCount);
            }
            
            QueryResult queryResult = QueryBuilder.Build(Query, EntityType, clauses, criterias, Profile, countAllRows);
            if (wrapperParts == null || !wrapperParts.Any())
            {
                return queryResult.Result;
            }

            return DefaultWrapperProvider.GetWrappedResult(wrapperParts, queryResult, clauses, QueryString);
        }

        private dynamic GetDefaultSelectableQuery()
        {
            ICollection<SelectColumn> selectColumns = EntityType.GetSelectableColumns(Profile);
            if (Profile.UseBaseType)
            {
                return Query.Select(SelectHelper.GetSelector<TEntity, TEntity>(selectColumns, Profile));
            }
            return Query.Select(SelectHelper.GetSelector<TEntity, object>(selectColumns, Profile));
        }
    }
}