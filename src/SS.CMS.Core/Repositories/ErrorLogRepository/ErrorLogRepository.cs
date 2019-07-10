using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly Repository<ErrorLog> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;

        public ErrorLogRepository(ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _repository = new Repository<ErrorLog>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _configRepository = configRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private class Attr
        {
            public const string Id = nameof(ErrorLog.Id);
            public const string CreatedDate = nameof(ErrorLog.CreatedDate);
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

        private async Task<int> InsertAsync(ErrorLog logInfo)
        {
            logInfo.Id = await _repository.InsertAsync(logInfo);

            return logInfo.Id;
        }

        public async Task DeleteAsync(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            await _repository.DeleteAsync(Q
                .WhereIn(Attr.Id, idList)
            );
        }

        private async Task DeleteIfThresholdAsync()
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (!configInfo.IsTimeThreshold) return;

            var days = configInfo.TimeThreshold;
            if (days <= 0) return;

            await _repository.DeleteAsync(Q
                .Where(Attr.CreatedDate, "<", DateTime.Now.AddDays(-days)));
        }

        public async Task<ErrorLog> GetErrorLogInfoAsync(int logId)
        {
            return await _repository.GetAsync(logId);
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

        public async Task DeleteAllAsync()
        {
            await _repository.DeleteAsync();
        }

        public async Task<int> AddErrorLogAsync(ErrorLog logInfo)
        {
            try
            {
                var configInfo = await _configRepository.GetConfigInfoAsync();

                if (!configInfo.IsLogError) return 0;

                await DeleteIfThresholdAsync();

                return await InsertAsync(logInfo);
            }
            catch
            {
                // ignored
            }

            return 0;
        }

        public async Task<int> AddErrorLogAsync(Exception ex, string summary = "")
        {
            var logInfo = new ErrorLog
            {
                Category = CategoryAdmin,
                PluginId = string.Empty,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary
            };

            return await AddErrorLogAsync(logInfo);
        }

        public async Task<int> AddErrorLogAsync(string pluginId, Exception ex, string summary = "")
        {
            var logInfo = new ErrorLog
            {
                Category = CategoryAdmin,
                PluginId = pluginId,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary
            };

            return await AddErrorLogAsync(logInfo);
        }

        public async Task<int> AddStlErrorLogAsync(string summary, string elementName, string stlContent, Exception ex)
        {
            var logInfo = new ErrorLog
            {
                Category = CategoryStl,
                PluginId = string.Empty,
                Message = ex.Message,
                Stacktrace = ex.StackTrace,
                Summary = summary
            };

            return await AddErrorLogAsync(logInfo);
        }
    }
}
