using System;
using System.Collections.Generic;
using System.Text;
using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class PageClause : Clause
    {
        public PageClause(AutoQueryableContext context) : base(context)
        {
            this.ClauseType = ClauseType.Page;
        }

        public void Calculate()
        {
            
        }
    }
}
