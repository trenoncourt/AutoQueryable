using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models.Clauses;

namespace AutoQueryable.Core.Models
{
    public class Clause
    {
        protected readonly AutoQueryableContext Context;

        public Clause(AutoQueryableContext context)
        {
            Context = context;
        }
        
        public ClauseType ClauseType { get; set; }

        public string Value { get; set; }

        public virtual void Parse() {}
    }

    public class AllClauses
    {
        public SelectClause Select { get; set; }
        
        public Clause Top { get; set; }
        
        public Clause Skip { get; set; }
        
        public Clause OrderBy { get; set; }
        
        public Clause OrderByDesc { get; set; }
        
        public Clause GroupBy { get; set; }
        
        public Clause First { get; set; }
        
        public Clause Last { get; set; }
        
        public Clause WrapWith { get; set; }
        
        public Clause Filter { get; set; }
        
        public Clause Expand { get; set; }
        
        public Clause Search { get; set; }
    }
}
