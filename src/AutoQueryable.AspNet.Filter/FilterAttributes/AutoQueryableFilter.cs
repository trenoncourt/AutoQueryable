using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.Abstractions;

namespace AutoQueryable.AspNet.Filter.FilterAttributes
{
    public class AutoQueryableAttribute : ActionFilterAttribute, IFilterProfile
    {
        public string[] SelectableProperties { get; set; }

        public string[] UnselectableProperties { get; set; }

        public string[] SortableProperties { get; set; }

        public string[] UnSortableProperties { get; set; }

        public string[] GroupableProperties { get; set; }

        public string[] UnGroupableProperties { get; set; }

        public ClauseType AllowedClauses { get; set; }

        public ClauseType DisAllowedClauses { get; set; }

        public ConditionType AllowedConditions { get; set; }

        public ConditionType DisAllowedConditions { get; set; }

        public WrapperPartType AllowedWrapperPartType { get; set; }

        public WrapperPartType DisAllowedWrapperPartType { get; set; }

        public int MaxToTake { get; set; }
        
        public int DefaultToTake { get; set; }

        public int MaxToSkip { get; set; }

        public int MaxDepth { get; set; }
        
        public Dictionary<string, bool> DefaultOrderBy { get; set; } = new Dictionary<string, bool>();
        
        public bool UseBaseType { get; set; }
        public bool ToListBeforeSelect { get; set; }


        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            if (context.Response.Content is ObjectContent content)
            {
                // Get the request lifetime scope so you can resolve services.
                var requestScope = context.Request.GetDependencyScope();

                // Resolve the service you want to use.

                if(!(requestScope.GetService(typeof(IAutoQueryableContext)) is IAutoQueryableContext autoQueryableContext))
                {
                    throw new NullReferenceException($"No instance registered as '{nameof(IAutoQueryableContext)}'");
                }

                dynamic query = content.Value;

                if (query == null)
                {
                    throw new Exception("Unable to retrieve value of IQueryable from context result.");
                }

                var result = autoQueryableContext.GetAutoQuery(query);
                context.Response.Content = new ObjectContent(result.GetType(), result, new JsonMediaTypeFormatter());
            }
        }
    }
}