using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class UserLogRepository : IUserLogRepository
    {
        private readonly Repository<UserLog> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;

        public UserLogRepository(ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _repository = new Repository<UserLog>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _configRepository = configRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;
        private static class Attr
        {
            public const string Id = nameof(UserLog.Id);
            public const string UserId = nameof(UserLog.UserId);
            public const string CreatedDate = nameof(UserLog.CreatedDate);
            public const string Action = nameof(UserLog.Action);
        }

        private async Task<UserLog> InsertAsync(int userId, UserLog logInfo)
        {
            logInfo.UserId = userId;

            logInfo.Id = await _repository.InsertAsync(logInfo);

            return logInfo;
        }

        private async Task DeleteIfThresholdAsync()
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (!configInfo.IsTimeThreshold) return;

            var days = configInfo.TimeThreshold;
            if (days <= 0) return;

            await _repository.DeleteAsync(Q.Where(Attr.CreatedDate, "<", DateTime.Now.AddDays(-days)));
        }

        public async Task DeleteAsync(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            await _repository.DeleteAsync(Q.WhereIn(Attr.Id, idList));
        }

        public async Task DeleteAllAsync()
        {
            await _repository.DeleteAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _repository.CountAsync();
        }

        // public string GetSelectCommend()
        // {
        //     return "SELECT ID, UserName, IPAddress, AddDate, Action, Summary FROM siteserver_UserLog";
        // }

        // public string GetSelectCommend(int userId, string keyword, string dateFrom, string dateTo)
        // {
        //     if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
        //     {
        //         return GetSelectCommend();
        //     }

        //     var whereString = new StringBuilder("WHERE ");

        //     var isWhere = false;

        //     if (!string.IsNullOrEmpty(userName))
        //     {
        //         isWhere = true;
        //         whereString.AppendFormat("(UserName = '{0}')", AttackUtils.FilterSql(userName));
        //     }

        //     if (!string.IsNullOrEmpty(keyword))
        //     {
        //         if (isWhere)
        //         {
        //             whereString.Append(" AND ");
        //         }
        //         isWhere = true;
        //         whereString.AppendFormat("(Action LIKE '%{0}%' OR Summary LIKE '%{0}%')", AttackUtils.FilterSql(keyword));
        //     }

        //     if (!string.IsNullOrEmpty(dateFrom))
        //     {
        //         if (isWhere)
        //         {
        //             whereString.Append(" AND ");
        //         }
        //         isWhere = true;
        //         whereString.Append($"(AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))})");
        //     }
        //     if (!string.IsNullOrEmpty(dateTo))
        //     {
        //         if (isWhere)
        //         {
        //             whereString.Append(" AND ");
        //         }
        //         whereString.Append($"(AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))})");
        //     }

        //     return "SELECT ID, UserName, IPAddress, AddDate, Action, Summary FROM siteserver_UserLog " + whereString;
        // }

        public async Task<IEnumerable<UserLog>> ListAsync(int userId, int totalNum, string action)
        {
            var query = Q.Where(Attr.UserId, userId);
            if (!string.IsNullOrEmpty(action))
            {
                query.Where(Attr.Action, action);
            }

            query.Limit(totalNum);
            query.OrderByDesc(Attr.Id);

            return await _repository.GetAllAsync(query);
        }

        public async Task<IEnumerable<UserLog>> ApiGetLogsAsync(int userId, int offset, int limit)
        {
            return await _repository.GetAllAsync(Q
                .Where(Attr.UserId, userId)
                .Offset(offset)
                .Limit(limit)
                .OrderByDesc(Attr.Id));
        }

        public async Task AddUserLogAsync(string ipAddress, int userId, string actionType, string summary)
        {
            var configInfo = await _configRepository.GetConfigInfoAsync();

            if (!configInfo.IsLogUser) return;

            await DeleteIfThresholdAsync();

            if (!string.IsNullOrEmpty(summary))
            {
                summary = StringUtils.MaxLengthText(summary, 250);
            }

            var userLogInfo = new UserLog
            {
                UserId = userId,
                IpAddress = ipAddress,
                Action = actionType,
                Summary = summary
            };

            await InsertAsync(userId, userLogInfo);
        }
    }
}
