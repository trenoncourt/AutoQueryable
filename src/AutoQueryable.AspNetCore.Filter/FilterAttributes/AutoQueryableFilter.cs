using System;
using System.Collections.Generic;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AutoQueryable.AspNetCore.Filter.FilterAttributes
{
    public class AutoQueryableFilter : IActionFilter
    {
        private readonly IAutoQueryableContext _autoQueryableContext;
        private readonly IAutoQueryableProfile _autoQueryableProfile;

        public AutoQueryableFilter(IAutoQueryableContext autoQueryableContext, IAutoQueryableProfile autoQueryableProfile)
        {
            _autoQueryableContext = autoQueryableContext;
            _autoQueryableProfile = autoQueryableProfile;

            SelectableProperties = autoQueryableProfile.SelectableProperties;
            UnselectableProperties = autoQueryableProfile.UnselectableProperties;
            SortableProperties = autoQueryableProfile.SortableProperties;
            UnSortableProperties = autoQueryableProfile.UnSortableProperties;
            GroupableProperties = autoQueryableProfile.GroupableProperties;
            UnGroupableProperties = autoQueryableProfile.UnGroupableProperties;
            if (autoQueryableProfile.AllowedClauses.HasValue)
            {
                AllowedClauses = autoQueryableProfile.AllowedClauses.Value;
            }
            if (autoQueryableProfile.DisAllowedClauses.HasValue)
            {
                DisAllowedClauses = autoQueryableProfile.DisAllowedClauses.Value;
            }
            if (autoQueryableProfile.AllowedConditions.HasValue)
            {
                AllowedConditions = autoQueryableProfile.AllowedConditions.Value;
            }
            if (autoQueryableProfile.DisAllowedConditions.HasValue)
            {
                DisAllowedConditions = autoQueryableProfile.DisAllowedConditions.Value;
            }
            if (autoQueryableProfile.AllowedWrapperPartType.HasValue)
            {
                AllowedWrapperPartType = autoQueryableProfile.AllowedWrapperPartType.Value;
            }
            if (autoQueryableProfile.DisAllowedWrapperPartType.HasValue)
            {
                DisAllowedWrapperPartType = autoQueryableProfile.DisAllowedWrapperPartType.Value;
            }
            if (autoQueryableProfile.MaxToTake.HasValue)
            {
                MaxToTake = autoQueryableProfile.MaxToTake.Value;
            }
            DefaultToTake = autoQueryableProfile.DefaultToTake;
            DefaultToSelect = autoQueryableProfile.DefaultToSelect;
            if (autoQueryableProfile.MaxToSkip.HasValue)
            {
                MaxToSkip = autoQueryableProfile.MaxToSkip.Value;
            }
            if (autoQueryableProfile.MaxDepth.HasValue)
            {
                MaxDepth = autoQueryableProfile.MaxDepth.Value;
            }
            DefaultOrderBy = autoQueryableProfile.DefaultOrderBy;
            UseBaseType = autoQueryableProfile.UseBaseType;
            ToListBeforeSelect = autoQueryableProfile.ToListBeforeSelect;
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

        public string DefaultToSelect { get; set; }

        public int MaxToSkip { get; set; }

        public int MaxDepth { get; set; }
        
        public Dictionary<string, bool> DefaultOrderBy { get; set; }
       
        public ProviderType ProviderType { get; set; }

        public bool UseBaseType { get; set; }
        public bool ToListBeforeSelect { get; set; }
        
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            dynamic query = ((ObjectResult)context.Result).Value;
            if (query == null) throw new Exception("Unable to retrieve value of IQueryable from context result.");
            context.Result = new OkObjectResult(_autoQueryableContext.GetAutoQuery(query));
        }
    }
}