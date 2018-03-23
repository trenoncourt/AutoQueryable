using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;

namespace AutoQueryable.Core.Models.Clauses
{
    public class SelectClause : Clause
    {
        private const string BasePropertiesSelector = "_"; 
        private const string AllPropertiesSelector = "_"; 
        private List<string> _rawSelection;
        
        public SelectClause(AutoQueryableContext context) : base(context)
        {
            ClauseType = ClauseType.Select;
        }

        public bool HasBasePropertiesSelector => _rawSelection.Any(s => s == BasePropertiesSelector);
        public bool HasAllPropertiesSelector => _rawSelection.Any(s => s == AllPropertiesSelector);

        public override void Parse()
        {
            _rawSelection = GetRawSelection(Value);
            ParseBasePropertiesSelection();
            ParseAllPropertiesSelection();
            
            List<string[]> selectionWithColumnPath = GetRawColumnPath(_rawSelection);
            
            un(selectionWithColumnPath);
        }

        private void un(List<string[]> selectionWithColumnPath)
        {
            List<SelectColumn> allSelectColumns = new List<SelectColumn>();
            List<SelectColumn> selectColumns = new List<SelectColumn>();
            foreach (string[] selectionColumnPath in selectionWithColumnPath)
            {
                var parentType = Context.EntityType;
                int? maxDepth = Context.Profile?.MaxDepth;
                for (int i = 0; i < selectionColumnPath.Length; i++)
                {
                    if (maxDepth.HasValue && i >= maxDepth.Value)
                    {
                        break;
                    }
                    string key = string.Join(".", selectionColumnPath.Take(i + 1)).ToLowerInvariant();

                    var columnName = selectionColumnPath[i];
                    var property = parentType.GetProperties().FirstOrDefault(x => x.Name.ToLowerInvariant() == columnName.ToLowerInvariant());
                    if (property == null)
                    {
                        if (key.EndsWith(".*") && (!maxDepth.HasValue || (i < maxDepth - 1))) {
                            SelectColumn inclusionColumn= allSelectColumns.FirstOrDefault(all => all.Key == key.Replace(".*",""));
                            inclusionColumn.InclusionType = SelectInclusingType.IncludeAllProperties;
                        }
                        break;
                    }
                    bool isCollection = property.PropertyType.IsEnumerableButNotString();
                    // Max depth & collection or object
                    if (maxDepth.HasValue && (i >= maxDepth - 1) && (isCollection || property.PropertyType.IsCustomObjectType()))
                    {
                        continue;
                    }
                    if (isCollection)
                    {
                        parentType = property.PropertyType.GetGenericArguments().FirstOrDefault();
                    }
                    else
                    {
                        parentType = property.PropertyType;
                    }
                    SelectColumn column = allSelectColumns.FirstOrDefault(all => all.Key == key);
                    if (column == null)
                    {
                        // pass non selectable properties
                        if (profile?.SelectableProperties != null && !profile.SelectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        // pass unselectable properties
                        if (profile?.UnselectableProperties != null && profile.UnselectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        column = new SelectColumn
                        {
                            Key = key,
                            Name = columnName,
                            SubColumns = new List<SelectColumn>(),
                            Type = property.PropertyType
                        };
                        allSelectColumns.Add(column);
                        if (i == 0)
                        {
                            selectColumns.Add(column);
                        }
                        else
                        {
                            string parentKey = string.Join(".", selectionColumnPath.Take(i)).ToLowerInvariant();
                            SelectColumn parentColumn = allSelectColumns.FirstOrDefault(all => all.Key == parentKey);
                            if (selection.Contains(parentKey + ".*", StringComparer.OrdinalIgnoreCase))
                            {
                                parentColumn.InclusionType = SelectInclusingType.IncludeAllProperties;
                            }
                            else if (selection.Contains(parentKey, StringComparer.OrdinalIgnoreCase))
                            {
                                parentColumn.InclusionType = SelectInclusingType.IncludeBaseProperties;
                            }

                            column.ParentColumn = parentColumn;
                            parentColumn.SubColumns.Add(column);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get raw selection from select clause value
        /// </summary>
        /// <example>
        /// "id,name,color,category.name" => ["id", "name", "color", "category.name"]
        /// </example>
        private List<string> GetRawSelection(string selectClauseValue)
        {
            return selectClauseValue.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                .ToList();
        }

        /// <summary>
        /// Get raw column path from selection
        /// </summary>
        /// <example>
        /// ["category.product.name", "color"] => ["category", "product", "name"], ["color"]
        /// </example>
        private List<string[]> GetRawColumnPath(List<string> selection)
        {
            var selectionWithColumnPath = new List<string[]>();
            foreach (string selectionItem in _rawSelection)
            {
                var columnPath = GetRawColumnPath(selectionItem);
                selectionWithColumnPath.Add(columnPath);
            }
            return selectionWithColumnPath;
        }

        /// <summary>
        /// Get raw column path from selection item
        /// </summary>
        /// <example>
        /// "category.product.name" => ["category", "product", "name"]
        /// </example>
        private string[] GetRawColumnPath(string selectionItem)
        {
            return selectionItem.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim()).ToArray();
        }

        private void ParseBasePropertiesSelection()
        {
            if (HasBasePropertiesSelector)
            {
                _rawSelection.Remove(BasePropertiesSelector);
                _rawSelection.AddRange(Context.EntityType.GetRawSelection(Context.Profile).Where(c => !_rawSelection.Contains(c, StringComparer.OrdinalIgnoreCase)));
            }
        }

        private void ParseAllPropertiesSelection()
        {
            if (HasAllPropertiesSelector)
            {
                _rawSelection.Remove(AllPropertiesSelector);
                _rawSelection.AddRange(Context.EntityType.GetRawSelection(Context.Profile, SelectInclusingType.IncludeAllProperties).Where(c => !_rawSelection.Contains(c, StringComparer.OrdinalIgnoreCase)));
            }
        }
    }
}