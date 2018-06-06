using System;
using System.Linq;

namespace AutoQueryable.Core.Models
{
    public interface IAutoQueryableContext
    {
        IQueryable<object> GetAutoQuery<T>(IQueryable<T> query) where T : class;
    }

}