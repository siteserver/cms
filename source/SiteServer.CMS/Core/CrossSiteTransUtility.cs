using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core
{
    public class CrossSiteTransUtility
    {
        private CrossSiteTransUtility()
        {
        }

        public static bool IsCrossSiteTrans(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var isCrossSiteTrans = false;

            if (nodeInfo != null && nodeInfo.Additional.TransType != ECrossSiteTransType.None)
            {
                var transType = nodeInfo.Additional.TransType;
                if (transType != ECrossSiteTransType.None)
                {
                    if (transType == ECrossSiteTransType.AllParentSite)
                    {
                        var parentPublishmentSystemId = PublishmentSystemManager.GetParentPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
                        if (parentPublishmentSystemId != 0)
                        {
                            isCrossSiteTrans = true;
                        }
                    }
                    else if (transType == ECrossSiteTransType.SelfSite)
                    {
                        isCrossSiteTrans = true;
                    }
                    else if (transType == ECrossSiteTransType.AllSite)
                    {
                        isCrossSiteTrans = true;
                    }
                    else if (transType == ECrossSiteTransType.SpecifiedSite)
                    {
                        if (nodeInfo.Additional.TransPublishmentSystemId > 0)
                        {
                            var thePublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(nodeInfo.Additional.TransPublishmentSystemId);
                            if (thePublishmentSystemInfo != null)
                            {
                                isCrossSiteTrans = true;
                            }
                        }
                    }
                    else if (transType == ECrossSiteTransType.ParentSite)
                    {
                        var parentPublishmentSystemId = PublishmentSystemManager.GetParentPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
                        if (parentPublishmentSystemId != 0)
                        {
                            isCrossSiteTrans = true;
                        }
                    }
                }
            }

            return isCrossSiteTrans;
        }

        public static bool IsAutomatic(NodeInfo nodeInfo)
        {
            var isAutomatic = false;

            if (nodeInfo != null)
            {
                if (nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite || nodeInfo.Additional.TransType == ECrossSiteTransType.ParentSite || nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    isAutomatic = nodeInfo.Additional.TransIsAutomatic;
                }
            }

            return isAutomatic;
        }

        public static void LoadPublishmentSystemIdDropDownList(DropDownList publishmentSystemIdDropDownList, PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            publishmentSystemIdDropDownList.Items.Clear();

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
            if (nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite || nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite || nodeInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
            {
                int thePublishmentSystemId;
                if (nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                {
                    thePublishmentSystemId = publishmentSystemInfo.PublishmentSystemId;
                }
                else if (nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    thePublishmentSystemId = nodeInfo.Additional.TransPublishmentSystemId;
                }
                else
                {
                    thePublishmentSystemId = PublishmentSystemManager.GetParentPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
                }
                if (thePublishmentSystemId > 0)
                {
                    var thePublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(thePublishmentSystemId);
                    if (thePublishmentSystemInfo != null)
                    {
                        var listitem = new ListItem(thePublishmentSystemInfo.PublishmentSystemName, thePublishmentSystemInfo.PublishmentSystemId.ToString());
                        publishmentSystemIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (nodeInfo.Additional.TransType == ECrossSiteTransType.AllParentSite || nodeInfo.Additional.TransType == ECrossSiteTransType.AllSite)
            {
                var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdList();

                var allParentPublishmentSystemIdList = new List<int>();
                if (nodeInfo.Additional.TransType == ECrossSiteTransType.AllParentSite)
                {
                    PublishmentSystemManager.GetAllParentPublishmentSystemIdList(allParentPublishmentSystemIdList, publishmentSystemIdList, publishmentSystemInfo.PublishmentSystemId);
                }

                foreach (int psId in publishmentSystemIdList)
                {
                    if (psId == publishmentSystemInfo.PublishmentSystemId) continue;
                    var psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(psId);
                    var show = false;
                    if (nodeInfo.Additional.TransType == ECrossSiteTransType.AllSite)
                    {
                        show = true;
                    }
                    else if (nodeInfo.Additional.TransType == ECrossSiteTransType.AllParentSite)
                    {
                        if (psInfo.IsHeadquarters || allParentPublishmentSystemIdList.Contains(psInfo.PublishmentSystemId))
                        {
                            show = true;
                        }
                    }
                    if (show)
                    {
                        var listitem = new ListItem(psInfo.PublishmentSystemName, psId.ToString());
                        if (psInfo.IsHeadquarters) listitem.Selected = true;
                        publishmentSystemIdDropDownList.Items.Add(listitem);
                    }
                }
            }
        }

        public static void LoadNodeIdListBox(ListBox nodeIdListBox, PublishmentSystemInfo publishmentSystemInfo, int psId, NodeInfo nodeInfo, string administratorName)
        {
            nodeIdListBox.Items.Clear();

            var isUseNodeNames = nodeInfo.Additional.TransType == ECrossSiteTransType.AllParentSite || nodeInfo.Additional.TransType == ECrossSiteTransType.AllSite;

            if (!isUseNodeNames)
            {
                var nodeIdList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.TransNodeIds);
                foreach (var theNodeId in nodeIdList)
                {
                    var theNodeInfo = NodeManager.GetNodeInfo(psId, theNodeId);
                    if (theNodeInfo != null)
                    {
                        var listitem = new ListItem(theNodeInfo.NodeName, theNodeInfo.NodeId.ToString());
                        nodeIdListBox.Items.Add(listitem);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(nodeInfo.Additional.TransNodeNames))
                {
                    var nodeNameArrayList = TranslateUtils.StringCollectionToStringList(nodeInfo.Additional.TransNodeNames);
                    var dic = NodeManager.GetNodeInfoDictionaryByPublishmentSystemId(psId);
                    if (dic != null)
                    {
                        foreach (var nodeName in nodeNameArrayList)
                        {
                            foreach (var theNodeId in dic.Keys)
                            {
                                var theNodeInfo = NodeManager.GetNodeInfo(psId, theNodeId);
                                if (theNodeInfo.NodeName == nodeName)
                                {
                                    var listitem = new ListItem(theNodeInfo.NodeName, theNodeInfo.NodeId.ToString());
                                    nodeIdListBox.Items.Add(listitem);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    NodeManager.AddListItemsForAddContent(nodeIdListBox.Items, PublishmentSystemManager.GetPublishmentSystemInfo(psId), false, administratorName);
                }
            }
        }

        public static string GetDescription(int publishmentSystemId, NodeInfo nodeInfo)
        {
            var results = string.Empty;

            if (nodeInfo != null)
            {
                results = ECrossSiteTransTypeUtils.GetText(nodeInfo.Additional.TransType);

                if (nodeInfo.Additional.TransType == ECrossSiteTransType.AllParentSite || nodeInfo.Additional.TransType == ECrossSiteTransType.AllSite)
                {
                    if (!string.IsNullOrEmpty(nodeInfo.Additional.TransNodeNames))
                    {
                        results += $"({nodeInfo.Additional.TransNodeNames})";
                    }
                }
                else if (nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite || nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite || nodeInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
                {
                    PublishmentSystemInfo publishmentSystemInfo = null;

                    if (nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                    {
                        publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                    }
                    else if (nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                    {
                        publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(nodeInfo.Additional.TransPublishmentSystemId);
                    }
                    else
                    {
                        var parentPublishmentSystemId = PublishmentSystemManager.GetParentPublishmentSystemId(publishmentSystemId);
                        if (parentPublishmentSystemId != 0)
                        {
                            publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(parentPublishmentSystemId);
                        }
                    }

                    if (publishmentSystemInfo != null && !string.IsNullOrEmpty(nodeInfo.Additional.TransNodeIds))
                    {
                        var nodeNameBuilder = new StringBuilder();
                        var nodeIdArrayList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.TransNodeIds);
                        foreach (int nodeId in nodeIdArrayList)
                        {
                            var theNodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                            if (theNodeInfo != null)
                            {
                                nodeNameBuilder.Append(theNodeInfo.NodeName).Append(",");
                            }
                        }
                        if (nodeNameBuilder.Length > 0)
                        {
                            nodeNameBuilder.Length--;
                            results += $"({publishmentSystemInfo.PublishmentSystemName}:{nodeNameBuilder})";
                        }
                    }
                }
            }
            return results;
        }

        public static void TransContentInfo(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, int contentId, PublishmentSystemInfo targetPublishmentSystemInfo, int targetNodeId)
        {
            var targetTableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeId);

            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
            FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);
            contentInfo.PublishmentSystemId = targetPublishmentSystemInfo.PublishmentSystemId;
            contentInfo.SourceId = nodeInfo.NodeId;
            contentInfo.NodeId = targetNodeId;
            contentInfo.IsChecked = targetPublishmentSystemInfo.Additional.IsCrossSiteTransChecked;
            contentInfo.CheckedLevel = 0;

            //复制
            if (Equals(nodeInfo.Additional.TransDoneType, ETranslateContentType.Copy))
            {
                contentInfo.SetExtendedAttribute(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
            }
            //引用地址
            else if (Equals(nodeInfo.Additional.TransDoneType, ETranslateContentType.Reference))
            {
                contentInfo.PublishmentSystemId = targetPublishmentSystemInfo.PublishmentSystemId;
                contentInfo.SourceId = nodeInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.ReferenceId = contentId;
                contentInfo.SetExtendedAttribute(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
            }
            //引用内容
            else if (Equals(nodeInfo.Additional.TransDoneType, ETranslateContentType.ReferenceContent))
            {
                contentInfo.PublishmentSystemId = targetPublishmentSystemInfo.PublishmentSystemId;
                contentInfo.SourceId = nodeInfo.NodeId;
                contentInfo.NodeId = targetNodeId;
                contentInfo.ReferenceId = contentId;
                contentInfo.SetExtendedAttribute(ContentAttribute.TranslateContentType, ETranslateContentType.ReferenceContent.ToString());
            }

            if (!string.IsNullOrEmpty(targetTableName))
            {
                DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);

                #region 复制资源
                //资源：图片，文件，视频
                if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl)))
                {
                    //修改图片
                    var sourceImageUrl = PathUtility.MapPath(publishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.ImageUrl));
                    CopyReferenceFiles(targetPublishmentSystemInfo, sourceImageUrl, publishmentSystemInfo);

                }
                else if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl))))
                {
                    var sourceImageUrls = TranslateUtils.StringCollectionToStringList(contentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)));

                    foreach (string imageUrl in sourceImageUrls)
                    {
                        var sourceImageUrl = PathUtility.MapPath(publishmentSystemInfo, imageUrl);
                        CopyReferenceFiles(targetPublishmentSystemInfo, sourceImageUrl, publishmentSystemInfo);
                    }
                }
                if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl)))
                {
                    //修改附件
                    var sourceFileUrl = PathUtility.MapPath(publishmentSystemInfo, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.FileUrl));
                    CopyReferenceFiles(targetPublishmentSystemInfo, sourceFileUrl, publishmentSystemInfo);

                }
                else if (!string.IsNullOrEmpty(contentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl))))
                {
                    var sourceFileUrls = TranslateUtils.StringCollectionToStringList(contentInfo.GetExtendedAttribute(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)));

                    foreach (string fileUrl in sourceFileUrls)
                    {
                        var sourceFileUrl = PathUtility.MapPath(publishmentSystemInfo, fileUrl);
                        CopyReferenceFiles(targetPublishmentSystemInfo, sourceFileUrl, publishmentSystemInfo);
                    }
                }
                #endregion
            }
        }

        private static void CopyReferenceFiles(PublishmentSystemInfo targetPublishmentSystemInfo, string sourceUrl, PublishmentSystemInfo sourcePublishmentSystemInfo)
        {
            var targetUrl = StringUtils.ReplaceFirst(sourcePublishmentSystemInfo.PublishmentSystemDir, sourceUrl, targetPublishmentSystemInfo.PublishmentSystemDir);
            if (!FileUtils.IsFileExists(targetUrl))
            {
                FileUtils.CopyFile(sourceUrl, targetUrl, true);
            }
        }
    }
}