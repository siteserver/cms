using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class UserLogDao : DataProviderBase
    {
        private const string ParmUserName = "@UserName";
        private const string ParmIpAddress = "@IPAddress";
        private const string ParmAddDate = "@AddDate";
        private const string ParmAction = "@Action";
        private const string ParmSummary = "@Summary";

        public void Insert(UserLogInfo userLog)
        {
            var sqlString = "INSERT INTO bairong_UserLog(UserName, IPAddress, AddDate, Action, Summary) VALUES (@UserName, @IPAddress, @AddDate, @Action, @Summary)";

            var parms = new IDataParameter[]
            {
                    GetParameter(ParmUserName, EDataType.VarChar, 50, userLog.UserName),
                    GetParameter(ParmIpAddress, EDataType.VarChar, 50, userLog.IpAddress),
                    GetParameter(ParmAddDate, EDataType.DateTime, userLog.AddDate),
                    GetParameter(ParmAction, EDataType.NVarChar, 255, userLog.Action),
                    GetParameter(ParmSummary, EDataType.NVarChar, 255, userLog.Summary)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(int days, int counter)
        {
            if (days > 0)
            {
                ExecuteNonQuery($@"DELETE FROM bairong_UserLog WHERE AddDate < '{DateUtils.GetDateAndTimeString(DateTime.Now.AddDays(-days))}'");
            }
            if (counter > 0)
            {
                ExecuteNonQuery($@"DELETE FROM bairong_UserLog WHERE ID IN(
SELECT ID from(
SELECT ID, ROW_NUMBER() OVER(ORDER BY AddDate DESC) as rowNum FROM bairong_UserLog) as t
WHERE t.rowNum > {counter})");
            }
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM bairong_UserLog WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll()
        {
            var sqlString = "DELETE FROM bairong_UserLog";

            ExecuteNonQuery(sqlString);
        }

        public int GetCount()
        {
            var count = 0;
            var sqlString = "SELECT Count(ID) FROM bairong_UserLog";

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
            var sqlString = "SELECT Count(ID) FROM bairong_UserLog";
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
            return "SELECT ID, UserName, IPAddress, AddDate, Action, Summary FROM bairong_UserLog";
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
                whereString.AppendFormat("(UserName = '{0}')", PageUtils.FilterSql(userName));
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.AppendFormat("(Action LIKE '%{0}%' OR Summary LIKE '%{0}%')", PageUtils.FilterSql(keyword));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.AppendFormat("(AddDate >= '{0}')", PageUtils.FilterSql(dateFrom));
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                whereString.AppendFormat("(AddDate <= '{0}')", PageUtils.FilterSql(dateTo));
            }

            return "SELECT ID, UserName, IPAddress, AddDate, Action, Summary FROM bairong_UserLog " + whereString;
        }

        public DateTime GetLastUserLoginDate(string userName)
        {
            var retval = DateTime.MinValue;
            //const string sqlString = "SELECT TOP 1 AddDate FROM bairong_UserLog WHERE UserName = @UserName ORDER BY ID DESC";
            var sqlString = SqlUtils.GetTopSqlString("bairong_UserLog", "AddDate", "WHERE UserName = @UserName ORDER BY ID DESC", 1);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmUserName, EDataType.VarChar, 50, userName)
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
            //const string sqlString = "SELECT TOP 1 AddDate FROM bairong_UserLog WHERE UserName = @UserName AND Action = '清空数据库日志' ORDER BY ID DESC";
            var sqlString = SqlUtils.GetTopSqlString("bairong_UserLog", "AddDate", "WHERE UserName = @UserName AND Action = '清空数据库日志' ORDER BY ID DESC", 1);

            var parms = new IDataParameter[]
			{
				GetParameter(ParmUserName, EDataType.VarChar, 50, userName)
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

        public List<UserLogInfo> List(string userName, int totalNum, string action)
        {
            var list = new List<UserLogInfo>();
            var sqlString = "SELECT * FROM bairong_UserLog WHERE UserName = @UserName";

            if (!string.IsNullOrEmpty(action))
            {
                sqlString += " And Action = @Action";
            }
            else
            {
                action = EUserActionTypeUtils.GetValue(EUserActionType.Login);
                sqlString += " And Action <> @Action";
            }
            sqlString += " ORDER BY ID DESC";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmUserName, EDataType.VarChar, 50, userName),
                GetParameter(ParmAction, EDataType.NVarChar, 255, action)
            };

            using (var rdr = ExecuteReader(sqlString, parms))
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
    }
}
