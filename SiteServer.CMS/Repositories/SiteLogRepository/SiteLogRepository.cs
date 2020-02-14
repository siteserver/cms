using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SqlKata;

namespace SiteServer.CMS.Repositories
{
    public class SiteLogRepository : IRepository
    {
        private readonly Repository<SiteLog> _repository;

        public SiteLogRepository()
        {
            _repository = new Repository<SiteLog>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(SiteLog log)
        {
            await _repository.InsertAsync(log);
        }

        public async Task DeleteIfThresholdAsync()
        {
            var config = await DataProvider.ConfigRepository.GetAsync();
            if (!config.IsTimeThreshold) return;

            var days = config.TimeThreshold;
            if (days <= 0) return;

            await _repository.DeleteAsync(Q
                .Where(nameof(SiteLog.AddDate), "<", DateTime.Now.AddDays(-days))
            );
        }

        public async Task DeleteAllAsync()
        {
            await _repository.DeleteAsync();
        }

        public async Task<int> GetCountAsync(List<int> siteIds, string logType, int adminId, string keyword, string dateFrom, string dateTo)
        {
            return await _repository.CountAsync(GetQuery(siteIds, logType, adminId, keyword, dateFrom, dateTo));
        }

        public async Task<List<SiteLog>> GetAllAsync(List<int> siteIds, string logType, int adminId, string keyword, string dateFrom, string dateTo, int offset, int limit)
        {
            var query = GetQuery(siteIds, logType, adminId, keyword, dateFrom, dateTo);
            query.Offset(offset).Limit(limit);
            return await _repository.GetAllAsync(query);
        }

        public Query GetQuery(List<int> siteIds, string logType, int adminId, string keyword, string dateFrom, string dateTo)
        {
            var query = Q.OrderByDesc(nameof(SiteLog.Id));

            if (siteIds != null && siteIds.Count > 0)
            {
                query.WhereIn(nameof(SiteLog.SiteId), siteIds);
            }

            if (!string.IsNullOrEmpty(logType))
            {
                if (StringUtils.EqualsIgnoreCase(logType, "Channel"))
                {
                    query.Where(nameof(SiteLog.ChannelId), ">", 0).Where(nameof(SiteLog.ContentId), 0);
                }
                else if (StringUtils.EqualsIgnoreCase(logType, "Content"))
                {
                    query.Where(nameof(SiteLog.ChannelId), ">", 0).Where(nameof(SiteLog.ContentId), ">", 0);
                }
            }

            if (adminId > 0)
            {
                query.Where(nameof(SiteLog.AdminId), adminId);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q =>
                    q.WhereLike(nameof(SiteLog.Action), like).OrWhereLike(nameof(SiteLog.Summary), like)
                );
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                query.WhereDate(nameof(SiteLog.AddDate), ">=", TranslateUtils.ToDateTime(dateFrom));
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                query.WhereDate(nameof(SiteLog.AddDate), "<=", TranslateUtils.ToDateTime(dateTo));
            }

            return query;
        }
    }
}
