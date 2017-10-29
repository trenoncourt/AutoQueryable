using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Helpers;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Providers;

namespace AutoQueryable.Providers.OData
{
    public class ODataColumnProvider : IColumnProvider
    {
        public IEnumerable<SelectColumn> GetSelectableColumns(Clauses clauses, AutoQueryableProfile profile, Type entityType)
        {
            List<SelectColumn> selectColumns = new List<SelectColumn>();
            
            // If select clause is null, take all properties.
            if (clauses.Select == null)
            {
                IEnumerable<SelectColumn> entityColumns = EntityColumnHelper.GetSelectableColumns(profile, entityType);
                selectColumns.AddRange(entityColumns);
            }
            else
            {
                string[] uriPropertyNames = clauses.Select.Value.Split(',');
                foreach (string n in uriPropertyNames)
                {
                    // remove properties who does not exist on the entity type
                    PropertyInfo property = entityType.GetProperties().FirstOrDefault(x =>
                        string.Equals(x.Name, n, StringComparison.OrdinalIgnoreCase));
                    if (property == null) continue;

                    // Remove non selectable properties.
                    if (profile?.SelectableProperties != null)
                    {
                        if (!profile.SelectableProperties.Contains(n, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }

                    // Remove unselectable properties.
                    if (profile?.UnselectableProperties != null)
                    {
                        if (profile.UnselectableProperties.Contains(n, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                    }
                    selectColumns.Add(new SelectColumn
                    {
                        Key = n,
                        Name = n,
                        Type = property.PropertyType
                    });
                }
            }
            
            // Nothing to expand, return values directly.
            if (clauses.Expand == null)
            {
                //foreach (string s in clauses.Expand.Value.Split(','))
                //{
                //    PropertyInfo property = entityType.GetProperties().FirstOrDefault(x => string.Equals(x.Name, s, StringComparison.OrdinalIgnoreCase));
                //    if (property == null) continue;
                    
                //    selectColumns.Add(new SelectColumn
                //    {
                //        Key = s,
                //        Name = s,
                //        Type = property.PropertyType
                //    });
                //}
                return selectColumns;
            }

            return selectColumns;
        }
    }
}