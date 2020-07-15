using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Core.Repositories
{
    public class OpenAccountRepository : IOpenAccountRepository
    {
        private readonly Repository<OpenAccount> _repository;

        public OpenAccountRepository(ISettingsManager settingsManager)
        {
            _repository = new Repository<OpenAccount>(settingsManager.Database, settingsManager.Redis);
        }

        public IDatabase Database => _repository.Database;

        public string TableName => _repository.TableName;

        public List<TableColumn> TableColumns => _repository.TableColumns;

        public async Task SetAsync(OpenAccount account)
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
                .Where(nameof(OpenAccount.SiteId), siteId)
            );
        }

        public async Task<OpenAccount> GetBySiteIdAsync(int siteId)
        {
            var account = await _repository.GetAsync(Q
                .Where(nameof(OpenAccount.SiteId), siteId)
            ) ?? new OpenAccount
            {
                SiteId = siteId
            };

            return account;
        }
    }
}
