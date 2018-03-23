using System.Collections.Generic;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Providers
{
    public interface IWrapperProvider
    {
        IEnumerable<WrapperPartType> GetWrapperParts(string[] queryStringWrapperParts, AutoQueryableProfile profile);
    }
}