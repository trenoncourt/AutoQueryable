namespace AutoQueryable.Models.Enums
{
    public enum ClauseType : short
    {
        Select = 1 << 1,
        Top = 1 << 2,
        Take = 1 << 3,
        Skip = 1 << 4,
        OrderBy = 1 << 5,
        OrderByDesc = 1 << 6,
        Include = 1 << 7,
        GroupBy = 1 << 8,
        First = 1 << 9,
        Last = 1 << 10,
        WrapWith = 1 << 11
    }
}