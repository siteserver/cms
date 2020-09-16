using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
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

        public async Task SetAsync(WxAccount account)
        {
            if (account.SiteId <= 0) return;

            if (account.Id > 0)
            {
                await _repository.UpdateAsync(account);
            }
            else
            {
                await _repository.InsertAsync(account);
            }
        }

        public async Task DeleteBySiteIdAsync(int siteId)
        {
            await _repository.DeleteAsync(Q
                .Where(nameof(WxAccount.SiteId), siteId)
            );
        }

        public async Task<WxAccount> GetBySiteIdAsync(int siteId)
        {
            var account = await _repository.GetAsync(Q
                .Where(nameof(WxAccount.SiteId), siteId)
            ) ?? new WxAccount
            {
                SiteId = siteId
            };

            return account;
        }
    }
}
