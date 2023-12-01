using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class WxAccountRepository : IWxAccountRepository
    {
        private readonly Repository<WxAccount> _repository;

        public WxAccountRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<WxAccount>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        private string GetCacheKey(int siteId)
        {
            return CacheUtils.GetEntityKey(TableName, "siteId", siteId.ToString());
        }

        public async Task SetAsync(WxAccount account)
        {
            if (account.SiteId <= 0) return;

            if (account.Id > 0)
            {
                await _repository.UpdateAsync(account, Q.CachingRemove(GetCacheKey(account.SiteId)));
            }
            else
            {
                await _repository.InsertAsync(account, Q.CachingRemove(GetCacheKey(account.SiteId)));
            }
        }

        public async Task DeleteAllAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(WxAccount.SiteId), siteId)
                .CachingRemove(GetCacheKey(siteId))
            );
        }

        public async Task<WxAccount> GetBySiteIdAsync(int siteId)
        {
            var account = await _repository.GetAsync(Q
                .Where(nameof(WxAccount.SiteId), siteId)
                .CachingGet(GetCacheKey(siteId))
            );

            if (account == null)
            {
                account = new WxAccount
                {
                    SiteId = siteId,
                    MpType = WxMpType.Subscription
                };
            }

            return account;
        }
    }
}
