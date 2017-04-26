using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class CreateTaskLogDao : DataProviderBase
    {
        private const string ParmCreateType = "@CreateType";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmTaskName = "@TaskName";
        private const string ParmTimeSpan = "@TimeSpan";
        private const string ParmIsSuccess = "@IsSuccess";
        private const string ParmErrorMessage = "@ErrorMessage";
        private const string ParmAddDate = "@AddDate";

        public void Insert(CreateTaskLogInfo log)
        {
            DeleteExcess90Days();

            var sqlString = "INSERT INTO siteserver_CreateTaskLog(CreateType, PublishmentSystemID, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate) VALUES (@CreateType, @PublishmentSystemID, @TaskName, @TimeSpan, @IsSuccess, @ErrorMessage, @AddDate)";

            var parms = new IDataParameter[]
            {
                GetParameter(ParmCreateType, EDataType.NVarChar, 50, ECreateTypeUtils.GetValue(log.CreateType)),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, log.PublishmentSystemID),
                GetParameter(ParmTaskName, EDataType.NVarChar, 50, log.TaskName),
                GetParameter(ParmTimeSpan, EDataType.NVarChar, 50, log.TimeSpan),
                GetParameter(ParmIsSuccess, EDataType.VarChar, 18, log.IsSuccess.ToString()),
                GetParameter(ParmErrorMessage, EDataType.NVarChar, 255, log.ErrorMessage),
                GetParameter(ParmAddDate, EDataType.DateTime, log.AddDate)
            };

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM siteserver_CreateTaskLog WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteExcess90Days()
        {
            ExecuteNonQuery("DELETE FROM siteserver_CreateTaskLog WHERE " + SqlUtils.GetDateDiffGreatThanDays("AddDate", 90.ToString()));
        }

        public void DeleteAll()
        {
            const string sqlString = "DELETE FROM siteserver_CreateTaskLog";

            ExecuteNonQuery(sqlString);
        }

        public List<CreateTaskLogInfo> GetList(int publishmentSystemId, int totalNum)
        {
            var list = new List<CreateTaskLogInfo>();

            if (publishmentSystemId > 0)
            {
                //string sqlString =
                //    $"SELECT TOP {totalNum} ID, CreateType, PublishmentSystemID, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate FROM siteserver_CreateTaskLog WHERE PublishmentSystemID = @PublishmentSystemID";
                string sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTaskLog", "ID, CreateType, PublishmentSystemID, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate", "WHERE PublishmentSystemID = @PublishmentSystemID", totalNum);

                var parms = new IDataParameter[]
                {
                    GetParameter(ParmPublishmentSystemId, EDataType.Integer, publishmentSystemId)
                };

                using (var rdr = ExecuteReader(sqlString, parms))
                {
                    while (rdr.Read())
                    {
                        var i = 0;
                        var info = new CreateTaskLogInfo(GetInt(rdr, i++), ECreateTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                        list.Add(info);
                    }
                    rdr.Close();
                }
            }
            else
            {
                //string sqlString =
                //    $"SELECT TOP {totalNum} ID, CreateType, PublishmentSystemID, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate FROM siteserver_CreateTaskLog";
                var sqlString = SqlUtils.GetTopSqlString("siteserver_CreateTaskLog", "ID, CreateType, PublishmentSystemID, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate", string.Empty, totalNum);

                using (var rdr = ExecuteReader(sqlString))
                {
                    while (rdr.Read())
                    {
                        var i = 0;
                        var info = new CreateTaskLogInfo(GetInt(rdr, i++), ECreateTypeUtils.GetEnumType(GetString(rdr, i++)), GetInt(rdr, i++), GetString(rdr, i++), GetString(rdr, i++), GetBool(rdr, i++), GetString(rdr, i++), GetDateTime(rdr, i));
                        list.Add(info);
                    }
                    rdr.Close();
                }
            }

            return list;
        }

        public string GetSelectCommend()
        {
            return "SELECT ID, CreateType, PublishmentSystemID, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate FROM siteserver_CreateTaskLog";
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
                whereString.Append("1 = 1");
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

            return "SELECT ID, CreateType, PublishmentSystemID, TaskName, TimeSpan, IsSuccess, ErrorMessage, AddDate FROM siteserver_CreateTaskLog " + whereString;
        }
    }
}
