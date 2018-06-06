using System.Collections.Generic;
using AutoQueryable.Core.CriteriaFilters;

namespace AutoQueryable.Core.Models
{
    public class Criteria
    { 
        public List<string> ColumnPath { get; set; } 
        public IQueryableFilter Filter { get; set; }
        public string[] Values { get; set; }
    }
}