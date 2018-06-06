using System;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public class DefaultClauseQueryFilter : IClauseQueryFilter
    {
        private readonly Func<string,Type, IAutoQueryableProfile, object> _parseValueAction;
        public string Alias { get; set; }
        public ClauseType ClauseType { get; set; }
        public object ParseValue(string value, Type type, IAutoQueryableProfile profile) => _parseValueAction(value, type, profile);
        

        public DefaultClauseQueryFilter(string alias, ClauseType clauseType, Func<string,Type, IAutoQueryableProfile, object> parseValueAction = null)
        {
            _parseValueAction = parseValueAction ?? ((value, type, profile) => value);
            ClauseType = clauseType;
            Alias = alias;
        }
    }
}