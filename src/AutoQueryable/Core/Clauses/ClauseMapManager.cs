using System.Text.RegularExpressions;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Clauses
{
    public class ClauseMapManager : IClauseMapManager
    {
        private readonly IClauseQueryFilterMap _clauseQueryFilterMap;

        public ClauseMapManager(ISelectClauseHandler selectClauseHandler, IOrderByClauseHandler orderByClauseHandler, IWrapWithClauseHandler wrapWithClauseHandler)
        {
            _clauseQueryFilterMap = new DefaultClauseQueryFilterMap(selectClauseHandler, orderByClauseHandler, wrapWithClauseHandler);
        }

        public IClauseQueryFilter GetClauseQueryFilter(string alias)
            => _clauseQueryFilterMap.GetClauseQueryFilter(alias);

        public IClauseQueryFilter GetClauseQueryFilter(ClauseType clauseType) => _clauseQueryFilterMap.GetClauseQueryFilter(clauseType);

        public IClauseQueryFilter FindClauseQueryFilter(string queryParameterKey)
            => _clauseQueryFilterMap.FindClauseQueryFilter(queryParameterKey);
        private string GetOperandValue(string q, string clauseAlias) => Regex.Split(q, clauseAlias, RegexOptions.IgnoreCase)[1];
    }
}