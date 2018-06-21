namespace AutoQueryable.Core.Aliases
{
    public static class ConditionAlias
    {
        public const string Equal = "=";
        public const string NotEqual = "!=";
        public const string LessThan = "<";
        public const string LessThanOrEqual = "<=";
        public const string GreaterThan = ">";
        public const string GreaterThanOrEqual = ">=";

        public const string Contains = "Contains=";
        public const string NotContains = "Contains!=";
        public const string StartsWith = "StartsWith=";
        public const string NotStartsWith = "StartsWith!=";
        public const string EndsWith = "EndsWith=";
        public const string NotEndsWith = "EndsWith!=";

        public const string ContainsIgnoreCase = "Contains:i=";
        public const string NotContainsIgnoreCase = "Contains:i!=";
        public const string StartsWithIgnoreCase = "StartsWith:i=";
        public const string NotStartsWithIgnoreCase = "StartsWith:i!=";
        public const string EndsWithIgnoreCase = "EndsWith:i=";
        public const string NotEndsWithIgnoreCase = "EndsWith:i!=";

        public const string DateInYear = ":Year=";
        public const string DateNotInYear = ":Year!=";
    }
}
