using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Provider
{
    public class TaskLogDao : DataProviderBase
    {
        public override string TableName => "siteserver_TaskLog";

        public override List<TableColumnInfo> TableColumns => new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = nameof(TaskLogInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskLogInfo.TaskId),
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskLogInfo.IsSuccess),
                DataType = DataType.VarChar,
                Length = 18
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskLogInfo.ErrorMessage),
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = nameof(TaskLogInfo.AddDate),
                DataType = DataType.DateTime
            }
        };

        private const string ParmTaskId = "@TaskID";
        private const string ParmIsSuccess = "@IsSuccess";
        private const string ParmErrorMessage = "@ErrorMessage";
        private const string ParmAddDate = "@AddDate";

        public void Insert(TaskLogInfo log)
        {
            if (ConfigManager.SystemConfigInfo.IsLogTask)
            {
                DeleteExcess90Days();

                var sqlString = "INSERT INTO siteserver_TaskLog(TaskID, IsSuccess, ErrorMessage, AddDate) VALUES (@TaskID, @IsSuccess, @ErrorMessage, @AddDate)";
                
                var parms = new IDataParameter[]
			    {
                    GetParameter(ParmTaskId, DataType.Integer, log.TaskId),
                    GetParameter(ParmIsSuccess, DataType.VarChar, 18, log.IsSuccess.ToString()),
                    GetParameter(ParmErrorMessage, DataType.VarChar, 255, log.ErrorMessage),
				    GetParameter(ParmAddDate, DataType.DateTime, log.AddDate)
			    };

                ExecuteNonQuery(sqlString, parms);
            }
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM siteserver_TaskLog WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteExcess90Days()
        {
            ExecuteNonQuery($"DELETE FROM siteserver_TaskLog WHERE {SqlUtils.GetDateDiffGreatThanDays("AddDate", 90.ToString())}");
        }

        public void DeleteAll()
        {
            var sqlString = "DELETE FROM siteserver_TaskLog";

            ExecuteNonQuery(sqlString);
        }

        public string GetSelectCommend()
        {
            return "SELECT ID, TaskID, IsSuccess, ErrorMessage, AddDate FROM siteserver_TaskLog";
        }

        public string GetSelectCommend(ETriState successState, string keyword, string dateFrom, string dateTo)
        {
            var whereString = new StringBuilder("WHERE ");

            if (successState != ETriState.All)
            {
                whereString.AppendFormat("IsSuccess = '{0}'", ETriStateUtils.GetValue(successState));
            }
            else
            {
                whereString.Append("1=1");
            }            

            if (!string.IsNullOrEmpty(keyword))
            {
                whereString.Append(" AND ");
                whereString.AppendFormat("(ErrorMessage LIKE '%{0}%')", PageUtils.FilterSql(keyword));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereString.Append(" AND ");
                whereString.AppendFormat("(AddDate >= '{0}')", dateFrom);
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereString.Append(" AND ");
                whereString.AppendFormat("(AddDate <= '{0}')", dateTo);
            }

            return "SELECT ID, TaskID, IsSuccess, ErrorMessage, AddDate FROM siteserver_TaskLog " + whereString;
        }
    }
}
