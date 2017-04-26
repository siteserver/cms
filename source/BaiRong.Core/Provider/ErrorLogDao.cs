using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;

namespace BaiRong.Core.Provider
{
    public class ErrorLogDao : DataProviderBase
    {
        private const string ParmAddDate = "@AddDate";
        private const string ParmMessage = "@Message";
        private const string ParmStacktrace = "@Stacktrace";
        private const string ParmSummary = "@Summary";

        public void Insert(ErrorLogInfo logInfo)
        {
            var sqlString = "INSERT INTO bairong_ErrorLog(AddDate, Message, Stacktrace, Summary) VALUES (@AddDate, @Message, @Stacktrace, @Summary)";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmAddDate, EDataType.DateTime, logInfo.AddDate),
                GetParameter(ParmMessage, EDataType.NVarChar, 255, logInfo.Message),
                GetParameter(ParmStacktrace, EDataType.NText, logInfo.Stacktrace),
                GetParameter(ParmSummary, EDataType.NText, logInfo.Summary)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM bairong_ErrorLog WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }

        public void Delete(int days, int counter)
        {
            const string sqlString = "DELETE FROM bairong_ErrorLog WHERE 1=1 ";
            if (days > 0)
            {
                var sql1 = sqlString + $@" AND AddDate < '{DateUtils.GetDateAndTimeString(DateTime.Now.AddDays(-days))}'";
                ExecuteNonQuery(sql1);
            }
            if (counter > 0)
            {
                var sql2 = sqlString + $@" AND ID IN(
SELECT ID from(
SELECT ID, ROW_NUMBER() OVER(ORDER BY AddDate DESC) as rowNum FROM bairong_ErrorLog) as t
WHERE t.rowNum > {counter})";
                ExecuteNonQuery(sql2);
            }
        }

        public void DeleteAll()
        {
            const string sqlString = "DELETE FROM bairong_ErrorLog";

            ExecuteNonQuery(sqlString);
        }

        public int GetCount()
        {
            var count = 0;
            const string sqlString = "SELECT Count(ID) FROM bairong_ErrorLog";

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
            return "SELECT ID, AddDate, Message, Stacktrace, Summary FROM bairong_ErrorLog";
        }

        public string GetSelectCommend(string keyword, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return GetSelectCommend();
            }

            var whereString = new StringBuilder("WHERE ");

            var isWhere = false;

            if (!string.IsNullOrEmpty(keyword))
            {
                isWhere = true;
                var filterKeyword = PageUtils.FilterSql(keyword);
                whereString.Append(
                    $"(Message LIKE '%{filterKeyword}%' OR Stacktrace LIKE '%{filterKeyword}%' OR Summary LIKE '%{filterKeyword}%')");
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.Append($"(AddDate >= '{dateFrom}')");
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                whereString.Append($"(AddDate <= '{dateTo}')");
            }

            return "SELECT ID, AddDate, Message, Stacktrace, Summary FROM bairong_ErrorLog " + whereString;
        }
    }
}
