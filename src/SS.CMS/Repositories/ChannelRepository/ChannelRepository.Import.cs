using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;

namespace SS.CMS.Repositories
{
    public partial class ChannelRepository
    {
        public async Task<Channel> ImportGetAsync(int channelId)
        {
            if (channelId == 0) return null;

            channelId = Math.Abs(channelId);
            return await _repository.GetAsync(channelId);
        }

        public async Task<List<string>> ImportGetIndexNameListAsync(int siteId)
        {
            return await _repository.GetAllAsync<string>(Q
                .Select(nameof(Channel.IndexName))
                .Where(nameof(Channel.SiteId), siteId)
                .WhereNotNull(nameof(Channel.IndexName))
                .WhereNot(nameof(Channel.IndexName), string.Empty)
            );
        }

        public async Task<int> ImportGetCountAsync(int siteId, int parentId)
        {
            return await _repository.CountAsync(Q
                .Where(nameof(Channel.SiteId), siteId)
                .Where(nameof(Channel.ParentId), parentId)
            );
        }

        private async Task<int> ImportGetIdByParentIdAndOrderAsync(int siteId, int parentId, int order)
        {
            var channelIds = await _repository.GetAllAsync<int>(Q
                .Select(nameof(Channel.Id))
                .Where(nameof(Channel.SiteId), siteId)
                .Where(nameof(Channel.ParentId), parentId)
                .OrderBy(nameof(Channel.Taxis), nameof(Channel.Id))
            );

            var channelId = parentId;

            var index = 1;
            foreach (var id in channelIds)
            {
                channelId = id;
                if (index == order)
                    break;
                index++;
            }
            return channelId;
        }

        public async Task<int> ImportGetIdAsync(int siteId, string orderString)
        {
            if (orderString == "1")
                return siteId;

            var channelId = siteId;

            var orderArr = orderString.Split('_');
            for (var index = 1; index < orderArr.Length; index++)
            {
                var order = TranslateUtils.ToInt(orderArr[index]);
                channelId = await ImportGetIdByParentIdAndOrderAsync(siteId, channelId, order);
            }
            return channelId;
        }

        public async Task<int> ImportGetChannelIdByParentIdAndChannelNameAsync(int siteId, int parentId, string channelName, bool recursive)
        {
            if (parentId <= 0 || string.IsNullOrEmpty(channelName)) return 0;

            var channelId = 0;

            if (recursive)
            {
                if (siteId == parentId)
                {
                    channelId = await _repository.GetAsync<int>(Q
                        .Select(nameof(Channel.Id))
                        .Where(nameof(Channel.SiteId), siteId)
                        .Where(nameof(Channel.ChannelName), channelName)
                        .OrderBy(nameof(Channel.Taxis))
                    );
                }
                else
                {
                    channelId = await _repository.GetAsync<int>(Q
                        .Select(nameof(Channel.Id))
                        .Where(nameof(Channel.SiteId), siteId)
                        .Where(nameof(Channel.ChannelName), channelName)
                        .Where(q => q
                            .Where(nameof(Channel.ParentId), parentId)
                            .OrWhere(nameof(Channel.ParentsPath), parentId)
                            .OrWhereLike(nameof(Channel.ParentsPath), $"{parentId},%")
                            .OrWhereLike(nameof(Channel.ParentsPath), $"%,{parentId},%")
                            .OrWhereLike(nameof(Channel.ParentsPath), $"%,{parentId}"))
                        .OrderBy(nameof(Channel.Taxis))
                    );
                }
            }
            else
            {
                channelId = await _repository.GetAsync<int>(Q
                    .Select(nameof(Channel.Id))
                    .Where(nameof(Channel.SiteId), siteId)
                    .Where(nameof(Channel.ParentId), parentId)
                    .Where(nameof(Channel.ChannelName), channelName)
                    .OrderBy(nameof(Channel.Taxis))
                );
            }

            return channelId;
        }

        private async Task<int> ImportGetParentIdAsync(int channelId)
        {
            var channel = await ImportGetAsync(channelId);
            return channel?.ParentId ?? 0;
        }

        /// <summary>
        /// 在节点树中得到此节点的排序号，以“1_2_5_2”的形式表示
        /// </summary>
        public async Task<string> ImportGetOrderStringInSiteAsync(int siteId, int channelId)
        {
            var retVal = "";
            if (channelId != 0)
            {
                var parentId = await ImportGetParentIdAsync(channelId);
                if (parentId != 0)
                {
                    var orderString = await ImportGetOrderStringInSiteAsync(siteId, parentId);
                    retVal = orderString + "_" + await ImportGetOrderInSiblingAsync(siteId, channelId, parentId);
                }
                else
                {
                    retVal = "1";
                }
            }
            return retVal;
        }

        private async Task<int> ImportGetOrderInSiblingAsync(int siteId, int channelId, int parentId)
        {
            var channelIds = await _repository.GetAllAsync<int>(Q
                .Select(nameof(Channel.Id))
                .Where(nameof(Channel.SiteId), siteId)
                .Where(nameof(Channel.ParentId), parentId)
                .OrderBy(nameof(Channel.Taxis), nameof(Channel.Id))
            );

            return channelIds.IndexOf(channelId) + 1;
        }
    }
}
