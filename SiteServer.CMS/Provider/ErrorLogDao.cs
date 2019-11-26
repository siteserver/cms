using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SqlKata;

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

        public async Task DeleteIfThresholdAsync()
        {
            var config = await DataProvider.ConfigDao.GetAsync();
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

        private Query GetQuery(string category, string pluginId, string keyword, string dateFrom, string dateTo)
        {
            var query = Q.OrderByDesc(nameof(ErrorLog.Id));

            if (string.IsNullOrEmpty(category) && string.IsNullOrEmpty(pluginId) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return query;
            }

            if (!string.IsNullOrEmpty(category))
            {
                query.Where(nameof(ErrorLog.Category), category);
            }

            if (!string.IsNullOrEmpty(pluginId))
            {
                query.Where(nameof(ErrorLog.PluginId), pluginId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var keywordId = TranslateUtils.ToInt(keyword);
                if (keywordId > 0)
                {
                    query.Where(nameof(ErrorLog.Id), keywordId);
                }
                else
                {
                    var like = $"%{keyword}%";
                    query.Where(q =>
                        q.WhereLike(nameof(ErrorLog.Message), like)
                            .OrWhereLike(nameof(ErrorLog.Stacktrace), like)
                            .OrWhereLike(nameof(ErrorLog.Summary), like)
                    );
                }
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                query.WhereDate(nameof(ErrorLog.AddDate), ">=", TranslateUtils.ToDateTime(dateFrom));
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                query.WhereDate(nameof(ErrorLog.AddDate), "<=", TranslateUtils.ToDateTime(dateTo));
            }

            return query;
        }

        public async Task<int> GetCountAsync(string category, string pluginId, string keyword, string dateFrom, string dateTo)
        {
            return await _repository.CountAsync(GetQuery(category, pluginId, keyword, dateFrom, dateTo));
        }

        public async Task<IEnumerable<ErrorLog>> GetAllAsync(string category, string pluginId, string keyword, string dateFrom, string dateTo, int offset, int limit)
        {
            var query = GetQuery(category, pluginId, keyword, dateFrom, dateTo);
            query.Offset(offset).Limit(limit);
            return await _repository.GetAllAsync(query);
        }
    }
}
