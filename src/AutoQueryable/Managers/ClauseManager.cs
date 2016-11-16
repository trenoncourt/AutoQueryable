using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AutoQueryable.Aliases;
using AutoQueryable.Extensions;

namespace AutoQueryable.Managers
{
    public class ClauseManager
    {
        public IEnumerable<Clause> GetClauses(string[] queryStringParts)
        {
            foreach (string q in queryStringParts)
            {
                Clause clause = null;
                if (q.Contains(ClauseAlias.Select, StringComparison.OrdinalIgnoreCase))
                {
                    clause = GetClause(q, ClauseAlias.Select, ClauseType.Select);
                }
                if (clause != null)
                {
                    yield return clause;
                }
            }
        }

        private Clause GetClause(string q, string clauseAlias, ClauseType clauseType)
        {
            string[] operands = Regex.Split(q, clauseAlias, RegexOptions.IgnoreCase);
            var criteria = new Clause
            {
                Value = operands[1],
                ClauseType = clauseType
            };
            return criteria;
        }
    }
}