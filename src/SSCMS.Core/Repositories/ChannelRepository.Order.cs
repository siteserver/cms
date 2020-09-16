using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;

namespace SSCMS.Core.Repositories
{
    public partial class ChannelRepository
    {
        private async Task SetTaxisAsync(int siteId, int channelId, int taxis)
        {
            await _repository.UpdateAsync(Q
                .Set(nameof(Channel.Taxis), taxis)
                .Where(nameof(Channel.Id), channelId)
                .CachingRemove(GetListKey(siteId), GetEntityKey(channelId))
            );
        }

        private async Task<int> GetMaxTaxisAsync(int siteId, int parentId)
        {
            var summaries = await GetSummariesAsync(siteId);
            return summaries
                .Where(x => x.ParentId == parentId)
                .Select(x => x.Taxis)
                .DefaultIfEmpty()
                .Max();
        }

        //public async Task UpdateTaxisDownAsync(int siteId, int channelId, int parentId, int taxis)
        //{
        //    var summaries = await GetSummariesAsync(siteId);
        //    var higher = summaries
        //        .Where(x => x.ParentId == parentId && x.Taxis > taxis && x.Id != channelId)
        //        .OrderBy(x => x.Taxis)
        //        .FirstOrDefault();

        //    if (higher != null)
        //    {
        //        await SetTaxisAsync(siteId, channelId, higher.Taxis);
        //        await SetTaxisAsync(siteId, higher.Id, taxis);
        //    }
        //}

        //public async Task UpdateTaxisUpAsync(int siteId, int channelId, int parentId, int taxis)
        //{
        //    var summaries = await GetSummariesAsync(siteId);

        //    var lower = summaries
        //        .Where(x => x.ParentId == parentId && x.Taxis < taxis && x.Id != channelId)
        //        .OrderByDescending(x => x.Taxis)
        //        .FirstOrDefault();

        //    if (lower != null)
        //    {
        //        await SetTaxisAsync(siteId, channelId, lower.Taxis);
        //        await SetTaxisAsync(siteId, lower.Id, taxis);
        //    }
        //}

        public async Task UpdateTaxisAsync(int siteId, int parentId, int channelId, bool isUp)
        {
            var summaries = await GetSummariesAsync(siteId);

            var channelIds = summaries
                .Where(x => x.ParentId == parentId)
                .OrderBy(x => x.Taxis)
                .Select(x => x.Id)
                .ToList();

            var index = channelIds.IndexOf(channelId);

            if (isUp && index == 0) return;
            if (!isUp && index == channelIds.Count - 1) return;

            if (isUp)
            {
                channelIds.Remove(channelId);
                channelIds.Insert(index - 1, channelId);
            }
            else
            {
                channelIds.Remove(channelId);
                channelIds.Insert(index + 1, channelId);
            }

            for (var taxis = 1; taxis <= channelIds.Count; taxis++)
            {
                await SetTaxisAsync(siteId, channelIds[taxis - 1], taxis);
            }
        }
    }
}
