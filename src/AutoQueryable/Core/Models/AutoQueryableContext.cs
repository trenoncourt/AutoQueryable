using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoQueryable.Core.Aliases;
using AutoQueryable.Core.Clauses;
using AutoQueryable.Core.CriteriaFilters;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models.Clauses;
using AutoQueryable.Helpers;
using Microsoft.AspNetCore.Http;
using Serilog;

namespace AutoQueryable.Core.Models
{
    public interface IAutoQueryableContext : IAutoQueryableContext<object, object> { } 
    public interface IAutoQueryableContext<TEntity> : IAutoQueryableContext<TEntity, TEntity> where TEntity : class { }
    public interface IAutoQueryableContext<TEntity, TAs> where TEntity : class where TAs : class
    {
        IQueryable<TAs> GetAutoQuery(IQueryable<TEntity> query);
    }
    public class AutoQueryableContext : AutoQueryableContext<object, object>
    {
        public AutoQueryableContext(IQueryStringAccessor queryStringAccessor, IAutoQueryableProfile profile) : base(queryStringAccessor, profile)
        {
            
        }
    }
    public class AutoQueryableContext<TEntity> : AutoQueryableContext<TEntity, TEntity> where TEntity : class
    {
        public AutoQueryableContext(IQueryStringAccessor queryStringAccessor, IAutoQueryableProfile profile) : base(queryStringAccessor, profile)
        {
            
        }
    }
    public class AutoQueryableContext<TEntity, TAs> : IAutoQueryableContext<TEntity, TAs> where TEntity : class where TAs : class
    {
        private readonly IQueryStringAccessor _queryStringAccessor;
        private readonly IAutoQueryableProfile _profile;
        private readonly ICriteriaFilterManager _criteriaFilterManager;
        private readonly IClauseMapManager _clauseMapManager;
        private readonly IClauseValueManager _clauseValueManager;
        private readonly IQueryBuilder<TEntity, TAs> _queryBuilder;


        public AutoQueryableContext(IQueryStringAccessor queryStringAccessor, IAutoQueryableProfile profile, ICriteriaFilterManager criteriaFilterManager, IClauseMapManager clauseMapManager, IClauseValueManager clauseValueManager, IQueryBuilder<TEntity, TAs> queryBuilder)
        {
            _queryStringAccessor = queryStringAccessor;
            _profile = profile;
            _criteriaFilterManager = criteriaFilterManager;
            _clauseMapManager = clauseMapManager;
            _clauseValueManager = clauseValueManager;
            _queryBuilder = queryBuilder;
        }

        public IQueryable<TAs> GetAutoQuery(IQueryable<TEntity> query) //IAutoQueryResult<TEntity>
        {
            // No query string, get only selectable columns
            if (string.IsNullOrEmpty(_queryStringAccessor.QueryString))
            {
                return GetDefaultSelectableQuery(query); //new AutoQueryResult<TEntity>(); //;
            }

            _getClauses();
            var criterias = _profile.IsClauseAllowed(ClauseType.Filter) ? GetCriterias().ToList() : null;
            
            var queryResult = _queryBuilder.Build(query, criterias);

            return queryResult;
        }
        
        private void _getClauses()
        {
            // Set the defaults to start with, then fill/overwrite with the query string values
            _clauseValueManager.SetDefaults(_profile);
            //var clauses = new List<Clause>();
            foreach (var q in _queryStringAccessor.QueryStringParts.Where(q => !q.IsHandled))
            {
                var clauseQueryFilter = _clauseMapManager.FindClauseQueryFilter(q.Value);
                if(clauseQueryFilter != null)
                {
                    var value = clauseQueryFilter.ParseValue(_getOperandValue(q.Value, clauseQueryFilter.Alias));
                    var propertyInfo = _clauseValueManager.GetType().GetProperty(clauseQueryFilter.ClauseType.ToString());
                    propertyInfo.SetValue(_clauseValueManager, value);
                    //clauses.Add(new Clause(clauseQueryFilter.ClauseType, value, clauseQueryFilter.ValueType));
                }
            }

            if (_clauseValueManager.Page != null)
            {
                //this.Logger.Information("Overwriting 'skip' clause value because 'page' is set");
                // Calculate skip from page if page query param was set
                var page = _clauseValueManager.Page;
                var take = _clauseValueManager.Top ?? _profile.DefaultToTake;
                _clauseValueManager.Skip = page*take;
            }

            if (_clauseValueManager.OrderBy == null && !string.IsNullOrEmpty(_profile.DefaultOrderBy))
            {
                _clauseValueManager.OrderBy = _profile.DefaultOrderBy;
            }

                // TODO: Is this right? empty string??
            if (_clauseValueManager.Select == null)
            {
                _clauseMapManager.GetClauseQueryFilter(ClauseType.Select).ParseValue("");
            }
        }
        private string _getOperandValue(string q, string clauseAlias) => Regex.Split(q, clauseAlias, RegexOptions.IgnoreCase)[1];

        public IEnumerable<Criteria> GetCriterias()
        {
            foreach (var qPart in _queryStringAccessor.QueryStringParts.Where(q => !q.IsHandled))
            {
                var q = WebUtility.UrlDecode(qPart.Value);
                var criteria = GetCriteria(q);

                if (criteria != null)
                {
                    yield return criteria;
                }
            }
        }

        private Criteria GetCriteria(string q)
        {

            var filter = _criteriaFilterManager.FindFilter(q);
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
                    property = typeof(TEntity).GetProperties().FirstOrDefault(p => p.Name.Equals(column, StringComparison.OrdinalIgnoreCase));
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

        private IQueryable<TAs> GetDefaultSelectableQuery(IQueryable<TEntity> query)
        {
            var selectColumns = typeof(TEntity).GetSelectableColumns(_profile);
            
            query = query.Take(_profile.DefaultToTake);

            if (_profile.MaxToTake.HasValue)
            {
                query = query.Take(_profile.MaxToTake.Value);
            }
            return query.Select(SelectHelper.GetSelector<TEntity, TAs>(selectColumns, _profile));
        }
    }

    public interface IQueryStringAccessor
    {
        string QueryString { get; }
        ICollection<QueryStringPart> QueryStringParts { get; }
    }

    public class AspNetCoreQueryStringAccessor : IQueryStringAccessor
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ICollection<QueryStringPart> QueryStringParts { get; } = new List<QueryStringPart>();

        public string QueryString
        {
            get
            {
                var queryString = _httpContextAccessor.HttpContext.Request.QueryString.Value;
                return Uri.UnescapeDataString(queryString ?? "");
            }
        }

        public AspNetCoreQueryStringAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;

            foreach(var queryStringPart in QueryString.Replace("?", "").Split('&'))
            {
                QueryStringParts.Add(new QueryStringPart(queryStringPart));
            }
        }
    }

    public class QueryStringPart
    {

        public string Value { get; set; }
        public bool IsHandled { get; set; }

        public QueryStringPart(string value)
        {
            Value = value;
        }
    }
}