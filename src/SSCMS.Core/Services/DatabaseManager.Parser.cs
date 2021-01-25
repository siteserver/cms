using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Datory;
using SqlKata;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Core.Services
{
    public partial class DatabaseManager
    {
        public async Task<List<KeyValuePair<int, IDictionary<string, object>>>> ParserGetSqlDataSourceAsync(DatabaseType databaseType, string connectionString, string queryString)
        {
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
            else if (taxisType == TaxisType.OrderByHitsByDay)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.HitsByDay))} DESC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByHitsByWeek)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.HitsByWeek))} DESC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByHitsByMonth)
            {
                retVal = $"ORDER BY {Quote(nameof(Content.HitsByMonth))} DESC, {Quote(nameof(Content.Id))} DESC";
            }
            else if (taxisType == TaxisType.OrderByRandom)
            {
                //retVal = SqlUtils.GetOrderByRandom();
            }

            return retVal;
        }
    }
}
