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

        public async Task<IEnumerable<KeyValuePair<int, string>>> GetLogIdWithNameListAsync(int siteId, int templateId)
        {
            var list = await _repository.GetAllAsync(Q
                .Where(nameof(TemplateLog.TemplateId), templateId)
                .OrderByDesc(nameof(TemplateLog.Id))
            );

            return list.Select(templateLog => new KeyValuePair<int, string>(templateLog.Id, $"修订时间：{DateUtils.GetDateAndTimeString(templateLog.AddDate)}，修订人：{templateLog.AddUserName}，字符数：{templateLog.ContentLength}"));
        }

        public async Task DeleteAsync(int logId)
        {
            await _repository.DeleteAsync(logId);
        }
    }
}
