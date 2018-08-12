using System;
using System.Collections.Generic;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses.ClauseHandlers
{
    public class DefaultOrderByClauseHandler : IOrderByClauseHandler
    {
        public Dictionary<string, bool> Handle(string orderByQueryStringPart, Type type = default, IAutoQueryableProfile profile = null)
        {
            var orderByValues = new Dictionary<string, bool>();
            foreach(var q in orderByQueryStringPart.Split(','))
            {
                if(q.StartsWith("-"))
                {
                    orderByValues.Add(q.Trim('-'), true);
                }else
                {
                    orderByValues.Add(q.Trim('+'), false);
                }
            }
            return orderByValues;
        }
    }
}