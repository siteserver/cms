using System;
using System.Collections;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

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
            return DataProvider.ChannelDao.GetIdListBySiteId(siteId);
        }

        public List<int> GetChannelIdList(int siteId, int parentId)
        {
            return DataProvider.ChannelDao.GetIdListByParentId(siteId, parentId);
        }

        public List<int> GetChannelIdList(int siteId, string adminName)
        {
            var channelIdList = new List<int>();
            if (string.IsNullOrEmpty(adminName)) return channelIdList;

            if (AdminManager.HasChannelPermissionIsConsoleAdministrator(adminName) || AdminManager.HasChannelPermissionIsSystemAdministrator(adminName))//如果是超级管理员或站点管理员
            {
                channelIdList = DataProvider.ChannelDao.GetIdListBySiteId(siteId);
            }
            else
            {
                var ps = new ProductAdministratorWithPermissions(adminName);
                ICollection channelIdCollection = ps.ChannelPermissionDict.Keys;
                foreach (int channelId in channelIdCollection)
                {
                    var allChannelIdList = DataProvider.ChannelDao.GetIdListForDescendant(channelId);
                    allChannelIdList.Insert(0, channelId);

                    foreach (var ownChannelId in allChannelIdList)
                    {
                        var nodeInfo = ChannelManager.GetChannelInfo(siteId, ownChannelId);
                        if (nodeInfo != null)
                        {
                            channelIdList.Add(nodeInfo.Id);
                        }
                    }
                }
            }
            return channelIdList;
        }
    }
}
