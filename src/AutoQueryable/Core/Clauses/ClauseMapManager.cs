using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Core.Aliases;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public class ClauseMapManager : IClauseMapManager
    {
        private readonly ISelectClauseHandler _selectClauseHandler;
        private readonly IOrderByClauseHandler _orderByClauseHandler;
        private readonly IWrapWithClauseHandler _wrapWithClauseHandler;
        private readonly IAutoQueryableProfile _autoQueryableProfile;
        private readonly ICollection<IClauseQueryFilter> _queryFilters = new List<IClauseQueryFilter>();

        public ClauseMapManager(ISelectClauseHandler selectClauseHandler, IOrderByClauseHandler orderByClauseHandler, IWrapWithClauseHandler wrapWithClauseHandler, IAutoQueryableProfile autoQueryableProfile)
        {
            _selectClauseHandler = selectClauseHandler;
            _orderByClauseHandler = orderByClauseHandler;
            _wrapWithClauseHandler = wrapWithClauseHandler;
            _autoQueryableProfile = autoQueryableProfile;
        }

        public void Init()
        {
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.Select))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Select, ClauseType.Select, _selectClauseHandler.Handle));    
            }
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.Top))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Top, ClauseType.Top, (value, type, profile) => int.Parse(value)));
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Take, ClauseType.Top, (value, type, profile) => int.Parse(value)));
            }
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.Skip))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Skip, ClauseType.Skip, (value, type, profile) => int.Parse(value)));
            }
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.First))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.First, ClauseType.First));
            }
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.Last))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Last, ClauseType.Last));
            }
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.OrderBy))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.OrderBy, ClauseType.OrderBy, _orderByClauseHandler.Handle));
            }
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.GroupBy))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.GroupBy, ClauseType.GroupBy));
            }
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.Page))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Page, ClauseType.Page, (value, type, profile) => int.Parse(value)));
            }
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.PageSize))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.PageSize, ClauseType.PageSize, (value, type, profile) => int.Parse(value)));
            }
            if (_autoQueryableProfile.IsClauseAllowed(ClauseType.WrapWith))
            {
                _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.WrapWith, ClauseType.WrapWith, _wrapWithClauseHandler.Handle));
            }
        }

        public IClauseQueryFilter GetClauseQueryFilter(ClauseType clauseType) => _queryFilters.FirstOrDefault(f => f.ClauseType == clauseType);

        public IClauseQueryFilter FindClauseQueryFilter(string queryParameterKey) => _queryFilters.FirstOrDefault(clause => queryParameterKey.Contains(clause.Alias.ToLowerInvariant()));
    }
}