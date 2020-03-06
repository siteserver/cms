using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS;
using SqlKata;
using SS.CMS.Core;

namespace SS.CMS.Repositories
{
    public partial class LogRepository : ILogRepository
    {
        private readonly Repository<Log> _repository;
        private readonly IConfigRepository _configRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public LogRepository(ISettingsManager settingsManager, IConfigRepository configRepository, IAdministratorRepository administratorRepository, IErrorLogRepository errorLogRepository)
        {
            _repository = new Repository<Log>(settingsManager.Database, settingsManager.Redis);
            _configRepository = configRepository;
            _administratorRepository = administratorRepository;
            _errorLogRepository = errorLogRepository;
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
            var config = await _configRepository.GetAsync();
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

        public Dictionary<DateTime, int> GetAdminLoginDictionaryByDate(DateTime dateFrom, DateTime dateTo, string xType, string actionType)
        {
            var analysisType = TranslateUtils.ToEnum(xType, AnalysisType.Day);
            var dict = new Dictionary<DateTime, int>();

            var builder = new StringBuilder();
            if (dateFrom > Constants.SqlMinValue)
            {
                builder.Append($" AND AddDate >= {SqlUtils.GetComparableDate(Database.DatabaseType, dateFrom)}");
            }
            if (dateTo != Constants.SqlMinValue)
            {
                builder.Append($" AND AddDate < {SqlUtils.GetComparableDate(Database.DatabaseType, dateTo)}");
            }

            string sqlSelectTrackingDay = $@"
SELECT COUNT(*) AS AddNum, AddYear, AddMonth, AddDay FROM (
    SELECT {SqlUtils.GetDatePartYear(Database.DatabaseType, "AddDate")} AS AddYear, {SqlUtils.GetDatePartMonth(Database.DatabaseType, "AddDate")} AS AddMonth, {SqlUtils.GetDatePartDay(Database.DatabaseType, "AddDate")} AS AddDay 
    FROM siteserver_Log 
    WHERE {SqlUtils.GetDateDiffLessThanDays(Database.DatabaseType, "AddDate", 30.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear, AddMonth, AddDay ORDER BY AddNum DESC";//添加日统计

            if (analysisType == AnalysisType.Month)
            {
                sqlSelectTrackingDay = $@"
SELECT COUNT(*) AS AddNum, AddYear, AddMonth FROM (
    SELECT {SqlUtils.GetDatePartYear(Database.DatabaseType, "AddDate")} AS AddYear, {SqlUtils.GetDatePartMonth(Database.DatabaseType, "AddDate")} AS AddMonth 
    FROM siteserver_Log 
    WHERE {SqlUtils.GetDateDiffLessThanMonths(Database.DatabaseType, "AddDate", 12.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear, AddMonth ORDER BY AddNum DESC";//添加月统计
            }
            else if (analysisType == AnalysisType.Year)
            {
                sqlSelectTrackingDay = $@"
SELECT COUNT(*) AS AddNum, AddYear FROM (
    SELECT {SqlUtils.GetDatePartYear(Database.DatabaseType, "AddDate")} AS AddYear
    FROM siteserver_Log
    WHERE {SqlUtils.GetDateDiffLessThanYears(Database.DatabaseType, "AddDate", 10.ToString())} {builder}
) DERIVEDTBL GROUP BY AddYear ORDER BY AddNum DESC
";//添加年统计
            }

            using (var connection = _repository.Database.GetConnection())
            {
                using (var rdr = connection.ExecuteReader(sqlSelectTrackingDay))
                {
                    while (rdr.Read())
                    {
                        var accessNum = rdr.IsDBNull(0) ? 0 : rdr.GetInt32(0);
                        if (analysisType == AnalysisType.Day)
                        {
                            var year = rdr.GetValue(1);
                            var month = rdr.GetValue(2);
                            var day = rdr.GetValue(3);
                            var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-{day}");
                            dict.Add(dateTime, accessNum);
                        }
                        else if (analysisType == AnalysisType.Month)
                        {
                            var year = rdr.GetValue(1);
                            var month = rdr.GetValue(2);

                            var dateTime = TranslateUtils.ToDateTime($"{year}-{month}-1");
                            dict.Add(dateTime, accessNum);
                        }
                        else if (analysisType == AnalysisType.Year)
                        {
                            var year = rdr.GetValue(1);
                            var dateTime = TranslateUtils.ToDateTime($"{year}-1-1");
                            dict.Add(dateTime, accessNum);
                        }
                    }
                    rdr.Close();
                }
            }

            
            return dict;
        }

        public async Task<Dictionary<string, int>> GetAdminLoginDictionaryByNameAsync(DateTime dateFrom, DateTime dateTo, string actionType)
        {
            var dict = new Dictionary<string, int>();

            var query = Q
                .WhereDate(nameof(Log.AddDate), ">", DateTime.Now.AddDays(-30))
                .Where(nameof(Log.AdminId), ">", 0);

            if (dateFrom > Constants.SqlMinValue)
            {
                query.WhereDate(nameof(Log.AddDate), ">=", dateFrom);
            }
            if (dateTo != Constants.SqlMinValue)
            {
                query.WhereDate(nameof(Log.AddDate), "<", dateTo);
            }

            var list = await _repository.GetAllAsync<(int adminId, int count)>(query
                .SelectRaw("AdminId, COUNT(*)")
                .GroupBy(nameof(Log.AdminId))
            );

            foreach (var (adminId, count) in list)
            {
                var admin = await _administratorRepository.GetByUserIdAsync(adminId);
                if (admin != null)
                {
                    dict[admin.UserName] = count;
                }
            }

//            string sqlSelectTrackingDay = $@"
//SELECT COUNT(*) AS AddNum, UserName FROM (
//    SELECT {SqlUtils.GetDatePartYear("AddDate")} AS AddYear, {SqlUtils.GetDatePartMonth("AddDate")} AS AddMonth, {SqlUtils.GetDatePartDay("AddDate")} AS AddDay, UserName 
//    FROM siteserver_Log 
//    WHERE {SqlUtils.GetDateDiffLessThanDays("AddDate", 30.ToString())} 
//    {builder}
//) DERIVEDTBL GROUP BY UserName ORDER BY AddNum DESC";//添加日统计


//            using (var connection = _repository.Database.GetConnection())
//            {
//                using (var rdr = connection.ExecuteReader(sqlSelectTrackingDay))
//                {
//                    while (rdr.Read())
//                    {
//                        var accessNum = rdr.IsDBNull(0) ? 0 : rdr.GetInt32(0);
//                        var userName = rdr.IsDBNull(1) ? string.Empty : rdr.GetString(1);
//                        if (!string.IsNullOrEmpty(userName))
//                        {
//                            dict[userName] = accessNum;
//                        }
                        
//                    }
//                    rdr.Close();
//                }
//            }
            
            return dict;
        }
    }
}
