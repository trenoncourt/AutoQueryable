using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Providers
{
    public interface IClauseProvider
    {
        Clauses GetClauses(string[] queryStringParts, AutoQueryableProfile profile);
    }
}