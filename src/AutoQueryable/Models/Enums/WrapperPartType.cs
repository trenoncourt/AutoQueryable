using System;

namespace AutoQueryable.Models.Enums
{
    [Flags]
    public enum WrapperPartType : byte
    {
        Count = 1 << 1,
        NextLink = 1 << 2,
        TotalCount = 1 << 3
    }
}