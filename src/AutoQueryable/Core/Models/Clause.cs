using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models.Clauses;

namespace AutoQueryable.Core.Models
{
    public class Clause
    {
        protected readonly AutoQueryableContext Context;

        public Clause(AutoQueryableContext context)
        {
            this.Context = context;
        }
        
        public ClauseType ClauseType { get; set; }

        public string Value { get; set; }
    }

    public class AllClauses
    {
        public SelectClause Select { get; set; }
        
        public TopClause Top { get; set; }
        
        public SkipClause Skip { get; set; }
        
        public OrderByClause OrderBy { get; set; }
        
        public OrderByDescClause OrderByDesc { get; set; }
        
        public GroupByClause GroupBy { get; set; }
        
        public FirstClause First { get; set; }
        
        public LastClause Last { get; set; }
        
        public WrapWithClause WrapWith { get; set; }
        
        public Clause Filter { get; set; }
        
        public Clause Expand { get; set; }
        
        public Clause Search { get; set; }
    }
}
