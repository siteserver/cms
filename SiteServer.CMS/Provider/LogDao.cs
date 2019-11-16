using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Utils;
using SqlKata;

namespace SiteServer.CMS.Provider
{
    public class LogDao : DataProviderBase, IRepository
    {
        private readonly Repository<Log> _repository;

        public LogDao()
        {
            _repository = new Repository<Log>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(Log log)
        {
            await _repository.InsertAsync(log);
        }

        public async Task DeleteAsync(List<int> idList)
        {
            if (idList == null || idList.Count <= 0) return;

            await _repository.DeleteAsync(Q
                .WhereIn(nameof(Log.Id), idList)
            );
        }

        public async Task DeleteIfThresholdAsync()
        {
            var config = await ConfigManager.GetInstanceAsync();
            if (!config.IsTimeThreshold) return;

            var days = config.TimeThreshold;
            if (days <= 0) return;

            await _repository.DeleteAsync(Q
                .Where(nameof(Log.CreatedDate), "<", DateTime.Now.AddDays(-days))
            );
        }

        public async Task DeleteAllAsync()
        {
            await _repository.DeleteAsync();
        }

        private Query GetQuery(string userName, string keyword, string dateFrom, string dateTo)
        {
            var query = Q.NewQuery();
            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return query;
            }

            if (!string.IsNullOrEmpty(userName))
            {
                query.Where(nameof(Log.UserName), userName);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                var like = $"%{keyword}%";
                query.Where(q =>
                    q.WhereLike(nameof(Log.Action), like).OrWhereLike(nameof(Log.Summary), like)
                );
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                query.WhereDate(nameof(Log.AddDate), ">=", TranslateUtils.ToDateTime(dateFrom));
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                query.WhereDate(nameof(Log.AddDate), "<=", TranslateUtils.ToDateTime(dateTo));
            }

            return query;
        }

        public async Task<int> GetCountAsync(string userName, string keyword, string dateFrom, string dateTo)
        {
            return await _repository.CountAsync(GetQuery(userName, keyword, dateFrom, dateTo));
        }

        public async Task<IEnumerable<Log>> GetAllAsync(string userName, string keyword, string dateFrom, string dateTo, int offset, int limit)
        {
            var query = GetQuery(userName, keyword, dateFrom, dateTo);
            query.Offset(offset).Limit(limit).OrderByDesc(nameof(Log.Id));
            return await _repository.GetAllAsync(query);
        }

        public async Task<int> GetCountAsync()
        {
            return await _repository.CountAsync();
        }

        public string GetSelectCommend()
        {
            return GetSelectCommend(string.Empty, string.Empty, string.Empty, string.Empty);
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

            return "SELECT ID, UserName, IPAddress, AddDate, Action, Summary FROM siteserver_Log " + whereString;
        }

        public async Task<DateTimeOffset> GetLastRemoveLogDateAsync()
        {
            var addDate = await _repository.GetAsync<DateTime?>(Q
                .Select(nameof(Log.CreatedDate))
                .Where(nameof(Log.Action), "清空数据库日志")
                .OrderByDesc(nameof(Log.Id))
            );

            return addDate ?? DateTime.MinValue;
        }

