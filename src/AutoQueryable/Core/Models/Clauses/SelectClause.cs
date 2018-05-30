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

        public ICollection<SelectColumn> SelectColumns { get; private set; }

        public SelectClause(AutoQueryableContext context) : base(context)
        {
            this.ClauseType = ClauseType.Select;
            this.SelectColumns = new List<SelectColumn>();
        }
        
        /// <summary>
        /// Parse Select clause to get Select Columns tree
        /// </summary>
        public void Parse()
        {
            if (string.IsNullOrEmpty(Value))
            {
                this.SelectColumns = this.Context.EntityType.GetSelectableColumns(this.Context.Profile);
                return;
            }
            this._rawSelection = this.GetRawSelection(this.Value);
            this.ParseBasePropertiesSelection();
            this.ParseAllPropertiesSelection();
            
            var selectionWithColumnPath = this.GetRawColumnPath(this._rawSelection);

            this.ParseColumns(selectionWithColumnPath);
        }

        private bool HasBasePropertiesSelector => this._rawSelection.Any(s => s == BasePropertiesSelector);
        private bool HasAllPropertiesSelector => this._rawSelection.Any(s => s == AllPropertiesSelector);

        private void ParseColumns(List<string[]> selectionWithColumnPath)
        {
            foreach (var selectionColumnPath in selectionWithColumnPath)
            {
                SelectColumn parentColumn = new RootColumn(this.Context.EntityType);
                for (var depth = 0; depth < selectionColumnPath.Length; depth++)
                {
                    if (this.IsGreaterThenMaxDepth(depth)) break;
                    
                    var columnName = selectionColumnPath[depth];
                    var property = parentColumn.Type.GetTypeOrGenericType().GetProperties().FirstOrDefault(x => x.Name.ToLowerInvariant() == columnName.ToLowerInvariant());
                    
                    if (property == null || this.IsGreaterThanMaxDepth(property, depth))
                        break;

                    var key = string.Join(".", selectionColumnPath.Take(depth + 1)).ToLowerInvariant();
                    var currentColumn = this._allSelectColumns.FirstOrDefault(all => all.Key == key);
                    if (currentColumn != null)
                    {
                        parentColumn = currentColumn;
                    }
                    else
                    {
                        // pass non selectable & unselectable properties
                        if (this.IsNotSelectableProperty(key) || this.IsUnselectableProperty(key))
                            continue;

                        var column = new SelectColumn(columnName, key, property.PropertyType);

                        if (this._rawSelection.Contains(column.Key + ".*", StringComparer.OrdinalIgnoreCase))
                        {
                            column.InclusionType = SelectInclusingType.IncludeAllProperties;
                            this.ProcessInclusingType(column);
                        }
                        else if (property.PropertyType.IsCustomObjectType() && this._rawSelection.Contains(column.Key, StringComparer.OrdinalIgnoreCase))
                        {
                            column.InclusionType = SelectInclusingType.IncludeBaseProperties;
                            this.ProcessInclusingType(column);
                        }

                        this._allSelectColumns.Add(column);
                        if (depth == 0) this.SelectColumns.Add(column);

                        column.ParentColumn = parentColumn;
                        parentColumn.SubColumns.Add(column);
                        parentColumn = column;
                    }
                }
            }
        }

        private void ProcessInclusingType(SelectColumn selectColumn)
        {
            var selectableColumns = selectColumn.GetRawSelection(this.Context.Profile);
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
                    this._allSelectColumns.Add(column);
                }

            }
        }

        private bool IsGreaterThenMaxDepth(int depth)
        {
            return this.Context.Profile.MaxDepth.HasValue && depth > this.Context.Profile.MaxDepth.Value;
        }

        private bool IsGreaterThanMaxDepth(PropertyInfo property, int depth)
        {
            return this.IsGreaterThenMaxDepth(depth + 1) && (property.PropertyType.IsEnumerableButNotString() ||
                                                        property.PropertyType.IsCustomObjectType());
        }

        private bool IsUnselectableProperty(string key)
        {
            return this.Context.Profile?.UnselectableProperties != null && this.Context.Profile.UnselectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        private bool IsNotSelectableProperty(string key)
        {
            return this.Context.Profile?.SelectableProperties != null &&
                   !this.Context.Profile.SelectableProperties.Contains(key, StringComparer.OrdinalIgnoreCase);
        }

        public bool CanIncludeAll(string key, int depth)
        {
            return key.EndsWith($".{AllPropertiesSelector}") && !this.IsGreaterThenMaxDepth(depth + 1);
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
            foreach (var selectionItem in this._rawSelection)
            {
                var columnPath = this.GetRawColumnPath(selectionItem);
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
            if (this.HasBasePropertiesSelector)
            {
                var selection = this.Context.EntityType.GetRawSelection(this.Context.Profile).Where(c => !this._rawSelection.Contains(c, StringComparer.OrdinalIgnoreCase));
                this._rawSelection.Remove(BasePropertiesSelector);
                this.AddBaseColumns(selection);
            }
        }

        private void ParseAllPropertiesSelection()
        {
            if (this.HasAllPropertiesSelector)
            {
                var selection = this.Context.EntityType.GetRawSelection(this.Context.Profile, SelectInclusingType.IncludeAllProperties).Where(c => !this._rawSelection.Contains(c, StringComparer.OrdinalIgnoreCase));
                this._rawSelection.Remove(AllPropertiesSelector);
                this.AddBaseColumns(selection);
            }
        }

        private void AddBaseColumns(IEnumerable<string> selection)
        {
            foreach (var columnName in selection)
            {
                var property = this.Context.EntityType.GetTypeOrGenericType().GetProperties().FirstOrDefault(x => x.Name.ToLowerInvariant() == columnName.ToLowerInvariant());

                if (property == null || this.IsGreaterThanMaxDepth(property, 0))
                    continue;

                var column = new SelectColumn(columnName, columnName, property.PropertyType);
                this._allSelectColumns.Add(column);
                this.SelectColumns.Add(column);

            }
        }
    }
}