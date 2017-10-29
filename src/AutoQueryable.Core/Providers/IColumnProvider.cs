using System;
using System.Collections.Generic;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Providers
{
    public interface IColumnProvider
    {
        IEnumerable<SelectColumn> GetSelectableColumns(Clauses clauses, AutoQueryableProfile profile, Type entityType);
    }
}