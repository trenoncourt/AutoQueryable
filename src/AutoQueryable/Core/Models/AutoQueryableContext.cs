using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoQueryable.Core.Aliases;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models.Clauses;
using AutoQueryable.Helpers;
using Serilog;

namespace AutoQueryable.Core.Models
{
    public abstract class AutoQueryableContext
    {
        private string[] _queryStringParts;
        
        public AutoQueryableProfile Profile { get; private set; }
        public string QueryString { get; private set; }
        public Type EntityType { get; private set; }
        public AllClauses Clauses { get; set; }
        
        public string[] QueryStringParts => this._queryStringParts ?? (this._queryStringParts = this.QueryString.Replace("?", "").Split('&'));

        public abstract dynamic GetAutoQuery();

        /// <summary>
        /// Create AutoQueryable context typed with default entity type
        /// </summary>
        public static AutoQueryableContext Create<TEntity>(IQueryable<TEntity> query, string queryString,
            AutoQueryableProfile profile) where TEntity : class 
        {
            var entityType = query.GetType().GenericTypeArguments[0];
            
            return new AutoQueryableContext<TEntity>
            {
                Query = query,
                QueryString = Uri.UnescapeDataString(queryString ?? ""),
                EntityType = entityType,
                Profile = profile
            };
        }
    }

    public class AutoQueryableContext<TEntity> : AutoQueryableContext where TEntity : class 
    {
        public IQueryable<TEntity> Query { get; set; }

        public override dynamic GetAutoQuery()
        {
            // No query string, get only selectable columns
            if (string.IsNullOrEmpty(this.QueryString))
            {
                return this.GetDefaultSelectableQuery();
            }

            this.Clauses = this.GetClauses(this.QueryStringParts);
            var criterias = this.Profile.IsClauseAllowed(ClauseType.Filter) ? this.GetCriterias().ToList() : null;
            
            var queryResult = QueryBuilder.Build(this.Query, this.EntityType, this.Clauses, criterias, this.Profile, this.Clauses.WrapWith != null && this.Clauses.WrapWith.CountAllRows);
            if (this.Clauses.WrapWith == null || !this.Clauses.WrapWith.Any)
            {
                return queryResult.Result;
            }

            return this.Clauses.WrapWith.GetWrappedResult(queryResult);
        }
        
        public AllClauses GetClauses(string[] queryStringParts)
        {
            var clauses = new AllClauses();
            foreach (var q in queryStringParts)
            {
                if (q.Contains(ClauseAlias.Select, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.Select))
                {
                    clauses.Select = new SelectClause(this) { Value = GetOperandValue(q, ClauseAlias.Select)};
                    clauses.Select.Parse();
                }
                else if (q.Contains(ClauseAlias.Top, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.Top))
                {
                    clauses.Top = new TopClause(this) { Value = GetOperandValue(q, ClauseAlias.Top)};
                }
                else if (q.Contains(ClauseAlias.Take, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.Top))
                {
                    clauses.Top = new TopClause(this) { Value = GetOperandValue(q, ClauseAlias.Take)};
                }
                else if (q.Contains(ClauseAlias.PageSize, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.Top))
                {
                    clauses.Top = new TopClause(this) { Value = GetOperandValue(q, ClauseAlias.PageSize) };
                }
                else if (q.Contains(ClauseAlias.Skip, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.Skip))
                {
                    clauses.Skip = new SkipClause(this) { Value = GetOperandValue(q, ClauseAlias.Skip)};
                }
                else if (q.Contains(ClauseAlias.First, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.First))
                {
                    clauses.First = new FirstClause(this);
                }
                else if (q.Contains(ClauseAlias.Last, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.Last))
                {
                    clauses.Last = new LastClause(this);
                }
                else if (q.Contains(ClauseAlias.OrderBy, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.OrderBy))
                {
                    clauses.OrderBy = new OrderByClause(this) { Value = GetOperandValue(q, ClauseAlias.OrderBy)};
                }
                else if (q.Contains(ClauseAlias.OrderByDesc, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.OrderByDesc))
                {
                    clauses.OrderByDesc = new OrderByDescClause(this) { Value = GetOperandValue(q, ClauseAlias.OrderByDesc)};
                }
                else if (q.Contains(ClauseAlias.WrapWith, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.WrapWith))
                {
                    clauses.WrapWith = new WrapWithClause(this) { Value = GetOperandValue(q, ClauseAlias.WrapWith)};
                    clauses.WrapWith.Parse();
                }
                else if (q.Contains(ClauseAlias.Page, StringComparison.OrdinalIgnoreCase) && this.Profile.IsClauseAllowed(ClauseType.Page))
                {
                    clauses.Page = new PageClause(this) { Value = GetOperandValue(q, ClauseAlias.Page) };
                }
            }

            if (clauses.Page != null)
            {
                //this.Logger.Information("Overwriting 'skip' clause value because 'page' is set");
                // Calculate skip from page if page query param was set
                var page = int.Parse(clauses.Page.Value);
                var take = clauses.Top != null ? int.Parse(clauses.Top.Value) : this.Profile.DefaultToTake;
                clauses.Skip = new SkipClause(this){ Value = (page*take).ToString()};
            }

            if (clauses.OrderBy == null && clauses.OrderByDesc == null && !string.IsNullOrEmpty(this.Profile.DefaultOrderBy))
            {
                clauses.OrderBy = new OrderByClause(this) { Value = this.Profile.DefaultOrderBy };
            }

            if (clauses.OrderBy == null && clauses.OrderByDesc == null && !string.IsNullOrEmpty(this.Profile.DefaultOrderByDesc))
            {
                clauses.OrderByDesc = new OrderByDescClause(this) { Value = this.Profile.DefaultOrderByDesc };
            }

            return clauses;
        }
        
