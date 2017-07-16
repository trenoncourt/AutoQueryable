using System;
using System.Collections.Generic;
using AutoQueryable.Aliases;
using AutoQueryable.Models;
using AutoQueryable.Extensions;
using System.Dynamic;
using System.Linq;
using System.Collections;
using System.Reflection;
using AutoQueryable.Models.Enums;

namespace AutoQueryable.Managers
{
    public class WrapperManager
    {
        public static IEnumerable<WrapperPartType> GetWrapperParts(string[] queryStringWrapperParts, AutoQueryableProfile profile)
        {
            foreach (string q in queryStringWrapperParts)
            {
                if (q.Equals(WrapperAlias.Count, StringComparison.OrdinalIgnoreCase) && profile.IsWrapperPartAllowed(WrapperPartType.Count))
                {
                    yield return WrapperPartType.Count;
                }
                else if (q.Equals(WrapperAlias.NextLink, StringComparison.OrdinalIgnoreCase) && profile.IsWrapperPartAllowed(WrapperPartType.NextLink))
                {
                    yield return WrapperPartType.NextLink;
                }
                else if (q.Equals(WrapperAlias.TotalCount, StringComparison.OrdinalIgnoreCase) && profile.IsWrapperPartAllowed(WrapperPartType.TotalCount))
                {
                    yield return WrapperPartType.TotalCount;
                }
            }
        }

        public static dynamic GetWrappedResult(IEnumerable<WrapperPartType> wrapperParts, QueryResult queryResult, Clauses clauses, string queryString)
        {
            dynamic wrapper = new ExpandoObject();
            wrapper.Result = (queryResult.Result as IQueryable<object>).ToList();
            foreach (var part in wrapperParts)
            {
                switch (part)
                {
                    case WrapperPartType.Count:
                        bool isResultEnumerableCount = typeof(IEnumerable).IsAssignableFrom((Type)queryResult.Result.GetType());
                        if (isResultEnumerableCount)
                        {
                            wrapper.Count = wrapper.Result.Count;
                        }
                        break;
                    case WrapperPartType.TotalCount:
                        wrapper.TotalCount = queryResult.TotalCount;
                        break;
                    case WrapperPartType.NextLink:
                        bool isResultEnumerableNextLink = typeof(IEnumerable).IsAssignableFrom((Type)queryResult.Result.GetType());

                        if (isResultEnumerableNextLink && clauses.Top != null)
                        {
                            int skip = clauses.Skip == null ? 0 : Convert.ToInt32(clauses.Skip.Value);
                            int take = Convert.ToInt32(clauses.Top.Value);
                            skip += take;
                            if (wrapper.Result.Count < take)
                            {
                                break;
                            }
                            if (clauses.Skip != null)
                            {
                                wrapper.NextLink = queryString.ToLower().Replace($"{ClauseAlias.Skip}{clauses.Skip.Value}", $"{ClauseAlias.Skip}{skip}");
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