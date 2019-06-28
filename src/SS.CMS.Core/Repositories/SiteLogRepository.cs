using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteLogRepository : ISiteLogRepository
    {
        private readonly Repository<SiteLogInfo> _repository;
        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private readonly ILogRepository _logRepository;

        public SiteLogRepository(ISettingsManager settingsManager, IConfigRepository configRepository, IErrorLogRepository errorLogRepository, ILogRepository logRepository)
        {
            _repository = new Repository<SiteLogInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _errorLogRepository = errorLogRepository;
            _logRepository = logRepository;
        }

        public IDatabase Database => _repository.Database;
        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(SiteLogInfo.Id);
            public const string CreatedDate = nameof(SiteLogInfo.CreatedDate);
        }

        public void Insert(SiteLogInfo logInfo)
        {
            _repository.Insert(logInfo);
        }

        public async Task DeleteIfThresholdAsync()
        {
            if (!_configRepository.Instance.IsTimeThreshold) return;

            var days = _configRepository.Instance.TimeThreshold;
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

        // public string GetSelectCommend()
        // {
        //     return "SELECT Id, SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary FROM siteserver_SiteLog";
        // }

        // public string GetSelectCommend(int siteId, string logType, string userName, string keyword, string dateFrom, string dateTo)
        // {
        //     if (siteId == 0 && (string.IsNullOrEmpty(logType) || StringUtils.EqualsIgnoreCase(logType, "All")) && string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
        //     {
        //         return GetSelectCommend();
        //     }

        //     var whereString = new StringBuilder("WHERE ");

        //     var isWhere = false;

        //     if (siteId > 0)
        //     {
        //         isWhere = true;
        //         whereString.AppendFormat("(SiteId = {0})", siteId);
        //     }

        //     if (!string.IsNullOrEmpty(logType) && !StringUtils.EqualsIgnoreCase(logType, "All"))
        //     {
        //         if (isWhere)
        //         {
        //             whereString.Append(" AND ");
        //         }
        //         isWhere = true;

        //         if (StringUtils.EqualsIgnoreCase(logType, "Channel"))
        //         {
        //             whereString.Append("(ChannelId > 0 AND ContentId = 0)");
        //         }
        //         else if (StringUtils.EqualsIgnoreCase(logType, "Content"))
        //         {
        //             whereString.Append("(ChannelId > 0 AND ContentId > 0)");
        //         }
        //     }

        //     if (!string.IsNullOrEmpty(userName))
        //     {
        //         if (isWhere)
        //         {
        //             whereString.Append(" AND ");
        //         }
        //         isWhere = true;
        //         whereString.AppendFormat("(UserName = '{0}')", userName);
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

        //     return "SELECT Id, SiteId, ChannelId, ContentId, UserName, IpAddress, AddDate, Action, Summary FROM siteserver_SiteLog " + whereString;
        // }
    }
}