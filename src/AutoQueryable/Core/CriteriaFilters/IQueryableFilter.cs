using System;

namespace AutoQueryable.Core.CriteriaFilters
{
    public interface IQueryableFilter
    {
        string Alias { get; set; }
        int Level { get; set; }
        IFormatProvider FormatProvider { get; set; }
    }
}