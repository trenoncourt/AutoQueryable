using System;
using System.Collections.Generic;
using AutoQueryable.Aliases;
using AutoQueryable.Models;
using AutoQueryable.Extensions;

namespace AutoQueryable.Managers
{
    public class WrapperManager
    {
        public static IEnumerable<WrapperPartType> GetWrapperParts(string[] queryStringWrapperParts)
        {
            foreach (string q in queryStringWrapperParts)
            {
                if (q.Contains(WrapperAlias.Count, StringComparison.OrdinalIgnoreCase))
                {
                    yield return WrapperPartType.Count;
                }
                else if (q.Contains(WrapperAlias.NextLink, StringComparison.OrdinalIgnoreCase))
                {
                    yield return WrapperPartType.NextLink;
                }
            }
        }
    }
}