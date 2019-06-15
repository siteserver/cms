using System;
using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Data;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class TemplateLogRepository : ITemplateLogRepository
    {
        private readonly Repository<TemplateLogInfo> _repository;
        public TemplateLogRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<TemplateLogInfo>(new Db(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDb Db => _repository.Db;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

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
            _repository.Insert(logInfo);
        }

        public string GetSelectCommend(int siteId, int templateId)
        {
            return
                $"SELECT ID, TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent FROM siteserver_TemplateLog WHERE SiteId = {siteId} AND TemplateId = {templateId}";
        }

        public string GetTemplateContent(int logId)
        {
            return _repository.Get<string>(Q
                .Select(Attr.TemplateContent)
                .Where(Attr.Id, logId));
        }

        public Dictionary<int, string> GetLogIdWithNameDictionary(int templateId)
        {
            var dictionary = new Dictionary<int, string>();

            var dataList = _repository.GetAll<(int Id, DateTime? AddDate, string AddUserName, int ContentLength)>(Q
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

            return dictionary;
        }

        public void Delete(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                _repository.Delete(Q.WhereIn(Attr.Id, idList));
            }
        }
    }
}


// using System.Collections.Generic;
// using System.Data;
// using Datory;
// using SiteServer.Utils;
// using SiteServer.CMS.Model;

// namespace SiteServer.CMS.Provider
// {
//     public class TemplateLogDao
//     {
//         public override string TableName => "siteserver_TemplateLog";

//         public override List<TableColumn> TableColumns => new List<TableColumn>
//         {
//             new TableColumn
//             {
//                 AttributeName = nameof(TemplateLogInfo.Id),
//                 DataType = DataType.Integer,
//                 IsIdentity = true,
//                 IsPrimaryKey = true
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TemplateLogInfo.TemplateId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TemplateLogInfo.SiteId),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TemplateLogInfo.AddDate),
//                 DataType = DataType.DateTime
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TemplateLogInfo.AddUserName),
//                 DataType = DataType.VarChar,
//                 DataLength = 255
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TemplateLogInfo.ContentLength),
//                 DataType = DataType.Integer
//             },
//             new TableColumn
//             {
//                 AttributeName = nameof(TemplateLogInfo.TemplateContent),
//                 DataType = DataType.Text
//             }
//         };

//         private const string ParmTemplateId = "@TemplateId";
//         private const string ParmSiteId = "@SiteId";
//         private const string ParmAddDate = "@AddDate";
//         private const string ParmAddUserName = "@AddUserName";
//         private const string ParmContentLength = "@ContentLength";
//         private const string ParmTemplateContent = "@TemplateContent";

//         public void Insert(TemplateLogInfo logInfo)
//         {
//             var sqlString = "INSERT INTO siteserver_TemplateLog(TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent) VALUES (@TemplateId, @SiteId, @AddDate, @AddUserName, @ContentLength, @TemplateContent)";

//             var parms = new IDataParameter[]
// 			{
//                 GetParameter(ParmTemplateId, DataType.Integer, logInfo.TemplateId),
//                 GetParameter(ParmSiteId, DataType.Integer, logInfo.SiteId),
//                 GetParameter(ParmAddDate, DataType.DateTime, logInfo.AddDate),
//                 GetParameter(ParmAddUserName, DataType.VarChar, 255, logInfo.AddUserName),
//                 GetParameter(ParmContentLength, DataType.Integer, logInfo.ContentLength),
// 				GetParameter(ParmTemplateContent, DataType.Text, logInfo.TemplateContent)
// 			};

//             ExecuteNonQuery(sqlString, parms);
//         }

//         public string GetSelectCommend(int siteId, int templateId)
//         {
//             return
//                 $"SELECT ID, TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent FROM siteserver_TemplateLog WHERE SiteId = {siteId} AND TemplateId = {templateId}";
//         }

//         public string GetTemplateContent(int logId)
//         {
//             var templateContent = string.Empty;

//             string sqlString = $"SELECT TemplateContent FROM siteserver_TemplateLog WHERE ID = {logId}";

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 if (rdr.Read())
//                 {
//                     templateContent = GetString(rdr, 0);
//                 }
//                 rdr.Close();
//             }

//             return templateContent;
//         }

//         public Dictionary<int, string> GetLogIdWithNameDictionary(int siteId, int templateId)
//         {
//             var dictionary = new Dictionary<int, string>();

//             string sqlString =
//                 $"SELECT ID, AddDate, AddUserName, ContentLength FROM siteserver_TemplateLog WHERE TemplateId = {templateId}";

//             using (var rdr = ExecuteReader(sqlString))
//             {
//                 while (rdr.Read())
//                 {
//                     var id = GetInt(rdr, 0);
//                     var addDate = GetDateTime(rdr, 1);
//                     var addUserName = GetString(rdr, 2);
//                     var contentLength = GetInt(rdr, 3);

//                     string name =
//                         $"修订时间：{DateUtils.GetDateAndTimeString(addDate)}，修订人：{addUserName}，字符数：{contentLength}";

//                     dictionary.Add(id, name);
//                 }
//                 rdr.Close();
//             }

//             return dictionary;
//         }

//         public void Delete(List<int> idList)
//         {
//             if (idList != null && idList.Count > 0)
//             {
//                 string sqlString =
//                     $"DELETE FROM siteserver_TemplateLog WHERE Id IN ({TranslateUtils.ToSqlInStringWithoutQuote(idList)})";

//                 ExecuteNonQuery(sqlString);
//             }
//         }
//     }
// }
