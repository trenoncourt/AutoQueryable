using System.Linq;

namespace AutoQueryable.Core.Models
{
    public interface IAutoQueryHandler
    {
        IQueryable<dynamic> GetAutoQuery<T>(IQueryable<T> query, IAutoQueryableProfile profile) where T : class;
        IQueryable<dynamic> TotalCountQuery { get; }
    }
}