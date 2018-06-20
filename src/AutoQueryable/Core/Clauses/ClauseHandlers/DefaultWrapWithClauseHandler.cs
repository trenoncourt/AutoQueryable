using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses.ClauseHandlers
{
    public class DefaultWrapWithClauseHandler : IWrapWithClauseHandler
    {
        public IEnumerable<string> Handle(string wrapWithQueryStringPart, Type type = default, IAutoQueryableProfile profile = null)
        {
            return wrapWithQueryStringPart.Split(',').Select(s => s.ToLowerInvariant());
        }
    }
}
