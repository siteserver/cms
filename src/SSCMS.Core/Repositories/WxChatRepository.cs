using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
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

        public async Task<bool> UserAdd(WxChat chat)
        {
            var isSession = await _repository.ExistsAsync(Q
                .Where(nameof(WxChat.SiteId), chat.SiteId)
                .Where(nameof(WxChat.OpenId), chat.OpenId)
                .Where(nameof(WxChat.IsReply), true)
                .WhereDate(nameof(WxChat.CreatedDate), ">", DateTime.Now.AddDays(-1))
            );

            await _repository.InsertAsync(chat);
            return isSession;
        }

        public async Task ReplyAdd(WxChat chat)
        {
            await _repository.InsertAsync(chat);
        }

        public async Task Star(int siteId, int chatId, bool star)
        {
            await _repository.UpdateAsync(Q.
                Set(nameof(WxChat.IsStar), star)
                .Where(nameof(WxChat.SiteId), siteId)
                .Where(nameof(WxChat.Id), chatId)
            );
        }

        public async Task DeleteAsync(int siteId, int chatId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(WxChat.Id), chatId)
            );
        }

        public async Task DeleteAllAsync(int siteId, string openId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(WxChat.SiteId), siteId)
                .Where(nameof(WxChat.OpenId), openId)
            );
        }

        public async Task<List<WxChat>> GetChatsAsyncByOpenId(int siteId, string openId)
        {
            if (string.IsNullOrEmpty(openId)) return new List<WxChat>();
            return await _repository.GetAllAsync(Q
                .Where(nameof(WxChat.SiteId), siteId)
                .Where(nameof(WxChat.OpenId), openId)
                .OrderByDesc(nameof(WxChat.Id))
            );
        }

        public async Task<int> GetCountAsync(int siteId, bool star, string keyword)
        {
            var query = Q
                .Where(nameof(WxChat.SiteId), siteId)
                .Where(nameof(WxChat.IsReply), false);
            if (star)
            {
                query.Where(nameof(WxChat.IsStar), true);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(WxChat.Text), $"%{keyword}%");
            }
            return await _repository.CountAsync(query);
        }

        public async Task<List<WxChat>> GetChatsAsync(int siteId, bool star, string keyword, int page, int perPage)
        {
            var query = Q
                .Where(nameof(WxChat.SiteId), siteId)
                .Where(nameof(WxChat.IsReply), false)
                .ForPage(page, perPage)
                .OrderByDesc(nameof(WxChat.Id));
            if (star)
            {
                query.Where(nameof(WxChat.IsStar), true);
            }
            if (!string.IsNullOrEmpty(keyword))
            {
                query.WhereLike(nameof(WxChat.Text), $"%{keyword}%");
            }

            return await _repository.GetAllAsync(query);
        }
    }
}
