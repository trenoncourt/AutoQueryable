using System;

namespace AutoQueryable.Core.CriteriaFilters
{
    public class QueryableFilter : IQueryableFilter
    {
        public QueryableFilter(string alias, int level, IFormatProvider formatProvider = null)
        {
            Alias = alias;
            Level = level;
            FormatProvider = formatProvider;
        }
        public string Alias { get; set; }
        /// <summary>
        /// Filters are applied from lowest level to highest level, so for example:
        /// Level 2: &lt;=
        /// Level 1: =, &lt;
        /// </summary>
        public int Level { get; set; }

        public IFormatProvider FormatProvider { get; set; }
    }
}