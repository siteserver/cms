using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class SiteRepository
    {
        private string GetEntityKey(int siteId)
        {
            return CacheUtils.GetEntityKey(_repository.TableName, siteId);
        }

        private string GetListKey()
        {
            return CacheUtils.GetListKey(_repository.TableName);
        }

        public async Task<Site> GetAsync(int siteId)
        {
            if (siteId <= 0) return null;

            var site = await _repository.GetAsync(siteId, Q
                .CachingGet(GetEntityKey(siteId))
            );
            if (site != null && string.IsNullOrEmpty(site.SiteType))
            {
                site.SiteType = Types.SiteTypes.Web;
            }

            return site;
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
