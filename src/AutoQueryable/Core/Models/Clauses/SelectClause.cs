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
        private const string AllPropertiesSelector = "*"; 
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
                SelectColumn ParentColumn = null;
                for (int depth = 0; depth < selectionColumnPath.Length; depth++)
                {
                    if (IsGreaterThenMaxDepth(depth)) break;
                    
                    string columnName = selectionColumnPath[depth];
                    PropertyInfo property = parentType.GetProperty(columnName, BindingFlags.IgnoreCase);
                    
                    string key = string.Join(".", selectionColumnPath.Take(depth + 1)).ToLowerInvariant();
                    if (property == null)
                    {
                        if (CanIncludeAll(key, depth)) {
                            if (ParentColumn != null)
                                ParentColumn.InclusionType = SelectInclusingType.IncludeAllProperties;
                        }
                        break;
                    }
                    // Max depth & collection or object
                    if (IsGreaterThanMaxDepth(property, depth))
                        continue;
                    parentType = property.PropertyType.GetParentType();
                    
                    if (allSelectColumns.Any(all => all.Key == key))
                    {
                        // pass non selectable & unselectable properties
                        if (IsNotSelectableProperty(key) || IsUnselectableProperty(key))
                            continue;

                        var column = new SelectColumn(columnName, key, property.PropertyType);
                        
                        allSelectColumns.Add(column);
                        if (depth == 0)
                        {
                            selectColumns.Add(column);
                        }
                        else
                        {
                            string parentKey = string.Join(".", selectionColumnPath.Take(depth)).ToLowerInvariant();
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

                        ParentColumn = column;
                    }
                }
            }
        }

        private bool IsGreaterThenMaxDepth(int depth)
        {
            return Context.Profile.MaxDepth.HasValue && depth > Context.Profile.MaxDepth.Value;
        }

        private bool IsGreaterThanMaxDepth(PropertyInfo property, int depth)
        {
            return IsGreaterThenMaxDepth(depth + 1) && (property.PropertyType.IsEnumerableButNotString() ||
                                                        property.PropertyType.IsCustomObjectType());
        }

        private bool IsUnselectableProperty(string key)
        {
            return Context.Profile?.UnselectableProperties != null &&
                   Context.Profile.UnselectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsNotSelectableProperty(string key)
        {
            return Context.Profile?.SelectableProperties != null &&
                   !Context.Profile.SelectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        public bool CanIncludeAll(string key, int depth)
        {
            return key.EndsWith($".{AllPropertiesSelector}") && !IsGreaterThenMaxDepth(depth + 1);
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