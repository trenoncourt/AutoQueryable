using System;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public interface IClauseHandler<T> where T : class 
    {
        T Handle(string queryStringPart, Type type = default, IAutoQueryableProfile profile = null);
    }
}