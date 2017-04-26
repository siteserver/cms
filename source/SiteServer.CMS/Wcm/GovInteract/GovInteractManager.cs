using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Wcm.Model;

namespace SiteServer.CMS.Wcm.GovInteract
{
	public class GovInteractManager
	{
        public static void Initialize(PublishmentSystemInfo publishmentSystemInfo)
        {
            if (publishmentSystemInfo.Additional.GovInteractNodeId > 0)
            {
                if (!DataProvider.NodeDao.IsExists(publishmentSystemInfo.Additional.GovInteractNodeId))
                {
                    publishmentSystemInfo.Additional.GovInteractNodeId = 0;
                }
            }
            if (publishmentSystemInfo.Additional.GovInteractNodeId == 0)
            {
                var govInteractNodeId = DataProvider.NodeDao.GetNodeIdByContentModelType(publishmentSystemInfo.PublishmentSystemId, EContentModelType.GovInteract);
                if (govInteractNodeId == 0)
                {
                    govInteractNodeId = DataProvider.NodeDao.InsertNodeInfo(publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.PublishmentSystemId, "互动交流", string.Empty, EContentModelTypeUtils.GetValue(EContentModelType.GovInteract));
                }
                publishmentSystemInfo.Additional.GovInteractNodeId = govInteractNodeId;
                DataProvider.PublishmentSystemDao.Update(publishmentSystemInfo);
            }
        }

        public static List<NodeInfo> GetNodeInfoList(PublishmentSystemInfo publishmentSystemInfo)
        {
            var nodeInfoList = new List<NodeInfo>();
            if (publishmentSystemInfo != null && publishmentSystemInfo.Additional.GovInteractNodeId > 0)
            {
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListForDescendant(publishmentSystemInfo.Additional.GovInteractNodeId);
                foreach (var nodeId in nodeIdList)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                    if (nodeInfo != null && EContentModelTypeUtils.Equals(nodeInfo.ContentModelId, EContentModelType.GovInteract))
                    {
                        nodeInfoList.Add(nodeInfo);
                    }
                }
            }
            return nodeInfoList;
        }

        public static void AddDefaultTypeInfos(int publishmentSystemId, int nodeId)
        {
            var typeInfo = new GovInteractTypeInfo(0, "求决", nodeId, publishmentSystemId, 0);
            DataProvider.GovInteractTypeDao.Insert(typeInfo);
            typeInfo = new GovInteractTypeInfo(0, "举报", nodeId, publishmentSystemId, 0);
            DataProvider.GovInteractTypeDao.Insert(typeInfo);
            typeInfo = new GovInteractTypeInfo(0, "投诉", nodeId, publishmentSystemId, 0);
            DataProvider.GovInteractTypeDao.Insert(typeInfo);
            typeInfo = new GovInteractTypeInfo(0, "咨询", nodeId, publishmentSystemId, 0);
            DataProvider.GovInteractTypeDao.Insert(typeInfo);
            typeInfo = new GovInteractTypeInfo(0, "建议", nodeId, publishmentSystemId, 0);
            DataProvider.GovInteractTypeDao.Insert(typeInfo);
            typeInfo = new GovInteractTypeInfo(0, "感谢", nodeId, publishmentSystemId, 0);
            DataProvider.GovInteractTypeDao.Insert(typeInfo);
            typeInfo = new GovInteractTypeInfo(0, "其他", nodeId, publishmentSystemId, 0);
            DataProvider.GovInteractTypeDao.Insert(typeInfo);
        }

        public static List<int> GetFirstDepartmentIdList(GovInteractChannelInfo channelInfo)
        {
            return string.IsNullOrEmpty(channelInfo?.DepartmentIDCollection) ? BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByParentId(0) : BaiRongDataProvider.DepartmentDao.GetDepartmentIdListByDepartmentIdCollection(channelInfo.DepartmentIDCollection);
        }

        public static string GetTypeName(int typeId)
        {
            return typeId > 0 ? DataProvider.GovInteractTypeDao.GetTypeName(typeId) : string.Empty;
        }

        public static bool IsPermission(int publishmentSystemId, int nodeId, string permission)
        {
            List<string> govInteractPermissionList = null;
            if (ProductPermissionsManager.Current.GovInteractPermissionDict.ContainsKey(publishmentSystemId))
            {
                govInteractPermissionList = ProductPermissionsManager.Current.GovInteractPermissionDict[publishmentSystemId];
            }
            if (govInteractPermissionList != null)
            {
                return govInteractPermissionList.Contains(permission);
            }
            return false;
        }
	}
}
