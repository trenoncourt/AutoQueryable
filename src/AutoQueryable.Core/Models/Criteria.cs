using System.Collections.Generic;
using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models
{
    public class Criteria
    { 
        public List<string> ColumnPath { get; set; } 
        public ConditionType ConditionType { get; set; }
        public dynamic[] Values { get; set; }
    }
}