using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;
using SqlKata;

namespace SiteServer.CMS.Repositories
{
    public class UserLogRepository : IRepository
    {
        private readonly Repository<UserLog> _repository;

        public UserLogRepository()
        {
            _repository = new Repository<UserLog>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(UserLog userLog)
        {
            await _repository.InsertAsync(userLog);
        }

        public async Task DeleteIfThresholdAsync()
        {
            var config = await DataProvider.ConfigRepository.GetAsync();
            if (!config.IsTimeThreshold) return;

            var days = config.TimeThreshold;
            if (days <= 0) return;

            await _repository.DeleteAsync(Q
                .Where(nameof(UserLog.CreatedDate), "<", DateTime.Now.AddDays(-days))
            );
        }

        public async Task DeleteAllAsync()
        {
            await _repository.DeleteAsync();
        }

        private Query GetQuery(string userName, string keyword, string dateFrom, string dateTo)
        {
            var query = Q.OrderByDesc(nameof(UserLog.Id));

            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return query;
            }

            if (!string.IsNullOrEmpty(userName))
            {
                query.Where(nameof(UserLog.UserName), userName);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q =>
                    q.WhereLike(nameof(UserLog.Action), like).OrWhereLike(nameof(UserLog.Summary), like)
                );
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                query.WhereDate(nameof(UserLog.AddDate), ">=", TranslateUtils.ToDateTime(dateFrom));
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                query.WhereDate(nameof(UserLog.AddDate), "<=", TranslateUtils.ToDateTime(dateTo));
            }

            return query;
        }

        public async Task<int> GetCountAsync(string userName, string keyword, string dateFrom, string dateTo)
        {
            return await _repository.CountAsync(GetQuery(userName, keyword, dateFrom, dateTo));
        }

        public async Task<List<UserLog>> GetAllAsync(string userName, string keyword, string dateFrom, string dateTo, int offset, int limit)
        {
            var query = GetQuery(userName, keyword, dateFrom, dateTo);
            query.Offset(offset).Limit(limit);
            return await _repository.GetAllAsync(query);
        }

        public async Task<List<UserLog>> ListAsync(string userName, int totalNum, string action)
        {
            var query = Q.Where(nameof(UserLog.UserName), userName);
            if (!string.IsNullOrEmpty(action))
            {
                query.Where(nameof(UserLog.Action), action);
            }

            query.Limit(totalNum);
            query.OrderByDesc(nameof(UserLog.Id));

            return await _repository.GetAllAsync(query);
        }

        public async Task<List<UserLog>> GetLogsAsync(string userName, int offset, int limit)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(UserLog.UserName), userName)
                .Offset(offset)
                .Limit(limit)
                .OrderByDesc(nameof(UserLog.Id))
            );
        }

        public async Task<UserLog> InsertAsync(string userName, UserLog log)
        {
            log.UserName = userName;
            log.IpAddress = PageUtils.GetIpAddress();
            log.AddDate = DateTime.Now;

            log.Id = await _repository.InsertAsync(log);

            return log;
        }
    }
}
