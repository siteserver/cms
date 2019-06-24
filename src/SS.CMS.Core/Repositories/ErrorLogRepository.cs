using System;
using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly Repository<ErrorLogInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;

        public ErrorLogRepository(ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _repository = new Repository<ErrorLogInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _configRepository = configRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private class Attr
        {
            public const string Id = nameof(ErrorLogInfo.Id);
            public const string CreatedDate = nameof(ErrorLogInfo.CreatedDate);
        }

        public const string CategoryStl = "stl";
        public const string CategoryAdmin = "admin";
        public const string CategoryHome = "home";
        public const string CategoryApi = "api";

        public readonly Lazy<List<KeyValuePair<string, string>>> AllCategoryList = new Lazy<List<KeyValuePair<string, string>>>(
            () =>
            {
                var list = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(CategoryStl, "STL 解析错误"),
                    new KeyValuePair<string, string>(CategoryAdmin, "后台错误"),
                    new KeyValuePair<string, string>(CategoryHome, "用户中心错误"),
                    new KeyValuePair<string, string>(CategoryApi, "API错误")
                };
                return list;
            });

        private int Insert(ErrorLogInfo logInfo)
        {
            logInfo.Id = _repository.Insert(logInfo);

            return logInfo.Id;
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            _repository.Delete(Q
                .WhereIn(Attr.Id, idList)
            );
        }

        private void DeleteIfThreshold()
        {
            if (!_configRepository.Instance.IsTimeThreshold) return;

            var days = _configRepository.Instance.TimeThreshold;
            if (days <= 0) return;

            _repository.Delete(Q
                .Where(Attr.CreatedDate, "<", DateTime.Now.AddDays(-days)));
        }

        public ErrorLogInfo GetErrorLogInfo(int logId)
        {
            return _repository.Get(logId);
        }

        // public string GetSelectCommend(string category, string pluginId, string keyword, string dateFrom, string dateTo)
        // {
        //     var whereString = new StringBuilder();

        //     if (!string.IsNullOrEmpty(category))
        //     {
        //         whereString.Append($"Category = '{AttackUtils.FilterSql(category)}'");
        //     }

        //     if (!string.IsNullOrEmpty(pluginId))
        //     {
        //         whereString.Append($"PluginId = '{AttackUtils.FilterSql(pluginId)}'");
        //     }

        //     if (!string.IsNullOrEmpty(keyword))
        //     {
        //         if (whereString.Length > 0)
        //         {
        //             whereString.Append(" AND ");
        //         }
        //         var filterKeyword = AttackUtils.FilterSql(keyword);
        //         var keywordId = TranslateUtils.ToInt(keyword);
        //         whereString.Append(keywordId > 0
        //             ? $"Id = {keywordId}"
        //             : $"(Message LIKE '%{filterKeyword}%' OR Stacktrace LIKE '%{filterKeyword}%' OR Summary LIKE '%{filterKeyword}%')");
        //     }
        //     if (!string.IsNullOrEmpty(dateFrom))
        //     {
        //         if (whereString.Length > 0)
        //         {
        //             whereString.Append(" AND ");
        //         }
        //         whereString.Append($"AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))}");
        //     }
        //     if (!string.IsNullOrEmpty(dateTo))
        //     {
        //         if (whereString.Length > 0)
        //         {
        //             whereString.Append(" AND ");
        //         }
        //         whereString.Append($"AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))}");
        //     }

        //     return whereString.Length > 0
        //         ? $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName} WHERE {whereString}"
        //         : $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName}";
        // }

        public void DeleteAll()
        {
            _repository.Delete();
        }

        public int AddErrorLog(ErrorLogInfo logInfo)
        {
            try
            {
                if (!_configRepository.Instance.IsLogError) return 0;

                DeleteIfThreshold();

                return Insert(logInfo);
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        public int AddErrorLog(Exception ex, string summary = "")
        {
            var logInfo = new ErrorLogInfo
            {
                Category = CategoryAdmin,
                PluginId = string.Empty,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary
            };

            return AddErrorLog(logInfo);
        }

        public int AddErrorLog(string pluginId, Exception ex, string summary = "")
        {
            var logInfo = new ErrorLogInfo
            {
                Category = CategoryAdmin,
                PluginId = pluginId,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary
            };

            return AddErrorLog(logInfo);
        }

        public int AddStlErrorLog(string summary, string elementName, string stlContent, Exception ex)
        {
            var logInfo = new ErrorLogInfo
            {
                Category = CategoryStl,
                PluginId = string.Empty,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary
            };

            return AddErrorLog(logInfo);
        }
    }
}

// using System;
// using System.Collections.Generic;
// using System.Data;
// using System.Text;
// using Datory;
// using SiteServer.CMS.Core;
// using SiteServer.CMS.DataCache;
// using SiteServer.CMS.Model;
// using SiteServer.Utils;

// namespace SiteServer.CMS.Provider
// {
//     public class ErrorLogDao
//     {
//         public override string TableName => "siteserver_ErrorLog";

//         public override List<TableColumn> TableColumns => new List<TableColumn>
//         {
//             new TableColumn
//             {
//                 AttributeName = nameof(ErrorLogInfo.Id),
//                 DataType = DataType.Integer,
//                 IsIdentity = true,
//                 IsPrimaryKey = true
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ErrorLogInfo.Category),
//                 DataType = DataType.VarChar,
//                 DataLength = 50
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ErrorLogInfo.PluginId),
//                 DataType = DataType.VarChar,
//                 DataLength = 200
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ErrorLogInfo.Message),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ErrorLogInfo.Stacktrace),
//                 DataType = DataType.Text
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ErrorLogInfo.Summary),
//                 DataType = DataType.Text
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(ErrorLogInfo.AddDate),
//                 DataType = DataType.DateTime
//             }
//         };

