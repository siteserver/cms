using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public class TemplateLogRepository : ITemplateLogRepository
    {
        private readonly Repository<TemplateLog> _repository;
        public TemplateLogRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<TemplateLog>(new Database(settingsManager.DatabaseType, settingsManager.DatabaseConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;
        public List<TableColumn> TableColumns => _repository.TableColumns;

        private static class Attr
        {
            public const string Id = nameof(TemplateLog.Id);
            public const string TemplateId = nameof(TemplateLog.TemplateId);
            public const string CreatedDate = nameof(TemplateLog.CreatedDate);
            public const string UserId = nameof(TemplateLog.UserId);
            public const string ContentLength = nameof(TemplateLog.ContentLength);
            public const string TemplateContent = nameof(TemplateLog.TemplateContent);
        }

        public async Task<int> InsertAsync(TemplateLog logInfo)
        {
            return await _repository.InsertAsync(logInfo);
        }

        public string GetSelectCommend(int siteId, int templateId)
        {
            return
                $"SELECT ID, TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent FROM siteserver_TemplateLog WHERE SiteId = {siteId} AND TemplateId = {templateId}";
        }

        public async Task<string> GetTemplateContentAsync(int logId)
        {
            return await _repository.GetAsync<string>(Q
                .Select(Attr.TemplateContent)
                .Where(Attr.Id, logId));
        }

        // public Dictionary<int, string> GetLogIdWithNameDictionary(int templateId)
        // {
        //     var dictionary = new Dictionary<int, string>();

        //     var dataList = _repository.GetAll<(int Id, DateTimeOffset? CreatedDate, string AddUserName, int ContentLength)>(Q
        //         .Select(Attr.Id, Attr.CreatedDate, Attr.UserId, Attr.ContentLength)
        //         .Where(Attr.TemplateId, templateId));

        //     foreach (var result in dataList)
        //     {
        //         var id = result.Id;
        //         var creationDate = result.CreatedDate;
        //         var addUserName = result.AddUserName;
        //         var contentLength = result.ContentLength;

        //         var name =
        //             $"修订时间：{DateUtils.GetDateAndTimeString(creationDate)}，修订人：{addUserName}，字符数：{contentLength}";

        //         dictionary.Add(id, name);
        //     }

        //     return dictionary;
        // }

        public async Task DeleteAsync(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                await _repository.DeleteAsync(Q.WhereIn(Attr.Id, idList));
            }
        }
    }
}