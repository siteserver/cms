using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ChannelApi : IChannelApi
    {
        private ChannelApi() { }

        private static ChannelApi _instance;
        public static ChannelApi Instance => _instance ?? (_instance = new ChannelApi());

        public async Task<IChannel> GetChannelAsync(int siteId, int channelId)
        {
            return await ChannelManager.GetChannelAsync(siteId, channelId);
        }

        public async Task<int> GetChannelIdAsync(int siteId, string channelIndex)
        {
            if (string.IsNullOrEmpty(channelIndex)) return 0;

            var channelInfoList = await ChannelManager.GetChannelListAsync(siteId);
            foreach (var channelInfo in channelInfoList)
            {
                if (channelInfo.IndexName == channelIndex)
                {
                    return channelInfo.Id;
                }
            }

            return 0;
        }

        public IChannel NewInstance(int siteId)
        {
            return new Channel
            {
                ParentId = siteId,
                SiteId = siteId,
                AddDate = DateTime.Now
            };
        }

        public async Task<int> InsertAsync(int siteId, IChannel nodeInfo)
        {
            return await DataProvider.ChannelDao.InsertAsync((Channel)nodeInfo);
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId)
        {
            return await ChannelManager.GetChannelIdListAsync(siteId);
        }

        public async Task<List<int>> GetChannelIdListAsync(int siteId, int parentId)
        {
            return await ChannelManager.GetChannelIdListAsync(await ChannelManager.GetChannelAsync(siteId, parentId == 0 ? siteId : parentId), EScopeType.Children, string.Empty, string.Empty, string.Empty);
        }

        

        //public List<int> GetChannelIdListByAdminName(int siteId, string adminName)
        //{
        //    var channelIdList = new List<int>();
        //    if (string.IsNullOrEmpty(adminName)) return channelIdList;

        //    var permissionManager = PermissionManager.GetInstance(adminName);

        //    if (permissionManager.IsConsoleAdministrator || permissionManager.IsSystemAdministrator)//如果是超级管理员或站点管理员
        //    {
        //        channelIdList = await ChannelManager.GetChannelIdListAsync(siteId);
        //    }
        //    else
        //    {
        //        foreach (var channelId in permissionManager.ChannelPermissionChannelIdList)
        //        {
        //            var channel = await ChannelManager.GetChannelAsync(siteId, channelId);
        //            var allChannelIdList = await ChannelManager.GetChannelIdListAsync(channel, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
        //            allChannelIdList.Insert(0, channelId);

        //            foreach (var ownChannelId in allChannelIdList)
        //            {
        //                var node = await ChannelManager.GetChannelAsync(siteId, ownChannelId);
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
            return await ChannelManager.GetChannelNameAsync(siteId, channelId);
        }

        public async Task UpdateAsync(int siteId, IChannel channelInfo)
        {
            if (channelInfo == null) return;
            await DataProvider.ChannelDao.UpdateAsync((Channel)channelInfo);
        }

        public async Task DeleteAsync(int siteId, int channelId)
        {
            await DataProvider.ChannelDao.DeleteAsync(siteId, channelId);
        }

        public async Task<string> GetChannelUrlAsync(int siteId, int channelId)
        {
            var site = await DataProvider.SiteDao.GetAsync(siteId);
            return await PageUtility.GetChannelUrlAsync(site, await ChannelManager.GetChannelAsync(siteId, channelId), false);
        }
    }
}
