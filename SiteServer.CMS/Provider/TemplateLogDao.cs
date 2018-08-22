using System.Collections.Generic;
using System.Data;
using SiteServer.CMS.Data;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Provider
{
    public class TemplateLogDao : DataProviderBase
    {
        public override string TableName => "siteserver_TemplateLog";

        public override List<TableColumn> TableColumns => new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = nameof(TemplateLogInfo.Id),
                DataType = DataType.Integer,
                IsIdentity = true,
                IsPrimaryKey = true
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateLogInfo.TemplateId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateLogInfo.SiteId),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateLogInfo.AddDate),
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateLogInfo.AddUserName),
                DataType = DataType.VarChar,
                DataLength = 255
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateLogInfo.ContentLength),
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = nameof(TemplateLogInfo.TemplateContent),
                DataType = DataType.Text
            }
        };

        private const string ParmTemplateId = "@TemplateId";
        private const string ParmSiteId = "@SiteId";
        private const string ParmAddDate = "@AddDate";
        private const string ParmAddUserName = "@AddUserName";
        private const string ParmContentLength = "@ContentLength";
        private const string ParmTemplateContent = "@TemplateContent";

        public void Insert(TemplateLogInfo logInfo)
        {
            var sqlString = "INSERT INTO siteserver_TemplateLog(TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent) VALUES (@TemplateId, @SiteId, @AddDate, @AddUserName, @ContentLength, @TemplateContent)";

            var parms = new IDataParameter[]
			{
                GetParameter(ParmTemplateId, DataType.Integer, logInfo.TemplateId),
                GetParameter(ParmSiteId, DataType.Integer, logInfo.SiteId),
                GetParameter(ParmAddDate, DataType.DateTime, logInfo.AddDate),
                GetParameter(ParmAddUserName, DataType.VarChar, 255, logInfo.AddUserName),
                GetParameter(ParmContentLength, DataType.Integer, logInfo.ContentLength),
				GetParameter(ParmTemplateContent, DataType.Text, logInfo.TemplateContent)
			};

            ExecuteNonQuery(sqlString, parms);
        }

        public string GetSelectCommend(int siteId, int templateId)
        {
            return
                $"SELECT ID, TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent FROM siteserver_TemplateLog WHERE SiteId = {siteId} AND TemplateId = {templateId}";
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

        public Dictionary<int, string> GetLogIdWithNameDictionary(int siteId, int templateId)
        {
            var dictionary = new Dictionary<int, string>();

            string sqlString =
                $"SELECT ID, AddDate, AddUserName, ContentLength FROM siteserver_TemplateLog WHERE TemplateId = {templateId}";

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
                    $"DELETE FROM siteserver_TemplateLog WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

                ExecuteNonQuery(sqlString);
            }
        }
    }
}
