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
                if (profile?.UnselectableProperties == null)
                {
                    return query;
                }
                IEnumerable<string> columns = SelectHelper.GetSelectableColumns(profile.UnselectableProperties, entityType);
                return query.Select(SelectHelper.GetSelector<TEntity>(string.Join(",", columns.ToArray())));
            }
            string[] queryStringParts = queryString?.Replace("?", "")?.Split('&');

            var criteriaManager = new CriteriaManager();
            IList<Criteria> criterias = criteriaManager.GetCriterias(entityType, queryStringParts).ToList();
            var clauseManager = new ClauseManager();
            IList<Clause> clauses = clauseManager.GetClauses(queryStringParts).ToList();

            return QueryBuilder.Build(query, entityType, clauses, criterias, profile?.UnselectableProperties);
        }
    }
}