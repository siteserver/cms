using System;
using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class TemplateLogRepository : ITemplateLogRepository
    {
        private readonly Repository<TemplateLogInfo> _repository;
        public TemplateLogRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<TemplateLogInfo>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(TemplateLogInfo.Id);
            public const string TemplateId = nameof(TemplateLogInfo.TemplateId);
            public const string CreationDate = nameof(TemplateLogInfo.CreationDate);
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

            var dataList = _repository.GetAll<(int Id, DateTimeOffset? CreationDate, string AddUserName, int ContentLength)>(Q
                .Select(Attr.Id, Attr.CreationDate, Attr.AddUserName, Attr.ContentLength)
                .Where(Attr.TemplateId, templateId));

            foreach (var result in dataList)
            {
                var id = result.Id;
                var creationDate = result.CreationDate;
                var addUserName = result.AddUserName;
                var contentLength = result.ContentLength;

                var name =
                    $"修订时间：{DateUtils.GetDateAndTimeString(creationDate)}，修订人：{addUserName}，字符数：{contentLength}";

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