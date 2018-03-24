using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Providers
{
    public interface IClauseProvider
    {
        AllClauses GetClauses(string[] queryStringParts, AutoQueryableContext context);
    }
}