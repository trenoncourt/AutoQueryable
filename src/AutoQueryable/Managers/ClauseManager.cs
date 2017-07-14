using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AutoQueryable.Aliases;
using AutoQueryable.Extensions;
using AutoQueryable.Models;

namespace AutoQueryable.Managers
{
    public static class ClauseManager
    {
        public static IEnumerable<Clause> GetClauses(string[] queryStringParts)
        {
            foreach (string q in queryStringParts)
            {
                Clause clause = null;
                if (q.Contains(ClauseAlias.Select, StringComparison.OrdinalIgnoreCase))
                {
                    clause = GetClause(q, ClauseAlias.Select, ClauseType.Select);
                }
                else if (q.Contains(ClauseAlias.Top, StringComparison.OrdinalIgnoreCase))
                {
                    clause = GetClause(q, ClauseAlias.Top, ClauseType.Top);
                }
                else if (q.Contains(ClauseAlias.Take, StringComparison.OrdinalIgnoreCase))
                {
                    clause = GetClause(q, ClauseAlias.Take, ClauseType.Top);
                }
                else if (q.Contains(ClauseAlias.Skip, StringComparison.OrdinalIgnoreCase))
                {
                    clause = GetClause(q, ClauseAlias.Skip, ClauseType.Skip);
                }
                else if (q.Contains(ClauseAlias.First, StringComparison.OrdinalIgnoreCase))
                {
                    clause = new Clause {ClauseType = ClauseType.First};
                }
                else if (q.Contains(ClauseAlias.Last, StringComparison.OrdinalIgnoreCase))
                {
                    clause = new Clause {ClauseType = ClauseType.Last};
                }
                else if (q.Contains(ClauseAlias.OrderBy, StringComparison.OrdinalIgnoreCase))
                {
                    clause = GetClause(q, ClauseAlias.OrderBy, ClauseType.OrderBy);
                }
                else if (q.Contains(ClauseAlias.OrderByDesc, StringComparison.OrdinalIgnoreCase))
                {
                    clause = GetClause(q, ClauseAlias.OrderByDesc, ClauseType.OrderByDesc);
                }
                else if (q.Contains(ClauseAlias.WrapWith, StringComparison.OrdinalIgnoreCase))
                {
                    clause = GetClause(q, ClauseAlias.WrapWith, ClauseType.WrapWith);
                }
                if (clause != null)
                {
                    yield return clause;
                }
            }
        }

        private static Clause GetClause(string q, string clauseAlias, ClauseType clauseType)
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