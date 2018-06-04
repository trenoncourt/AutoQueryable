using System;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models.Clauses;

namespace AutoQueryable.Core.Models
{
    public class Clause : IClause
    {
        public string ClauseType { get; }

        public object Value { get; }
        public Type ValueType { get; }

        public Clause(string type, object value, Type valueType = null)
        {
            ClauseType = type;
            Value = value;
            ValueType = valueType ?? typeof(string);
        }

    }
}
