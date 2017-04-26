using System.Collections.Generic;
using System.Data;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model;

namespace SiteServer.CMS.Provider
{
    public class TemplateLogDao : DataProviderBase
    {
        private const string ParmTemplateId = "@TemplateID";
        private const string ParmPublishmentSystemId = "@PublishmentSystemID";
        private const string ParmAddDate = "@AddDate";
        private const string ParmAddUserName = "@AddUserName";
        private const string ParmContentLength = "@ContentLength";
        private const string ParmTemplateContent = "@TemplateContent";

        public void Insert(TemplateLogInfo logInfo)
        {
            var sqlString = "INSERT INTO siteserver_TemplateLog(TemplateID, PublishmentSystemID, AddDate, AddUserName, ContentLength, TemplateContent) VALUES (@TemplateID, @PublishmentSystemID, @AddDate, @AddUserName, @ContentLength, @TemplateContent)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmTemplateId, EDataType.Integer, logInfo.TemplateId),
                GetParameter(ParmPublishmentSystemId, EDataType.Integer, logInfo.PublishmentSystemId),
                GetParameter(ParmAddDate, EDataType.DateTime, logInfo.AddDate),
                GetParameter(ParmAddUserName, EDataType.NVarChar, 255, logInfo.AddUserName),
                GetParameter(ParmContentLength, EDataType.Integer, logInfo.ContentLength),
				GetParameter(ParmTemplateContent, EDataType.NText, logInfo.TemplateContent)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public string GetSelectCommend(int publishmentSystemId, int templateId)
        {
            return
                $"SELECT ID, TemplateID, PublishmentSystemID, AddDate, AddUserName, ContentLength, TemplateContent FROM siteserver_TemplateLog WHERE PublishmentSystemID = {publishmentSystemId} AND TemplateID = {templateId}";
        }

        public string GetTemplateContent(int logId)
        {
            var templateContent = string.Empty;

            string sqlString = $"SELECT TemplateContent FROM siteserver_TemplateLog WHERE ID = {logId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                if (rdr.Read())
                {
                    templateContent = GetString(rdr, 0);
                }
                rdr.Close();
            }

            return templateContent;
        }

        public Dictionary<int, string> GetLogIdWithNameDictionary(int publishmentSystemId, int templateId)
        {
            var dictionary = new Dictionary<int, string>();

            string sqlString =
                $"SELECT ID, AddDate, AddUserName, ContentLength FROM siteserver_TemplateLog WHERE TemplateID = {templateId}";

            using (var rdr = ExecuteReader(sqlString))
            {
                while (rdr.Read())
                {
                    var id = GetInt(rdr, 0);
                    var addDate = GetDateTime(rdr, 1);
                    var addUserName = GetString(rdr, 2);
                    var contentLength = GetInt(rdr, 3);

                    string name =
                        $"修订时间：{DateUtils.GetDateAndTimeString(addDate)}，修订人：{addUserName}，字符数：{contentLength}";

                    dictionary.Add(id, name);
                }
                rdr.Close();
            }

            return dictionary;
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                string sqlString =
                    $"DELETE FROM siteserver_TemplateLog WHERE ID IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }
    }
}
