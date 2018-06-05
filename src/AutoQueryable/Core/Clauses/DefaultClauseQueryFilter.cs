using System;
using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Clauses
{
    public class DefaultClauseQueryFilter : IClauseQueryFilter
    {
        private readonly Func<string, object> _parseValueAction;
        public string Alias { get; set; }
        public ClauseType ClauseType { get; set; }
        public object ParseValue(string value) => _parseValueAction(value);
        

        public DefaultClauseQueryFilter(string alias, ClauseType clauseType, Func<string,object> parseValueAction = null)
        {
            _parseValueAction = parseValueAction ?? (value => value);
            ClauseType = clauseType;
            Alias = alias;
        }
    }
}