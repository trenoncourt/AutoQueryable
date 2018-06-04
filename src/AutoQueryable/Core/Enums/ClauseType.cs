using System;

namespace AutoQueryable.Core.Enums
{
    public static class ClauseType
    {
        public const string Select = "Select";
        public const string Top = "Top";
        public const string Skip = "Skip";
        public const string Page = "Page";
        public const string PageSize = "PageSize";
        public const string OrderBy = "OrderBy";
        public const string First = "First";
        public const string Last = "Last";
        public const string GroupBy = "GroupBy";
    }
}