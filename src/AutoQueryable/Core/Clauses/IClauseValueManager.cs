using System;
using System.Collections.Generic;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public interface IClauseValueManager
    {
        bool First { get; set; }
        string GroupBy { get; set; }
        bool Last { get; set; }
        Dictionary<string, bool> OrderBy { get; set; }
        int? Page { get; set; }
        int? PageSize { get; set; }
        ICollection<SelectColumn> Select { get; set; }
        int? Skip { get; set; }
        int? Take { get; set; }
        int? Top { get; set; }
        IEnumerable<string> WrapWith { get; set; }
        void SetDefaults(Type type, IAutoQueryableProfile profile);
    }
}