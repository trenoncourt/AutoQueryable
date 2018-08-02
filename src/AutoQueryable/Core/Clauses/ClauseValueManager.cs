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
        private readonly IAutoQueryableProfile _profile;

        public ClauseValueManager(ISelectClauseHandler selectClauseHandler, IOrderByClauseHandler orderByClauseHandler, IWrapWithClauseHandler wrapWithClauseHandler, IAutoQueryableProfile profile)
        {
            _selectClauseHandler = selectClauseHandler;
            _orderByClauseHandler = orderByClauseHandler;
            _wrapWithClauseHandler = wrapWithClauseHandler;
            _profile = profile;
        }
        public ICollection<SelectColumn> Select { get; set; }
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

        public void SetDefaults(Type type)
        {
            Select = Select ?? _selectClauseHandler.Handle("", type, _profile);
            OrderBy = OrderBy ?? _orderByClauseHandler.Handle("", type, _profile);
            WrapWith = WrapWith ?? _wrapWithClauseHandler.Handle("", type, _profile);
        }
    }
}