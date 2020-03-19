using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS;
using SSCMS.Core.Utils;

namespace SSCMS.Core.Repositories.SiteRepository
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

            return await _repository.GetAsync(siteId, Q
                .CachingGet(GetEntityKey(siteId))
            );
        }

        private async Task<List<SiteSummary>> GetSummariesAsync(int parentId)
        {
            var summaries = await GetSummariesAsync();
            return summaries.Where(x => x.ParentId == parentId).ToList();
        }

        private async Task<List<SiteSummary>> GetSummariesAsync()
        {
            return await _repository.GetAllAsync<SiteSummary>(Q
                .Select(nameof(Site.Id), nameof(Site.SiteName), nameof(Site.SiteDir), nameof(Site.TableName), nameof(Site.Root), nameof(Site.ParentId), nameof(Site.Taxis))
                .WhereNot(nameof(Site.Id), 0)
                .OrderBy(nameof(Site.Taxis), nameof(Site.Id))
                .CachingGet(GetListKey())
            );
        }
    }
}
