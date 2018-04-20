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
            this.ClauseType = ClauseType.WrapWith;
        }

        public bool CountAllRows { get; private set; }

        public bool Any => this._wrapperPartTypes.Any();

        public void Parse()
        {
            var wrapperParts = this.Value.Split(',');
            this._wrapperPartTypes = new List<WrapperPartType>();
            
            foreach (var q in wrapperParts)
            {
                if (q.Equals(WrapperAlias.Count, StringComparison.OrdinalIgnoreCase) && this.Context.Profile.IsWrapperPartAllowed(WrapperPartType.Count))
                {
                    this._wrapperPartTypes.Add(WrapperPartType.Count);
                }
                else if (q.Equals(WrapperAlias.NextLink, StringComparison.OrdinalIgnoreCase) && this.Context.Profile.IsWrapperPartAllowed(WrapperPartType.NextLink))
                {
                    this._wrapperPartTypes.Add(WrapperPartType.NextLink);
                }
                else if (q.Equals(WrapperAlias.TotalCount, StringComparison.OrdinalIgnoreCase) && this.Context.Profile.IsWrapperPartAllowed(WrapperPartType.TotalCount))
                {
                    this.CountAllRows = true;
                    this._wrapperPartTypes.Add(WrapperPartType.TotalCount);
                }
            }
        }
        
        public dynamic GetWrappedResult(QueryResult queryResult)
        {
            dynamic wrapper = new ExpandoObject();
            wrapper.Result = (queryResult.Result as IQueryable<object>).ToList();
            foreach (var part in this._wrapperPartTypes)
            {
                switch (part)
                {
                    case WrapperPartType.Count:
                        var isResultEnumerableCount = typeof(IEnumerable).IsAssignableFrom((Type)queryResult.Result.GetType());
                        if (isResultEnumerableCount)
                        {
                            wrapper.Count = wrapper.Result.Count;
                        }
                        break;
                    case WrapperPartType.TotalCount:
                        wrapper.TotalCount = queryResult.TotalCount;
                        break;
                    case WrapperPartType.NextLink:
                        var isResultEnumerableNextLink = typeof(IEnumerable).IsAssignableFrom((Type)queryResult.Result.GetType());

                        if (isResultEnumerableNextLink && this.Context.Clauses.Top != null)
                        {
                            var skip = this.Context.Clauses.Skip == null ? 0 : Convert.ToInt32(this.Context.Clauses.Skip.Value);
                            var take = Convert.ToInt32(this.Context.Clauses.Top.Value);
                            skip += take;
                            if (wrapper.Result.Count < take)
                            {
                                break;
                            }
                            if (this.Context.Clauses.Skip != null)
                            {
                                wrapper.NextLink = this.Context.QueryString.ToLower().Replace($"{ClauseAlias.Skip}{this.Context.Clauses.Skip.Value}", $"{ClauseAlias.Skip}{skip}");
                            }
                            else
                            {
                                wrapper.NextLink = $"{this.Context.QueryString.ToLower()}&{ClauseAlias.Skip}{skip}";
                            }

                        }
                        break;
                }
            }
            return wrapper;
        }
    }
}