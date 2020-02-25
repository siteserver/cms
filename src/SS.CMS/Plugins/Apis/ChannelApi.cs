using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ChannelApi
    {
        private ChannelApi() { }

        private static ChannelApi _instance;
        public static ChannelApi Instance => _instance ??= new ChannelApi();

        public async Task<Channel> GetChannelAsync(int siteId, int channelId)
        {
            return await DataProvider.ChannelRepository.GetAsync(channelId);
        }

        public async Task<int> GetChannelIdAsync(int siteId, string channelIndex)
        {
            if (string.IsNullOrEmpty(channelIndex)) return 0;

            var channelInfoList = await DataProvider.ChannelRepository.GetChannelListAsync(siteId);
            foreach (var channelInfo in channelInfoList)
            {
                if (channelInfo.IndexName == channelIndex)
                {
                    return channelInfo.Id;
                }
            }

            return 0;
        }

        public Channel NewInstance(int siteId)
        {
            return new Channel
            {
                ParentId = siteId,
                SiteId = siteId,
                AddDate = DateTime.Now
            };
        }

        public async Task<int> InsertAsync(int siteId, Channel nodeInfo)
        {
            return await DataProvider.ChannelRepository.InsertAsync((Channel)nodeInfo);
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId)
        {
            return await DataProvider.ChannelRepository.GetChannelIdListAsync(siteId);
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId, int parentId)
        {
            return await DataProvider.ChannelRepository.GetChannelIdsAsync(siteId, parentId == 0 ? siteId : parentId, ScopeType.Children);
        }

        

        //public List<int> GetChannelIdListByAdminName(int siteId, string adminName)
        //{
        //    var channelIdList = new List<int>();
        //    if (string.IsNullOrEmpty(adminName)) return channelIdList;

        //    var permissionManager = PermissionManager.GetInstance(adminName);

        //    if (permissionManager.IsConsoleAdministrator || permissionManager.IsSystemAdministrator)//如果是超级管理员或站点管理员
        //    {
        //        channelIdList = await DataProvider.ChannelRepository.GetChannelIdListAsync(siteId);
        //    }
        //    else
        //    {
        //        foreach (var channelId in permissionManager.ChannelPermissionChannelIdList)
        //        {
        //            var channel = await DataProvider.ChannelRepository.GetAsync(siteId, channelId);
        //            var allChannelIdList = await DataProvider.ChannelRepository.GetChannelIdListAsync(channel, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
        //            allChannelIdList.Insert(0, channelId);

        //            foreach (var ownChannelId in allChannelIdList)
        //            {
        //                var node = await DataProvider.ChannelRepository.GetAsync(siteId, ownChannelId);
        //                if (node != null)
        //                {
        //                    channelIdList.Add(node.Id);
        //                }
        //            }
        //        }
        //    }
        //    return channelIdList;
        //}

        public async Task<string> GetChannelNameAsync(int siteId, int channelId)
        {
            return await DataProvider.ChannelRepository.GetChannelNameAsync(siteId, channelId);
        }

        public async Task UpdateAsync(int siteId, Channel channelInfo)
        {
            if (channelInfo == null) return;
            await DataProvider.ChannelRepository.UpdateAsync((Channel)channelInfo);
        }

        public async Task DeleteAsync(int siteId, int channelId)
        {
            await DataProvider.ChannelRepository.DeleteAsync(siteId, channelId, 0);
        }

        public async Task<string> GetChannelUrlAsync(int siteId, int channelId)
        {
            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            return await PageUtility.GetChannelUrlAsync(site, await DataProvider.ChannelRepository.GetAsync(channelId), false);
        }
    }
}
