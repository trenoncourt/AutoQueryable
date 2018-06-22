using System;
using System.Collections.Generic;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;


namespace AutoQueryable.AspNetCore.Filter.FilterAttributes
{
    public class AutoQueryableAttribute : ActionFilterAttribute, IFilterProfile
    {
        private readonly IAutoQueryableContext _autoQueryableContext;
        private readonly IAutoQueryableProfile _autoQueryableProfile;

        public AutoQueryableAttribute(IAutoQueryableContext autoQueryableContext, IAutoQueryableProfile autoQueryableProfile)
        {
            _autoQueryableContext = autoQueryableContext;
            _autoQueryableProfile = autoQueryableProfile;
        }
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
        
        public Dictionary<string, bool> DefaultOrderBy { get; set; }
       
        public ProviderType ProviderType { get; set; }

        public bool UseBaseType { get; set; }
        public bool ToListBeforeSelect { get; set; }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            dynamic query = ((ObjectResult)context.Result).Value;
            // TODO: Set attribute property values in profile
            //context.HttpContext
            if (query == null) throw new Exception("Unable to retrieve value of IQueryable from context result.");
            context.Result = new OkObjectResult(_autoQueryableContext.GetAutoQuery(query));
        }
    }
}