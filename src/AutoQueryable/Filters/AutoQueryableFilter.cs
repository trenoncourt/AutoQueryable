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
            if (_autoQueryableProfile.QueryableType != null || _autoQueryableProfile.EntityType != null)
                return;
            dynamic query = ((ObjectResult)context.Result).Value;
            if (query == null) throw new Exception($"Unable to retreive value of IQueryable from context result.");
            Type entityType = query.GetType().GenericTypeArguments[0];

            string queryString = context.HttpContext.Request.QueryString.HasValue ? context.HttpContext.Request.QueryString.Value : null;

            var dbContext = GetInstanceField<DbContext>(query, "_context");
            
            // Work on IEntityType that allow more actions
            if (dbContext != null)
            {
                IEntityType model = dbContext.Model.FindEntityType(entityType);
                context.Result = new OkObjectResult(QueryableHelper.GetAutoQuery(queryString, model, query, _autoQueryableProfile));
            }
            // Work on generic type directly, for dto projection
            else
            {
                context.Result = new OkObjectResult(QueryableHelper.GetAutoQuery(queryString, entityType, query, _autoQueryableProfile));
            }

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
            return (TResult)field?.GetValue(instance);
        }
        
    }
}