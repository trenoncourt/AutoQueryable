using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Core.Aliases;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Clauses
{
    public class DefaultClauseQueryFilterMap : IClauseQueryFilterMap
    {
        private readonly ICollection<IClauseQueryFilter> _queryFilters = new List<IClauseQueryFilter>();
        public DefaultClauseQueryFilterMap(ISelectClauseHandler selectClauseHandler, IOrderByClauseHandler orderByClauseHandler, IWrapWithClauseHandler wrapWithClauseHandler)
        {
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Select, ClauseType.Select, selectClauseHandler.Handle));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Top, ClauseType.Top, (value, type, profile) => int.Parse(value)));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Skip, ClauseType.Skip, (value, type, profile) => int.Parse(value)));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Take, ClauseType.Top, (value, type, profile) => int.Parse(value)));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.First, ClauseType.First));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Last, ClauseType.Last));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.OrderBy, ClauseType.OrderBy, orderByClauseHandler.Handle));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.GroupBy, ClauseType.GroupBy));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Page, ClauseType.Page, (value, type, profile) => int.Parse(value)));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.PageSize, ClauseType.PageSize, (value, type, profile) => int.Parse(value)));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.WrapWith, ClauseType.WrapWith, wrapWithClauseHandler.Handle));
        }


        public IClauseQueryFilter GetClauseQueryFilter(string alias) => _queryFilters.FirstOrDefault(f =>
            string.Equals(f.Alias.ToLowerInvariant(), alias.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase));

        public IClauseQueryFilter FindClauseQueryFilter(string queryParameterKey) => _queryFilters.FirstOrDefault(clause => queryParameterKey.Contains(clause.Alias.ToLowerInvariant()));
        public IClauseQueryFilter GetClauseQueryFilter(ClauseType clauseType) => _queryFilters.FirstOrDefault(f => f.ClauseType == clauseType);
    }
}