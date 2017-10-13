using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using SiteServer.Plugin.Models;

namespace BaiRong.Core.Provider
{
    public class ErrorLogDao : DataProviderBase
    {
        public override string TableName => "bairong_ErrorLog";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(ErrorLogInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ErrorLogInfo.PluginId),
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ErrorLogInfo.Message),
                DataType = DataType.NVarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ErrorLogInfo.Stacktrace),
                DataType = DataType.NText
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ErrorLogInfo.Summary),
                DataType = DataType.NText
            },
            new TableColumnInfo
            {
                ColumnName = nameof(ErrorLogInfo.AddDate),
                DataType = DataType.DateTime
            }
        };

        private const string ParmPluginId = "@PluginId";
        private const string ParmMessage = "@Message";
        private const string ParmStacktrace = "@Stacktrace";
        private const string ParmSummary = "@Summary";
        private const string ParmAddDate = "@AddDate";

        public void Insert(ErrorLogInfo logInfo)
        {
            var sqlString = $"INSERT INTO {TableName} (PluginId, Message, Stacktrace, Summary, AddDate) VALUES (@PluginId, @Message, @Stacktrace, @Summary, @AddDate)";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmPluginId, DataType.VarChar, 200, logInfo.PluginId),
                GetParameter(ParmMessage, DataType.NVarChar, 255, logInfo.Message),
                GetParameter(ParmStacktrace, DataType.NText, logInfo.Stacktrace),
                GetParameter(ParmSummary, DataType.NText, logInfo.Summary),
                GetParameter(ParmAddDate, DataType.DateTime, logInfo.AddDate),
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            var sqlString =
                $"DELETE FROM {TableName} WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            ExecuteNonQuery(sqlString);
        }

        public void Delete(int days)
        {
            if (days <= 0) return;
            ExecuteNonQuery($@"DELETE FROM {TableName} WHERE AddDate < '{DateUtils.GetDateAndTimeString(DateTime.Now.AddDays(-days))}'");
        }

        public void DeleteAll()
        {
            var sqlString = $"DELETE FROM {TableName}";

            ExecuteNonQuery(sqlString);
        }

        public int GetCount()
        {
            var count = 0;
            var sqlString = $"SELECT Count(*) FROM {TableName}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return count;
        }

        public string GetSelectCommend(string pluginId, string keyword, string dateFrom, string dateTo)
        {
            var whereString = new StringBuilder($"PluginId = '{PageUtils.FilterSql(pluginId)}'");

            if (!string.IsNullOrEmpty(keyword))
            {
                var filterKeyword = PageUtils.FilterSql(keyword);
                whereString.Append(
                    $" AND (Message LIKE '%{filterKeyword}%' OR Stacktrace LIKE '%{filterKeyword}%' OR Summary LIKE '%{filterKeyword}%')");
            }
            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereString.Append($" AND AddDate >= '{dateFrom}'");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereString.Append($" AND AddDate <= '{dateTo}'");
            }

            return $"SELECT Id, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName} WHERE {whereString}";
        }
    }
}
