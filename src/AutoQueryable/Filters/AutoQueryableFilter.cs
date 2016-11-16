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
using AutoQueryable.Managers;
using System.Linq.Expressions;
using System.Dynamic;
using System.Reflection.Emit;

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

                var dbSet = dbSetProperty.GetValue(dbContext, null) as IQueryable<TEntity>;
                
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

                var clauseManager = new ClauseManager();
                IEnumerable<Clause> clauses = clauseManager.GetClauses(queryStringParts).ToList();

                Clause selectClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Select);

                string selectClauseValue = "*";

                if (selectClause != null)
                {
                    selectClauseValue = string.Join(",", selectClause.Value.Split(',').Where(v =>
                    {
                        PropertyInfo property = _autoQueryableProfile.EntityType.GetProperty(v,
                            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                        return property != null;
                    }));
                }

                if (!criterias.Any())
                {
                    if (selectClause != null)
                    {
                        context.Result = new OkObjectResult(dbSet.Select(GetSelector(selectClauseValue)));
                        return;
                    }
                    context.Result = new OkObjectResult(dbSet);
                    return;
                }

                string whereClauseString = " WHERE " + string.Join(" AND ",
                        criterias.Select((c, index) => c.ConditionType.ToSqlCondition(c.Column, c.DbParameters)));

                string selectClauseString =
                    $"SELECT {selectClauseValue} FROM {table.Schema + "."}[{table.TableName}]";

                string sqlRequest = selectClauseString + whereClauseString;

                var query = dbSet.FromSql(sqlRequest, criterias.SelectMany(c => c.DbParameters).ToArray());

                if (selectClause != null)
                {
                    context.Result = new OkObjectResult(query.Select(GetSelector(selectClauseValue)));
                }
                else
                {
                    context.Result = new OkObjectResult(query);
                }
            }
            catch (Exception e)
            {
                if (_autoQueryableProfile.UseFallbackValue)
                {
                    await next();
                    return;
                }
                throw;
            }
        }

        private Expression<Func<TEntity, object>> GetSelector(string columns)
        {
            AssemblyBuilder dynamicAssembly = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("AutoQueryableDynamicAssembly"), AssemblyBuilderAccess.Run);
            ModuleBuilder dynamicModule = dynamicAssembly.DefineDynamicModule("AutoQueryableDynamicAssemblyModule");
            TypeBuilder dynamicTypeBuilder = dynamicModule.DefineType("AutoQueryableDynamicType", TypeAttributes.Public);
            foreach (string column in columns.Split(','))
            {
                PropertyInfo property = _autoQueryableProfile.EntityType.GetProperty(column, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                dynamicTypeBuilder.AddProperty(property.Name, property.PropertyType);
            }

            Type dynamicType = dynamicTypeBuilder.CreateTypeInfo().AsType();

            var ctor = Expression.New(dynamicType);

            ParameterExpression parameter = Expression.Parameter(_autoQueryableProfile.EntityType, "p");

            var memberAssignments = dynamicType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p =>
            {
                PropertyInfo propertyInfo = _autoQueryableProfile.EntityType.GetProperty(p.Name, BindingFlags.Public | BindingFlags.Instance);
                MemberExpression memberExpression = Expression.Property(parameter, propertyInfo);
                return Expression.Bind(p, memberExpression);
            });
            var memberInit = Expression.MemberInit(ctor, memberAssignments);
            return Expression.Lambda<Func<TEntity, object>>(memberInit, parameter);
        }

    }
}
