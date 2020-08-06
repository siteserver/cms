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
    public class WxChatRepository : IWxChatRepository
    {
        private readonly Repository<WxChat> _repository;

        public WxChatRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<WxChat>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(int siteId) => CacheUtils.GetListKey(_repository.TableName, siteId);

        public async Task<int> InsertAsync(WxChat chat)
        {
            return await _repository.InsertAsync(chat, Q
                .CachingRemove(GetCacheKey(chat.SiteId))
            );
        }

        public async Task DeleteAsync(int siteId, int chatId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(WxChat.Id), chatId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task DeleteAllAsync(int siteId, string openId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(WxChat.SiteId), siteId)
                .Where(nameof(WxChat.OpenId), openId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task<List<WxChat>> GetChatsAsync(int siteId, string openId)
        {
            if (string.IsNullOrEmpty(openId)) return new List<WxChat>();
            var allMessages = await GetAllAsync(siteId);
            return allMessages.Where(x => x.OpenId == openId).ToList();
        }

        public async Task<WxChat> GetMessageAsync(int siteId, int chatId)
        {
            var allMessages = await GetAllAsync(siteId);
            return allMessages.FirstOrDefault(x => x.Id == chatId);
        }

        private async Task<List<WxChat>> GetAllAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(WxReplyKeyword.SiteId), siteId)
                .CachingGet(GetCacheKey(siteId))
            );
        }
    }
}
