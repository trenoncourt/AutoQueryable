using System.Collections.Generic;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public interface IClauseQueryFilter
    {
        string Alias { get; set; }
        ClauseType ClauseType { get; set; }
        object ParseValue(string value);
    }
}