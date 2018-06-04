using System.Collections.Generic;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public class ClauseValueManager<TEntity, TAs> : IClauseValueManager where TEntity : class where TAs : class
    {
        public ICollection<SelectColumn> Select { get; set; }
        public int? Take { get; set; }
        public int? Top { get; set; }
        public void SetDefaults(IAutoQueryableProfile profile, ISelectClauseHandler selectClauseHandler)
        {
            OrderBy = profile.DefaultOrderBy;
            Select = selectClauseHandler.Handle("");
        }

        public int? Skip { get; set; }
        
        public string OrderBy { get; set; }
        
        public string GroupBy { get; set; }
        
        public bool First { get; set; }
        
        public bool Last { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}