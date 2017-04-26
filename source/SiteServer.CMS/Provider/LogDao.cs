using System.Collections.Generic;
using System.Data;
using System.Text;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;

namespace SiteServer.CMS.Provider
{
    public class LogDao : DataProviderBase
    {
        private const string ParmPublishmentsystemid = "@PublishmentSystemID";
        private const string ParmChannelid = "@ChannelID";
        private const string ParmContentid = "@ContentID";
        private const string ParmUserName = "@UserName";
        private const string ParmIpAddress = "@IPAddress";
        private const string ParmAddDate = "@AddDate";
        private const string ParmAction = "@Action";
        private const string ParmSummary = "@Summary";

        public void Insert(Model.LogInfo log)
        {
            var sqlString = "INSERT INTO siteserver_Log(PublishmentSystemID, ChannelID, ContentID, UserName, IPAddress, AddDate, Action, Summary) VALUES (@PublishmentSystemID, @ChannelID, @ContentID, @UserName, @IPAddress, @AddDate, @Action, @Summary)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmPublishmentsystemid, EDataType.Integer, log.PublishmentSystemID),
                GetParameter(ParmChannelid, EDataType.Integer, log.ChannelID),
                GetParameter(ParmContentid, EDataType.Integer, log.ContentID),
				GetParameter(ParmUserName, EDataType.VarChar, 50, log.UserName),
				GetParameter(ParmIpAddress, EDataType.VarChar, 50, log.IPAddress),
                GetParameter(ParmAddDate, EDataType.DateTime, log.AddDate),
				GetParameter(ParmAction, EDataType.NVarChar, 255, log.Action),
				GetParameter(ParmSummary, EDataType.NVarChar, 255, log.Summary)
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

        public void DeleteAll()
        {
            var sqlString = "DELETE FROM siteserver_Log";

            ExecuteNonQuery(sqlString);
        }

        public string GetSelectCommend()
        {
            return "SELECT ID, PublishmentSystemID, ChannelID, ContentID, UserName, IPAddress, AddDate, Action, Summary FROM siteserver_Log";
        }

        public string GetSelectCommend(int publishmentSystemId, string logType, string userName, string keyword, string dateFrom, string dateTo)
        {
            if (publishmentSystemId == 0 && (string.IsNullOrEmpty(logType) || StringUtils.EqualsIgnoreCase(logType, "All")) && string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(keyword) && string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo))
            {
                return GetSelectCommend();
            }

            var whereString = new StringBuilder("WHERE ");

            var isWhere = false;

            if (publishmentSystemId > 0)
            {
                isWhere = true;
                whereString.AppendFormat("(PublishmentSystemID = {0})", publishmentSystemId);
            }

            if (!string.IsNullOrEmpty(logType) && !StringUtils.EqualsIgnoreCase(logType, "All"))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;

                if (StringUtils.EqualsIgnoreCase(logType, "Channel"))
                {
                    whereString.Append("(ChannelID > 0 AND ContentID = 0)");
                }
                else if (StringUtils.EqualsIgnoreCase(logType, "Content"))
                {
                    whereString.Append("(ChannelID > 0 AND ContentID > 0)");
                }
            }

            if (!string.IsNullOrEmpty(userName))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.AppendFormat("(UserName = '{0}')", userName);
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.AppendFormat("(Action LIKE '%{0}%' OR Summary LIKE '%{0}%')",PageUtils.FilterSql(keyword));
            }

            if (!string.IsNullOrEmpty(dateFrom))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                isWhere = true;
                whereString.AppendFormat("(AddDate >= '{0}')", dateFrom);
            }
            if (!string.IsNullOrEmpty(dateTo))
            {
                if (isWhere)
                {
                    whereString.Append(" AND ");
                }
                whereString.AppendFormat("(AddDate <= '{0}')", dateTo);
            }

            return "SELECT ID, PublishmentSystemID, ChannelID, ContentID, UserName, IPAddress, AddDate, Action, Summary FROM siteserver_Log " + whereString;
        }
    }
}
