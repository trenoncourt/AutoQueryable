using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoQueryable.Aliases;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using AutoQueryable.Models.Enums;

namespace AutoQueryable.Managers
{
    public static class CriteriaManager
    {
        public static IEnumerable<Criteria> GetCriterias(Type entityType, string[] queryStringParts, AutoQueryableProfile profile)
        {
            foreach (string qPart in queryStringParts)
            {
                string q = WebUtility.UrlDecode(qPart);
                Criteria criteria = null;
                if (q.Contains(ConditionAlias.NotEqual, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.NotEqual))
                {
                    criteria = GetCriteria(q, ConditionAlias.NotEqual, ConditionType.NotEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.LessEqual, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.LessEqual))
                {
                    criteria = GetCriteria(q, ConditionAlias.LessEqual, ConditionType.LessEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Less, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.Less))
                {
                    criteria = GetCriteria(q, ConditionAlias.Less, ConditionType.Less, entityType);
                }
                else if (q.Contains(ConditionAlias.GreaterEqual, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.GreaterEqual))
                {
                    criteria = GetCriteria(q, ConditionAlias.GreaterEqual, ConditionType.GreaterEqual, entityType);
                }
                else if (q.Contains(ConditionAlias.Greater, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.Greater))
                {
                    criteria = GetCriteria(q, ConditionAlias.Greater, ConditionType.Greater, entityType);
                }
                else if (q.Contains(ConditionAlias.Contains, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.Contains))
                {
                    criteria = GetCriteria(q, ConditionAlias.Contains, ConditionType.Contains, entityType);
                }
                else if (q.Contains(ConditionAlias.StartsWith, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.StartsWith))
                {
                    criteria = GetCriteria(q, ConditionAlias.StartsWith, ConditionType.StartsWith, entityType);
                }
                else if (q.Contains(ConditionAlias.EndsWith, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.EndsWith))
                {
                    criteria = GetCriteria(q, ConditionAlias.EndsWith, ConditionType.EndsWith, entityType);
                }
                else if (q.Contains(ConditionAlias.Between, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.Between))
                {
                    criteria = GetCriteria(q, ConditionAlias.Between, ConditionType.Between, entityType);
                }
                else if (q.Contains(ConditionAlias.Equal, StringComparison.OrdinalIgnoreCase)  && profile.IsConditionAllowed(ConditionType.Equal))
                {
                    criteria = GetCriteria(q, ConditionAlias.Equal, ConditionType.Equal, entityType);
                }
                if (criteria != null)
                {
                    yield return criteria;
                }
            }
        }

        private static Criteria GetCriteria(string q, string conditionAlias, ConditionType conditionType, Type entityType)
        {
            string[] operands = Regex.Split(q, conditionAlias, RegexOptions.IgnoreCase);

            PropertyInfo property = null;
            var columnPath = new List<string>();
            var columns = operands[0].Split('.');
            foreach (var column in columns)
            {
                if (property == null)
                {
                    property = entityType.GetProperties().FirstOrDefault(p => p.Name.Equals(column, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    var isCollection = property.PropertyType.GetInterfaces().Any(x => x.Name == "IEnumerable");
                    if (isCollection)
                    {
                        var childType = property.PropertyType.GetGenericArguments()[0];
                        property = childType.GetProperties().FirstOrDefault(p => p.Name.Equals(column, StringComparison.OrdinalIgnoreCase));
                    }
                    else
                    {
                        property = property.PropertyType.GetProperties().FirstOrDefault(p => p.Name.Equals(column, StringComparison.OrdinalIgnoreCase));

                    }
                }

                if (property == null)
                {
                    return null;
                }
                columnPath.Add(property.Name);
            }
            var criteria = new Criteria
            {
                ColumnPath = columnPath,
                ConditionType = conditionType,
                Values = operands[1].Split(',')
            };
            return criteria;
        }
    }
}
