using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Helpers;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Providers.OData
{
    public class ODataColumnProvider
    {
        IEnumerable<SelectColumn> GetSelectableColumns(Clauses clauses, AutoQueryableProfile profile, Type entityType)
        {
            IList<SelectColumn> selectColumns = new List<SelectColumn>();
            
            // If select clause is null, take all properties.
            if (clauses.Select == null)
            {
                IEnumerable<string> columns = EntityColumnHelper.GetSelectableColumns(profile, entityType);
                clauses.Select = new Clause { ClauseType = ClauseType.Select, Value = string.Join(",", columns.ToArray()) };
            }
            
            // Nothing to expand, return values directly.
            if (clauses.Expand == null)
            {
                foreach (string s in clauses.Select.Value.Split(','))
                {
                    PropertyInfo property = entityType.GetProperties().FirstOrDefault(x => string.Equals(x.Name, s, StringComparison.InvariantCultureIgnoreCase));
                    if (property == null) continue;
                    
                    selectColumns.Add(new SelectColumn
                    {
                        Key = s,
                        Name = s,
                        Type = property.PropertyType
                    });
                }
                return selectColumns;
            }

            return selectColumns;
        }
    }
}