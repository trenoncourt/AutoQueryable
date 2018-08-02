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

        private bool _useBaseTypeOverrided;
        private bool _useBaseType;
        public bool UseBaseType
        {
            get { return _useBaseType;}
            set
            {
                _useBaseTypeOverrided = true;
                _useBaseType = value;
            }
        }

        private bool _toListBeforeSelectOverrided;
        private bool _toListBeforeSelect;
        public bool ToListBeforeSelect
        {
            get { return _toListBeforeSelect;}
            set
            {
                _toListBeforeSelectOverrided = true;
                _toListBeforeSelect = value; 
            } 
        }
        
        public void OnActionExecuting(ActionExecutingContext context)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            
            _autoQueryableProfile.SelectableProperties = SelectableProperties;
            _autoQueryableProfile.UnselectableProperties = UnselectableProperties;
            _autoQueryableProfile.SortableProperties = SortableProperties;
            _autoQueryableProfile.UnSortableProperties = UnSortableProperties;
            _autoQueryableProfile.GroupableProperties = GroupableProperties;
            _autoQueryableProfile.UnGroupableProperties = UnGroupableProperties;
            if (_autoQueryableProfile.AllowedClauses.HasValue)
            {
                _autoQueryableProfile.AllowedClauses = AllowedClauses;
            }
            if (_autoQueryableProfile.DisAllowedClauses.HasValue)
            {
                _autoQueryableProfile.DisAllowedClauses = DisAllowedClauses;
            }
            if (_autoQueryableProfile.AllowedConditions.HasValue)
            {
                _autoQueryableProfile.AllowedConditions = AllowedConditions;
            }
            if (_autoQueryableProfile.DisAllowedConditions.HasValue)
            {
                _autoQueryableProfile.DisAllowedConditions = DisAllowedConditions;
            }
            if (_autoQueryableProfile.AllowedWrapperPartType.HasValue)
            {
                _autoQueryableProfile.AllowedWrapperPartType = AllowedWrapperPartType;
            }
            if (_autoQueryableProfile.DisAllowedWrapperPartType.HasValue)
            {
                _autoQueryableProfile.DisAllowedWrapperPartType = DisAllowedWrapperPartType;
            }
            if (_autoQueryableProfile.MaxToTake.HasValue)
            {
                _autoQueryableProfile.MaxToTake = MaxToTake;
            }

            if (DefaultToTake != 0)
            {
                _autoQueryableProfile.DefaultToTake = DefaultToTake;
            }

            if (DefaultToSelect != null)
            {
                _autoQueryableProfile.DefaultToSelect = DefaultToSelect;
            }

            if (_autoQueryableProfile.MaxToSkip.HasValue)
            {
                _autoQueryableProfile.MaxToSkip = MaxToSkip;
            }
            if (_autoQueryableProfile.MaxDepth.HasValue)
            {
                _autoQueryableProfile.MaxDepth = MaxDepth;
            }
            DefaultOrderBy = _autoQueryableProfile.DefaultOrderBy;
            
            if (_useBaseTypeOverrided)
            {
                _autoQueryableProfile.UseBaseType = UseBaseType;
            }
            if (_toListBeforeSelectOverrided)
            {
                _autoQueryableProfile.ToListBeforeSelect = ToListBeforeSelect;
            }
            
            dynamic query = ((ObjectResult)context.Result).Value;
            if (query == null) throw new Exception("Unable to retrieve value of IQueryable from context result.");
            context.Result = new OkObjectResult(_autoQueryableContext.GetAutoQuery(query));
        }
    }
}