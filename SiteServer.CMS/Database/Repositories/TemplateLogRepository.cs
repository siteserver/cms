using System;
using System.Collections.Generic;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils;

namespace SiteServer.CMS.Database.Repositories
{
    public class TemplateLogRepository : GenericRepository<TemplateLogInfo>
    {
        private static class Attr
        {
            public const string Id = nameof(TemplateLogInfo.Id);
            public const string TemplateId = nameof(TemplateLogInfo.TemplateId);
            public const string AddDate = nameof(TemplateLogInfo.AddDate);
            public const string AddUserName = nameof(TemplateLogInfo.AddUserName);
            public const string ContentLength = nameof(TemplateLogInfo.ContentLength);
            public const string TemplateContent = nameof(TemplateLogInfo.TemplateContent);
        }

        public void Insert(TemplateLogInfo logInfo)
        {
            //var sqlString = "INSERT INTO siteserver_TemplateLog(TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent) VALUES (@TemplateId, @SiteId, @AddDate, @AddUserName, @ContentLength, @TemplateContent)";

            //IDataParameter[] parameters =
            //{
            //    GetParameter(ParamTemplateId, logInfo.TemplateId),
            //    GetParameter(ParamSiteId, logInfo.SiteId),
            //    GetParameter(ParamAddDate,logInfo.AddDate),
            //    GetParameter(ParamAddUserName, logInfo.AddUserName),
            //    GetParameter(ParamContentLength, logInfo.ContentLength),
            //    GetParameter(ParamTemplateContent,logInfo.TemplateContent)
            //};

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);

            InsertObject(logInfo);
        }

        public string GetSelectCommend(int siteId, int templateId)
        {
            return
                $"SELECT ID, TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent FROM siteserver_TemplateLog WHERE SiteId = {siteId} AND TemplateId = {templateId}";
        }

        public string GetTemplateContent(int logId)
        {
            //var templateContent = string.Empty;

            //var sqlString = $"SELECT TemplateContent FROM siteserver_TemplateLog WHERE ID = {logId}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    if (rdr.Read())
            //    {
            //        templateContent = DatabaseApi.GetString(rdr, 0);
            //    }
            //    rdr.Close();
            //}

            //return templateContent;

            return GetValue<string>(Q
                .Select(Attr.TemplateContent)
                .Where(Attr.Id, logId));
        }

        public Dictionary<int, string> GetLogIdWithNameDictionary(int templateId)
        {
            var dictionary = new Dictionary<int, string>();

            var dataList = GetValueList<(int Id, DateTime? AddDate, string AddUserName, int ContentLength)>(Q
                .Select(Attr.Id, Attr.AddDate, Attr.AddUserName, Attr.ContentLength)
                .Where(Attr.TemplateId, templateId));

            foreach (var result in dataList)
            {
                var id = result.Id;
                var addDate = result.AddDate;
                var addUserName = result.AddUserName;
                var contentLength = result.ContentLength;

                var name =
                    $"修订时间：{DateUtils.GetDateAndTimeString(addDate)}，修订人：{addUserName}，字符数：{contentLength}";

                dictionary.Add(id, name);
            }

            //string sqlString =
            //    $"SELECT ID, AddDate, AddUserName, ContentLength FROM siteserver_TemplateLog WHERE TemplateId = {templateId}";

            //using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
            //{
            //    while (rdr.Read())
            //    {
            //        var id = DatabaseApi.GetInt(rdr, 0);
            //        var addDate = DatabaseApi.GetDateTime(rdr, 1);
            //        var addUserName = DatabaseApi.GetString(rdr, 2);
            //        var contentLength = DatabaseApi.GetInt(rdr, 3);

            //        string name =
            //            $"修订时间：{DateUtils.GetDateAndTimeString(addDate)}，修订人：{addUserName}，字符数：{contentLength}";

            //        dictionary.Add(id, name);
            //    }
            //    rdr.Close();
            //}

            return dictionary;
        }

        public void Delete(List<int> idList)
        {
            //string sqlString =
            //    $"DELETE FROM siteserver_TemplateLog WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

            //DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);

            if (idList != null && idList.Count > 0)
            {
                DeleteAll(Q.WhereIn(Attr.Id, idList));
            }
        }
    }
}


