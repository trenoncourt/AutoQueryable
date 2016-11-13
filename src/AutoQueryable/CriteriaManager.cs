using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AutoQueryable.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoQueryable
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
                    criteria = GetCriteria(q, ConditionAlias.NotEqual, Condition.NotEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Less, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Less, Condition.Less, entityType);
                }
                else if (q.Contains(ConditionAlias.LessEqual, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.LessEqual, Condition.LessEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Greater, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Greater, Condition.Greater, entityType);
                }
                else if (q.Contains(ConditionAlias.GreaterEqual, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.GreaterEqual, Condition.GreaterEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Contains, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Contains, Condition.Contains, entityType);
                }
                else if (q.Contains(ConditionAlias.StartsWith, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.StartsWith, Condition.StartsWith, entityType);
                }
                else if (q.Contains(ConditionAlias.EndsWith, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.EndsWith, Condition.EndsWith, entityType);
                }
                else if (q.Contains(ConditionAlias.Between, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Between, Condition.Between, entityType);
                }
                else if (q.Contains(ConditionAlias.Equal, StringComparison.OrdinalIgnoreCase))
                {
                    criteria = GetCriteria(q, ConditionAlias.Equal, Condition.Equal, entityType);
                }
                if (criteria != null)
                {
                    yield return criteria;
                }
            }
        }

        private Criteria GetCriteria(string q, string conditionAlias, Condition condition, IEntityType entityType)
        {
            string[] operands = q.Split(new[] { conditionAlias }, StringSplitOptions.None);
            var criteria = new Criteria
            {
                Column = operands[0],
                Condition = condition
            };
            string[] operandValues = operands[1].Split(',');

            if (condition == Condition.Contains)
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
            criteria.Column = property.SqlServer().ColumnName;
            return criteria;
        }
    }
}
