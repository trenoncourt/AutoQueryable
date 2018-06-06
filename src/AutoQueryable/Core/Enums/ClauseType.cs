using System;

namespace AutoQueryable.Core.Enums
{
    [Flags]
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