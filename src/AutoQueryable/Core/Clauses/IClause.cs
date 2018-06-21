using System;

namespace AutoQueryable.Core.Clauses
{
    public interface IClause
    {
        string ClauseType { get; }
        object Value { get; }
        Type ValueType { get; }
    }
}
