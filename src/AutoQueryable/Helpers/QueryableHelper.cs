using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Managers;
using AutoQueryable.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoQueryable.Helpers
{
    public static class QueryableHelper
    {
        public static IQueryable<object> GetAutoQuery<TEntity>(string queryString, IEntityType entityType, IQueryable<TEntity> dbSet, AutoQueryableProfile profile) where TEntity : class
        {
            IRelationalEntityTypeAnnotations table = entityType.Relational();

            string[] queryStringParts = queryString?.Replace("?", "")?.Split('&');
            dbSet = dbSet.AsNoTracking();
            if (queryStringParts == null)
            {
                List<Column> columns = SelectHelper.GetSelectableColumns(null, null, profile.UnselectableProperties, entityType).ToList();
                return dbSet.Select(SelectHelper.GetSelector<TEntity>(string.Join(",", columns.Select(c => c.PropertyName))));
            }

            var criteriaManager = new CriteriaManager();
            IList<Criteria> criterias = criteriaManager.GetCriterias(entityType, queryStringParts).ToList();
            var clauseManager = new ClauseManager();
            IList<Clause> clauses = clauseManager.GetClauses(queryStringParts).ToList();

            return QueryBuilder.Build(dbSet, entityType, table, clauses, criterias, profile.UnselectableProperties);
        }

        public static IQueryable<object> GetAutoQuery<TEntity>(string queryString, Type entityType, IQueryable<TEntity> query, AutoQueryableProfile profile) where TEntity : class
        {
            string[] queryStringParts = queryString?.Replace("?", "")?.Split('&');
            query = query.AsNoTracking();
            if (queryStringParts == null)
            {
                List<Column> columns = SelectHelper.GetSelectableColumns(null, profile.UnselectableProperties, entityType).ToList();
                return query.Select(SelectHelper.GetSelector<TEntity>(string.Join(",", columns.Select(c => c.PropertyName))));
            }

            var criteriaManager = new CriteriaManager();
            IList<Criteria> criterias = criteriaManager.GetCriterias(entityType, queryStringParts).ToList();
            var clauseManager = new ClauseManager();
            IList<Clause> clauses = clauseManager.GetClauses(queryStringParts).ToList();

            return QueryBuilder.Build(query, entityType, clauses, criterias, profile.UnselectableProperties);
        }
    }
}