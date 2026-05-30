using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Text.RegularExpressions;
using Dapper;
using Datory;
using Datory.Utils;
using SqlKata;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Core.Services
{
    public partial class DatabaseManager
    {
        public static bool IsReadOnlySelectSql(string sqlString)
        {
            var sql = GetSqlOutsideLiteralsAndComments(sqlString);
            if (string.IsNullOrWhiteSpace(sql)) return false;
            if (sql.Contains(';')) return false;

            var normalized = sql.Trim();
            if (!Regex.IsMatch(normalized, @"^(select|with)\b", RegexOptions.IgnoreCase))
            {
                return false;
            }

            if (Regex.IsMatch(normalized, @"\b(insert|update|delete|drop|alter|create|truncate|merge|exec|execute|grant|revoke|backup|restore|call)\b", RegexOptions.IgnoreCase))
            {
                return false;
            }

            if (Regex.IsMatch(normalized, @"\binto\s+(out|dump)?file\b", RegexOptions.IgnoreCase))
            {
                return false;
            }

            return !Regex.IsMatch(normalized, @"^select\b[\s\S]*\binto\b", RegexOptions.IgnoreCase);
        }

        public static bool IsSafeRawSqlCondition(string condition)
        {
            if (string.IsNullOrWhiteSpace(condition)) return false;

            var sql = GetSqlOutsideLiteralsAndComments(condition, out var hasComment, out var hasUnterminatedLiteral);
            if (hasComment || hasUnterminatedLiteral) return false;

            var normalized = sql.Trim();
            if (string.IsNullOrWhiteSpace(normalized)) return false;
            if (normalized.Contains(';')) return false;

            return !Regex.IsMatch(normalized, @"\b(select|with|union|intersect|except|insert|update|delete|drop|alter|create|truncate|merge|exec|execute|grant|revoke|backup|restore|call)\b", RegexOptions.IgnoreCase);
        }

        private static string GetSqlOutsideLiteralsAndComments(string sqlString)
        {
            return GetSqlOutsideLiteralsAndComments(sqlString, out _, out _);
        }

        private static string GetSqlOutsideLiteralsAndComments(string sqlString, out bool hasComment, out bool hasUnterminatedLiteral)
        {
            hasComment = false;
            hasUnterminatedLiteral = false;
            if (string.IsNullOrEmpty(sqlString)) return string.Empty;

            var builder = new StringBuilder(sqlString.Length);
            var quote = '\0';
            var lineComment = false;
            var blockComment = false;

            for (var i = 0; i < sqlString.Length; i++)
            {
                var c = sqlString[i];
                var next = i + 1 < sqlString.Length ? sqlString[i + 1] : '\0';

                if (lineComment)
                {
                    if (c == '\r' || c == '\n')
                    {
                        lineComment = false;
                        builder.Append(c);
                    }
                    else
                    {
                        builder.Append(' ');
                    }
                    continue;
                }

                if (blockComment)
                {
                    if (c == '*' && next == '/')
                    {
                        blockComment = false;
                        builder.Append("  ");
                        i++;
                    }
                    else
                    {
                        builder.Append(' ');
                    }
                    continue;
                }

                if (quote != '\0')
                {
                    builder.Append(' ');
                    if (c == quote)
                    {
                        if (quote == '\'' && next == '\'')
                        {
                            builder.Append(' ');
                            i++;
                            continue;
                        }
                        quote = '\0';
                    }
                    continue;
                }

                if (c == '-' && next == '-')
                {
                    lineComment = true;
                    hasComment = true;
                    builder.Append("  ");
                    i++;
                    continue;
                }

                if (c == '/' && next == '*')
                {
                    blockComment = true;
                    hasComment = true;
                    builder.Append("  ");
                    i++;
                    continue;
                }

                if (c == '\'' || c == '"' || c == '`')
                {
                    quote = c;
                    builder.Append(' ');
                    continue;
                }

                builder.Append(c);
            }

            hasUnterminatedLiteral = quote != '\0' || blockComment;
            return builder.ToString();
        }

        public async Task<List<KeyValuePair<int, IDictionary<string, object>>>> ParserGetSqlDataSourceAsync(DatabaseType databaseType, string connectionString, string queryString)
        {
            if (!IsReadOnlySelectSql(queryString))
            {
                throw new InvalidOperationException("Only read-only SELECT SQL is allowed.");
            }

            var rows = new List<KeyValuePair<int, IDictionary<string, object>>>();
            var itemIndex = 0;
            using (var connection = GetConnection(databaseType, connectionString))
            {
                using var reader = await connection.ExecuteReaderAsync(queryString);
                while (reader.Read())
                {
                    var dict = new Dictionary<string, object>();
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        dict[reader.GetName(i)] = reader.GetValue(i);
                    }
                    rows.Add(new KeyValuePair<int, IDictionary<string, object>>(itemIndex, dict));
                }
            }
            return rows;
        }

        public async Task<List<KeyValuePair<int, IDictionary<string, object>>>> ParserGetSqlDataSourceAsync(DatabaseType databaseType, string connectionString, Query query)
        {
            var rows = new List<KeyValuePair<int, IDictionary<string, object>>>();

            var component = query.GetOneComponent<FromClause>("from");
            if (component != null)
            {
                var tableName = component.Table;

                var database = new Database(databaseType, connectionString);
                var columns = await database.GetTableColumnsAsync(tableName);
                var repository = new Repository(database, tableName, columns);

                var list = await repository.GetAllAsync<object>(query);
                var itemIndex = 0;
                foreach (var row in list)
                {
                    var fields = row as IDictionary<string, object>;
                    rows.Add(new KeyValuePair<int, IDictionary<string, object>>(itemIndex, fields));
                }
            }

            return rows;
        }

        public string GetContentOrderByString(TaxisType taxisType)
        {
            return GetContentOrderByString(taxisType, string.Empty);
        }

        private string Quote(string identifier)
        {
            return _settingsManager.Database.GetQuotedIdentifier(identifier);
        }

        public string GetContentOrderByString(TaxisType taxisType, string orderByString)
        {
            if (!string.IsNullOrEmpty(orderByString))
            {
                if (orderByString.Trim().ToUpper().StartsWith("ORDER BY "))
                {
                    return orderByString;
                }
                return "ORDER BY " + orderByString;
            }

            var retVal = string.Empty;

            if (taxisType == TaxisType.OrderById)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.Id))} ASC";
            }
            else if (taxisType == TaxisType.OrderByIdDesc)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByChannelId)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.ChannelId))} ASC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByChannelIdDesc)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.ChannelId))} DESC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByAddDate)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.AddDate))} ASC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByAddDateDesc)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.AddDate))} DESC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByLastModifiedDate)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.LastModifiedDate))} ASC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByLastModifiedDateDesc)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.LastModifiedDate))} DESC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByTaxis)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.Taxis))} ASC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByTaxisDesc)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Top))} DESC, {Quote(nameof(Content.Taxis))} DESC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByHits)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.Hits))} DESC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByRandom)
            {
                var orderBy = DbUtils.GetOrderByRandomString(_settingsManager.DatabaseType);
                if (!string.IsNullOrEmpty(orderBy))
                {
                    retVal = $"ORDER BY {orderBy}";
                }
            }

            return retVal;
        }
    }
}
