using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Context;

namespace SiteServer.CMS.Repositories
{
    public class TemplateLogRepository : IRepository
    {
        private readonly Repository<TemplateLog> _repository;

        public TemplateLogRepository()
        {
            _repository = new Repository<TemplateLog>(new Database(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString));
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task InsertAsync(TemplateLog log)
        {
            await _repository.InsertAsync(log);
        }

        public string GetSelectCommend(int siteId, int templateId)
        {
            return
                $"SELECT ID, TemplateId, SiteId, AddDate, AddUserName, ContentLength, TemplateContent FROM siteserver_TemplateLog WHERE SiteId = {siteId} AND TemplateId = {templateId}";
        }

        public async Task<string> GetTemplateContentAsync(int logId)
        {
            return await _repository.GetAsync<string>(Q.Select(nameof(TemplateLog.TemplateContent))
                .Where(nameof(TemplateLog.Id), logId)
            );
        }

        public async Task<Dictionary<int, string>> GetLogIdWithNameDictionaryAsync(int siteId, int templateId)
        {
            var list = await _repository.GetAllAsync(Q
                .Where(nameof(TemplateLog.TemplateId), templateId)
                .OrderByDesc(nameof(TemplateLog.Id))
            );

            return list.ToDictionary(templateLog => templateLog.Id,
                templateLog =>
                    $"修订时间：{DateUtils.GetDateAndTimeString(templateLog.AddDate)}，修订人：{templateLog.AddUserName}，字符数：{templateLog.ContentLength}");
        }

        public async Task DeleteAsync(List<int> idList)
        {
            if (idList != null && idList.Count > 0)
            {
                await _repository.DeleteAsync(Q.WhereIn(nameof(TemplateLog.Id), idList));
            }
        }
    }
}
