using System;
using System.Collections.Generic;
using System.Text;
using AutoQueryable.Core.Enums;

namespace AutoQueryable.Core.Models.Clauses
{
    public class PageSizeClause : Clause
    {
        public PageSizeClause()
        {
            ClauseType = ClauseType.PageSize;
        }
    }
}
