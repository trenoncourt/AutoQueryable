using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using AutoQueryable.Managers;
using AutoQueryable.Helpers;
using AutoQueryable.Models;

namespace AutoQueryable.Filters
{
    public class AutoQueryableFilter<TEntity> : IAsyncActionFilter where TEntity : class 
    {
        private readonly AutoQueryableProfile _autoQueryableProfile;
        private readonly IServiceProvider _sp;

        public AutoQueryableFilter(AutoQueryableProfile autoQueryableProfile, IServiceProvider sp)
        {
            _autoQueryableProfile = autoQueryableProfile;
            _sp = sp;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                DbContext dbContext = _sp.GetService(_autoQueryableProfile.DbContextType) as DbContext;
                if (dbContext == null) throw new Exception($"Unable to find DbContext of type {_autoQueryableProfile.DbContextType.Name}.");


                IEntityType entityType = dbContext.Model.FindEntityType(_autoQueryableProfile.EntityType);
                IRelationalEntityTypeAnnotations table = entityType.Relational();

                var dbSetProperty = dbContext.GetType().GetProperties().FirstOrDefault(p => p.PropertyType.GenericTypeArguments.Contains(_autoQueryableProfile.EntityType));
                if (dbSetProperty == null) throw new Exception($"Unable to find DbSet of type DbSet<{_autoQueryableProfile.EntityType.Name}> in DbContext {_autoQueryableProfile.DbContextType.Name}.");

                var dbSet = dbSetProperty.GetValue(dbContext, null) as IQueryable<TEntity>;
                if (dbSet == null) throw new Exception($"Unable to retreive value of DbSet with type DbSet<{_autoQueryableProfile.EntityType.Name}> in DbContext {_autoQueryableProfile.DbContextType.Name}.");

                string[] queryStringParts = context.HttpContext.Request.QueryString.HasValue
                    ? context.HttpContext.Request.QueryString.Value.Replace("?", "").Split('&')
                    : null;
                dbSet = dbSet.AsNoTracking();
                if (queryStringParts == null)
                {
                    List<Column> columns = SelectHelper.GetSelectableColumns(null, null, _autoQueryableProfile.UnselectableProperties, entityType).ToList();
                    context.Result = new OkObjectResult(dbSet.Select(SelectHelper.GetSelector<TEntity>(string.Join(",", columns.Select(c => c.PropertyName)))));
                    return;
                }

                var criteriaManager = new CriteriaManager();
                IList<Criteria> criterias = criteriaManager.GetCriterias(entityType, queryStringParts).ToList();
                var clauseManager = new ClauseManager();
                IList<Clause> clauses = clauseManager.GetClauses(queryStringParts).ToList();
                
                 var result = QueryBuilder.Build(dbSet, entityType, table, clauses, criterias, _autoQueryableProfile.UnselectableProperties);
                context.Result = new OkObjectResult(result);
            }
            catch (Exception)
            {
                if (_autoQueryableProfile.UseFallbackValue)
                {
                    await next();
                    return;
                }
                throw;
            }
        }
    }
}
