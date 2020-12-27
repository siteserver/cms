using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Models;
using SSCMS.Utils;

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

        //public int ParentId { get; set; }
        //public List<int> ParentsPath { get; set; }
        //public int ParentsCount { get; set; }
        //public int ChildrenCount { get; set; }

        public async Task DropAsync(int siteId, int sourceId, int targetId, string dropType)
        {
            var summaries = new List<ChannelSummary>(await GetSummariesAsync(siteId));
            var source = summaries.FirstOrDefault(x => x.Id == sourceId);
            var target = summaries.FirstOrDefault(x => x.Id == targetId);
            if (source == null || target == null) return;

            if (dropType == "after")
            {
                if (source.ParentId != target.ParentId)
                {
                    var parent = summaries.FirstOrDefault(x => x.Id == target.ParentId);

                    await _repository.DecrementAsync(nameof(Channel.ChildrenCount), Q
                        .Where(nameof(Channel.Id), source.ParentId)
                    );

                    await _repository.IncrementAsync(nameof(Channel.ChildrenCount), Q
                        .Where(nameof(Channel.Id), target.ParentId)
                    );

                    await UpdateParentAsync(summaries, sourceId, parent);
                }

                var childIds = summaries
                    .Where(x => x.ParentId == target.ParentId)
                    .OrderBy(x => x.Taxis)
                    .Select(x => x.Id)
                    .ToList();
                childIds.Remove(sourceId);
                var index = childIds.IndexOf(targetId);
                childIds.Insert(index + 1, sourceId);
                await UpdateTaxisAsync(siteId, childIds);
            }
            else if (dropType == "before")
            {
                if (source.ParentId != target.ParentId)
                {
                    var parent = summaries.FirstOrDefault(x => x.Id == target.ParentId);

                    await _repository.DecrementAsync(nameof(Channel.ChildrenCount), Q
                        .Where(nameof(Channel.Id), source.ParentId)
                    );

                    await _repository.IncrementAsync(nameof(Channel.ChildrenCount), Q
                        .Where(nameof(Channel.Id), target.ParentId)
                    );

                    await UpdateParentAsync(summaries, sourceId, parent);
                }

                var childIds = summaries
                    .Where(x => x.ParentId == target.ParentId)
                    .OrderBy(x => x.Taxis)
                    .Select(x => x.Id)
                    .ToList();
                childIds.Remove(sourceId);
                var index = childIds.IndexOf(targetId);
                childIds.Insert(index, sourceId);
                await UpdateTaxisAsync(siteId, childIds);
            }
            else if (dropType == "inner")
            {
                var parentId = source.ParentId;
                var childIds = summaries
                    .Where(x => x.ParentId == targetId)
                    .OrderBy(x => x.Taxis)
                    .Select(x => x.Id)
                    .ToList();
                childIds.Add(sourceId);
                await UpdateTaxisAsync(siteId, childIds);

                await _repository.DecrementAsync(nameof(Channel.ChildrenCount), Q
                    .Where(nameof(Channel.Id), parentId)
                );

                await _repository.IncrementAsync(nameof(Channel.ChildrenCount), Q
                    .Where(nameof(Channel.Id), target.Id)
                );

                await UpdateParentAsync(summaries, sourceId, target);
            }
        }

        private async Task UpdateParentAsync(List<ChannelSummary> summaries, int sourceId, IChannelSummary parent)
        {
            var source = await GetAsync(sourceId);

            source.ParentId = parent.Id;
            source.ParentsPath = ListUtils.AddIfNotExists(parent.ParentsPath, parent.Id);
            if (!source.ParentsPath.Contains(source.SiteId))
            {
                source.ParentsPath.Insert(0, source.SiteId);
            }
            source.ParentsCount = source.ParentsPath.Count;

            await UpdateAsync(source);

            var childIds = summaries
                .Where(x => x.ParentId == source.Id)
                .Select(x => x.Id)
                .ToList();

            if (childIds.Count > 0)
            {
                foreach (var childId in childIds)
                {
                    await UpdateParentAsync(summaries, childId, source);
                }
            }
        }

        private async Task UpdateTaxisAsync(int siteId, List<int> channelIds)
        {
            for (var taxis = 1; taxis <= channelIds.Count; taxis++)
            {
                await SetTaxisAsync(siteId, channelIds[taxis - 1], taxis);
            }
        }

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
