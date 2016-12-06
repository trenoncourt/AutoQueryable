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
    public class AutoQueryableFilter : IActionFilter
    {
        private readonly AutoQueryableProfile _autoQueryableProfile;

        public AutoQueryableFilter(AutoQueryableProfile autoQueryableProfile)
        {
            _autoQueryableProfile = autoQueryableProfile;
        }
        public virtual void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public virtual void OnActionExecuted(ActionExecutedContext context)
        {
            if (_autoQueryableProfile.DbContextType != null || _autoQueryableProfile.EntityType != null)
                return;
            dynamic dbSet = ((ObjectResult)context.Result).Value;
            if (dbSet == null) throw new Exception($"Unable to retreive value of DbSet from context result.");
            Type entityType = dbSet.GetType().GenericTypeArguments[0];
            var dbContext = GetInstanceField<DbContext>(dbSet, "_context");
            IEntityType model = dbContext.Model.FindEntityType(entityType);

            string queryString = context.HttpContext.Request.QueryString.HasValue ? context.HttpContext.Request.QueryString.Value : null;
            context.Result = new OkObjectResult(QueryableHelper.GetAutoQuery(queryString, model, dbSet, _autoQueryableProfile));
        }

        ///  <summary>
        ///  Uses reflection to get the field value from an object.
        ///  </summary>
        /// <param name="instance">The instance object.</param>
        ///  <param name="fieldName">The field's name which is to be fetched.</param>
        /// 
        ///  <returns>The field value from the object.</returns>
        internal static TResult GetInstanceField<TResult>(object instance, string fieldName)
        {
            BindingFlags bindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
            FieldInfo field = instance.GetType().GetField(fieldName, bindFlags);
            return (TResult)field.GetValue(instance);
        }
        
    }
}