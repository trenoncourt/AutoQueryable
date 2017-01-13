using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Models;

namespace AutoQueryable.Helpers
{
    public static class IncludeHelper
    {
        public static IEnumerable<string> GetIncludableColumns(Clause includeClause, string[] unselectableProperties, Type entityType)
        {
            IEnumerable<string> columns = includeClause.Value.Split(',');
            if (unselectableProperties != null)
            {
                columns = columns.Where(c => !unselectableProperties.Contains(c, StringComparer.OrdinalIgnoreCase));
            }
            return columns;
        }
    }
}