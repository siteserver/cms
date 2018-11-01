using System;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Plugin.Apis
{
    public class ChannelApi : IChannelApi
    {
        private ChannelApi() { }

        private static ChannelApi _instance;
        public static ChannelApi Instance => _instance ?? (_instance = new ChannelApi());

        public IChannelInfo GetChannelInfo(int siteId, int channelId)
        {
            return ChannelManager.GetChannelInfo(siteId, channelId);
        }

        public int GetChannelId(int siteId, string channelIndex)
        {
            if (string.IsNullOrEmpty(channelIndex)) return 0;

            var channelInfoList = ChannelManager.GetChannelInfoList(siteId);
            foreach (var channelInfo in channelInfoList)
            {
                if (channelInfo.IndexName == channelIndex)
                {
                    return channelInfo.Id;
                }
            }

            return 0;
        }

        public IChannelInfo NewInstance(int siteId)
        {
            return new ChannelInfo
            {
                ParentId = siteId,
                SiteId = siteId,
                AddDate = DateTime.Now
            };
        }

        public int Insert(int siteId, IChannelInfo nodeInfo)
        {
            return DataProvider.ChannelDao.Insert(nodeInfo);
        }

        public List<int> GetChannelIdList(int siteId)
        {
            return ChannelManager.GetChannelIdList(siteId);
        }

        public List<int> GetChannelIdList(int siteId, int parentId)
        {
            return ChannelManager.GetChannelIdList(ChannelManager.GetChannelInfo(siteId, parentId == 0 ? siteId : parentId), EScopeType.Children, string.Empty, string.Empty, string.Empty);
        }

        //public List<int> GetChannelIdListByAdminName(int siteId, string adminName)
        //{
        //    var channelIdList = new List<int>();
        //    if (string.IsNullOrEmpty(adminName)) return channelIdList;

        //    var permissionManager = PermissionManager.GetInstance(adminName);

        //    if (permissionManager.IsConsoleAdministrator || permissionManager.IsSystemAdministrator)//如果是超级管理员或站点管理员
        //    {
        //        channelIdList = ChannelManager.GetChannelIdList(siteId);
        //    }
        //    else
        //    {
        //        foreach (var channelId in permissionManager.ChannelPermissionChannelIdList)
        //        {
        //            var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
        //            var allChannelIdList = ChannelManager.GetChannelIdList(channelInfo, EScopeType.Descendant, string.Empty, string.Empty, string.Empty);
        //            allChannelIdList.Insert(0, channelId);

        //            foreach (var ownChannelId in allChannelIdList)
        //            {
        //                var nodeInfo = ChannelManager.GetChannelInfo(siteId, ownChannelId);
        //                if (nodeInfo != null)
        //                {
        //                    channelIdList.Add(nodeInfo.Id);
        //                }
        //            }
        //        }
        //    }
        //    return channelIdList;
        //}

        public string GetChannelName(int siteId, int channelId)
        {
            return ChannelManager.GetChannelName(siteId, channelId);
        }

        public void Update(int siteId, IChannelInfo channelInfo)
        {
            if (channelInfo == null) return;
            DataProvider.ChannelDao.Update((ChannelInfo)channelInfo);
        }

        public void Delete(int siteId, int channelId)
        {
            DataProvider.ChannelDao.Delete(siteId, channelId);
        }

        public string GetChannelUrl(int siteId, int channelId)
        {
            var siteInfo = SiteManager.GetSiteInfo(siteId);
            return PageUtility.GetChannelUrl(siteInfo, ChannelManager.GetChannelInfo(siteId, channelId), false);
        }
    }
}
