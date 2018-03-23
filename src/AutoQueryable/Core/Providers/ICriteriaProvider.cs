using System;
using System.Collections.Generic;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Providers
{
    public interface ICriteriaProvider
    {
        IEnumerable<Criteria> GetCriterias(Type entityType, string[] queryStringParts, AutoQueryableProfile profile);
    }
}