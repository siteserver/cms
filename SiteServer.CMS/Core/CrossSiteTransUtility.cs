using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Db;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Core
{
    public static class CrossSiteTransUtility
    {
        public static async Task<bool> IsCrossSiteTransAsync(Site site, ChannelInfo channelInfo)
        {
            var isCrossSiteTrans = false;

            if (channelInfo != null && channelInfo.Additional.TransType != ECrossSiteTransType.None)
            {
                var transType = channelInfo.Additional.TransType;
                if (transType != ECrossSiteTransType.None)
                {
                    if (transType == ECrossSiteTransType.AllParentSite)
                    {
                        var parentSiteId = await SiteManager.GetParentSiteIdAsync(site.Id);
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
                            var theSite = await SiteManager.GetSiteAsync(channelInfo.Additional.TransSiteId);
                            if (theSite != null)
                            {
                                isCrossSiteTrans = true;
                            }
                        }
                    }
                    else if (transType == ECrossSiteTransType.ParentSite)
                    {
                        var parentSiteId = await SiteManager.GetParentSiteIdAsync(site.Id);
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

        public static async Task LoadSiteIdDropDownListAsync(DropDownList siteIdDropDownList, Site site, int channelId)
        {
            siteIdDropDownList.Items.Clear();

            var channelInfo = ChannelManager.GetChannelInfo(site.Id, channelId);
            if (channelInfo.Additional.TransType == ECrossSiteTransType.SelfSite || channelInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite || channelInfo.Additional.TransType == ECrossSiteTransType.ParentSite)
            {
                int theSiteId;
                if (channelInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                {
                    theSiteId = site.Id;
                }
                else if (channelInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    theSiteId = channelInfo.Additional.TransSiteId;
                }
                else
                {
                    theSiteId = await SiteManager.GetParentSiteIdAsync(site.Id);
                }
                if (theSiteId > 0)
                {
                    var theSite = await SiteManager.GetSiteAsync(theSiteId);
                    if (theSite != null)
                    {
                        var listitem = new ListItem(theSite.SiteName, theSite.Id.ToString());
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (channelInfo.Additional.TransType == ECrossSiteTransType.AllParentSite)
            {
                var siteIdList = await SiteManager.GetSiteIdListAsync();

                var allParentSiteIdList = new List<int>();
                await SiteManager.GetAllParentSiteIdListAsync(allParentSiteIdList, siteIdList, site.Id);

                foreach (var psId in siteIdList)
                {
                    if (psId == site.Id) continue;
                    var psInfo = await SiteManager.GetSiteAsync(psId);
                    var show = psInfo.Root || allParentSiteIdList.Contains(psInfo.Id);
                    if (show)
                    {
                        var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                        if (psInfo.Root) listitem.Selected = true;
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (channelInfo.Additional.TransType == ECrossSiteTransType.AllSite)
            {
                var siteIdList = await SiteManager.GetSiteIdListAsync();

                foreach (var psId in siteIdList)
                {
                    var psInfo = await SiteManager.GetSiteAsync(psId);
                    var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                    if (psInfo.Root) listitem.Selected = true;
                    siteIdDropDownList.Items.Add(listitem);
                }
            }
        }

        public static async Task LoadChannelIdListBoxAsync(ListBox channelIdListBox, Site site, int psId, ChannelInfo channelInfo, PermissionsImpl permissionsImpl)
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
                    ChannelManager.AddListItemsForAddContent(channelIdListBox.Items, await SiteManager.GetSiteAsync(psId), false, permissionsImpl);
                }
            }
        }

        public static async Task<string> GetDescriptionAsync(int siteId, ChannelInfo channelInfo)
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
                    Site site = null;

                    if (channelInfo.Additional.TransType == ECrossSiteTransType.SelfSite)
                    {
                        site = await SiteManager.GetSiteAsync(siteId);
                    }
                    else if (channelInfo.Additional.TransType == ECrossSiteTransType.SpecifiedSite)
                    {
                        site = await SiteManager.GetSiteAsync(channelInfo.Additional.TransSiteId);
                    }
                    else
                    {
                        var parentSiteId = await SiteManager.GetParentSiteIdAsync(siteId);
                        if (parentSiteId != 0)
                        {
                            site = await SiteManager.GetSiteAsync(parentSiteId);
                        }
                    }

                    if (site != null && !string.IsNullOrEmpty(channelInfo.Additional.TransChannelIds))
                    {
                        var nodeNameBuilder = new StringBuilder();
                        var channelIdArrayList = TranslateUtils.StringCollectionToIntList(channelInfo.Additional.TransChannelIds);
                        foreach (int channelId in channelIdArrayList)
                        {
                            var theNodeInfo = ChannelManager.GetChannelInfo(site.Id, channelId);
                            if (theNodeInfo != null)
                            {
                                nodeNameBuilder.Append(theNodeInfo.ChannelName).Append(",");
                            }
                        }
                        if (nodeNameBuilder.Length > 0)
                        {
                            nodeNameBuilder.Length--;
                            results += $"({site.SiteName}:{nodeNameBuilder})";
                        }
                    }
                }
            }
            return results;
        }

        public static void TransContentInfo(Site site, ChannelInfo channelInfo, int contentId, Site targetSite, int targetChannelId)
        {
            var targetTableName = ChannelManager.GetTableName(targetSite, targetChannelId);

            var contentInfo = ContentManager.GetContentInfo(site, channelInfo, contentId);
            FileUtility.MoveFileByContentInfo(site, targetSite, contentInfo);
            contentInfo.SiteId = targetSite.Id;
            contentInfo.SourceId = channelInfo.Id;
            contentInfo.ChannelId = targetChannelId;
            contentInfo.IsChecked = targetSite.Additional.IsCrossSiteTransChecked;
            contentInfo.CheckedLevel = 0;

            //复制
            if (Equals(channelInfo.Additional.TransDoneType, ETranslateContentType.Copy))
            {
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Copy.ToString());
            }
            //引用地址
            else if (Equals(channelInfo.Additional.TransDoneType, ETranslateContentType.Reference))
            {
                contentInfo.SiteId = targetSite.Id;
                contentInfo.SourceId = channelInfo.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.Reference.ToString());
            }
            //引用内容
            else if (Equals(channelInfo.Additional.TransDoneType, ETranslateContentType.ReferenceContent))
            {
                contentInfo.SiteId = targetSite.Id;
                contentInfo.SourceId = channelInfo.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, ETranslateContentType.ReferenceContent.ToString());
            }

            if (!string.IsNullOrEmpty(targetTableName))
            {
                DataProvider.ContentDao.Insert(targetTableName, targetSite, channelInfo, contentInfo);

                #region 复制资源
                //资源：图片，文件，视频
                if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.ImageUrl)))
                {
                    //修改图片
                    var sourceImageUrl = PathUtility.MapPath(site, contentInfo.GetString(BackgroundContentAttribute.ImageUrl));
                    CopyReferenceFiles(targetSite, sourceImageUrl, site);

                }
                else if (!string.IsNullOrEmpty(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl))))
                {
                    var sourceImageUrls = TranslateUtils.StringCollectionToStringList(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl)));

                    foreach (string imageUrl in sourceImageUrls)
                    {
                        var sourceImageUrl = PathUtility.MapPath(site, imageUrl);
                        CopyReferenceFiles(targetSite, sourceImageUrl, site);
                    }
                }
                if (!string.IsNullOrEmpty(contentInfo.GetString(BackgroundContentAttribute.FileUrl)))
                {
                    //修改附件
                    var sourceFileUrl = PathUtility.MapPath(site, contentInfo.GetString(BackgroundContentAttribute.FileUrl));
                    CopyReferenceFiles(targetSite, sourceFileUrl, site);

                }
                else if (!string.IsNullOrEmpty(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl))))
                {
                    var sourceFileUrls = TranslateUtils.StringCollectionToStringList(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl)));

                    foreach (string fileUrl in sourceFileUrls)
                    {
                        var sourceFileUrl = PathUtility.MapPath(site, fileUrl);
                        CopyReferenceFiles(targetSite, sourceFileUrl, site);
                    }
                }
                #endregion
            }
        }

        private static void CopyReferenceFiles(Site targetSite, string sourceUrl, Site sourceSite)
        {
            var targetUrl = StringUtils.ReplaceFirst(sourceSite.SiteDir, sourceUrl, targetSite.SiteDir);
            if (!FileUtils.IsFileExists(targetUrl))
            {
                FileUtils.CopyFile(sourceUrl, targetUrl, true);
            }
        }
    }
}