using System;
using System.Linq;

namespace AutoQueryable.Core.Models
{
    public interface IAutoQueryableContext
    {
        IQueryable<dynamic> GetAutoQuery<T>(IQueryable<T> query) where T : class;
        IQueryable<dynamic> TotalCountQuery { get; }
    }

}