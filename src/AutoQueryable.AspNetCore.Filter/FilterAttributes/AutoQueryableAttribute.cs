using System;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AutoQueryable.Helpers;

namespace AutoQueryable.AspNetCore.Filter.FilterAttributes
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
        
        public ProviderType ProviderType { get; set; }

        public bool UseBaseType { get; set; }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            dynamic query = ((ObjectResult)context.Result).Value;
            if (query == null) throw new Exception("Unable to retreive value of IQueryable from context result.");
            Type entityType = query.GetType().GenericTypeArguments[0];

            string queryString = context.HttpContext.Request.QueryString.HasValue ? context.HttpContext.Request.QueryString.Value : null;
            context.Result = new OkObjectResult(QueryableHelper.GetAutoQuery(queryString, entityType, query, new AutoQueryableProfile
            {
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
                MaxDepth = MaxDepth,
                ProviderType = ProviderType,
                UseBaseType = UseBaseType
            }));
        }
    }
}