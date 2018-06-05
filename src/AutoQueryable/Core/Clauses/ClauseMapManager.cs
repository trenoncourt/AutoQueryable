using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AutoQueryable.Core.Aliases;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public class ClauseMapManager : IClauseMapManager
    {
        private readonly IClauseQueryFilterMap _clauseQueryFilterMap;

        public ClauseMapManager(ISelectClauseHandler selectClauseHandler)
        {
            _clauseQueryFilterMap = new DefaultClauseQueryFilterMap(selectClauseHandler);
            //FilterMap.AddFilters(typeof(string), StringFilterMapFactory.InitializeMap(QueryableFilterMap));
            //FilterMap.AddFilters(typeof(DateTime), DateTimeFilterMapFactory.InitializeMap(QueryableFilterMap));
            //FilterMap.AddFilters(typeof(object), BaseFilterMapFactory.InitializeMap(QueryableFilterMap));
            // FilterMap.AddFilters(typeof(DateTime), DateTimeFilter);
        }

        public IClauseQueryFilter GetClauseQueryFilter(string alias)
            => _clauseQueryFilterMap.GetClauseQueryFilter(alias);

        public IClauseQueryFilter GetClauseQueryFilter(ClauseType clauseType) => _clauseQueryFilterMap.GetClauseQueryFilter(clauseType);

        public IClauseQueryFilter FindClauseQueryFilter(string queryParameterKey)
            => _clauseQueryFilterMap.FindClauseQueryFilter(queryParameterKey);
        private string GetOperandValue(string q, string clauseAlias) => Regex.Split(q, clauseAlias, RegexOptions.IgnoreCase)[1];
    }

    public interface IClauseQueryFilterMap
    {
        IClauseQueryFilter GetClauseQueryFilter(string alias);
        IClauseQueryFilter FindClauseQueryFilter(string queryParameterKey);
        IClauseQueryFilter GetClauseQueryFilter(ClauseType clauseType);
    }

    public class DefaultClauseQueryFilterMap : IClauseQueryFilterMap
    {
        private readonly ICollection<IClauseQueryFilter> _queryFilters = new List<IClauseQueryFilter>();
        public DefaultClauseQueryFilterMap(ISelectClauseHandler selectClauseHandler)
        {
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Select, ClauseType.Select, selectClauseHandler.Handle));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Top, ClauseType.Top, value => int.Parse(value)));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Skip, ClauseType.Skip, value => int.Parse(value)));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Take, ClauseType.Top, value => int.Parse(value)));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.First, ClauseType.First));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Last, ClauseType.Last));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.OrderBy, ClauseType.OrderBy));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.GroupBy, ClauseType.GroupBy));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.Page, ClauseType.Page, value => int.Parse(value)));
            _queryFilters.Add(new DefaultClauseQueryFilter(ClauseAlias.PageSize, ClauseType.PageSize, value => int.Parse(value)));
        }


        public IClauseQueryFilter GetClauseQueryFilter(string alias) => _queryFilters.FirstOrDefault(f => f.Alias == alias);

        public IClauseQueryFilter FindClauseQueryFilter(string queryParameterKey) => _queryFilters.FirstOrDefault(clause => queryParameterKey.Contains(clause.Alias));
        public IClauseQueryFilter GetClauseQueryFilter(ClauseType clauseType) => _queryFilters.FirstOrDefault(f => f.ClauseType == clauseType);
    }
}