//using System.Collections.Generic;
//using System.Data;
//using SiteServer.CMS.Database.Core;
//using SiteServer.CMS.Database.Models;
//using SiteServer.Plugin;
//using SiteServer.Utils;

//namespace SiteServer.CMS.Database.Repositories
//{
//    public class TemplateLog : DataProviderBase
//    {
//        public override string TableName => "siteserver_TemplateLog";

//        public override List<TableColumn> TableColumns => new List<TableColumn>
//        {
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateLogInfo.Id),
//                DataType = DataType.Integer,
//                IsIdentity = true,
//                IsPrimaryKey = true
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateLogInfo.TemplateId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateLogInfo.SiteId),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateLogInfo.AddDate),
//                DataType = DataType.DateTime
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateLogInfo.AddUserName),
//                DataType = DataType.VarChar
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateLogInfo.ContentLength),
//                DataType = DataType.Integer
//            },
//            new TableColumn
//            {
//                AttributeName = nameof(TemplateLogInfo.TemplateContent),
//                DataType = DataType.Text
//            }
//        };

//        private const string ParamTemplateId = "@TemplateId";
//        private const string ParamSiteId = "@SiteId";
//        private const string ParamAddDate = "@AddDate";
//        private const string ParamAddUserName = "@AddUserName";
//        private const string ParamContentLength = "@ContentLength";
//        private const string ParamTemplateContent = "@TemplateContent";

//        public void InsertObject(TemplateLogInfo logInfo)
//        {
//            var sqlString = "INSERT INTO siteserver_TemplateLog(TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent) VALUES (@TemplateId, @SiteId, @AddDate, @AddUserName, @ContentLength, @TemplateContent)";

//            IDataParameter[] parameters =
//			{
//                GetParameter(ParamTemplateId, logInfo.TemplateId),
//                GetParameter(ParamSiteId, logInfo.SiteId),
//                GetParameter(ParamAddDate,logInfo.AddDate),
//                GetParameter(ParamAddUserName, logInfo.AddUserName),
//                GetParameter(ParamContentLength, logInfo.ContentLength),
//				GetParameter(ParamTemplateContent,logInfo.TemplateContent)
//			};

//            DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString, parameters);
//        }

//        public string GetSelectCommend(int siteId, int templateId)
//        {
//            return
//                $"SELECT ID, TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent FROM siteserver_TemplateLog WHERE SiteId = {siteId} AND TemplateId = {templateId}";
//        }

//        public string GetTemplateContent(int logId)
//        {
//            var templateContent = string.Empty;

//            string sqlString = $"SELECT TemplateContent FROM siteserver_TemplateLog WHERE ID = {logId}";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                if (rdr.Read())
//                {
//                    templateContent = DatabaseApi.GetString(rdr, 0);
//                }
//                rdr.Close();
//            }

//            return templateContent;
//        }

//        public Dictionary<int, string> GetLogIdWithNameDictionary(int siteId, int templateId)
//        {
//            var dictionary = new Dictionary<int, string>();

//            string sqlString =
//                $"SELECT ID, AddDate, AddUserName, ContentLength FROM siteserver_TemplateLog WHERE TemplateId = {templateId}";

//            using (var rdr = DatabaseApi.ExecuteReader(ConnectionString, sqlString))
//            {
//                while (rdr.Read())
//                {
//                    var id = DatabaseApi.GetInt(rdr, 0);
//                    var addDate = DatabaseApi.GetDateTime(rdr, 1);
//                    var addUserName = DatabaseApi.GetString(rdr, 2);
//                    var contentLength = DatabaseApi.GetInt(rdr, 3);

//                    string name =
//                        $"修订时间：{DateUtils.GetDateAndTimeString(addDate)}，修订人：{addUserName}，字符数：{contentLength}";

//                    dictionary.Add(id, name);
//                }
//                rdr.Close();
//            }

//            return dictionary;
//        }

//        public void DeleteById(List<int> idList)
//        {
//            if (idList != null && idList.Count > 0)
//            {
//                string sqlString =
//                    $"DELETE FROM siteserver_TemplateLog WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

//                DatabaseApi.ExecuteNonQuery(ConnectionString, sqlString);
//            }
//        }
//    }
//}