        /// <summary>
        /// 统计管理员actionType的操作次数
        /// </summary>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="xType"></param>
        /// <param name="actionType"></param>
        /// <returns></returns>
        public Dictionary<DateTime, int> GetAdminLoginDictionaryByDate(DateTime dateFrom, DateTime dateTo, string xType, string actionType)
        {
            var dict = new Dictionary<DateTime, int>();
            if (string.IsNullOrEmpty(xType))
            {
                xType = EStatictisXTypeUtils.GetValue(EStatictisXType.Day);
            }

            var builder = new StringBuilder();
            if (dateFrom > Constants.SqlMinValue)
            {
                builder.Append($" AND AddDate >= {SqlUtils.GetComparableDate(dateFrom)}");
            }
            if (dateTo != Constants.SqlMinValue)
            {
                builder.Append($" AND AddDate < {SqlUtils.GetComparableDate(dateTo)}");
            }

            string sqlSelectTrackingDay = $@"
SELECT COUNT(*) AS AddNum, AddYear, AddMonth, AddDay FROM (
    SELECT {SqlUtils.GetDatePartYear("AddDate")} AS AddYear, {SqlUtils.GetDatePartMonth("AddDate")} AS AddMonth, {SqlUtils.GetDatePartDay("AddDate")} AS AddDay 
    FROM siteserver_Log 
    WHERE {SqlUtils.GetDateDiffLessThanDays("AddDate", 30.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear, AddMonth, AddDay ORDER BY AddNum DESC";//添加日统计

            if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
            {
                sqlSelectTrackingDay = $@"
SELECT COUNT(*) AS AddNum, AddYear, AddMonth FROM (
    SELECT {SqlUtils.GetDatePartYear("AddDate")} AS AddYear, {SqlUtils.GetDatePartMonth("AddDate")} AS AddMonth 
    FROM siteserver_Log 
    WHERE {SqlUtils.GetDateDiffLessThanMonths("AddDate", 12.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear, AddMonth ORDER BY AddNum DESC";//添加月统计
            }
            else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
            {
                sqlSelectTrackingDay = $@"
SELECT COUNT(*) AS AddNum, AddYear FROM (
    SELECT {SqlUtils.GetDatePartYear("AddDate")} AS AddYear
    FROM siteserver_Log
    WHERE {SqlUtils.GetDateDiffLessThanYears("AddDate", 10.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear ORDER BY AddNum DESC
";//添加年统计
            }

            using (var rdr = ExecuteReader(sqlSelectTrackingDay))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Day))
                    {
                        var year = GetString(rdr, 1);
                        var month = GetString(rdr, 2);
                        var day = GetString(rdr, 3);
                        var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-{day}");
                        dict.Add(dateTime, accessNum);
                    }
                    else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Month))
                    {
                        var year = GetString(rdr, 1);
                        var month = GetString(rdr, 2);

                        var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-1");
                        dict.Add(dateTime, accessNum);
                    }
                    else if (EStatictisXTypeUtils.Equals(xType, EStatictisXType.Year))
                    {
                        var year = GetString(rdr, 1);
                        var dateTime = TranslateUtils.ToDateTime($"{year}-1-1");
                        dict.Add(dateTime, accessNum);
                    }
                }
                rdr.Close();
            }
            return dict;
        }

        /// <summary>
        /// 统计管理员actionType的操作次数
        /// </summary>
        public Dictionary<string, int> GetAdminLoginDictionaryByName(DateTime dateFrom, DateTime dateTo, string actionType)
        {
            var dict = new Dictionary<string, int>();

            var builder = new StringBuilder();
            if (dateFrom > Constants.SqlMinValue)
            {
                builder.Append($" AND AddDate >= {SqlUtils.GetComparableDate(dateFrom)}");
            }
            if (dateTo != Constants.SqlMinValue)
            {
                builder.Append($" AND AddDate < {SqlUtils.GetComparableDate(dateTo)}");
            }

            string sqlSelectTrackingDay = $@"
SELECT COUNT(*) AS AddNum, UserName FROM (
    SELECT {SqlUtils.GetDatePartYear("AddDate")} AS AddYear, {SqlUtils.GetDatePartMonth("AddDate")} AS AddMonth, {SqlUtils.GetDatePartDay("AddDate")} AS AddDay, UserName 
    FROM siteserver_Log 
    WHERE {SqlUtils.GetDateDiffLessThanDays("AddDate", 30.ToString())} {builder}
) DERIVEDTBL GROUP BY UserName ORDER BY AddNum DESC";//添加日统计


            using (var rdr = ExecuteReader(sqlSelectTrackingDay))
            {
                while (rdr.Read())
                {
                    var accessNum = GetInt(rdr, 0);
                    var userName = GetString(rdr, 1);
                    dict.Add(userName, accessNum);
                }
                rdr.Close();
            }
            return dict;
        }
    }
}