        public IEnumerable<Criteria> GetCriterias()
        {
            CriteriaFilterManager.InitializeFilterMap();

            foreach (var qPart in this.QueryStringParts)
            {
                var q = WebUtility.UrlDecode(qPart);
                var criteria = GetCriteria(q);

                if (criteria != null)
                {
                    yield return criteria;
                }
            }
        }

        private Criteria GetCriteria(string q)
        {

            var filter = CriteriaFilterManager.FindFilter(q);
            if (filter == null)
            {
                return null;
            }

            var operands = Regex.Split(q, filter.Alias, RegexOptions.IgnoreCase);

            PropertyInfo property = null;
            var columnPath = new List<string>();
            var columns = operands[0].Split('.');
            foreach (var column in columns)
            {
                if (property == null)
                {
                    property = this.EntityType.GetProperties().FirstOrDefault(p => p.Name.Equals(column, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    var isCollection = property.PropertyType.GetInterfaces().Any(x => x.Name == "IEnumerable");
                    if (isCollection)
                    {
                        var childType = property.PropertyType.GetGenericArguments()[0];
                        property = childType.GetProperties().FirstOrDefault(p => p.Name.Equals(column, StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        property = property.PropertyType.GetProperties().FirstOrDefault(p => p.Name.Equals(column, StringComparison.OrdinalIgnoreCase));

                    }
                }

                if (property == null)
                {
                    return null;
                }
                columnPath.Add(property.Name);
            }
            var criteria = new Criteria
            {
                ColumnPath = columnPath,
                Filter = filter,
                Values = operands[1].Split(',')
            };
            return criteria;
        }

        private static string GetOperandValue(string q, string clauseAlias)
        {
            return Regex.Split(q, clauseAlias, RegexOptions.IgnoreCase)[1];
        }

        private dynamic GetDefaultSelectableQuery()
        {
            var selectColumns = this.EntityType.GetSelectableColumns(this.Profile);
            
            this.Query = this.Query.Take(this.Profile.DefaultToTake);

            if (this.Profile.MaxToTake.HasValue)
            {
                this.Query = this.Query.Take(this.Profile.MaxToTake.Value);
            }
            if (this.Profile.UseBaseType)
            {
                return this.Query.Select(SelectHelper.GetSelector<TEntity, TEntity>(selectColumns, this.Profile));
            }
            return this.Query.Select(SelectHelper.GetSelector<TEntity, object>(selectColumns, this.Profile));
        }
    }
}