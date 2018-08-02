using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Serilog;


namespace AutoQueryable.AspNetCore.Filter.FilterAttributes
{
    public class AutoQueryableAttribute : Attribute, IFilterFactory, IAutoQueryableAttribute
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

        public bool IsReusable { get; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var autoQueryableFilter = serviceProvider.GetService<AutoQueryableFilter>();
            if (SelectableProperties != null)
            {
                autoQueryableFilter.SelectableProperties = SelectableProperties;
            }
            if (UnselectableProperties != null)
            {
                autoQueryableFilter.UnselectableProperties = UnselectableProperties;
            }
            if (SortableProperties != null)
            {
                autoQueryableFilter.SortableProperties = SortableProperties;
            }
            if (UnSortableProperties != null)
            {
                autoQueryableFilter.UnSortableProperties = UnSortableProperties;
            }
            if (GroupableProperties != null)
            {
                autoQueryableFilter.GroupableProperties = GroupableProperties;
            }
            if (UnGroupableProperties != null)
            {
                autoQueryableFilter.UnGroupableProperties = UnGroupableProperties;
            }
            if (AllowedClauses != ClauseType.None)
            {
                autoQueryableFilter.AllowedClauses = AllowedClauses;
            }
            if (DisAllowedClauses != ClauseType.None)
            {
                autoQueryableFilter.DisAllowedClauses = DisAllowedClauses;
            }
            if (AllowedConditions != ConditionType.None)
            {
                autoQueryableFilter.AllowedConditions = AllowedConditions;
            }
            if (DisAllowedConditions != ConditionType.None)
            {
                autoQueryableFilter.DisAllowedConditions = DisAllowedConditions;
            }
            if (AllowedWrapperPartType != WrapperPartType.None)
            {
                autoQueryableFilter.AllowedWrapperPartType = AllowedWrapperPartType;
            }
            if (DisAllowedWrapperPartType != WrapperPartType.None)
            {
                autoQueryableFilter.DisAllowedWrapperPartType = DisAllowedWrapperPartType;
            }
            if (MaxToTake != 0)
            {
                autoQueryableFilter.MaxToTake = MaxToTake;
            }
            if (DefaultToTake != 0)
            {
                autoQueryableFilter.DefaultToTake = DefaultToTake;
            }
            if (MaxToSkip != 0)
            {
                autoQueryableFilter.MaxToSkip = MaxToSkip;
            }
            if (MaxDepth != 0)
            {
                autoQueryableFilter.MaxDepth = MaxDepth;
            }
            if (DefaultOrderBy != null)
            {
                autoQueryableFilter.DefaultOrderBy = DefaultOrderBy;
            }
            if (_useBaseTypeOverrided)
            {
                autoQueryableFilter.UseBaseType = UseBaseType;
            }
            if (_toListBeforeSelectOverrided)
            {
                autoQueryableFilter.ToListBeforeSelect = ToListBeforeSelect;
            }
            if (DefaultToSelect != null)
            {
                autoQueryableFilter.DefaultToSelect = DefaultToSelect;
            }

            return autoQueryableFilter;
        }
    }
}