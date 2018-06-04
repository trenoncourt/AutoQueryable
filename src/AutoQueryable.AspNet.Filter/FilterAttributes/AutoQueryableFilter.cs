using System;
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
        
        public string DefaultOrderBy { get; set; }
        
        public string DefaultOrderByDesc { get; set; }
        
        public bool UseBaseType { get; set; }


        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var content = context.Response.Content as ObjectContent;
            if (content != null)
            {
                dynamic query = content.Value;
                if (query == null) throw new Exception("Unable to retreive value of IQueryable from context result.");
                
                var queryString = context.Request.RequestUri.Query;
                var autoQueryableContext = new
                    AutoQueryableContext<object>(queryString); //AutoQueryableProfile.From(this));
                var result = autoQueryableContext.GetAutoQuery(query);
                context.Response.Content = new ObjectContent(result.GetType(), result, new JsonMediaTypeFormatter());
            }
        }
    }
}