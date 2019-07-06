using System.Threading.Tasks;
using SS.CMS.Utils;

namespace SS.CMS.Core.Repositories
{
    public partial class SiteRepository
    {
        public async Task<int> GetSiteIdByIsRootAsync()
        {
            var list = await GetSiteInfoKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var siteInfo = pair.Value;
                if (siteInfo == null) continue;

                if (siteInfo.IsRoot)
                {
                    return siteInfo.Id;
                }
            }
            return 0;
        }

        public async Task<int> GetSiteIdBySiteDirAsync(string siteDir)
        {
            var list = await GetSiteInfoKeyValuePairListAsync();

            foreach (var pair in list)
            {
                var siteInfo = pair.Value;
                if (siteInfo == null) continue;

                if (StringUtils.EqualsIgnoreCase(siteDir, siteInfo.SiteDir))
                {
                    return siteInfo.Id;
                }
            }
            return 0;
        }
    }
}
