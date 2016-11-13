using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoQueryable.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

namespace AutoQueryable.Filters
{
    public class AutoQueryableFilter : IAsyncActionFilter
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

                if (dbContext == null)
                {
                    throw new Exception($"Unable to find DbContext of type {_autoQueryableProfile.DbContextType.Name}.");
                }

                IEntityType entityType = dbContext.Model.FindEntityType(_autoQueryableProfile.EntityType);
                IRelationalEntityTypeAnnotations table = entityType.Relational();

                var dbSetProperty = dbContext.GetType().GetProperties().FirstOrDefault(p => p.PropertyType.GenericTypeArguments.Contains(_autoQueryableProfile.EntityType));

                if (dbSetProperty == null)
                {
                    throw new Exception($"Unable to find DbSet of type DbSet<{_autoQueryableProfile.EntityType.Name}> in DbContext {_autoQueryableProfile.DbContextType.Name}.");
                }

                var dbSet = dbSetProperty.GetValue(dbContext, null) as IQueryable<object>;

                if (entityType == null)
                {
                    throw new Exception($"Unable to find DbSet of type DbSet<{_autoQueryableProfile.EntityType.Name}> in DbContext {_autoQueryableProfile.DbContextType.Name}.");
                }
                if (dbSet == null)
                {
                    throw new Exception($"Unable to retreive value of DbSet with type DbSet<{_autoQueryableProfile.EntityType.Name}> in DbContext {_autoQueryableProfile.DbContextType.Name}.");
                }

                string[] queryStringParts = context.HttpContext.Request.QueryString.HasValue
                    ? context.HttpContext.Request.QueryString.Value.Replace("?", "").Split('&')
                    : null;

                if (queryStringParts == null)
                {
                    context.Result = new OkObjectResult(dbSet);
                    return;
                }

                var criteriaManager = new CriteriaManager();
                IEnumerable<Criteria> criterias = criteriaManager.GetCriterias(entityType, queryStringParts).ToList();

                if (!criterias.Any())
                {
                    context.Result = new OkObjectResult(dbSet);
                    return;
                }

                string whereClause = string.Join(" AND ", criterias.Select((c, index) => c.Condition.ToSqlCondition(c.Column, c.DbParameters)));

                string sqlRequest = "SELECT * FROM " + table.Schema + "." + table.TableName + " WHERE " + whereClause;


                IEnumerable<object> values = dbSet.FromSql(sqlRequest, criterias.SelectMany(c => c.DbParameters).ToArray());
                context.Result = new OkObjectResult(values);
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
