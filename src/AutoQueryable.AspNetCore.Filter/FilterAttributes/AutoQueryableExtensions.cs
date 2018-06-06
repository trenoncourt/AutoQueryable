using System.Linq;
using AutoQueryable.Core.Models;

namespace AutoQueryable.AspNetCore.Filter.FilterAttributes
{
    public class AutoQueryable
    {
        private readonly IAutoQueryableContext _autoQueryableContext;

        public AutoQueryable(IAutoQueryableContext autoQueryableContext)
        {
            _autoQueryableContext = autoQueryableContext;
        }
        public dynamic AutoQuery<TEntity>(IQueryable<TEntity> query) where TEntity : class => _autoQueryableContext.GetAutoQuery(query);
    }
}
