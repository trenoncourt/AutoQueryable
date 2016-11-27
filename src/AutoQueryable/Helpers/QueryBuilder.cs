using System.Collections.Generic;
using System.Linq;
using AutoQueryable.Extensions;
using AutoQueryable.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AutoQueryable.Helpers
{
    public static class QueryBuilder
    {
        public static dynamic Build<T>(IQueryable<T> query, IEntityType entityType, IRelationalEntityTypeAnnotations table, IList<Clause> clauses, IList<Criteria> criterias, string[] unselectableProperties) where T : class
        {
            Clause selectClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Select);
            Clause topClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Top);
            Clause skipClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Skip);
            Clause firstClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.First);
            Clause lastClause = clauses.FirstOrDefault(c => c.ClauseType == ClauseType.Last);

            List<Column> columns = SelectHelper.GetSelectableColumns(selectClause, unselectableProperties, entityType).ToList();
            //columns.RemoveAll(c => unselectableProperties.Contains(c.PropertyName));
            string selectClauseValue = "*";
            if (columns != null && columns.Any())
                selectClauseValue = string.Join(",", columns.Select(c => c.ColumnName));
            
            string whereClauseString = "";
            if (criterias.Any())
            {
                whereClauseString = " WHERE " + string.Join(" AND ", criterias.Select((c, index) => c.ConditionType.ToSqlCondition(c.Column, c.DbParameters)));
            }
            string selectClauseString = $"SELECT {selectClauseValue} FROM {table.Schema + "."}[{table.TableName}]";

            string sqlRequest = selectClauseString + whereClauseString;

            query = query.FromSql(sqlRequest, criterias.SelectMany(c => c.DbParameters).ToArray());

            if (skipClause != null)
            {
                int skip;
                int.TryParse(skipClause.Value, out skip);
                query = query.Skip(skip);
            }
            if (topClause != null)
            {
                int take;
                int.TryParse(topClause.Value, out take);
                query = query.Take(take);
            }

            var queryProjection = query.Select(SelectHelper.GetSelector<T>(string.Join(",", columns.Select(c => c.PropertyName))));

            if (firstClause != null)
            {
                return queryProjection.FirstOrDefault();
            }
            if (lastClause != null)
            {
                return queryProjection.LastOrDefault();
            }
            return queryProjection;
        }
    }
}