using System;
using AutoQueryable.Helpers;
using AutoQueryable.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoQueryable.AspNetCore.Filter.Filters
{
    public class AutoQueryableFilter : ActionFilterAttribute, IActionFilter
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
            dynamic query = ((ObjectResult)context.Result).Value;
            if (query == null) throw new Exception("Unable to retreive value of IQueryable from context result.");
            Type entityType = query.GetType().GenericTypeArguments[0];

            string queryString = context.HttpContext.Request.QueryString.HasValue ? context.HttpContext.Request.QueryString.Value : null;
            // Work on generic type directly, for dto projection
            context.Result = new OkObjectResult(QueryableHelper.GetAutoQuery(queryString, entityType, query, _autoQueryableProfile));
        }
    }
}