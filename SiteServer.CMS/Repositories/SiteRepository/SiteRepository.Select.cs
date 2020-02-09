using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class SiteRepository
    {
        private string GetEntityKey(int siteId)
        {
            return Caching.GetEntityKey(_repository.TableName, siteId);
        }

        private string GetListKey()
        {
            return Caching.GetListKey(_repository.TableName);
        }

        public async Task<Site> GetAsync(int siteId)
        {
            if (siteId <= 0) return null;

            return await _repository.GetAsync(siteId, Q.CachingGet(GetEntityKey(siteId)));
        }

        private async Task<List<SiteSummary>> GetSummariesAsync()
        {
            return await _repository.GetAllAsync<SiteSummary>(Q
                .Select(nameof(Site.Id), nameof(Site.SiteName), nameof(Site.SiteDir), nameof(Site.TableName), nameof(Site.Root), nameof(Site.ParentId), nameof(Site.Taxis))
                .OrderBy(nameof(Site.Taxis), nameof(Site.Id))
                .CachingGet(GetListKey())
            );
        }
    }
}
