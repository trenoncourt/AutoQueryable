using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using AutoQueryable.Aliases;
using AutoQueryable.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoQueryable.Managers
{
    public class CriteriaManager
    {
        private int _criteriaIndex;

        public IEnumerable<Criteria> GetCriterias(IEntityType entityType, string[] queryStringParts)
        {
            foreach (string q in queryStringParts)
            {
                Criteria criteria = null;
                if (q.Contains(ConditionAlias.NotEqual, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.NotEqual, ConditionType.NotEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Less, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Less, ConditionType.Less, entityType);
                }
                else if (q.Contains(ConditionAlias.LessEqual, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.LessEqual, ConditionType.LessEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Greater, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Greater, ConditionType.Greater, entityType);
                }
                else if (q.Contains(ConditionAlias.GreaterEqual, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.GreaterEqual, ConditionType.GreaterEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Contains, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Contains, ConditionType.Contains, entityType);
                }
                else if (q.Contains(ConditionAlias.StartsWith, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.StartsWith, ConditionType.StartsWith, entityType);
                }
                else if (q.Contains(ConditionAlias.EndsWith, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.EndsWith, ConditionType.EndsWith, entityType);
                }
                else if (q.Contains(ConditionAlias.Between, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Between, ConditionType.Between, entityType);
                }
                else if (q.Contains(ConditionAlias.Equal, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Equal, ConditionType.Equal, entityType);
                }
                if (criteria != null)
                {
                    yield return criteria;
                }
            }
        }

        private Criteria GetCriteria(string q, string conditionAlias, ConditionType conditionType, IEntityType entityType)
        {
            string[] operands = Regex.Split(q, conditionAlias, RegexOptions.IgnoreCase);
            var criteria = new Criteria
            {
                Column = operands[0],
                ConditionType = conditionType
            };
            string[] operandValues = operands[1].Split(',');

            if (conditionType == ConditionType.Contains)
            {
                for (var i = 0; i < operandValues.Length; i++)
                {
                    operandValues[i] = $"%{operandValues[i]}%";
                }
            }
            
            criteria.DbParameters = operandValues.Select(v => new SqlParameter(criteria.Column + _criteriaIndex++, v)).ToList();
            IProperty property = entityType.GetProperties().FirstOrDefault(p => p.Name.Equals(criteria.Column, StringComparison.OrdinalIgnoreCase));
            if (property == null)
            {
                return null;
            }
            criteria.Column = property.Relational().ColumnName;
            return criteria;
        }
    }
}
