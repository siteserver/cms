using System;
using System.Collections;
using System.Collections.Generic;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class NodeApi : INodeApi
    {
        private NodeApi() { }

        public static NodeApi Instance { get; } = new NodeApi();

        public INodeInfo GetNodeInfo(int publishmentSystemId, int channelId)
        {
            return NodeManager.GetNodeInfo(publishmentSystemId, channelId);
        }

        public INodeInfo NewInstance(int publishmentSystemId)
        {
            return new NodeInfo
            {
                ParentId = publishmentSystemId,
                PublishmentSystemId = publishmentSystemId,
                AddDate = DateTime.Now
            };
        }

        public int Insert(int publishmentSystemId, INodeInfo nodeInfo)
        {
            return DataProvider.NodeDao.InsertNodeInfo(nodeInfo);
        }

        public List<int> GetNodeIdList(int publishmentSystemId)
        {
            return DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemId);
        }

        public List<int> GetNodeIdList(int publishmentSystemId, int parentId)
        {
            return DataProvider.NodeDao.GetNodeIdListByParentId(publishmentSystemId, parentId);
        }

        public List<int> GetNodeIdList(int publishmentSystemId, string adminName)
        {
            var nodeIdList = new List<int>();
            if (string.IsNullOrEmpty(adminName)) return nodeIdList;

            if (AdminManager.HasChannelPermissionIsConsoleAdministrator(adminName) || AdminManager.HasChannelPermissionIsSystemAdministrator(adminName))//如果是超级管理员或站点管理员
            {
                nodeIdList = DataProvider.NodeDao.GetNodeIdListByPublishmentSystemId(publishmentSystemId);
            }
            else
            {
                var ps = new ProductAdministratorWithPermissions(adminName);
                ICollection nodeIdCollection = ps.ChannelPermissionDict.Keys;
                foreach (int nodeId in nodeIdCollection)
                {
                    var allNodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(nodeId);
                    allNodeIdList.Insert(0, nodeId);

                    foreach (var ownNodeId in allNodeIdList)
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, ownNodeId);
                        if (nodeInfo != null)
                        {
                            nodeIdList.Add(nodeInfo.NodeId);
                        }
                    }
                }
            }
            return nodeIdList;
        }
    }
}
