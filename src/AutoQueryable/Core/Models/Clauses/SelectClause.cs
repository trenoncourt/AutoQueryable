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
        private readonly ICollection<SelectColumn> _allSelectColumns = new List<SelectColumn>();

        public ICollection<SelectColumn> SelectColumns { get; }

        public SelectClause(AutoQueryableContext context) : base(context)
        {
            ClauseType = ClauseType.Select;
            SelectColumns = new List<SelectColumn>();
        }
        
        /// <summary>
        /// Parse Select clause to get Select Columns tree
        /// </summary>
        public override void Parse()
        {
            _rawSelection = GetRawSelection(Value);
            ParseBasePropertiesSelection();
            ParseAllPropertiesSelection();
            
            List<string[]> selectionWithColumnPath = GetRawColumnPath(_rawSelection);
            
            ParseColumns(selectionWithColumnPath);
        }

        private bool HasBasePropertiesSelector => _rawSelection.Any(s => s == BasePropertiesSelector);
        private bool HasAllPropertiesSelector => _rawSelection.Any(s => s == AllPropertiesSelector);

        private void ParseColumns(List<string[]> selectionWithColumnPath)
        {
            foreach (string[] selectionColumnPath in selectionWithColumnPath)
            {
                SelectColumn parentColumn = new RootColumn(Context.EntityType);
                for (int depth = 0; depth < selectionColumnPath.Length; depth++)
                {
                    if (IsGreaterThenMaxDepth(depth)) break;
                    
                    string columnName = selectionColumnPath[depth];
                    PropertyInfo property = parentColumn.Type.GetTypeOrGenericType().GetProperties().FirstOrDefault(x => x.Name.ToLowerInvariant() == columnName.ToLowerInvariant());
                    
                    if (property == null || IsGreaterThanMaxDepth(property, depth))
                        break;

                    string key = string.Join(".", selectionColumnPath.Take(depth + 1)).ToLowerInvariant();
                    SelectColumn currentColumn = _allSelectColumns.FirstOrDefault(all => all.Key == key);
                    if (currentColumn != null)
                    {
                        parentColumn = currentColumn;
                    }
                    else
                    {
                        // pass non selectable & unselectable properties
                        if (IsNotSelectableProperty(key) || IsUnselectableProperty(key))
                            continue;

                        var column = new SelectColumn(columnName, key, property.PropertyType);

                        if (_rawSelection.Contains(column.Key + ".*", StringComparer.OrdinalIgnoreCase))
                        {
                            column.InclusionType = SelectInclusingType.IncludeAllProperties;
                            ProcessInclusingType(column);
                        }
                        else if (property.PropertyType.IsCustomObjectType() && _rawSelection.Contains(column.Key, StringComparer.OrdinalIgnoreCase))
                        {
                            column.InclusionType = SelectInclusingType.IncludeBaseProperties;
                            ProcessInclusingType(column);
                        }
                        _allSelectColumns.Add(column);
                        if (depth == 0)
                            SelectColumns.Add(column);

                        column.ParentColumn = parentColumn;
                        parentColumn.SubColumns.Add(column);
                        parentColumn = column;
                    }
                }
            }
        }

        private void ProcessInclusingType(SelectColumn selectColumn)
        {
            IEnumerable<string> selectableColumns = selectColumn.GetRawSelection(Context.Profile);
            foreach (string columnName in selectableColumns)
            {
                if (!selectColumn.SubColumns.Any(x => x.Name.Equals(columnName, StringComparison.OrdinalIgnoreCase)))
                {
                    var subColumnKey = (selectColumn.Key + "." + columnName).ToLowerInvariant();

                    var type = selectColumn.Type;
                    if (selectColumn.Type.IsEnumerable())
                    {
                        type = selectColumn.Type.GetGenericArguments().FirstOrDefault();
                    }

                    var column = new SelectColumn(columnName, subColumnKey,
                        type.GetProperties().Single(x => x.Name == columnName).PropertyType, selectColumn);
                    selectColumn.SubColumns.Add(column);
                    _allSelectColumns.Add(column);
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
                IEnumerable<string> selection = Context.EntityType.GetRawSelection(Context.Profile).Where(c => !_rawSelection.Contains(c, StringComparer.OrdinalIgnoreCase));
                _rawSelection.Remove(BasePropertiesSelector);
                AddBaseColumns(selection);
            }
        }

        private void ParseAllPropertiesSelection()
        {
            if (HasAllPropertiesSelector)
            {
                IEnumerable<string> selection = Context.EntityType.GetRawSelection(Context.Profile, SelectInclusingType.IncludeAllProperties).Where(c => !_rawSelection.Contains(c, StringComparer.OrdinalIgnoreCase));
                _rawSelection.Remove(AllPropertiesSelector);
                AddBaseColumns(selection);
            }
        }

        private void AddBaseColumns(IEnumerable<string> selection)
        {
            foreach (var columnName in selection)
            {
                PropertyInfo property = Context.EntityType.GetTypeOrGenericType().GetProperties().FirstOrDefault(x => x.Name.ToLowerInvariant() == columnName.ToLowerInvariant());

                if (property == null || IsGreaterThanMaxDepth(property, 0))
                    continue;

                var column = new SelectColumn(columnName, columnName, property.PropertyType);
                _allSelectColumns.Add(column);
                SelectColumns.Add(column);

            }
        }
    }
}