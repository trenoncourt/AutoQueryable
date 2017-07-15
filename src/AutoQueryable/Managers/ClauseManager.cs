using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AutoQueryable.Aliases;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using AutoQueryable.Models.Enums;

namespace AutoQueryable.Managers
{
    public static class ClauseManager
    {
        public static Clauses GetClauses(string[] queryStringParts)
        {
            var clauses = new Clauses();
            foreach (string q in queryStringParts)
            {
                if (q.Contains(ClauseAlias.Select, StringComparison.OrdinalIgnoreCase))
                {
                    clauses.Select = GetClause(q, ClauseAlias.Select, ClauseType.Select);
                }
                else if (q.Contains(ClauseAlias.Top, StringComparison.OrdinalIgnoreCase))
                {
                    clauses.Top = GetClause(q, ClauseAlias.Top, ClauseType.Top);
                }
                else if (q.Contains(ClauseAlias.Take, StringComparison.OrdinalIgnoreCase))
                {
                    clauses.Top = GetClause(q, ClauseAlias.Take, ClauseType.Top);
                }
                else if (q.Contains(ClauseAlias.Skip, StringComparison.OrdinalIgnoreCase))
                {
                    clauses.Skip = GetClause(q, ClauseAlias.Skip, ClauseType.Skip);
                }
                else if (q.Contains(ClauseAlias.First, StringComparison.OrdinalIgnoreCase))
                {
                    clauses.First = new Clause {ClauseType = ClauseType.First};
                }
                else if (q.Contains(ClauseAlias.Last, StringComparison.OrdinalIgnoreCase))
                {
                    clauses.Last = new Clause {ClauseType = ClauseType.Last};
                }
                else if (q.Contains(ClauseAlias.OrderBy, StringComparison.OrdinalIgnoreCase))
                {
                    clauses.OrderBy = GetClause(q, ClauseAlias.OrderBy, ClauseType.OrderBy);
                }
                else if (q.Contains(ClauseAlias.OrderByDesc, StringComparison.OrdinalIgnoreCase))
                {
                    clauses.OrderByDesc = GetClause(q, ClauseAlias.OrderByDesc, ClauseType.OrderByDesc);
                }
                else if (q.Contains(ClauseAlias.WrapWith, StringComparison.OrdinalIgnoreCase))
                {
                    clauses.WrapWith = GetClause(q, ClauseAlias.WrapWith, ClauseType.WrapWith);
                }
            }
            return clauses;
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