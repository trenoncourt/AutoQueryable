using System.Collections.Generic;

namespace AutoQueryable.Core.Models
{
    public interface IQueryStringAccessor
    {
        string QueryString { get; }
        ICollection<QueryStringPart> QueryStringParts { get; }
    }
}