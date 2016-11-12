using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoQueryable.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace AutoQueryable.Attributes
{
    public class AutoQueryableFilterAttribute<TDbContext, TEntity> : IAsyncActionFilter
        where TDbContext : DbContext
        where TEntity : class
    {
        private readonly bool _useFallbackValue;
        private readonly IServiceProvider _sp;

        private int _parameterIdentifier;
        private readonly List<Criteria> _comparisons;

        public AutoQueryableFilterAttribute(bool useFallbackValue = false, [FromServices] IServiceProvider sp = null)
        {
            _useFallbackValue = useFallbackValue;
            _sp = sp;
            _comparisons = new List<Criteria>();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            DbContext dbContext = _sp.GetService<TDbContext>();
            IEntityType entityType = dbContext.Model.FindEntityType(typeof(TEntity));
            IRelationalEntityTypeAnnotations table = entityType.Relational();

            var dbSetProperty = dbContext.GetType().GetProperties().FirstOrDefault(p => p.PropertyType == typeof(DbSet<TEntity>));

            if (dbSetProperty == null)
            {
                if (_useFallbackValue)
                {
                    await next();
                    return;
                }
                throw new Exception($"Unable to find DbSet of type DbSet<{typeof(TEntity).Name}> in DbContext {typeof(TDbContext).Name}.");
            }

            var dbSet = dbSetProperty.GetValue(dbContext, null) as DbSet<TEntity>;

            if (entityType == null)
            {
                if (_useFallbackValue)
                {
                    await next();
                    return;
                }
                throw new Exception($"Unable to find DbSet of type DbSet<{typeof(TEntity).Name}> in DbContext {typeof(TDbContext).Name}.");
            }
            if (dbSet == null)
            {
                if (_useFallbackValue)
                {
                    await next();
                    return;
                }
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

            AddComparisons(entityType, queryStringParts);

            if (!_comparisons.Any())
            {
                context.Result = new OkObjectResult(dbSet);
                return;
            }

            string whereClause = string.Join(" AND ", _comparisons.Select((c, index) => c.Condition.ToSqlCondition(c.Column, c.DbParameters)));

            string sqlRequest = "SELECT * FROM " + table.Schema + "." + table.TableName + " WHERE " + whereClause;
            

            IEnumerable<object> values = dbSet.FromSql(sqlRequest, _comparisons.SelectMany(c => c.DbParameters).ToArray());
            context.Result = new OkObjectResult(values);
        }

        private void AddComparisons(IEntityType entityType, string[] queryStringParts)
        {
            foreach (string q in queryStringParts)
            {
                if (q.Contains(ConditionAlias.NotEqual, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.NotEqual, Condition.NotEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Less, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.Less, Condition.Less, entityType);
                }
                else if (q.Contains(ConditionAlias.LessEqual, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.LessEqual, Condition.LessEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Greater, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.Greater, Condition.Greater, entityType);
                }
                else if (q.Contains(ConditionAlias.GreaterEqual, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.GreaterEqual, Condition.GreaterEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Contains, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.Contains, Condition.Contains, entityType);
                }
                else if (q.Contains(ConditionAlias.StartsWith, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.StartsWith, Condition.StartsWith, entityType);
                }
                else if (q.Contains(ConditionAlias.EndsWith, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.EndsWith, Condition.EndsWith, entityType);
                }
                else if (q.Contains(ConditionAlias.Between, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.Between, Condition.Between, entityType);
                }
                else if (q.Contains(ConditionAlias.Equal, StringComparison.OrdinalIgnoreCase))
                {
                    AddComparison(q, ConditionAlias.Equal, Condition.Equal, entityType);
                }
            }
        }

        private void AddComparison(string q, string conditionAlias, Condition condition, IEntityType entityType)
        {
            string[] operands = q.Split(new[] { conditionAlias }, StringSplitOptions.None);
            var criteria = new Criteria
            {
                Column = operands[0],
                Condition = condition
            };
            string[] operandValues = operands[1].Split(',');

            if (condition == Condition.Contains)
            {
                for (var i = 0; i < operandValues.Length; i++)
                {
                    operandValues[i] = $"%{operandValues[i]}%";
                }
            }
            criteria.DbParameters = operandValues.Select(v => new SqlParameter(criteria.Column + _parameterIdentifier++, v)).ToList();
            IProperty property = entityType.GetProperties().FirstOrDefault(p => p.Name.Equals(criteria.Column, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                return;
            }
            criteria.Column = property.SqlServer().ColumnName;
            _comparisons.Add(criteria);
        }
    }
}