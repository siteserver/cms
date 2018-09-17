using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class LogDao : DataProviderBase
    {
        public override string TableName => "siteserver_Log";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(LogInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.UserName),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.IpAddress),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.Action),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(LogInfo.Summary),
                DataType = DataType.VarChar,
                DataLength = 255
            }
        };

        private const string ParmUserName = "@UserName";
        private const string ParmIpAddress = "@IPAddress";
        private const string ParmAddDate = "@AddDate";
        private const string ParmAction = "@Action";
        private const string ParmSummary = "@Summary";

        public void Insert(LogInfo log)
        {
            var sqlString = "INSERT INTO siteserver_Log(UserName, IPAddress, AddDate, Action, Summary) VALUES (@UserName, @IPAddress, @AddDate, @Action, @Summary)";
            
            var parms = new IDataParameter[]
            {
                    GetParameter(ParmUserName, DataType.VarChar, 50, log.UserName),
                    GetParameter(ParmIpAddress, DataType.VarChar, 50, log.IpAddress),
                    GetParameter(ParmAddDate, DataType.DateTime, log.AddDate),
                    GetParameter(ParmAction, DataType.VarChar, 255, log.Action),
                    GetParameter(ParmSummary, DataType.VarChar, 255, log.Summary)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM siteserver_Log WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteIfThreshold()
        {
            if (!ConfigManager.SystemConfigInfo.IsTimeThreshold) return;

            var days = ConfigManager.SystemConfigInfo.TimeThreshold;
            if (days <= 0) return;

            ExecuteNonQuery($@"DELETE FROM siteserver_Log WHERE AddDate < {SqlUtils.GetComparableDateTime(DateTime.Now.AddDays(-days))}");
        }

        public void DeleteAll()
        {
            const string sqlString = "DELETE FROM siteserver_Log";

            ExecuteNonQuery(sqlString);
        }

        public int GetCount()
        {
            var count = 0;
            const string sqlString = "SELECT Count(*) FROM siteserver_Log";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read() && !rdr.IsDBNull(0))
                {
                    count = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return count;
        }

        public string GetSelectCommend()
        {
            return "SELECT ID, UserName, IPAddress, AddDate, Action, Summary FROM siteserver_Log";
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

        public DateTime GetLastRemoveLogDate(string userName)
        {
            var retval = DateTime.MinValue;
            var sqlString = SqlUtils.ToTopSqlString("siteserver_Log", "AddDate", "WHERE Action = '清空数据库日志'", "ORDER BY ID DESC", 1);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmUserName, DataType.VarChar, 50, userName)
			};

            using (var rdr = ExecuteReader(sqlString, parms))
            {
                if (rdr.Read())
                {
                    retval = GetDateTime(rdr, 0);
                }
                rdr.Close();
            }
            return retval;
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
            if (dateFrom > DateUtils.SqlMinValue)
            {
                builder.Append($" AND AddDate >= {SqlUtils.GetComparableDate(dateFrom)}");
            }
            if (dateTo != DateUtils.SqlMinValue)
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
            if (dateFrom > DateUtils.SqlMinValue)
            {
                builder.Append($" AND AddDate >= {SqlUtils.GetComparableDate(dateFrom)}");
            }
            if (dateTo != DateUtils.SqlMinValue)
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
