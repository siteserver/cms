using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Context;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class UserLogDao : IRepository
    {
        private readonly Repository<UserLog> _repository;

        public UserLogDao()
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
            var config = await ConfigManager.GetInstanceAsync();
            if (!config.IsTimeThreshold) return;

            var days = config.TimeThreshold;
            if (days <= 0) return;

            await _repository.DeleteAsync(Q
                .Where(nameof(UserLog.CreatedDate), "<", DateTime.Now.AddDays(-days))
            );
        }

        public async Task DeleteAsync(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            await _repository.DeleteAsync(Q.WhereIn(nameof(UserLog.Id), idList));
        }

        public async Task DeleteAllAsync()
        {
            await _repository.DeleteAsync();
        }

        public async Task<int> GetCountAsync()
        {
            return await _repository.CountAsync();
        }

        public string GetSelectCommend()
        {
            return "SELECT ID, UserName, IPAddress, AddDate, Action, Summary FROM siteserver_UserLog";
        }

        public string GetSelectCommend(string userName, string keyword, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return GetSelectCommend();
            }

            var whereString = new StringBuilder("WHERE ");

            var isWhere = false;

            if (!string.IsNullOrEmpty(userName))
            {
                isWhere = true;
                whereString.AppendFormat("(UserName = '{0}')", AttackUtils.FilterSql(userName));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.AppendFormat("(Action LIKE '%{0}%' OR Summary LIKE '%{0}%')", AttackUtils.FilterSql(keyword));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.Append($"(AddDate >= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateFrom))})");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                whereString.Append($"(AddDate <= {SqlUtils.GetComparableDate(TranslateUtils.ToDateTime(dateTo))})");
            }

            return "SELECT ID, UserName, IPAddress, AddDate, Action, Summary FROM siteserver_UserLog " + whereString;
        }

        public async Task<IEnumerable<UserLog>> ListAsync(string userName, int totalNum, string action)
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

        public async Task<IEnumerable<UserLog>> GetLogsAsync(string userName, int offset, int limit)
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
