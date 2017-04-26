using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
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

        public static bool IsTranslatable(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var isTranslatable = false;

            if (nodeInfo != null && nodeInfo.Additional.TransType != ECrossSiteTransType.None)
            {
                var transType = nodeInfo.Additional.TransType;
                if (transType != ECrossSiteTransType.None)
                {
                    if (transType == ECrossSiteTransType.AllParentSite)
                    {
                        var parentPublishmentSystemID = PublishmentSystemManager.GetParentPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
                        if (parentPublishmentSystemID != 0)
                        {
                            isTranslatable = true;
                        }
                    }
                    else if (transType == ECrossSiteTransType.SelfSite)
                    {
                        isTranslatable = true;
                    }
                    else if (transType == ECrossSiteTransType.AllSite)
                    {
                        isTranslatable = true;
                    }
                    else if (transType == ECrossSiteTransType.SpecifiedSite)
                    {
                        if (nodeInfo.Additional.TransPublishmentSystemID > 0)
                        {
                            var thePublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(nodeInfo.Additional.TransPublishmentSystemID);
                            if (thePublishmentSystemInfo != null)
                            {
                                isTranslatable = true;
                            }
                        }
                    }
                    else if (transType == ECrossSiteTransType.ParentSite)
                    {
                        var parentPublishmentSystemID = PublishmentSystemManager.GetParentPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
                        if (parentPublishmentSystemID != 0)
                        {
                            isTranslatable = true;
                        }
                    }
                }
            }

            return isTranslatable;
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

        public static void LoadPublishmentSystemIDDropDownList(DropDownList publishmentSystemIDDropDownList, PublishmentSystemInfo publishmentSystemInfo, int nodeID)
        {
            publishmentSystemIDDropDownList.Items.Clear();

            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
            if (nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite || nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite || nodeInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
            {
                int thePublishmentSystemID;
                if (nodeInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                {
                    thePublishmentSystemID = publishmentSystemInfo.PublishmentSystemId;
                }
                else if (nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    thePublishmentSystemID = nodeInfo.Additional.TransPublishmentSystemID;
                }
                else
                {
                    thePublishmentSystemID = PublishmentSystemManager.GetParentPublishmentSystemId(publishmentSystemInfo.PublishmentSystemId);
                }
                if (thePublishmentSystemID > 0)
                {
                    var thePublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(thePublishmentSystemID);
                    if (thePublishmentSystemInfo != null)
                    {
                        var listitem = new ListItem(thePublishmentSystemInfo.PublishmentSystemName, thePublishmentSystemInfo.PublishmentSystemId.ToString());
                        publishmentSystemIDDropDownList.Items.Add(listitem);
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
                        publishmentSystemIDDropDownList.Items.Add(listitem);
                    }
                }
            }
        }

        public static void LoadNodeIDListBox(ListBox nodeIDListBox, PublishmentSystemInfo publishmentSystemInfo, int psID, NodeInfo nodeInfo, string administratorName)
        {
            nodeIDListBox.Items.Clear();

            var isUseNodeNames = false;
            if (nodeInfo.Additional.TransType == ECrossSiteTransType.AllParentSite || nodeInfo.Additional.TransType == ECrossSiteTransType.AllSite)
            {
                isUseNodeNames = true;
            }

            if (!isUseNodeNames)
            {
                var nodeIDArrayList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.TransNodeIDs);
                foreach (int theNodeID in nodeIDArrayList)
                {
                    var theNodeInfo = NodeManager.GetNodeInfo(psID, theNodeID);
                    if (theNodeInfo != null)
                    {
                        var listitem = new ListItem(theNodeInfo.NodeName, theNodeInfo.NodeId.ToString());
                        nodeIDListBox.Items.Add(listitem);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(nodeInfo.Additional.TransNodeNames))
                {
                    var nodeNameArrayList = TranslateUtils.StringCollectionToStringList(nodeInfo.Additional.TransNodeNames);
                    var dic = NodeManager.GetNodeInfoHashtableByPublishmentSystemId(psID);
                    if (dic != null)
                    {
                        foreach (string nodeName in nodeNameArrayList)
                        {
                            foreach (var theNodeId in dic.Keys)
                            {
                                var theNodeInfo = NodeManager.GetNodeInfo(psID, (int)theNodeId);
                                if (theNodeInfo.NodeName == nodeName)
                                {
                                    var listitem = new ListItem(theNodeInfo.NodeName, theNodeInfo.NodeId.ToString());
                                    nodeIDListBox.Items.Add(listitem);
                                    break;
                                }
                            }
                        }
                    }
                }
                else
                {
                    NodeManager.AddListItemsForAddContent(nodeIDListBox.Items, PublishmentSystemManager.GetPublishmentSystemInfo(psID), false, administratorName);
                }
            }
        }

        public static string GetDescription(int publishmentSystemID, NodeInfo nodeInfo)
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
                        publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
                    }
                    else if (nodeInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                    {
                        publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(nodeInfo.Additional.TransPublishmentSystemID);
                    }
                    else
                    {
                        var parentPublishmentSystemID = PublishmentSystemManager.GetParentPublishmentSystemId(publishmentSystemID);
                        if (parentPublishmentSystemID != 0)
                        {
                            publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(parentPublishmentSystemID);
                        }
                    }

                    if (publishmentSystemInfo != null && !string.IsNullOrEmpty(nodeInfo.Additional.TransNodeIDs))
                    {
                        var nodeNameBuilder = new StringBuilder();
                        var nodeIDArrayList = TranslateUtils.StringCollectionToIntList(nodeInfo.Additional.TransNodeIDs);
                        foreach (int nodeID in nodeIDArrayList)
                        {
                            var theNodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeID);
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

        public static void TransContentInfo(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, int contentID, PublishmentSystemInfo targetPublishmentSystemInfo, int targetNodeID)
        {
            var targetTableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeID);

            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentID);
            FileUtility.MoveFileByContentInfo(publishmentSystemInfo, targetPublishmentSystemInfo, contentInfo);
            contentInfo.PublishmentSystemId = targetPublishmentSystemInfo.PublishmentSystemId;
            contentInfo.SourceId = nodeInfo.NodeId;
            contentInfo.NodeId = targetNodeID;
            if (targetPublishmentSystemInfo.Additional.IsCrossSiteTransChecked)
            {
                contentInfo.IsChecked = true;
            }
            else
            {
                contentInfo.IsChecked = false;
            }
            contentInfo.CheckedLevel = 0;

            //复制
            if (Equals(nodeInfo.Additional.TransDoneType, ETranslateContentType.Copy))
            {
                contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
            }
            //引用地址
            else if (Equals(nodeInfo.Additional.TransDoneType, ETranslateContentType.Reference))
            {
                contentInfo.PublishmentSystemId = targetPublishmentSystemInfo.PublishmentSystemId;
                contentInfo.SourceId = nodeInfo.NodeId;
                contentInfo.NodeId = targetNodeID;
                contentInfo.ReferenceId = contentID;
                contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
            }
            //引用内容
            else if (Equals(nodeInfo.Additional.TransDoneType, ETranslateContentType.ReferenceContent))
            {
                contentInfo.PublishmentSystemId = targetPublishmentSystemInfo.PublishmentSystemId;
                contentInfo.SourceId = nodeInfo.NodeId;
                contentInfo.NodeId = targetNodeID;
                contentInfo.ReferenceId = contentID;
                contentInfo.Attributes.Add(ContentAttribute.TranslateContentType, ETranslateContentType.ReferenceContent.ToString());
            }


            if (!string.IsNullOrEmpty(targetTableName))
            {
                var theContentID = DataProvider.ContentDao.Insert(targetTableName, targetPublishmentSystemInfo, contentInfo);

                #region 复制资源
                var targetPulishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemInfo.PublishmentSystemId);
                var targetTableStyle = NodeManager.GetTableStyle(targetPulishmentSystemInfo, targetNodeID);
                var targetContentInfo = DataProvider.ContentDao.GetContentInfo(targetTableStyle, targetTableName, theContentID);
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

                    foreach (string FileUrl in sourceFileUrls)
                    {
                        var sourceFileUrl = PathUtility.MapPath(publishmentSystemInfo, FileUrl);
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