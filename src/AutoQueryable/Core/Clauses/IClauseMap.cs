namespace AutoQueryable.Core.Clauses
{
    public interface IClauseMap
    {
        IClause GetClause(string alias);
        IClause FindClause(string queryParameterKey);
    }
}