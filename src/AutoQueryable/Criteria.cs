using System.Collections.Generic;
using System.Data.Common;

namespace AutoQueryable
{
    public class Criteria
    {
        public string Column { get; set; }
        public Condition Condition { get; set; }
        public IEnumerable<DbParameter> DbParameters { get; set; }
    }
}