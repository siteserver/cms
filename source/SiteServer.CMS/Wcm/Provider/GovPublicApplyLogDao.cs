using System.Collections;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.Provider
{
    public class GovPublicApplyLogDao : DataProviderBase
    {
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmApplyId = "@ApplyID";
        private const string ParmDepartmentId = "@DepartmentID";
        private const string ParmUserName = "@UserName";
        private const string ParmLogType = "@LogType";
        private const string ParmIpAddress = "@IPAddress";
        private const string ParmAddDate = "@AddDate";
        private const string ParmSummary = "@Summary";

        public void Insert(GovPublicApplyLogInfo logInfo)
        {
            var sqlString = "INSERT INTO wcm_GovPublicApplyLog(PublishmentSystemID, ApplyID, DepartmentID, UserName, LogType, IPAddress, AddDate, Summary) VALUES (@PublishmentSystemID, @ApplyID, @DepartmentID, @UserName, @LogType, @IPAddress, @AddDate, @Summary)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, logInfo.PublishmentSystemID),
                GetParameter(ParmApplyId, EDataType.Integer, logInfo.ApplyID),
                GetParameter(ParmDepartmentId, EDataType.Integer, logInfo.DepartmentID),
				GetParameter(ParmUserName, EDataType.VarChar, 50, logInfo.UserName),
                GetParameter(ParmLogType, EDataType.VarChar, 50, EGovPublicApplyLogTypeUtils.GetValue(logInfo.LogType)),
				GetParameter(ParmIpAddress, EDataType.VarChar, 50, logInfo.IPAddress),
                GetParameter(ParmAddDate, EDataType.DateTime, logInfo.AddDate),
				GetParameter(ParmSummary, EDataType.NVarChar, 255, logInfo.Summary)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public void Delete(ArrayList idArrayList)
        {
            if (idArrayList != null && idArrayList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM wcm_GovPublicApplyLog WHERE LogID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idArrayList)})";

                ExecuteNonQuery(sqlString);
            }
        }

        public void DeleteAll(int publishmentSystemId)
        {
            string sqlString = $"DELETE FROM wcm_GovPublicApplyLog WHERE PublishmentSystemID = {publishmentSystemId}";

            ExecuteNonQuery(sqlString);
        }

        public IEnumerable GetDataSourceByApplyId(int applyId)
        {
            string sqlString =
                $"SELECT LogID, PublishmentSystemID, ApplyID, DepartmentID, UserName, LogType, IPAddress, AddDate, Summary FROM wcm_GovPublicApplyLog WHERE ApplyID = {applyId}";

            var enumerable = (IEnumerable)ExecuteReader(sqlString);
            return enumerable;
        }

        public ArrayList GetLogInfoArrayList(int applyId)
        {
            var arraylist = new ArrayList();

            string sqlString =
                $"SELECT LogID, PublishmentSystemID, ApplyID, DepartmentID, UserName, LogType, IPAddress, AddDate, Summary FROM wcm_GovPublicApplyLog WHERE ApplyID = {applyId}";
            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var i = 0;
                    var logInfo = new GovPublicApplyLogInfo(GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetInt(rdr, i++), GetString(rdr, i++), EGovPublicApplyLogTypeUtils.GetEnumType(GetString(rdr, i++)), GetString(rdr, i++), GetDateTime(rdr, i++), GetString(rdr, i));
                    arraylist.Add(logInfo);
                }
                rdr.Close();
            }
            return arraylist;
        }

        public string GetSelectCommend(int publishmentSystemId)
        {
            return
                $"SELECT LogID, PublishmentSystemID, ApplyID, DepartmentID, UserName, LogType, IPAddress, AddDate, Summary FROM wcm_GovPublicApplyLog WHERE PublishmentSystemID = {publishmentSystemId}";
        }

        public string GetSelectCommend(int publishmentSystemId, string keyword, string dateFrom, string dateTo)
        {
            if (string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return GetSelectCommend(publishmentSystemId);
            }

            var whereString = new StringBuilder();
            whereString.AppendFormat("WHERE (PublishmentSystemID = {0})", publishmentSystemId);

            if (!string.IsNullOrEmpty(keyword))
            {
                whereString.AppendFormat(" AND (UserName LIKE '%{0}%' OR Summary LIKE '%{0}%')",PageUtils.FilterSql(keyword));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                whereString.AppendFormat(" AND (AddDate >= '{0}')", dateFrom);
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                whereString.AppendFormat(" AND (AddDate <= '{0}')", dateTo);
            }

            return "SELECT LogID, PublishmentSystemID, ApplyID, DepartmentID, UserName, LogType, IPAddress, AddDate, Summary FROM wcm_GovPublicApplyLog " + whereString;
        }
    }
}
