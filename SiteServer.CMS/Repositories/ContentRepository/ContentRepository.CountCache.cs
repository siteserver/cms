using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Caching;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.CMS.Repositories
{
    public partial class ContentRepository
    {
        public async Task<int> GetCountCheckingAsync(Site site)
        {
            //var tableNames = await DataProvider.SiteRepository.GetTableNameListAsync(site);
            //var isChecked = false.ToString();

            //var count = 0;
            //foreach (var tableName in tableNames)
            //{
            //    var list = await GetCountCacheAsync(tableName);
            //    count += list.Where(x => x.SiteId == site.Id && x.IsChecked == isChecked && x.CheckedLevel != -site.CheckContentLevel && x.CheckedLevel != CheckManager.LevelInt.CaoGao)
            //        .Sum(x => x.Count);
            //}

            //return count;

            return 0;
        }
    }
}