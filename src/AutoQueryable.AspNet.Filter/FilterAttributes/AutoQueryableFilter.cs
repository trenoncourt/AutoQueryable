using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using AutoQueryable.Helpers;
using AutoQueryable.Models;

namespace AutoQueryable.AspNet.Filter.FilterAttributes
{
    public class AutoQueryableAttribute : ActionFilterAttribute
    {
        public string[] UnselectableProperties { get; set; }
        

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var content = context.Response.Content as ObjectContent;
            if (content != null)
            {
                dynamic query = content.Value;
                if (query == null) throw new Exception("Unable to retreive value of IQueryable from context result.");
                Type entityType = query.GetType().GenericTypeArguments[0];
                string queryString = context.Request.RequestUri.Query;
                var result = QueryableHelper.GetAutoQuery(queryString, entityType, query, new AutoQueryableProfile {UnselectableProperties = UnselectableProperties});
                context.Response.Content = new ObjectContent(result.GetType(), result, new JsonMediaTypeFormatter());
            }
        }
    }
}