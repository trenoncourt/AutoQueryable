using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoQueryable.Core.Models
{
    public enum SelectInclusingType
    {
        Default = 0,
        IncludeBaseProperties = 1,
        IncludeAllProperties = 2
    }
    public class SelectColumn
    {
        public string Key { get; set; }
        public Type Type;
        public SelectInclusingType InclusionType { get; set; }

        public string Name { get; set; }

        public bool HasSubColumn => SubColumns.Any();

        public ICollection<SelectColumn> SubColumns { get; set; }

        public SelectColumn ParentColumn { get; set; }
    }
}