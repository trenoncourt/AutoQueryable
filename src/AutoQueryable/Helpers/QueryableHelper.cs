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
            dynamic result = QueryBuilder.Build(query, entityType, clauses, criterias, profile?.UnselectableProperties);
            if (wrapWithClause == null)
            {
                return result;
            }

            IEnumerable<WrapperPartType> wrapperParts = WrapperManager.GetWrapperParts(wrapWithClause.Value.Split(',')).ToList();
            dynamic wrapper = new ExpandoObject();
            wrapper.Result = (result as IQueryable<object>).ToList();
            foreach (var part in wrapperParts)
            {
                switch (part)
                {
                    case WrapperPartType.Count:
                        bool isResultEnumerableCount = typeof(IEnumerable).IsAssignableFrom((Type)result.GetType());
                        if (isResultEnumerableCount)
                        {
                            wrapper.Count = wrapper.Result.Count;
                        }
                        break;
                    case WrapperPartType.NextLink:
                        bool isResultEnumerableNextLink = typeof(IEnumerable).IsAssignableFrom((Type)result.GetType());

                        Clause topClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Top);
                        Clause skipClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Skip);
                        if (isResultEnumerableNextLink && topClause != null)
                        {
                            int skip = skipClause == null ? 0 : Convert.ToInt32(skipClause.Value);
                            int take = Convert.ToInt32(topClause.Value);
                            skip += take;
                            if (wrapper.Result.Count < take)
                            {
                                break;
                            }
                            if (skipClause != null)
                            {
                                wrapper.NextLink = queryString.ToLower().Replace($"{ClauseAlias.Skip}{skipClause.Value}", $"{ClauseAlias.Skip}{skip}");
                            }
                            else
                            {
                                wrapper.NextLink = $"{queryString.ToLower()}&{ClauseAlias.Skip}{skip}";
                            }
                            
                        }
                        break;
                }
            }
            return wrapper;
        }
    }
}