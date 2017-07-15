namespace AutoQueryable.Models.Enums
{
    public enum WrapperPartType : byte
    {
        Count = 1 << 1,
        NextLink = 1 << 2,
        TotalCount = 1 << 3
    }
}