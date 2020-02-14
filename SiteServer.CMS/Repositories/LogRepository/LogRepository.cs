using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context.Enumerations;
using SqlKata;

namespace SiteServer.CMS.Repositories
{
    public class LogRepository : DataProviderBase, IRepository
    {
        private readonly Repository<Log> _repository;

        public LogRepository()
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

        public async Task DeleteIfThresholdAsync()
        {
            var config = await DataProvider.ConfigRepository.GetAsync();
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

        private Query GetQuery(int adminId, string keyword, string dateFrom, string dateTo)
        {
            var query = Q.OrderByDesc(nameof(Log.Id));

            if (adminId == 0 && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return query;
            }

            if (adminId > 0)
            {
                query.Where(nameof(Log.AdminId), adminId);
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

        public async Task<int> GetCountAsync(int adminId, string keyword, string dateFrom, string dateTo)
        {
            return await _repository.CountAsync(GetQuery(adminId, keyword, dateFrom, dateTo));
        }

        public async Task<List<Log>> GetAllAsync(int adminId, string keyword, string dateFrom, string dateTo, int offset, int limit)
        {
            var query = GetQuery(adminId, keyword, dateFrom, dateTo);
            query.Offset(offset).Limit(limit);
            return await _repository.GetAllAsync(query);
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
