using System;
using System.Text.RegularExpressions;
using AutoQueryable.Core.Enums;
using AutoQueryable.Core.Extensions;
using AutoQueryable.Core.Models;
using AutoQueryable.Core.Models.Clauses;
using AutoQueryable.Core.Providers;
using AutoQueryable.Extensions;
using AutoQueryable.Providers.Default.Aliases;

namespace AutoQueryable.Providers.Default
{
//    public class DefaultClauseProvider : IClauseProvider
//    {
//        public AllClauses GetClauses(string[] queryStringParts, AutoQueryableContext context)
//        {
//            var clauses = new AllClauses();
//            foreach (string q in queryStringParts)
//            {
//                if (q.Contains(ClauseAlias.Select, StringComparison.OrdinalIgnoreCase) && context.Profile.IsClauseAllowed(ClauseType.Select))
//                {
//                    clauses.Select = new SelectClause(context) { Value = GetOperandValue(q, ClauseAlias.Select)};
//                    clauses.Select.Parse();
//                }
//                else if (q.Contains(ClauseAlias.Top, StringComparison.OrdinalIgnoreCase) && context.Profile.IsClauseAllowed(ClauseType.Top))
//                {
//                    clauses.Top = GetClause(q, ClauseAlias.Top, ClauseType.Top, context);
//                }
//                else if (q.Contains(ClauseAlias.Take, StringComparison.OrdinalIgnoreCase) && context.Profile.IsClauseAllowed(ClauseType.Top))
//                {
//                    clauses.Top = GetClause(q, ClauseAlias.Take, ClauseType.Top, context);
//                }
//                else if (q.Contains(ClauseAlias.Skip, StringComparison.OrdinalIgnoreCase) && context.Profile.IsClauseAllowed(ClauseType.Skip))
//                {
//                    clauses.Skip = GetClause(q, ClauseAlias.Skip, ClauseType.Skip, context);
//                }
//                else if (q.Contains(ClauseAlias.First, StringComparison.OrdinalIgnoreCase) && context.Profile.IsClauseAllowed(ClauseType.First))
//                {
//                    clauses.First = new Clause(context) {ClauseType = ClauseType.First};
//                }
//                else if (q.Contains(ClauseAlias.Last, StringComparison.OrdinalIgnoreCase) && context.Profile.IsClauseAllowed(ClauseType.Last))
//                {
//                    clauses.Last = new Clause(context) {ClauseType = ClauseType.Last};
//                }
//                else if (q.Contains(ClauseAlias.OrderBy, StringComparison.OrdinalIgnoreCase) && context.Profile.IsClauseAllowed(ClauseType.OrderBy))
//                {
//                    clauses.OrderBy = GetClause(q, ClauseAlias.OrderBy, ClauseType.OrderBy, context);
//                }
//                else if (q.Contains(ClauseAlias.OrderByDesc, StringComparison.OrdinalIgnoreCase) && context.Profile.IsClauseAllowed(ClauseType.OrderByDesc))
//                {
//                    clauses.OrderByDesc = GetClause(q, ClauseAlias.OrderByDesc, ClauseType.OrderByDesc, context);
//                }
//                else if (q.Contains(ClauseAlias.WrapWith, StringComparison.OrdinalIgnoreCase) && context.Profile.IsClauseAllowed(ClauseType.WrapWith))
//                {
//                    clauses.WrapWith = GetClause(q, ClauseAlias.WrapWith, ClauseType.WrapWith, context);
//                }
//            }
//            return clauses;
//        }
//
//        private static string GetOperandValue(string q, string clauseAlias)
//        {
//            return Regex.Split(q, clauseAlias, RegexOptions.IgnoreCase)[1];
//        }
//
//        private static Clause GetClause(string q, string clauseAlias, ClauseType clauseType, AutoQueryableContext context)
//        {
//            string[] operands = Regex.Split(q, clauseAlias, RegexOptions.IgnoreCase);
//            var criteria = new Clause(context)
//            {
//                Value = operands[1],
//                ClauseType = clauseType
//            };
//            return criteria;
//        }
//    }
}