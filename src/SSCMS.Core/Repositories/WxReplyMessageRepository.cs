using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class WxReplyMessageRepository : IWxReplyMessageRepository
    {
        private readonly Repository<WxReplyMessage> _repository;

        public WxReplyMessageRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<WxReplyMessage>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(int siteId) => CacheUtils.GetListKey(_repository.TableName, siteId);

        public async Task<int> InsertAsync(WxReplyMessage message)
        {
            return await _repository.InsertAsync(message, Q
                .CachingRemove(GetCacheKey(message.SiteId))
            );
        }

        public async Task UpdateAsync(WxReplyMessage message)
        {
            await _repository.UpdateAsync(message, Q
                .CachingRemove(GetCacheKey(message.SiteId))
            );
        }

        public async Task DeleteAllAsync(int siteId, int ruleId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(WxReplyMessage.SiteId), siteId)
                .Where(nameof(WxReplyMessage.RuleId), ruleId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task<List<WxReplyMessage>> GetMessagesAsync(int siteId, int ruleId)
        {
            if (ruleId == 0) return new List<WxReplyMessage>();
            var allMessages = await GetAllAsync(siteId);
            return allMessages.Where(x => x.RuleId == ruleId).ToList();
        }

        public async Task<WxReplyMessage> GetMessageAsync(int siteId, int messageId)
        {
            var allMessages = await GetAllAsync(siteId);
            return allMessages.FirstOrDefault(x => x.Id == messageId);
        }

        private async Task<List<WxReplyMessage>> GetAllAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(WxReplyKeyword.SiteId), siteId)
                .CachingGet(GetCacheKey(siteId))
            );
        }
    }
}
