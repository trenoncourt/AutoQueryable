using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoQueryable.Core.Aliases;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models.Clauses;
using AutoQueryable.Helpers;
using AutoQueryable.Models;

namespace AutoQueryable.Core.Models
{
    public abstract class AutoQueryableContext
    {
        private string[] _queryStringParts;
        
        public AutoQueryableProfile Profile { get; private set; }
        public string QueryString { get; private set; }
        public Type EntityType { get; private set; }
        public AllClauses Clauses { get; set; }
        
        public string[] QueryStringParts => _queryStringParts ?? (_queryStringParts = QueryString.Replace("?", "").Split('&'));

        public abstract dynamic GetAutoQuery();

        /// <summary>
        /// Create AutoQueryable context typed with default entity type
        /// </summary>
        public static AutoQueryableContext Create<TEntity>(IQueryable<TEntity> query, string queryString,
            AutoQueryableProfile profile) where TEntity : class 
        {
            Type entityType = query.GetType().GenericTypeArguments[0];
            
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
            if (string.IsNullOrEmpty(QueryString))
                return GetDefaultSelectableQuery();
            
            Clauses = GetClauses(QueryStringParts);
            ICollection<Criteria> criterias = Profile.IsClauseAllowed(ClauseType.Filter) ? GetCriterias() : null;
            
            QueryResult queryResult = QueryBuilder.Build(Query, EntityType, Clauses, criterias, Profile, Clauses.WrapWith != null && Clauses.WrapWith.CountAllRows);
            if (Clauses.WrapWith == null || !Clauses.WrapWith.Any)
            {
                return queryResult.Result;
            }

            return Clauses.WrapWith.GetWrappedResult(queryResult);
        }
        
        public AllClauses GetClauses(string[] queryStringParts)
        {
            var clauses = new AllClauses();
            foreach (string q in queryStringParts)
            {
                if (q.Contains(ClauseAlias.Select, StringComparison.OrdinalIgnoreCase) && Profile.IsClauseAllowed(ClauseType.Select))
                {
                    clauses.Select = new SelectClause(this) { Value = GetOperandValue(q, ClauseAlias.Select)};
                    clauses.Select.Parse();
                }
                else if (q.Contains(ClauseAlias.Top, StringComparison.OrdinalIgnoreCase) && Profile.IsClauseAllowed(ClauseType.Top))
                {
                    clauses.Top = new TopClause(this) { Value = GetOperandValue(q, ClauseAlias.Top)};
                }
                else if (q.Contains(ClauseAlias.Take, StringComparison.OrdinalIgnoreCase) && Profile.IsClauseAllowed(ClauseType.Top))
                {
                    clauses.Top = new TopClause(this) { Value = GetOperandValue(q, ClauseAlias.Take)};
                }
                else if (q.Contains(ClauseAlias.Skip, StringComparison.OrdinalIgnoreCase) && Profile.IsClauseAllowed(ClauseType.Skip))
                {
                    clauses.Skip = new SkipClause(this) { Value = GetOperandValue(q, ClauseAlias.Skip)};
                }
                else if (q.Contains(ClauseAlias.First, StringComparison.OrdinalIgnoreCase) && Profile.IsClauseAllowed(ClauseType.First))
                {
                    clauses.First = new FirstClause(this);
                }
                else if (q.Contains(ClauseAlias.Last, StringComparison.OrdinalIgnoreCase) && Profile.IsClauseAllowed(ClauseType.Last))
                {
                    clauses.Last = new LastClause(this);
                }
                else if (q.Contains(ClauseAlias.OrderBy, StringComparison.OrdinalIgnoreCase) && Profile.IsClauseAllowed(ClauseType.OrderBy))
                {
                    clauses.OrderBy = new OrderByClause(this) { Value = GetOperandValue(q, ClauseAlias.OrderBy)};
                }
                else if (q.Contains(ClauseAlias.OrderByDesc, StringComparison.OrdinalIgnoreCase) && Profile.IsClauseAllowed(ClauseType.OrderByDesc))
                {
                    clauses.OrderByDesc = new OrderByDescClause(this) { Value = GetOperandValue(q, ClauseAlias.OrderByDesc)};
                }
                else if (q.Contains(ClauseAlias.WrapWith, StringComparison.OrdinalIgnoreCase) && Profile.IsClauseAllowed(ClauseType.WrapWith))
                {
                    clauses.WrapWith = new WrapWithClause(this) { Value = GetOperandValue(q, ClauseAlias.WrapWith)};
                    clauses.WrapWith.Parse();
                }
            }
            
            if (clauses.OrderBy == null && clauses.OrderByDesc == null && !string.IsNullOrEmpty(Profile.DefaultOrderBy))
                clauses.OrderBy = new OrderByClause(this) { Value = Profile.DefaultOrderBy };
            
            if (clauses.OrderBy == null && clauses.OrderByDesc == null && !string.IsNullOrEmpty(Profile.DefaultOrderByDesc))
                clauses.OrderByDesc = new OrderByDescClause(this) { Value = Profile.DefaultOrderByDesc };
            return clauses;
        }
        
