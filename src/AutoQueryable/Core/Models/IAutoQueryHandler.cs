using System.Linq;
using AutoQueryable.Core.Clauses;

namespace AutoQueryable.Core.Models
{
    public interface IAutoQueryHandler
    {
        IQueryable<dynamic> GetAutoQuery<T>(IQueryable<T> query, IAutoQueryableProfile profile) where T : class;
        IClauseValueManager ClauseValueManager { get; }
        IQueryable<dynamic> TotalCountQuery { get; }
        string QueryString { get; }
    }
}