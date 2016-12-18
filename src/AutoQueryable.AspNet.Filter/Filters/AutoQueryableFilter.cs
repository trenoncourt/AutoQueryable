using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using AutoQueryable.Extensions;
using AutoQueryable.Models;

namespace AutoQueryable.AspNet.Filter.Filters
{
    public class AutoQueryableFilter : ActionFilterAttribute
    {
        public string[] UnselectableProperties { get; set; }
        

        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var content = context.Response.Content as ObjectContent;
            if (content != null)
            {
                IQueryable<object> query = content.Value as IQueryable<object>;
                if (query == null) throw new Exception("Unable to retreive value of IQueryable from context result.");
                Type entityType = query.GetType().GenericTypeArguments[0];

                string queryString = context.Request.RequestUri.Query;
                // Work on generic type directly, for dto projection
                var result = query.AutoQueryable(queryString, new AutoQueryableProfile {UnselectableProperties = UnselectableProperties});
                context.Response.Content = new ObjectContent(result.GetType(), result, new JsonMediaTypeFormatter());
            }
        }
    }
}