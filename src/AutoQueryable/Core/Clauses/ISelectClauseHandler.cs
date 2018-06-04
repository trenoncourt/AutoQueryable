using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public interface ISelectClauseHandler
    {
        ICollection<SelectColumn> Handle(string queryString);
    }

    public class SelectClauseHandler<TEntity, TAs> : ISelectClauseHandler where TEntity : class where TAs : class
    {
        private readonly IAutoQueryableProfile _profile;
        private readonly IAutoQueryableContext _context;
        private const string BasePropertiesSelector = "_"; 
        private const string AllPropertiesSelector = "*"; 
        private List<string> _rawSelection;
        private readonly ICollection<SelectColumn> _allSelectColumns = new List<SelectColumn>();
        private readonly ICollection<SelectColumn> _selectColumns = new List<SelectColumn>();

        public SelectClauseHandler(IAutoQueryableProfile profile)
        {
            _profile = profile;
        }
        public ICollection<SelectColumn> Handle(string queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return typeof(TEntity).GetSelectableColumns(_profile);
            }
            _rawSelection = GetRawSelection(queryString);
            ParseBasePropertiesSelection();
            ParseAllPropertiesSelection();
            
            var selectionWithColumnPath = GetRawColumnPath(_rawSelection);

            ParseColumns(selectionWithColumnPath);
            return _selectColumns;
        }

        
        /// <summary>
        /// Parse Select clause to get Select Columns tree
        /// </summary>
        public void Parse()
        {
            
        }

        private bool HasBasePropertiesSelector => _rawSelection.Any(s => s == BasePropertiesSelector);
        private bool HasAllPropertiesSelector => _rawSelection.Any(s => s == AllPropertiesSelector);

        private void ParseColumns(IEnumerable<string[]> selectionWithColumnPath)
        {
            foreach (var selectionColumnPath in selectionWithColumnPath)
            {
                SelectColumn parentColumn = new RootColumn(typeof(TEntity));
                for (var depth = 0; depth < selectionColumnPath.Length; depth++)
                {
                    if (IsGreaterThenMaxDepth(depth)) break;
                    
                    var columnName = selectionColumnPath[depth];
                    var property = parentColumn.Type.GetTypeOrGenericType().GetProperties().FirstOrDefault(x => x.Name.ToLowerInvariant() == columnName.ToLowerInvariant());
                    
                    if (property == null || IsGreaterThanMaxDepth(property, depth))
                        break;

                    var key = string.Join(".", selectionColumnPath.Take(depth + 1)).ToLowerInvariant();
                    var currentColumn = _allSelectColumns.FirstOrDefault(all => all.Key == key);
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
                        if (depth == 0) _selectColumns.Add(column);

                        column.ParentColumn = parentColumn;
                        parentColumn.SubColumns.Add(column);
                        parentColumn = column;
                    }
                }
            }
        }

        private void ProcessInclusingType(SelectColumn selectColumn)
        {
            var selectableColumns = selectColumn.GetRawSelection(_profile);
            foreach (var columnName in selectableColumns)
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
            return _profile.MaxDepth.HasValue && depth > _profile.MaxDepth.Value;
        }

        private bool IsGreaterThanMaxDepth(PropertyInfo property, int depth)
        {
            return IsGreaterThenMaxDepth(depth + 1) && (property.PropertyType.IsEnumerableButNotString() ||
                                                        property.PropertyType.IsCustomObjectType());
        }

        private bool IsUnselectableProperty(string key)
        {
            return _profile?.UnselectableProperties != null && _profile.UnselectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsNotSelectableProperty(string key)
        {
            return _profile?.SelectableProperties != null &&
                   !_profile.SelectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase);
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
            foreach (var selectionItem in _rawSelection)
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
                var selection = typeof(TEntity).GetRawSelection(_profile).Where(c => !_rawSelection.Contains(c, StringComparer.OrdinalIgnoreCase));
                _rawSelection.Remove(BasePropertiesSelector);
                AddBaseColumns(selection);
            }
        }

        private void ParseAllPropertiesSelection()
        {
            if (HasAllPropertiesSelector)
            {
                var selection = typeof(TEntity).GetRawSelection(_profile, SelectInclusingType.IncludeAllProperties).Where(c => !_rawSelection.Contains(c, StringComparer.OrdinalIgnoreCase));
                _rawSelection.Remove(AllPropertiesSelector);
                AddBaseColumns(selection);
            }
        }

        private void AddBaseColumns(IEnumerable<string> selection)
        {
            foreach (var columnName in selection)
            {
                var property = typeof(TEntity).GetTypeOrGenericType().GetProperties().FirstOrDefault(x =>
                    string.Equals(x.Name.ToLowerInvariant(), columnName.ToLowerInvariant(), StringComparison.Ordinal));

                if (property == null || IsGreaterThanMaxDepth(property, 0))
                    continue;

                var column = new SelectColumn(columnName, columnName, property.PropertyType);
                _allSelectColumns.Add(column);
                _selectColumns.Add(column);

            }
        }
    }
}