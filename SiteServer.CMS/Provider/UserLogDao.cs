using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using Dapper.Contrib.Extensions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Data;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.CMS.Provider
{
    public class UserLogDao : DataProviderBase
    {
        public override string TableName => "siteserver_UserLog";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(UserLogInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(UserLogInfo.UserName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserLogInfo.IpAddress),
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = nameof(UserLogInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(UserLogInfo.Action),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(UserLogInfo.Summary),
                DataType = DataType.VarChar,
                DataLength = 255
            }
        };

        private const string ParmUserName = "@UserName";
        private const string ParmIpAddress = "@IPAddress";
        private const string ParmAddDate = "@AddDate";
        private const string ParmAction = "@Action";
        private const string ParmSummary = "@Summary";

        public void Insert(UserLogInfo userLog)
        {
            var sqlString = "INSERT INTO siteserver_UserLog(UserName, IPAddress, AddDate, Action, Summary) VALUES (@UserName, @IPAddress, @AddDate, @Action, @Summary)";

            var parms = new IDataParameter[]
            {
                    GetParameter(ParmUserName, DataType.VarChar, 50, userLog.UserName),
                    GetParameter(ParmIpAddress, DataType.VarChar, 50, userLog.IpAddress),
                    GetParameter(ParmAddDate, DataType.DateTime, userLog.AddDate),
                    GetParameter(ParmAction, DataType.VarChar, 255, userLog.Action),
                    GetParameter(ParmSummary, DataType.VarChar, 255, userLog.Summary)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void DeleteIfThreshold()
        {
            if (!ConfigManager.SystemConfigInfo.IsTimeThreshold) return;

            var days = ConfigManager.SystemConfigInfo.TimeThreshold;
            if (days <= 0) return;

            ExecuteNonQuery($@"DELETE FROM siteserver_UserLog WHERE AddDate < {SqlUtils.GetComparableDateTime(DateTime.Now.AddDays(-days))}");
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM siteserver_UserLog WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll()
        {
            var sqlString = "DELETE FROM siteserver_UserLog";

            ExecuteNonQuery(sqlString);
        }

        public int GetCount()
        {
            var count = 0;
            var sqlString = "SELECT Count(ID) FROM siteserver_UserLog";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    count = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return count;
        }

        public int GetCount(string where)
        {
            var count = 0;
            var sqlString = "SELECT Count(ID) FROM siteserver_UserLog";
            if (!string.IsNullOrEmpty(where))
                sqlString += " WHERE " + where;

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    count = GetInt(rdr, 0);
                }
                rdr.Close();
            }

            return count;
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

        public DateTime GetLastUserLoginDate(string userName)
        {
            var retval = DateTime.MinValue;
            //const string sqlString = "SELECT TOP 1 AddDate FROM siteserver_UserLog WHERE UserName = @UserName ORDER BY ID DESC";
            var sqlString = SqlUtils.ToTopSqlString("siteserver_UserLog", "AddDate", "WHERE UserName = @UserName",
                "ORDER BY ID DESC", 1);

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

        public DateTime GetLastRemoveUserLogDate(string userName)
        {
            var retval = DateTime.MinValue;
            //const string sqlString = "SELECT TOP 1 AddDate FROM siteserver_UserLog WHERE UserName = @UserName AND Action = '清空数据库日志' ORDER BY ID DESC";
            var sqlString = SqlUtils.ToTopSqlString("siteserver_UserLog", "AddDate",
                "WHERE UserName = @UserName AND Action = '清空数据库日志'", "ORDER BY ID DESC", 1);

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

        public List<ILogInfo> List(string userName, int totalNum, string action)
        {
            var list = new List<ILogInfo>();
            var sqlString = "SELECT * FROM siteserver_UserLog WHERE UserName = @UserName";

            if (!string.IsNullOrEmpty(action))
            {
                sqlString += " And Action = @Action";
            }
            sqlString += " ORDER BY ID DESC";

            var parameters = new List<IDataParameter>
            {
                GetParameter(ParmUserName, DataType.VarChar, 50, userName)
            };
            if (!string.IsNullOrEmpty(action))
            {
                parameters.Add(GetParameter(ParmAction, DataType.VarChar, 255, action));
            }

            using (var rdr = ExecuteReader(sqlString, parameters.ToArray()))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var info = new UserLogInfo(GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i++), GetString(rdr, i));
                    list.Add(info);
                }
            }

            return list;
        }

        public List<UserLogInfo> ApiGetLogs(string userName, int offset, int limit)
        {
            var sqlString =
                DataProvider.DatabaseDao.GetPageSqlString(TableName, "*", $"WHERE {nameof(UserLogInfo.UserName)} = @{nameof(UserLogInfo.UserName)}", "ORDER BY Id DESC", offset, limit);

            using (var connection = GetConnection())
            {
                return connection.Query<UserLogInfo>(sqlString, new {UserName = userName}).ToList();
            }
        }

        public UserLogInfo ApiInsert(string userName, UserLogInfo logInfo)
        {
            logInfo.UserName = userName;
            logInfo.IpAddress = PageUtils.GetIpAddress();
            logInfo.AddDate = DateTime.Now;

            using (var connection = GetConnection())
            {
                var identity = connection.Insert(logInfo);
                if (identity > 0)
                {
                    logInfo.Id = Convert.ToInt32(identity);
                }
            }

            return logInfo;
        }
    }
}
