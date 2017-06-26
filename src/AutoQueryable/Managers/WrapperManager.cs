using System;
using System.Collections.Generic;
using AutoQueryable.Aliases;
using AutoQueryable.Models;
using AutoQueryable.Extensions;
using System.Dynamic;
using System.Linq;
using System.Collections;
using System.Reflection;

namespace AutoQueryable.Managers
{
    public class WrapperManager
    {
        public static IEnumerable<WrapperPartType> GetWrapperParts(string[] queryStringWrapperParts)
        {
            foreach (string q in queryStringWrapperParts)
            {
                if (q.Contains(WrapperAlias.Count, StringComparison.OrdinalIgnoreCase))
                {
                    yield return WrapperPartType.Count;
                }
                else if (q.Contains(WrapperAlias.NextLink, StringComparison.OrdinalIgnoreCase))
                {
                    yield return WrapperPartType.NextLink;
                }
            }
        }

        public static dynamic GetWrappedResult(IEnumerable<WrapperPartType> wrapperParts, dynamic result, IList<Clause> clauses, string queryString)
        {
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