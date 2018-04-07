using System;

namespace AutoQueryable.Core.Enums
{
    [Flags]
    public enum ConditionType : short
    {
        None = 0,
        Equal = 1 << 1,
        NotEqual = 1 << 2,
        Less = 1 << 3,
        LessEqual = 1 << 4,
        Greater = 1 << 5,
        GreaterEqual = 1 << 6,
        Contains = 1 << 7,
        StartsWith = 1 << 8,
        EndsWith = 1 << 9,
        Between = 1 << 10
    }
}