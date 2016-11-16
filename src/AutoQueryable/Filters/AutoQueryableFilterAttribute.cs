using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoQueryable.Extensions;
using AutoQueryable.Managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace AutoQueryable.Filters
{
    public class AutoQueryableFilterAttribute<TDbContext, TEntity> : IAsyncActionFilter
        where TDbContext : DbContext
        where TEntity : class
    {
        private readonly bool _useFallbackValue;
        private readonly IServiceProvider _sp;

        public AutoQueryableFilterAttribute(bool useFallbackValue = false, [FromServices] IServiceProvider sp = null)
        {
            _useFallbackValue = useFallbackValue;
            _sp = sp;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            try
            {
                DbContext dbContext = _sp.GetService<TDbContext>();
                IEntityType entityType = dbContext.Model.FindEntityType(typeof(TEntity));
                IRelationalEntityTypeAnnotations table = entityType.Relational();

                var dbSetProperty = dbContext.GetType().GetProperties().FirstOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));

                if (dbSetProperty == null)
                {
                    throw new Exception($"Unable to find DbSet of type DbSet<{typeof(TEntity).Name}> in DbContext {typeof(TDbContext).Name}.");
                }

                var dbSet = dbSetProperty.GetValue(dbContext, null) as DbSet<TEntity>;

                if (entityType == null)
                {
                    throw new Exception($"Unable to find DbSet of type DbSet<{typeof(TEntity).Name}> in DbContext {typeof(TDbContext).Name}.");
                }
                if (dbSet == null)
                {
                    throw new Exception($"Unable to retreive value of DbSet with type DbSet<{typeof(TEntity).Name}> in DbContext {typeof(TDbContext).Name}.");
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

                string whereClause = string.Join(" AND ", criterias.Select((c, index) => c.ConditionType.ToSqlCondition(c.Column, c.DbParameters)));

                string sqlRequest = "SELECT * FROM " + table.Schema + "." + table.TableName + " WHERE " + whereClause;


                IEnumerable<object> values = dbSet.FromSql(sqlRequest, criterias.SelectMany(c => c.DbParameters).ToArray());
                context.Result = new OkObjectResult(values);
            }
            catch (Exception)
            {

                if (_useFallbackValue)
                {
                    await next();
                    return;
                }
                throw;
            }
        }
    }
}