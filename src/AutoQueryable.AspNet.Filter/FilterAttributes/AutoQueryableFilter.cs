using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Filters;
using AutoQueryable.Helpers;
using AutoQueryable.Models;
using AutoQueryable.Models.Enums;

namespace AutoQueryable.AspNet.Filter.FilterAttributes
{
    public class AutoQueryableAttribute : ActionFilterAttribute
    {
        public string[] SelectableProperties { get; set; }

        public string[] UnselectableProperties { get; set; }

        public string[] SortableProperties { get; set; }

        public string[] UnSortableProperties { get; set; }

        public string[] GroupableProperties { get; set; }

        public string[] UnGroupableProperties { get; set; }

        public ClauseType? AllowedClauses { get; set; }

        public ClauseType? DisAllowedClauses { get; set; }

        public ConditionType? AllowedConditions { get; set; }

        public ConditionType? DisAllowedConditions { get; set; }

        public WrapperPartType? AllowedWrapperPartType { get; set; }

        public WrapperPartType? DisAllowedWrapperPartType { get; set; }

        public int? MaxToTake { get; set; }

        public int? MaxToSkip { get; set; }

        public int? MaxDepth { get; set; }


        public override void OnActionExecuted(HttpActionExecutedContext context)
        {
            var content = context.Response.Content as ObjectContent;
            if (content != null)
            {
                dynamic query = content.Value;
                if (query == null) throw new Exception("Unable to retreive value of IQueryable from context result.");
                Type entityType = query.GetType().GenericTypeArguments[0];
                string queryString = context.Request.RequestUri.Query;
                var result = QueryableHelper.GetAutoQuery(queryString, entityType, query, new AutoQueryableProfile {
                    SelectableProperties = SelectableProperties,
                    UnselectableProperties = UnselectableProperties,
                    SortableProperties = SortableProperties,
                    UnSortableProperties = UnSortableProperties,
                    GroupableProperties = GroupableProperties,
                    UnGroupableProperties = UnGroupableProperties,
                    AllowedClauses = AllowedClauses,
                    DisAllowedClauses = DisAllowedClauses,
                    AllowedConditions = AllowedConditions,
                    DisAllowedConditions = DisAllowedConditions,
                    AllowedWrapperPartType = AllowedWrapperPartType,
                    DisAllowedWrapperPartType = DisAllowedWrapperPartType,
                    MaxToTake = MaxToTake,
                    MaxToSkip = MaxToSkip,
                    MaxDepth = MaxDepth
                });
                context.Response.Content = new ObjectContent(result.GetType(), result, new JsonMediaTypeFormatter());
            }
        }
    }
}