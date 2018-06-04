using System;
using System.Collections.Generic;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public interface IClauseQueryFilter
    {
        string Alias { get; set; }
        string ClauseType { get; set; }
        object ParseValue(string value);
    }

    public class DefaultClauseQueryFilter : IClauseQueryFilter
    {
        private readonly Func<string, object> _parseValueAction;
        public string Alias { get; set; }
        public string ClauseType { get; set; }
        public object ParseValue(string value) => _parseValueAction(value);
        

        public DefaultClauseQueryFilter(string alias, string clauseType, Func<string,object> parseValueAction = null)
        {
            _parseValueAction = parseValueAction ?? (value => value);
            ClauseType = clauseType;
            Alias = alias;
        }
    }
}