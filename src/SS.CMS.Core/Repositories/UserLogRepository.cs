using System;
using System.Collections.Generic;
using System.Linq;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class UserLogRepository : IUserLogRepository
    {
        private readonly Repository<UserLogInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;

        public UserLogRepository(ISettingsManager settingsManager, IConfigRepository configRepository)
        {
            _repository = new Repository<UserLogInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _configRepository = configRepository;
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;
        private static class Attr
        {
            public const string Id = nameof(UserLogInfo.Id);
            public const string UserName = nameof(UserLogInfo.UserName);
            public const string CreationDate = nameof(UserLogInfo.CreationDate);
            public const string Action = nameof(UserLogInfo.Action);
        }

        private UserLogInfo Insert(string userName, UserLogInfo logInfo)
        {
            logInfo.UserName = userName;

            logInfo.Id = _repository.Insert(logInfo);

            return logInfo;
        }

        private void DeleteIfThreshold()
        {
            if (!_configRepository.Instance.IsTimeThreshold) return;

            var days = _configRepository.Instance.TimeThreshold;
            if (days <= 0) return;

            _repository.Delete(Q.Where(Attr.CreationDate, "<", DateTime.Now.AddDays(-days)));
        }

        public void Delete(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            _repository.Delete(Q.WhereIn(Attr.Id, idList));
        }

        public void DeleteAll()
        {
            _repository.Delete();
        }

        public int GetCount()
        {
            return _repository.Count();
        }

        // public string GetSelectCommend()
        // {
        //     return "SELECT ID, UserName, IPAddress, AddDate, Action, Summary FROM siteserver_UserLog";
        // }

        // public string GetSelectCommend(string userName, string keyword, string dateFrom, string dateTo)
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

        public IList<UserLogInfo> List(string userName, int totalNum, string action)
        {
            var query = Q.Where(Attr.UserName, userName);
            if (!string.IsNullOrEmpty(action))
            {
                query.Where(Attr.Action, action);
            }

            query.Limit(totalNum);
            query.OrderByDesc(Attr.Id);

            return _repository.GetAll(query).ToList();
        }

        public IList<UserLogInfo> ApiGetLogs(string userName, int offset, int limit)
        {
            return _repository.GetAll(Q
                .Where(Attr.UserName, userName)
                .Offset(offset)
                .Limit(limit)
                .OrderByDesc(Attr.Id)).ToList();
        }

        public void AddUserLog(string ipAddress, string userName, string actionType, string summary)
        {
            if (!_configRepository.Instance.IsLogUser) return;

            DeleteIfThreshold();

            if (!string.IsNullOrEmpty(summary))
            {
                summary = StringUtils.MaxLengthText(summary, 250);
            }

            var userLogInfo = new UserLogInfo
            {
                UserName = userName,
                IpAddress = ipAddress,
                Action = actionType,
                Summary = summary
            };

            Insert(userName, userLogInfo);
        }
    }
}