//         private const string ParmCategory = "@Category";
//         private const string ParmPluginId = "@PluginId";
//         private const string ParmMessage = "@Message";
//         private const string ParmStacktrace = "@Stacktrace";
//         private const string ParmSummary = "@Summary";
//         private const string ParmAddDate = "@AddDate";

//         public int Insert(ErrorLogInfo logInfo)
//         {
//             var sqlString = $"INSERT INTO {TableName} (Category, PluginId, Message, Stacktrace, Summary, AddDate) VALUES (@Category, @PluginId, @Message, @Stacktrace, @Summary, @AddDate)";

//             var parms = new IDataParameter[]
//             {
//                 GetParameter(ParmCategory, DataType.VarChar, 50, logInfo.Category),
//                 GetParameter(ParmPluginId, DataType.VarChar, 200, logInfo.PluginId),
//                 GetParameter(ParmMessage, DataType.VarChar, 255, logInfo.Message),
//                 GetParameter(ParmStacktrace, DataType.Text, logInfo.Stacktrace),
//                 GetParameter(ParmSummary, DataType.Text, logInfo.Summary),
//                 GetParameter(ParmAddDate, DataType.DateTime, logInfo.AddDate),
//             };

//             return ExecuteNonQueryAndReturnId(TableName, nameof(ErrorLogInfo.Id), sqlString, parms);
//         }

//         public void Delete(List<int> idList)
//         {
//             if (idList == null || idList.Count <= 0) return;

//             var sqlString =
//                 $"DELETE FROM {TableName} WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

//             ExecuteNonQuery(sqlString);
//         }

//         public void DeleteIfThreshold()
//         {
//             if (!_settingsManager.ConfigInfo.IsTimeThreshold) return;

//             var days = _settingsManager.ConfigInfo.TimeThreshold;
//             if (days <= 0) return;

//             ExecuteNonQuery($@"DELETE FROM {TableName} WHERE AddDate < {SqlUtils.GetComparableDateTime(DateTime.Now.AddDays(-days))}");
//         }

//         public void DeleteAll()
//         {
//             var sqlString = $"DELETE FROM {TableName}";

//             ExecuteNonQuery(sqlString);
//         }

//         public ErrorLogInfo GetErrorLogInfo(int logId)
//         {
//             ErrorLogInfo logInfo = null;

//             var sqlString = $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName} WHERE Id = @Id";

//             var parms = new IDataParameter[]
//             {
//                 GetParameter("@Id", DataType.Integer, logId)
//             };

//             using (var rdr = ExecuteReader(sqlString, parms))
//             {
//                 if (rdr.Read())
//                 {
//                     var i = 0;
//                     logInfo = new ErrorLogInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
//                 }
//                 rdr.Close();
//             }

//             return logInfo;
//         }

//         public KeyValuePair<string, string> GetMessageAndStacktrace(int logId)
//         {
//             var pair = new KeyValuePair<string, string>();

//             var sqlString = $"SELECT Message, Stacktrace FROM {TableName} WHERE Id = {logId}";

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 if (rdr.Read())
//                 {
//                     pair = new KeyValuePair<string, string>(GetString(rdr, 0), GetString(rdr, 1));
//                 }
//                 rdr.Close();
//             }

//             return pair;
//         }

//         public string GetSelectCommend(string category, string pluginId, string keyword, string dateFrom, string dateTo)
//         {
//             var whereString = new StringBuilder();

//             if (!string.IsNullOrEmpty(category))
//             {
//                 whereString.Append($"Category = '{AttackUtils.FilterSql(category)}'");
//             }

//             if (!string.IsNullOrEmpty(pluginId))
//             {
//                 whereString.Append($"PluginId = '{AttackUtils.FilterSql(pluginId)}'");
//             }

//             if (!string.IsNullOrEmpty(keyword))
//             {
//                 if (whereString.Length > 0)
//                 {
//                     whereString.Append(" AND ");
//                 }
//                 var filterKeyword = AttackUtils.FilterSql(keyword);
//                 var keywordId = TranslateUtils.ToInt(keyword);
//                 whereString.Append(keywordId > 0
//                     ? $"Id = {keywordId}"
//                     : $"(Message LIKE '%{filterKeyword}%' OR Stacktrace LIKE '%{filterKeyword}%' OR Summary LIKE '%{filterKeyword}%')");
//             }
//             if (!string.IsNullOrEmpty(dateFrom))
//             {
//                 if (whereString.Length > 0)
//                 {
//                     whereString.Append(" AND ");
//                 }
//                 whereString.Append($"AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))}");
//             }
//             if (!string.IsNullOrEmpty(dateTo))
//             {
//                 if (whereString.Length > 0)
//                 {
//                     whereString.Append(" AND ");
//                 }
//                 whereString.Append($"AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))}");
//             }

//             return whereString.Length > 0
//                 ? $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName} WHERE {whereString}"
//                 : $"SELECT Id, Category, PluginId, Message, Stacktrace, Summary, AddDate FROM {TableName}";
//         }
//     }
// }
