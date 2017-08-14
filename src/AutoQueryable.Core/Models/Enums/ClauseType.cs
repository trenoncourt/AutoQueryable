using System;

namespace AutoQueryable.Core.Models.Enums
{
    [Flags]
    public enum ClauseType : short
    {
        Select = 1 << 1,
        Top = 1 << 2,
        Skip = 1 << 3,
        OrderBy = 1 << 4,
        OrderByDesc = 1 << 5,
        GroupBy = 1 << 6,
        First = 1 << 7,
        Last = 1 << 8,
        WrapWith = 1 << 9,
        Filter = 1 << 10
    }
}