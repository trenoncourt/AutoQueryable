using System;
using System.Text.RegularExpressions;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Providers;
using AutoQueryable.Providers.OData.Aliases;

namespace AutoQueryable.Providers.OData
{
    public class ODataClauseProvider : IClauseProvider
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
                else if (q.Contains(ClauseAlias.Skip, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.Skip))
                {
                    clauses.Skip = GetClause(q, ClauseAlias.Skip, ClauseType.Skip);
                }
                else if (q.Contains(ClauseAlias.OrderBy, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.OrderBy))
                {
                    clauses.OrderBy = GetClause(q, ClauseAlias.OrderBy, ClauseType.OrderBy);
                }
                else if (q.Contains(ClauseAlias.Filter, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.Filter))
                {
                    clauses.Filter = GetClause(q, ClauseAlias.Filter, ClauseType.Filter);
                }
                else if (q.Contains(ClauseAlias.Expand, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.Expand))
                {
                    clauses.Expand = GetClause(q, ClauseAlias.Expand, ClauseType.Expand);
                }
                else if (q.Contains(ClauseAlias.Search, StringComparison.OrdinalIgnoreCase) && profile.IsClauseAllowed(ClauseType.Search))
                {
                    clauses.Search = GetClause(q, ClauseAlias.Search, ClauseType.Search);
                }
            }
            return clauses;
        }

        private static Clause GetClause(string q, string clauseAlias, ClauseType clauseType)
        {
            string res = Regex.Replace(
                q,
                Regex.Escape(clauseAlias),
                "",
                RegexOptions.IgnoreCase
            );
            var criteria = new Clause
            {
                Value = res,
                ClauseType = clauseType
            };
            return criteria;
        }
    }
}