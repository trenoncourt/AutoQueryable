using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Core.Extensions;

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
        public SelectColumn(string name, string key, Type type, List<SelectColumn> subColumns = null)
        {
            Name = name;
            Key = key;
            Type = type;
            SubColumns = subColumns ?? new List<SelectColumn>();
        }
        
        public SelectColumn(string name, string key, Type type, SelectColumn parentColumn, List<SelectColumn> subColumns = null)
        {
            Name = name;
            Key = key;
            Type = type;
            ParentColumn = parentColumn;
            SubColumns = subColumns ?? new List<SelectColumn>();
        }
        
        public string Name { get; set; }
        public string Key { get; set; }
        public Type Type;
        public SelectColumn ParentColumn { get; set; }
        public ICollection<SelectColumn> SubColumns { get; set; }
        public SelectInclusingType InclusionType { get; set; }


        public bool HasSubColumn => SubColumns != null && SubColumns.Any();



        public IEnumerable<string> GetRawSelection(IAutoQueryableProfile profile)
        {
            return Type.GetRawSelection(profile, InclusionType);
        }
    }
}