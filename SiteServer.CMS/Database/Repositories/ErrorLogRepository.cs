using System;
using System.Collections.Generic;
using System.Text;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class ErrorLogRepository : GenericRepository<ErrorLogInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(ErrorLogInfo.Id);
            public const string AddDate = nameof(ErrorLogInfo.AddDate);
        }

        public int Insert(ErrorLogInfo logInfo)
        {
            //var sqlString = $"INSERT INTO {TableName} (Category, PluginId, Message, Stacktrace, Summary, AddDate) VALUES (@Category, @PluginId, @Message, @Stacktrace, @Summary, @AddDate)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamCategory, logInfo.Category),
            //    GetParameter(ParamPluginId, logInfo.PluginId),
            //    GetParameter(ParamMessage, logInfo.Message),
            //    GetParameter(ParamStacktrace,logInfo.Stacktrace),
            //    GetParameter(ParamSummary,logInfo.Summary),
            //    GetParameter(ParamAddDate,logInfo.AddDate),
            //};

            //return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(ErrorLogInfo.Id), sqlString, parameters);

            return InsertObject(logInfo);
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            //var sqlString =
            //    $"DELETE FROM {TableName} WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            DeleteAll(Q
                .WhereIn(Attr.Id, idList));
        }

        public void DeleteIfThreshold()
        {
            if (!ConfigManager.Instance.IsTimeThreshold) return;

            var days = ConfigManager.Instance.TimeThreshold;
            if (days <= 0) return;

            //DatabaseApi.ExecuteNonQuery(ConnectionString, $@"DELETE FROM {TableName} WHERE AddDate < {SqlUtils.GetComparableDateTime(DateTime.Now.AddDays(-days))}");

            DeleteAll(Q
                .Where(Attr.AddDate, "<", DateTime.Now.AddDays(-days)));
        }

        //public void DeleteAll()
        //{
        //    var sqlString = $"DELETE FROM {TableName}";

        //    DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
        //}

        public ErrorLogInfo GetErrorLogInfo(int logId)
        {
            //if (logId <= 0) return null;
            //ErrorLogInfo logInfo = null;

            //var sqlString = $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName} WHERE Id = @Id";

            //IDataParameter[] parameters =
            //{
            //    GetParameter("@Id", logId)
            //};

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
            //{
            //    if (rdr.Read())
            //    {
            //        var i = 0;
            //        logInfo = new ErrorLogInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetDateTime(rdr, i));
            //    }
            //    rdr.Close();
            //}

            //return logInfo;

            return GetObjectById(logId);
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

        public void DeleteAll()
        {
            base.DeleteAll();
        }
    }
}


//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;
//using SiteServer.CMS.Database.Caches;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class ErrorLog : DataProviderBase
//    {
//        public override string TableName => "siteserver_ErrorLog";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(ErrorLogInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ErrorLogInfo.Category),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ErrorLogInfo.PluginId),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ErrorLogInfo.Message),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ErrorLogInfo.Stacktrace),
//                DataType = DataType.Text
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ErrorLogInfo.Summary),
//                DataType = DataType.Text
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(ErrorLogInfo.AddDate),
//                DataType = DataType.DateTime
//            }
//        };

//        private const string ParamCategory = "@Category";
//        private const string ParamPluginId = "@PluginId";
//        private const string ParamMessage = "@Message";
//        private const string ParamStacktrace = "@Stacktrace";
//        private const string ParamSummary = "@Summary";
//        private const string ParamAddDate = "@AddDate";

//        public int InsertObject(ErrorLogInfo logInfo)
//        {
//            var sqlString = $"INSERT INTO {TableName} (Category, PluginId, Message, Stacktrace, Summary, AddDate) VALUES (@Category, @PluginId, @Message, @Stacktrace, @Summary, @AddDate)";

//            IDataParameter[] parameters =
//            {
//                GetParameter(ParamCategory, logInfo.Category),
//                GetParameter(ParamPluginId, logInfo.PluginId),
//                GetParameter(ParamMessage, logInfo.Message),
//                GetParameter(ParamStacktrace,logInfo.Stacktrace),
//                GetParameter(ParamSummary,logInfo.Summary),
//                GetParameter(ParamAddDate,logInfo.AddDate),
//            };

//            return DatabaseApi.ExecuteNonQueryAndReturnId(ConnectionString, TableName, nameof(ErrorLogInfo.Id), sqlString, parameters);
//        }

//        public void DeleteById(List<int> idList)
//        {
//            if (idList == null || idList.Count <= 0) return;

//            var sqlString =
//                $"DELETE FROM {TableName} WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        public void DeleteIfThreshold()
//        {
//            if (!ConfigManager.Instance.IsTimeThreshold) return;

//            var days = ConfigManager.Instance.TimeThreshold;
//            if (days <= 0) return;

//            DatabaseApi.ExecuteNonQuery(ConnectionString, $@"DELETE FROM {TableName} WHERE AddDate < {SqlUtils.GetComparableDateTime(DateTime.Now.AddDays(-days))}");
//        }

//        public void DeleteAll()
//        {
//            var sqlString = $"DELETE FROM {TableName}";

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//        }

//        public ErrorLogInfo GetErrorLogInfo(int logId)
//        {
//            if (logId <= 0) return null;
//            ErrorLogInfo logInfo = null;

//            var sqlString = $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName} WHERE Id = @Id";

//            IDataParameter[] parameters =
//            {
//                GetParameter("@Id", logId)
//            };

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString, parameters))
//            {
//                if (rdr.Read())
//                {
//                    var i = 0;
//                    logInfo = new ErrorLogInfo(DatabaseApi.GetInt(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetString(rdr, i++), DatabaseApi.GetDateTime(rdr, i));
//                }
//                rdr.Close();
//            }

//            return logInfo;
//        }

//        public string GetSelectCommend(string category, string pluginId, string keyword, string dateFrom, string dateTo)
//        {
//            var whereString = new StringBuilder();

//            if (!string.IsNullOrEmpty(category))
//            {
//                whereString.Append($"Category = '{AttackUtils.FilterSql(category)}'");
//            }

//            if (!string.IsNullOrEmpty(pluginId))
//            {
//                whereString.Append($"PluginId = '{AttackUtils.FilterSql(pluginId)}'");
//            }

//            if (!string.IsNullOrEmpty(keyword))
//            {
//                if (whereString.Length > 0)
//                {
//                    whereString.Append(" AND ");
//                }
//                var filterKeyword = AttackUtils.FilterSql(keyword);
//                var keywordId = TranslateUtils.ToInt(keyword);
//                whereString.Append(keywordId > 0
//                    ? $"Id = {keywordId}"
//                    : $"(Message LIKE '%{filterKeyword}%' OR Stacktrace LIKE '%{filterKeyword}%' OR Summary LIKE '%{filterKeyword}%')");
//            }
//            if (!string.IsNullOrEmpty(dateFrom))
//            {
//                if (whereString.Length > 0)
//                {
//                    whereString.Append(" AND ");
//                }
//                whereString.Append($"AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))}");
//            }
//            if (!string.IsNullOrEmpty(dateTo))
//            {
//                if (whereString.Length > 0)
//                {
//                    whereString.Append(" AND ");
//                }
//                whereString.Append($"AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))}");
//            }

//            return whereString.Length > 0
//                ? $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName} WHERE {whereString}"
//                : $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName}";
//        }
//    }
//}
