using System;
using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Core.Models;

namespace AutoQueryable.Core.Clauses
{
    public class OrderByClauseHandler : IOrderByClauseHandler
    {
        public Dictionary<string, bool> Handle(string queryString, Type type = default(Type), IAutoQueryableProfile profile = null)
        {
            var orderByValues = new Dictionary<string, bool>();
            foreach(var q in queryString.Split(','))
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