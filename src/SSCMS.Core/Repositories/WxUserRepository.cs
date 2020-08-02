using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Repositories
{
    public class WxUserRepository : IWxUserRepository
    {
        private readonly Repository<WxUser> _repository;

        public WxUserRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<WxUser>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(int siteId)
        {
            return CacheUtils.GetListKey(_repository.TableName, siteId);
        }

        public async Task<int> InsertAsync(int siteId, WxUser user)
        {
            user.SiteId = siteId;
            return await _repository.InsertAsync(user, Q
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task UpdateAllAsync(int siteId, List<WxUser> users)
        {
            var dbUsers = await GetAllAsync(siteId);
            foreach (var user in users)
            {
                var dbUser = dbUsers.FirstOrDefault(x => x.OpenId == user.OpenId);
                if (dbUser == null) continue;

                if (ListUtils.ToString(dbUser.TagIdList) != ListUtils.ToString(user.TagIdList) ||
                    dbUser.Nickname != user.Nickname ||
                    dbUser.Remark != user.Remark)
                {
                    await _repository.UpdateAsync(Q
                        .Set(nameof(WxUser.TagIdList), ListUtils.ToString(user.TagIdList))
                        .Set(nameof(WxUser.Nickname), user.Nickname)
                        .Set(nameof(WxUser.Remark), user.Remark)
                        .Where(nameof(WxUser.OpenId), user.OpenId)
                        .CachingRemove(GetCacheKey(siteId))
                    );
                }
            }
        }

        public async Task DeleteAllAsync(int siteId, List<string> openIds)
        {
            if (openIds == null || openIds.Count == 0) return;

            await _repository.DeleteAsync(Q
                .WhereIn(nameof(WxUser.OpenId), openIds)
                .Where(nameof(WxUser.SiteId), siteId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        private async Task<List<WxUser>> GetAllAsync(int siteId)
        {
            return await _repository.GetAllAsync(Q
                .Where(nameof(WxUser.SiteId), siteId)
                .OrderByDesc(nameof(WxUser.SubscribeTime))
                .CachingGet(GetCacheKey(siteId))
            );
        }

        public async Task<(int Total, int Count, List<string> Results)> GetPageOpenIds(int siteId, int tagId, string keyword, int page, int perPage)
        {
            var users = await GetAllAsync(siteId);
            var total = users.Count;

            if (tagId != 0)
            {
                users = users.Where(x => ListUtils.Contains(x.TagIdList, tagId)).ToList();
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                users = users.Where(x =>
                    StringUtils.ContainsIgnoreCase(x.Nickname, keyword) ||
                    StringUtils.ContainsIgnoreCase(x.Remark, keyword)).ToList();
            }

            var count = users.Count;
            
            var results = users.Skip((page - 1) * perPage).Take(perPage).Select(x => x.OpenId).ToList();
            return (total, count, results);
        }

        public async Task<List<string>> GetAllOpenIds(int siteId)
        {
            var users = await GetAllAsync(siteId);
            return users.Select(x => x.OpenId).ToList();
        }
    }
}
