using System;
using System.Collections.Generic;
using AutoQueryable.Core.Clauses.ClauseHandlers;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public class ClauseValueManager : IClauseValueManager
    {
        private readonly ISelectClauseHandler _selectClauseHandler;
        private readonly IOrderByClauseHandler _orderByClauseHandler;
        private readonly IWrapWithClauseHandler _wrapWithClauseHandler;

        public ClauseValueManager(ISelectClauseHandler selectClauseHandler, IOrderByClauseHandler orderByClauseHandler, IWrapWithClauseHandler wrapWithClauseHandler)
        {
            _selectClauseHandler = selectClauseHandler;
            _orderByClauseHandler = orderByClauseHandler;
            _wrapWithClauseHandler = wrapWithClauseHandler;
        }
        public ICollection<SelectColumn> Select { get; set; } = new List<SelectColumn>();
        public int? Take { get; set; }
        public int? Top { get; set; }

        public int? Skip { get; set; }
        
        public Dictionary<string, bool> OrderBy { get; set; } = new Dictionary<string, bool>();
        
        public string GroupBy { get; set; }
        
        public bool First { get; set; }
        
        public bool Last { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public IEnumerable<string> WrapWith { get; set; }

        public void SetDefaults(Type type, IAutoQueryableProfile profile)
        {
            OrderBy = profile.DefaultOrderBy;
            Select = _selectClauseHandler.Handle("", type, profile);
            OrderBy = _orderByClauseHandler.Handle("", type, profile);
            WrapWith = _wrapWithClauseHandler.Handle("", type, profile);
        }
    }
}