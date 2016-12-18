using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoQueryable.Helpers;
using AutoQueryable.Models;

namespace AutoQueryable.AspNetCore.Filter.FilterAttributes
{
    public class AutoQueryableAttribute : ActionFilterAttribute
    {
        public string[] UnselectableProperties { get; set; }
        
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            dynamic query = ((ObjectResult)context.Result).Value;
            if (query == null) throw new Exception("Unable to retreive value of IQueryable from context result.");
            Type entityType = query.GetType().GenericTypeArguments[0];

            string queryString = context.HttpContext.Request.QueryString.HasValue ? context.HttpContext.Request.QueryString.Value : null;
            context.Result = new OkObjectResult(QueryableHelper.GetAutoQuery(queryString, entityType, query, new AutoQueryableProfile { UnselectableProperties = UnselectableProperties }));
        }
    }
}