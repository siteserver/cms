using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Core
{
    public static class CrossSiteTransUtility
    {
        public static bool IsCrossSiteTrans(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            var isCrossSiteTrans = false;

            if (channelInfo != null && channelInfo.Additional.TransType != ECrossSiteTransType.None)
            {
                var transType = channelInfo.Additional.TransType;
                if (transType != ECrossSiteTransType.None)
                {
                    if (transType == ECrossSiteTransType.AllParentSite)
                    {
                        var parentSiteId = SiteManager.GetParentSiteId(siteInfo.Id);
                        if (parentSiteId != 0)
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
                        if (channelInfo.Additional.TransSiteId > 0)
                        {
                            var theSiteInfo = SiteManager.GetSiteInfo(channelInfo.Additional.TransSiteId);
                            if (theSiteInfo != null)
                            {
                                isCrossSiteTrans = true;
                            }
                        }
                    }
                    else if (transType == ECrossSiteTransType.ParentSite)
                    {
                        var parentSiteId = SiteManager.GetParentSiteId(siteInfo.Id);
                        if (parentSiteId != 0)
                        {
                            isCrossSiteTrans = true;
                        }
                    }
                }
            }

            return isCrossSiteTrans;
        }

        public static bool IsAutomatic(ChannelInfo channelInfo)
        {
            var isAutomatic = false;

            if (channelInfo != null)
            {
                if (channelInfo.Additional.TransType == ECrossSiteTransType.SelfSite || channelInfo.Additional.TransType == ECrossSiteTransType.ParentSite || channelInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    isAutomatic = channelInfo.Additional.TransIsAutomatic;
                }
            }

            return isAutomatic;
        }

        public static void LoadSiteIdDropDownList(DropDownList siteIdDropDownList, SiteInfo siteInfo, int channelId)
        {
            siteIdDropDownList.Items.Clear();

            var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
            if (channelInfo.Additional.TransType == ECrossSiteTransType.SelfSite || channelInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite || channelInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
            {
                int theSiteId;
                if (channelInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                {
                    theSiteId = siteInfo.Id;
                }
                else if (channelInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    theSiteId = channelInfo.Additional.TransSiteId;
                }
                else
                {
                    theSiteId = SiteManager.GetParentSiteId(siteInfo.Id);
                }
                if (theSiteId > 0)
                {
                    var theSiteInfo = SiteManager.GetSiteInfo(theSiteId);
                    if (theSiteInfo != null)
                    {
                        var listitem = new ListItem(theSiteInfo.SiteName, theSiteInfo.Id.ToString());
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (channelInfo.Additional.TransType == ECrossSiteTransType.AllParentSite)
            {
                var siteIdList = SiteManager.GetSiteIdList();

                var allParentSiteIdList = new List<int>();
                SiteManager.GetAllParentSiteIdList(allParentSiteIdList, siteIdList, siteInfo.Id);

                foreach (var psId in siteIdList)
                {
                    if (psId == siteInfo.Id) continue;
                    var psInfo = SiteManager.GetSiteInfo(psId);
                    var show = psInfo.IsRoot || allParentSiteIdList.Contains(psInfo.Id);
                    if (show)
                    {
                        var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                        if (psInfo.IsRoot) listitem.Selected = true;
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (channelInfo.Additional.TransType == ECrossSiteTransType.AllSite)
            {
                var siteIdList = SiteManager.GetSiteIdList();

                foreach (var psId in siteIdList)
                {
                    var psInfo = SiteManager.GetSiteInfo(psId);
                    var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                    if (psInfo.IsRoot) listitem.Selected = true;
                    siteIdDropDownList.Items.Add(listitem);
                }
            }
        }

        public static void LoadChannelIdListBox(ListBox channelIdListBox, SiteInfo siteInfo, int psId, ChannelInfo channelInfo, PermissionsImpl permissionsImpl)
        {
            channelIdListBox.Items.Clear();

            var isUseNodeNames = channelInfo.Additional.TransType == ECrossSiteTransType.AllParentSite || channelInfo.Additional.TransType == ECrossSiteTransType.AllSite;

            if (!isUseNodeNames)
            {
                var channelIdList = TranslateUtils.StringCollectionToIntList(channelInfo.Additional.TransChannelIds);
                foreach (var theChannelId in channelIdList)
                {
                    var theNodeInfo = ChannelManager.GetChannelInfo(psId, theChannelId);
                    if (theNodeInfo != null)
                    {
                        var listitem = new ListItem(theNodeInfo.ChannelName, theNodeInfo.Id.ToString());
                        channelIdListBox.Items.Add(listitem);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(channelInfo.Additional.TransChannelNames))
                {
                    var nodeNameArrayList = TranslateUtils.StringCollectionToStringList(channelInfo.Additional.TransChannelNames);
                    var channelIdList = ChannelManager.GetChannelIdList(psId);
                    foreach (var nodeName in nodeNameArrayList)
                    {
                        foreach (var theChannelId in channelIdList)
                        {
                            var theNodeInfo = ChannelManager.GetChannelInfo(psId, theChannelId);
                            if (theNodeInfo.ChannelName == nodeName)
                            {
                                var listitem = new ListItem(theNodeInfo.ChannelName, theNodeInfo.Id.ToString());
                                channelIdListBox.Items.Add(listitem);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    ChannelManager.AddListItemsForAddContent(channelIdListBox.Items, SiteManager.GetSiteInfo(psId), false, permissionsImpl);
                }
            }
        }

        public static string GetDescription(int siteId, ChannelInfo channelInfo)
        {
            var results = string.Empty;

            if (channelInfo != null)
            {
                results = ECrossSiteTransTypeUtils.GetText(channelInfo.Additional.TransType);

                if (channelInfo.Additional.TransType == ECrossSiteTransType.AllParentSite || channelInfo.Additional.TransType == ECrossSiteTransType.AllSite)
                {
                    if (!string.IsNullOrEmpty(channelInfo.Additional.TransChannelNames))
                    {
                        results += $"({channelInfo.Additional.TransChannelNames})";
                    }
                }
                else if (channelInfo.Additional.TransType == ECrossSiteTransType.SelfSite || channelInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite || channelInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
                {
                    SiteInfo siteInfo = null;

                    if (channelInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                    {
                        siteInfo = SiteManager.GetSiteInfo(siteId);
                    }
                    else if (channelInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                    {
                        siteInfo = SiteManager.GetSiteInfo(channelInfo.Additional.TransSiteId);
                    }
                    else
                    {
                        var parentSiteId = SiteManager.GetParentSiteId(siteId);
                        if (parentSiteId != 0)
                        {
                            siteInfo = SiteManager.GetSiteInfo(parentSiteId);
                        }
                    }

                    if (siteInfo != null && !string.IsNullOrEmpty(channelInfo.Additional.TransChannelIds))
                    {
                        var nodeNameBuilder = new StringBuilder();
                        var channelIdArrayList = TranslateUtils.StringCollectionToIntList(channelInfo.Additional.TransChannelIds);
                        foreach (int channelId in channelIdArrayList)
                        {
                            var theNodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                            if (theNodeInfo != null)
                            {
                                nodeNameBuilder.Append(theNodeInfo.ChannelName).Append(",");
                            }
                        }
                        if (nodeNameBuilder.Length > 0)
                        {
                            nodeNameBuilder.Length--;
                            results += $"({siteInfo.SiteName}:{nodeNameBuilder})";
                        }
                    }
                }
            }
            return results;
        }

        public static void TransContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId, SiteInfo targetSiteInfo, int targetChannelId)
        {
            var targetTableName = ChannelManager.GetTableName(targetSiteInfo, targetChannelId);

            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
            FileUtility.MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);
            contentInfo.SiteId = targetSiteInfo.Id;
            contentInfo.SourceId = channelInfo.Id;
            contentInfo.ChannelId = targetChannelId;
            contentInfo.IsChecked = targetSiteInfo.Additional.IsCrossSiteTransChecked;
            contentInfo.CheckedLevel = 0;

            //复制
            if (Equals(channelInfo.Additional.TransDoneType, ETranslateContentType.Copy))
            {
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
            }
            //引用地址
            else if (Equals(channelInfo.Additional.TransDoneType, ETranslateContentType.Reference))
            {
                contentInfo.SiteId = targetSiteInfo.Id;
                contentInfo.SourceId = channelInfo.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
            }
            //引用内容
            else if (Equals(channelInfo.Additional.TransDoneType, ETranslateContentType.ReferenceContent))
            {
                contentInfo.SiteId = targetSiteInfo.Id;
                contentInfo.SourceId = channelInfo.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.ReferenceContent.ToString());
            }

            if (!string.IsNullOrEmpty(targetTableName))
            {
                DataProvider.ContentDao.Insert(targetTableName, targetSiteInfo, channelInfo, contentInfo);

                #region 复制资源
                //资源：图片，文件，视频
                if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.ImageUrl)))
                {
                    //修改图片
                    var sourceImageUrl = PathUtility.MapPath(siteInfo, contentInfo.GetString(BackgroundContentAttribute.ImageUrl));
                    CopyReferenceFiles(targetSiteInfo, sourceImageUrl, siteInfo);

                }
                else if (!string.IsNullOrEmpty(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl))))
                {
                    var sourceImageUrls = TranslateUtils.StringCollectionToStringList(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)));

                    foreach (string imageUrl in sourceImageUrls)
                    {
                        var sourceImageUrl = PathUtility.MapPath(siteInfo, imageUrl);
                        CopyReferenceFiles(targetSiteInfo, sourceImageUrl, siteInfo);
                    }
                }
                if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.FileUrl)))
                {
                    //修改附件
                    var sourceFileUrl = PathUtility.MapPath(siteInfo, contentInfo.GetString(BackgroundContentAttribute.FileUrl));
                    CopyReferenceFiles(targetSiteInfo, sourceFileUrl, siteInfo);

                }
                else if (!string.IsNullOrEmpty(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl))))
                {
                    var sourceFileUrls = TranslateUtils.StringCollectionToStringList(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)));

                    foreach (string fileUrl in sourceFileUrls)
                    {
                        var sourceFileUrl = PathUtility.MapPath(siteInfo, fileUrl);
                        CopyReferenceFiles(targetSiteInfo, sourceFileUrl, siteInfo);
                    }
                }
                #endregion
            }
        }

        private static void CopyReferenceFiles(SiteInfo targetSiteInfo, string sourceUrl, SiteInfo sourceSiteInfo)
        {
            var targetUrl = StringUtils.ReplaceFirst(sourceSiteInfo.SiteDir, sourceUrl, targetSiteInfo.SiteDir);
            if (!FileUtils.IsFileExists(targetUrl))
            {
                FileUtils.CopyFile(sourceUrl, targetUrl, true);
            }
        }
    }
}