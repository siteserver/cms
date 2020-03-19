using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SqlKata;
using SSCMS;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories.UserLogRepository
{
    public partial class UserLogRepository : IUserLogRepository
    {
        private readonly Repository<UserLog> _repository;
        private readonly IConfigRepository _configRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public UserLogRepository(ISettingsManager settingsManager, IConfigRepository configRepository, IErrorLogRepository errorLogRepository)
        {
            _repository = new Repository<UserLog>(settingsManager.Database, settingsManager.Redis);
            _configRepository = configRepository;
            _errorLogRepository = errorLogRepository;
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
            var config = await _configRepository.GetAsync();
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

        private Query GetQuery(int userId, string keyword, string dateFrom, string dateTo)
        {
            var query = Q.OrderByDesc(nameof(UserLog.Id));

            if (userId == 0 && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return query;
            }

            if (userId > 0)
            {
                query.Where(nameof(UserLog.UserId), userId);
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
                query.WhereDate(nameof(UserLog.CreatedDate), ">=", TranslateUtils.ToDateTime(dateFrom));
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                query.WhereDate(nameof(UserLog.CreatedDate), "<=", TranslateUtils.ToDateTime(dateTo));
            }

            return query;
        }

        public async Task<int> GetCountAsync(int userId, string keyword, string dateFrom, string dateTo)
        {
            return await _repository.CountAsync(GetQuery(userId, keyword, dateFrom, dateTo));
        }

        public async Task<List<UserLog>> GetAllAsync(int userId, string keyword, string dateFrom, string dateTo, int offset, int limit)
        {
            var query = GetQuery(userId, keyword, dateFrom, dateTo);
            query.Offset(offset).Limit(limit);
            return await _repository.GetAllAsync(query);
        }

        public async Task<List<UserLog>> GetLogsAsync(int userId, int offset, int limit)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(UserLog.UserId), userId)
                .Offset(offset)
                .Limit(limit)
                .OrderByDesc(nameof(UserLog.Id))
            );
        }

        public async Task<UserLog> InsertAsync(int userId, UserLog log)
        {
            log.UserId = userId;

            log.Id = await _repository.InsertAsync(log);

            return log;
        }
    }
}
