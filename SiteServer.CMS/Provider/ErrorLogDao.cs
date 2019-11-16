using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class ErrorLogDao : IRepository
    {
        private readonly Repository<ErrorLog> _repository;

        public ErrorLogDao()
        {
            _repository = new Repository<ErrorLog>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task<int> InsertAsync(ErrorLog logInfo)
        {
            logInfo.Id = await _repository.InsertAsync(logInfo);

            return logInfo.Id;
        }

        public async Task DeleteAsync(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            await _repository.DeleteAsync(Q
                .WhereIn(nameof(ErrorLog.Id), idList)
            );
        }

        public async Task DeleteIfThresholdAsync()
        {
            var config = await ConfigManager.GetInstanceAsync();
            if (!config.IsTimeThreshold) return;

            var days = config.TimeThreshold;
            if (days <= 0) return;

            await _repository.DeleteAsync(Q
                .Where(nameof(ErrorLog.CreatedDate), "<", DateTime.Now.AddDays(-days))
            );
        }

        public async Task DeleteAllAsync()
        {
            await _repository.DeleteAsync();
        }

        public async Task<ErrorLog> GetErrorLogAsync(int logId)
        {
            return await _repository.GetAsync(logId);
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
