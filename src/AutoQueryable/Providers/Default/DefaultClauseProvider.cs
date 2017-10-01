using System;
using System.Text.RegularExpressions;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Providers;
using AutoQueryable.Extensions;
using AutoQueryable.Providers.Default.Aliases;

namespace AutoQueryable.Providers.Default
{
    public class DefaultClauseProvider : IClauseProvider
    {
        public Clauses GetClauses(string[] queryStringParts, AutoQueryableProfile profile)
        {
            var clauses = new Clauses();
            foreach (string q in queryStringParts)
            {
                if (q.Contains(ClauseAlias.Select, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.Select))
                {
                    clauses.Select = GetClause(q, ClauseAlias.Select, ClauseType.Select);
                }
                else if (q.Contains(ClauseAlias.Top, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.Top))
                {
                    clauses.Top = GetClause(q, ClauseAlias.Top, ClauseType.Top);
                }
                else if (q.Contains(ClauseAlias.Take, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.Top))
                {
                    clauses.Top = GetClause(q, ClauseAlias.Take, ClauseType.Top);
                }
                else if (q.Contains(ClauseAlias.Skip, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.Skip))
                {
                    clauses.Skip = GetClause(q, ClauseAlias.Skip, ClauseType.Skip);
                }
                else if (q.Contains(ClauseAlias.First, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.First))
                {
                    clauses.First = new Clause {ClauseType = ClauseType.First};
                }
                else if (q.Contains(ClauseAlias.Last, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.Last))
                {
                    clauses.Last = new Clause {ClauseType = ClauseType.Last};
                }
                else if (q.Contains(ClauseAlias.OrderBy, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.OrderBy))
                {
                    clauses.OrderBy = GetClause(q, ClauseAlias.OrderBy, ClauseType.OrderBy);
                }
                else if (q.Contains(ClauseAlias.OrderByDesc, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.OrderByDesc))
                {
                    clauses.OrderByDesc = GetClause(q, ClauseAlias.OrderByDesc, ClauseType.OrderByDesc);
                }
                else if (q.Contains(ClauseAlias.WrapWith, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.WrapWith))
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