using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Core.Aliases;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Models;

namespace AutoQueryable.Core.Models.Clauses
{
    public class WrapWithClause : Clause
    {
        private ICollection<WrapperPartType> _wrapperPartTypes;
            
        public WrapWithClause(AutoQueryableContext context) : base(context)
        {
            ClauseType = ClauseType.WrapWith;
        }

        public bool CountAllRows { get; private set; }

        public bool Any => _wrapperPartTypes.Any();

        public void Parse()
        {
            string[] wrapperParts = Value.Split(',');
            _wrapperPartTypes = new List<WrapperPartType>();
            
            foreach (string q in wrapperParts)
            {
                if (q.Equals(WrapperAlias.Count, StringComparison.OrdinalIgnoreCase) && Context.Profile.IsWrapperPartAllowed(WrapperPartType.Count))
                {
                    _wrapperPartTypes.Add(WrapperPartType.Count);
                }
                else if (q.Equals(WrapperAlias.NextLink, StringComparison.OrdinalIgnoreCase) && Context.Profile.IsWrapperPartAllowed(WrapperPartType.NextLink))
                {
                    _wrapperPartTypes.Add(WrapperPartType.NextLink);
                }
                else if (q.Equals(WrapperAlias.TotalCount, StringComparison.OrdinalIgnoreCase) && Context.Profile.IsWrapperPartAllowed(WrapperPartType.TotalCount))
                {
                    CountAllRows = true;
                    _wrapperPartTypes.Add(WrapperPartType.TotalCount);
                }
            }
        }
        
        public dynamic GetWrappedResult(QueryResult queryResult)
        {
            dynamic wrapper = new ExpandoObject();
            wrapper.Result = (queryResult.Result as IQueryable<object>).ToList();
            foreach (var part in _wrapperPartTypes)
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

                        if (isResultEnumerableNextLink && Context.Clauses.Top != null)
                        {
                            int skip = Context.Clauses.Skip == null ? 0 : Convert.ToInt32(Context.Clauses.Skip.Value);
                            int take = Convert.ToInt32(Context.Clauses.Top.Value);
                            skip += take;
                            if (wrapper.Result.Count < take)
                            {
                                break;
                            }
                            if (Context.Clauses.Skip != null)
                            {
                                wrapper.NextLink = Context.QueryString.ToLower().Replace($"{ClauseAlias.Skip}{Context.Clauses.Skip.Value}", $"{ClauseAlias.Skip}{skip}");
                            }
                            else
                            {
                                wrapper.NextLink = $"{Context.QueryString.ToLower()}&{ClauseAlias.Skip}{skip}";
                            }

                        }
                        break;
                }
            }
            return wrapper;
        }
    }
}