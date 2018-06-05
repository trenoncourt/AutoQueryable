using System;
using System.ComponentModel;

namespace AutoQueryable.Core.Enums
{
    public enum ClauseType
    {
        None,
        Select,
        Top,
        Skip,
        Page,
        PageSize,
        OrderBy,
        OrderByDesc,
        GroupBy,
        First,
        Last,
        WrapWith,
        Filter
    }
}