        public ICollection<Criteria> GetCriterias()
        {
            ICollection<Criteria> criterias = new List<Criteria>();
            foreach (string qPart in QueryStringParts)
            {
                string q = WebUtility.UrlDecode(qPart);
                Criteria criteria = null;
                if (q.Contains(ConditionAlias.NotEqual, StringComparison.OrdinalIgnoreCase) && Profile.IsConditionAllowed(ConditionType.NotEqual))
                {
                    criteria = GetCriteria(q, ConditionAlias.NotEqual, ConditionType.NotEqual);
                }
                else if (q.Contains(ConditionAlias.LessEqual, StringComparison.OrdinalIgnoreCase)  && Profile.IsConditionAllowed(ConditionType.LessEqual))
                {
                    criteria = GetCriteria(q, ConditionAlias.LessEqual, ConditionType.LessEqual);
                }
                else if (q.Contains(ConditionAlias.Less, StringComparison.OrdinalIgnoreCase)  && Profile.IsConditionAllowed(ConditionType.Less))
                {
                    criteria = GetCriteria(q, ConditionAlias.Less, ConditionType.Less);
                }
                else if (q.Contains(ConditionAlias.GreaterEqual, StringComparison.OrdinalIgnoreCase)  && Profile.IsConditionAllowed(ConditionType.GreaterEqual))
                {
                    criteria = GetCriteria(q, ConditionAlias.GreaterEqual, ConditionType.GreaterEqual);
                }
                else if (q.Contains(ConditionAlias.Greater, StringComparison.OrdinalIgnoreCase)  && Profile.IsConditionAllowed(ConditionType.Greater))
                {
                    criteria = GetCriteria(q, ConditionAlias.Greater, ConditionType.Greater);
                }
                else if (q.Contains(ConditionAlias.Contains, StringComparison.OrdinalIgnoreCase)  && Profile.IsConditionAllowed(ConditionType.Contains))
                {
                    criteria = GetCriteria(q, ConditionAlias.Contains, ConditionType.Contains);
                }
                else if (q.Contains(ConditionAlias.StartsWith, StringComparison.OrdinalIgnoreCase)  && Profile.IsConditionAllowed(ConditionType.StartsWith))
                {
                    criteria = GetCriteria(q, ConditionAlias.StartsWith, ConditionType.StartsWith);
                }
                else if (q.Contains(ConditionAlias.EndsWith, StringComparison.OrdinalIgnoreCase)  && Profile.IsConditionAllowed(ConditionType.EndsWith))
                {
                    criteria = GetCriteria(q, ConditionAlias.EndsWith, ConditionType.EndsWith);
                }
                else if (q.Contains(ConditionAlias.Between, StringComparison.OrdinalIgnoreCase)  && Profile.IsConditionAllowed(ConditionType.Between))
                {
                    criteria = GetCriteria(q, ConditionAlias.Between, ConditionType.Between);
                }
                else if (q.Contains(ConditionAlias.Equal, StringComparison.OrdinalIgnoreCase)  && Profile.IsConditionAllowed(ConditionType.Equal))
                {
                    criteria = GetCriteria(q, ConditionAlias.Equal, ConditionType.Equal);
                }
                if (criteria != null)
                {
                    criterias.Add(criteria);
                }
            }

            return criterias;
        }

        private Criteria GetCriteria(string q, string conditionAlias, ConditionType conditionType)
        {
            string[] operands = Regex.Split(q, conditionAlias, RegexOptions.IgnoreCase);

            PropertyInfo property = null;
            var columnPath = new List<string>();
            var columns = operands[0].Split('.');
            foreach (var column in columns)
            {
                if (property == null)
                {
                    property = EntityType.GetProperties().FirstOrDefault(p => p.Name.Equals(column, StringComparison.OrdinalIgnoreCase));
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
                ConditionType = conditionType,
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
            ICollection<SelectColumn> selectColumns = EntityType.GetSelectableColumns(Profile);
            
            if (Profile.DefaultToTake.HasValue)
            {
                Query = Query.Take(Profile.DefaultToTake.Value);
            }
            else if (Profile.MaxToTake.HasValue)
            {
                Query = Query.Take(Profile.MaxToTake.Value);
            }
            if (Profile.UseBaseType)
            {
                return Query.Select(SelectHelper.GetSelector<TEntity, TEntity>(selectColumns, Profile));
            }
            return Query.Select(SelectHelper.GetSelector<TEntity, object>(selectColumns, Profile));
        }
    }
}