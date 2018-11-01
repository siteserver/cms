using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class ErrorLogDao : DataProviderBase
    {
        public override string TableName => "siteserver_ErrorLog";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(ErrorLogInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(ErrorLogInfo.Category),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(ErrorLogInfo.PluginId),
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = nameof(ErrorLogInfo.Message),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(ErrorLogInfo.Stacktrace),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(ErrorLogInfo.Summary),
                DataType = DataType.Text
            },
            new TableColumn
            {
                AttributeName = nameof(ErrorLogInfo.AddDate),
                DataType = DataType.DateTime
            }
        };

        private const string ParmCategory = "@Category";
        private const string ParmPluginId = "@PluginId";
        private const string ParmMessage = "@Message";
        private const string ParmStacktrace = "@Stacktrace";
        private const string ParmSummary = "@Summary";
        private const string ParmAddDate = "@AddDate";

        public int Insert(ErrorLogInfo logInfo)
        {
            var sqlString = $"INSERT INTO {TableName} (Category, PluginId, Message, Stacktrace, Summary, AddDate) VALUES (@Category, @PluginId, @Message, @Stacktrace, @Summary, @AddDate)";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmCategory, DataType.VarChar, 50, logInfo.Category),
                GetParameter(ParmPluginId, DataType.VarChar, 200, logInfo.PluginId),
                GetParameter(ParmMessage, DataType.VarChar, 255, logInfo.Message),
                GetParameter(ParmStacktrace, DataType.Text, logInfo.Stacktrace),
                GetParameter(ParmSummary, DataType.Text, logInfo.Summary),
                GetParameter(ParmAddDate, DataType.DateTime, logInfo.AddDate),
            };

            return ExecuteNonQueryAndReturnId(TableName, nameof(ErrorLogInfo.Id), sqlString, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            var sqlString =
                $"DELETE FROM {TableName} WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            ExecuteNonQuery(sqlString);
        }

        public void DeleteIfThreshold()
        {
            if (!ConfigManager.SystemConfigInfo.IsTimeThreshold) return;

            var days = ConfigManager.SystemConfigInfo.TimeThreshold;
            if (days <= 0) return;

            ExecuteNonQuery($@"DELETE FROM {TableName} WHERE AddDate < {SqlUtils.GetComparableDateTime(DateTime.Now.AddDays(-days))}");
        }

        public void DeleteAll()
        {
            var sqlString = $"DELETE FROM {TableName}";

            ExecuteNonQuery(sqlString);
        }

        public ErrorLogInfo GetErrorLogInfo(int logId)
        {
            ErrorLogInfo logInfo = null;

            var sqlString = $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName} WHERE Id = @Id";

            var parms = new IDataParameter[]
            {
                GetParameter("@Id", DataType.Integer, logId)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    var i = 0;
                    logInfo = new ErrorLogInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                }
                rdr.Close();
            }

            return logInfo;
        }

        public KeyValuePair<string, string> GetMessageAndStacktrace(int logId)
        {
            var pair = new KeyValuePair<string, string>();

            var sqlString = $"SELECT Message, Stacktrace FROM {TableName} WHERE Id = {logId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    pair = new KeyValuePair<string, string>(GetString(rdr, 0), GetString(rdr, 1));
                }
                rdr.Close();
            }

            return pair;
        }

        public string GetSelectCommend(string category, string pluginId, string keyword, string dateFrom, string dateTo)
        {
            var whereString = new StringBuilder();

            if (!string.IsNullOrEmpty(category))
            {
                whereString.Append($"Category = '{AttackUtils.FilterSql(category)}'");
            }

            if (!string.IsNullOrEmpty(pluginId))
            {
                whereString.Append($"PluginId = '{AttackUtils.FilterSql(pluginId)}'");
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                if (whereString.Length > 0)
                {
                    whereString.Append(" AND ");
                }
                var filterKeyword = AttackUtils.FilterSql(keyword);
                var keywordId = TranslateUtils.ToInt(keyword);
                whereString.Append(keywordId > 0
                    ? $"Id = {keywordId}"
                    : $"(Message LIKE '%{filterKeyword}%' OR Stacktrace LIKE '%{filterKeyword}%' OR Summary LIKE '%{filterKeyword}%')");
            }
            if (!string.IsNullOrEmpty(dateFrom))
            {
                if (whereString.Length > 0)
                {
                    whereString.Append(" AND ");
                }
                whereString.Append($"AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))}");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                if (whereString.Length > 0)
                {
                    whereString.Append(" AND ");
                }
                whereString.Append($"AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))}");
            }

            return whereString.Length > 0
                ? $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName} WHERE {whereString}"
                : $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName}";
        }
    }
}
