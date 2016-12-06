using System;
using System.Linq;
using System.Reflection;
using AutoQueryable.Helpers;
using AutoQueryable.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoQueryable.Filters
{
    public class AutoQueryableFilter<TEntity> : AutoQueryableFilter where TEntity : class 
    {
        private readonly AutoQueryableProfile _autoQueryableProfile;
        private readonly IServiceProvider _sp;

        public AutoQueryableFilter(AutoQueryableProfile autoQueryableProfile, IServiceProvider sp) : base(autoQueryableProfile)
        {
            _autoQueryableProfile = autoQueryableProfile;
            _sp = sp;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_autoQueryableProfile.DbContextType == null || _autoQueryableProfile.EntityType == null)
                return;
            DbContext dbContext = _sp.GetService(_autoQueryableProfile.DbContextType) as DbContext;
            if (dbContext == null) throw new Exception($"Unable to find DbContext of type {_autoQueryableProfile.DbContextType.Name}.");
            IEntityType model = dbContext.Model.FindEntityType(_autoQueryableProfile.EntityType);

            var dbSetProperty = dbContext.GetType().GetProperties().FirstOrDefault(p => p.PropertyType.GenericTypeArguments.Contains(_autoQueryableProfile.EntityType));
            if (dbSetProperty == null) throw new Exception($"Unable to find DbSet of type DbSet<{_autoQueryableProfile.EntityType.Name}> in DbContext {_autoQueryableProfile.DbContextType.Name}.");

            var dbSet = dbSetProperty.GetValue(dbContext, null) as IQueryable<TEntity>;
            if (dbSet == null) throw new Exception($"Unable to retreive value of DbSet with type DbSet<{_autoQueryableProfile.EntityType.Name}> in DbContext {_autoQueryableProfile.DbContextType.Name}.");

            string queryString = context.HttpContext.Request.QueryString.HasValue ? context.HttpContext.Request.QueryString.Value : null;
            context.Result = new OkObjectResult(QueryableHelper.GetAutoQuery(queryString, model, dbSet, _autoQueryableProfile));
        }
    }
}