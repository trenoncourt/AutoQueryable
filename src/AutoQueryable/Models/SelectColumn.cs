using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoQueryable.Models
{
    public class SelectColumn
    {
        public string Key { get; set; }
        public Type Type;
        public bool IncludeBaseProperties { get; set; }

        public string Name { get; set; }

        public bool HasSubColumn => SubColumns.Any();

        public ICollection<SelectColumn> SubColumns { get; set; }

        public SelectColumn ParentColumn { get; set; }
    }
}