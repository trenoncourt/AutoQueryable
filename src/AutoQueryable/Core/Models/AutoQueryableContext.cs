using System.Linq;
using AutoQueryable.Core.Clauses;

namespace AutoQueryable.Core.Models
{
    public class AutoQueryableContext : IAutoQueryableContext
    {
        private readonly IAutoQueryHandler _autoQueryHandler;
        public IQueryable<dynamic> TotalCountQuery { get; private set; }
        public IClauseValueManager ClauseValueManager { get; private set; }
        public string QueryString { get; private set; }

        public AutoQueryableContext(IAutoQueryHandler autoQueryHandler)
        {
            _autoQueryHandler = autoQueryHandler;
        }

        public dynamic GetAutoQuery<T>(IQueryable<T> query) where T : class
        {
            var result = _autoQueryHandler.GetAutoQuery(query);
            TotalCountQuery = _autoQueryHandler.TotalCountQuery;
            ClauseValueManager = _autoQueryHandler.ClauseValueManager;
            QueryString = _autoQueryHandler.QueryString;
            return result;
        }
    }
}