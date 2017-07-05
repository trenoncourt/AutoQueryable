using System.Collections.Generic;

namespace AutoQueryable.Models
{
    public class Criteria
    { 
        public List<string> ColumnPath { get; set; } 
        public ConditionType ConditionType { get; set; }
        public dynamic[] Values { get; set; }
    }
}