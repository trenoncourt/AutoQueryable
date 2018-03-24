using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.Clauses;
using AutoQueryable.Core.Providers;

namespace AutoQueryable.Providers.Default
{
//    public class DefaultColumnProvider : IColumnProvider
//    {
//        public IEnumerable<SelectColumn> GetSelectableColumns(AllClauses clauses, AutoQueryableProfile profile, Type entityType)
//        {
//            if (clauses.Select == null)
//            {
//                IEnumerable<string> columns = GetSelectableColumns(profile, entityType);
//                clauses.Select = new SelectClause { Value = string.Join(",", columns.ToArray()) };
//            }
//            List<SelectColumn> allSelectColumns = new List<SelectColumn>();
//            List<SelectColumn> selectColumns = new List<SelectColumn>();
//            var selection = clauses.Select.Value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToList();
//            var basePropertySelection = selection.Find(s => s == "_");
//            if (basePropertySelection != null)
//            {
//                selection.Remove(basePropertySelection);
//                selection.AddRange(GetSelectableColumns(profile, entityType).Where(c => !selection.Contains(c, StringComparer.OrdinalIgnoreCase)));
//            }
//            var allPropertySelection = selection.Find(s => s == "*");
//            if (allPropertySelection != null)
//            {
//                selection.Remove(basePropertySelection);
//                selection.AddRange(GetSelectableColumns(profile, entityType, SelectInclusingType.IncludeAllProperties).Where(c => !selection.Contains(c, StringComparer.OrdinalIgnoreCase)));
//            }
//
//            var selectionWithColumnPath = new List<string[]>();
//            foreach (string selectionItem in selection)
//            {
//                var columnPath = selectionItem.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
//                selectionWithColumnPath.Add(columnPath);
//
//            }
//            foreach (string[] selectionColumnPath in selectionWithColumnPath)
//            {
//                var parentType = entityType;
//                int? maxDepth = profile?.MaxDepth;
//                
//                for (int i = 0; i < selectionColumnPath.Length; i++)
//                {
//                    if (maxDepth.HasValue && i >= maxDepth.Value)
//                    {
//                        break;
//                    }
//                    string key = string.Join(".", selectionColumnPath.Take(i + 1)).ToLowerInvariant();
//
//                    var columnName = selectionColumnPath[i];
//                    var property = parentType.GetProperties().FirstOrDefault(x => x.Name.ToLowerInvariant() == columnName.ToLowerInvariant());
//                    if (property == null)
//                    {
//                        if (key.EndsWith(".*") && (!maxDepth.HasValue || (i < maxDepth - 1))) {
//                            var inclusionColumn= allSelectColumns.FirstOrDefault(all => all.Key == key.Replace(".*",""));
//                            inclusionColumn.InclusionType = SelectInclusingType.IncludeAllProperties;
//                        }
//                        break;
//                    }
//                    bool isCollection = property.PropertyType.IsEnumerableButNotString();
//                    // Max depth & collection or object
//                    if (maxDepth.HasValue && (i >= maxDepth - 1) && (isCollection || property.PropertyType.IsCustomObjectType()))
//                    {
//                        continue;
//                    }
//                    if (isCollection)
//                    {
//                        parentType = property.PropertyType.GetGenericArguments().FirstOrDefault();
//                    }
//                    else
//                    {
//                        parentType = property.PropertyType;
//                    }
//                    SelectColumn column = allSelectColumns.FirstOrDefault(all => all.Key == key);
//                    if (column == null)
//                    {
//                        // pass non selectable properties
//                        if (profile?.SelectableProperties != null && !profile.SelectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase))
//                        {
//                            continue;
//                        }
//                        // pass unselectable properties
//                        if (profile?.UnselectableProperties != null && profile.UnselectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase))
//                        {
//                            continue;
//                        }
//                        column = new SelectColumn
//                        {
//                            Key = key,
//                            Name = columnName,
//                            SubColumns = new List<SelectColumn>(),
//                            Type = property.PropertyType
//                        };
//                        allSelectColumns.Add(column);
//                        if (i == 0)
//                        {
//                            selectColumns.Add(column);
//                        }
//                        else
//                        {
//                            string parentKey = string.Join(".", selectionColumnPath.Take(i)).ToLowerInvariant();
//                            SelectColumn parentColumn = allSelectColumns.FirstOrDefault(all => all.Key == parentKey);
//                            if (selection.Contains(parentKey + ".*", StringComparer.OrdinalIgnoreCase))
//                            {
//                                parentColumn.InclusionType = SelectInclusingType.IncludeAllProperties;
//                            }
//                            else if (selection.Contains(parentKey, StringComparer.OrdinalIgnoreCase))
//                            {
//                                parentColumn.InclusionType = SelectInclusingType.IncludeBaseProperties;
//                            }
//
//                            column.ParentColumn = parentColumn;
//                            parentColumn.SubColumns.Add(column);
//                        }
//                    }
//                }
//            }
//            ProcessInclusingType(profile, selectColumns);
//
//            return selectColumns;
//        }
//        
//        private IEnumerable<string> GetSelectableColumns(AutoQueryableProfile profile, Type entityType, SelectInclusingType selectInclusingType = SelectInclusingType.IncludeBaseProperties)
//        {
//            IEnumerable<string> columns = null;
//            bool isCollection = entityType.IsEnumerable();
//            var type = entityType;
//            if (isCollection)
//            {
//                type = entityType.GetGenericArguments().FirstOrDefault();
//            } 
//            // Get all properties without navigation properties.
//            if (selectInclusingType == SelectInclusingType.IncludeBaseProperties)
//            {
//                columns = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                    .Where(p =>
//                    (p.PropertyType.GetTypeInfo().IsGenericType && p.PropertyType.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>))
//                    || (!p.PropertyType.GetTypeInfo().IsClass && !p.PropertyType.GetTypeInfo().IsGenericType)
//                    || p.PropertyType.GetTypeInfo().IsArray
//                    || p.PropertyType == typeof(string)
//                    )
//                    .Select(p => p.Name);
//            }
//            // Get all properties.
//            else if (selectInclusingType == SelectInclusingType.IncludeAllProperties)
//            {
//                columns = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
//                   .Select(p => p.Name);
//            }
//
//            // Remove non selectable properties.
//            if (profile?.SelectableProperties != null)
//            {
//                columns = columns?.Where(c => profile.SelectableProperties.Contains(c, StringComparer.OrdinalIgnoreCase));
//            }
//            
//            // Remove unselectable properties.
//            if (profile?.UnselectableProperties != null)
//            {
//                columns = columns?.Where(c => !profile.UnselectableProperties.Contains(c, StringComparer.OrdinalIgnoreCase));
//            }
//            
//            return columns?.ToList();
//        }
//
//        private void ProcessInclusingType(AutoQueryableProfile profile, ICollection<SelectColumn> selectColumns)
//        {
//            foreach (var selectColumn in selectColumns)
//            {
//                if (selectColumn.InclusionType != SelectInclusingType.Default)
//                {
//                    var selectableColumns = GetSelectableColumns(profile, selectColumn.Type, selectColumn.InclusionType);
//                    foreach (var columnName in selectableColumns)
//                    {
//                        if (!selectColumn.SubColumns.Any(x => x.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase)))
//                        {
//                            var subColumnKey = selectColumn.Key + "." + columnName;
//                            // pass non selectable properties.
//                            if (profile?.SelectableProperties != null &&
//                                !profile.SelectableProperties.Contains(subColumnKey, StringComparer.OrdinalIgnoreCase))
//                            {
//                                continue;
//                            }
//                            // pass unselectable properties.
//                            if (profile?.UnselectableProperties != null &&
//                                profile.UnselectableProperties.Contains(subColumnKey, StringComparer.OrdinalIgnoreCase))
//                            {
//                                continue;
//                            }
//                            var type = selectColumn.Type;
//                            if (selectColumn.Type.IsEnumerable())
//                            {
//                                type = selectColumn.Type.GetGenericArguments().FirstOrDefault();
//                            }
//                            var column = new SelectColumn
//                            {
//                                Key = subColumnKey,
//                                Name = columnName,
//                                SubColumns = new List<SelectColumn>(),
//                                Type = type.GetProperties().Single(x => x.Name == columnName).PropertyType,
//                                ParentColumn = selectColumn
//                            };
//                            selectColumn.SubColumns.Add(column);
//                        }
//
//                    }
//                }
//                ProcessInclusingType(profile, selectColumn.SubColumns);
//            }
//        }
//    }
}