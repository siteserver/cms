using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Enumerations;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;

namespace SiteServer.CMS.Core
{
    public static class CrossSiteTransUtility
    {
        public static async Task<bool> IsCrossSiteTransAsync(Site site, Channel channel)
        {
            var isCrossSiteTrans = false;

            if (channel != null && channel.TransType != ECrossSiteTransType.None)
            {
                var transType = channel.TransType;
                if (transType != ECrossSiteTransType.None)
                {
                    if (transType == ECrossSiteTransType.AllParentSite)
                    {
                        var parentSiteId = await DataProvider.SiteDao.GetParentSiteIdAsync(site.Id);
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
                        if (channel.TransSiteId > 0)
                        {
                            var theSite = await DataProvider.SiteDao.GetAsync(channel.TransSiteId);
                            if (theSite != null)
                            {
                                isCrossSiteTrans = true;
                            }
                        }
                    }
                    else if (transType == ECrossSiteTransType.ParentSite)
                    {
                        var parentSiteId = await DataProvider.SiteDao.GetParentSiteIdAsync(site.Id);
                        if (parentSiteId != 0)
                        {
                            isCrossSiteTrans = true;
                        }
                    }
                }
            }

            return isCrossSiteTrans;
        }

        public static bool IsAutomatic(Channel channel)
        {
            var isAutomatic = false;

            if (channel != null)
            {
                if (channel.TransType == ECrossSiteTransType.SelfSite || channel.TransType == ECrossSiteTransType.ParentSite || channel.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    isAutomatic = channel.TransIsAutomatic;
                }
            }

            return isAutomatic;
        }

        public static async Task LoadSiteIdDropDownListAsync(DropDownList siteIdDropDownList, Site site, int channelId)
        {
            siteIdDropDownList.Items.Clear();

            var channelInfo = await ChannelManager.GetChannelAsync(site.Id, channelId);
            if (channelInfo.TransType == ECrossSiteTransType.SelfSite || channelInfo.TransType == ECrossSiteTransType.SpecifiedSite || channelInfo.TransType == ECrossSiteTransType.ParentSite)
            {
                int theSiteId;
                if (channelInfo.TransType == ECrossSiteTransType.SelfSite)
                {
                    theSiteId = site.Id;
                }
                else if (channelInfo.TransType == ECrossSiteTransType.SpecifiedSite)
                {
                    theSiteId = channelInfo.TransSiteId;
                }
                else
                {
                    theSiteId = await DataProvider.SiteDao.GetParentSiteIdAsync(site.Id);
                }
                if (theSiteId > 0)
                {
                    var theSite = await DataProvider.SiteDao.GetAsync(theSiteId);
                    if (theSite != null)
                    {
                        var listitem = new ListItem(theSite.SiteName, theSite.Id.ToString());
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (channelInfo.TransType == ECrossSiteTransType.AllParentSite)
            {
                var siteIdList = await DataProvider.SiteDao.GetSiteIdListAsync();

                var allParentSiteIdList = new List<int>();
                await DataProvider.SiteDao.GetAllParentSiteIdListAsync(allParentSiteIdList, siteIdList, site.Id);

                foreach (var psId in siteIdList)
                {
                    if (psId == site.Id) continue;
                    var psInfo = await DataProvider.SiteDao.GetAsync(psId);
                    var show = psInfo.Root || allParentSiteIdList.Contains(psInfo.Id);
                    if (show)
                    {
                        var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                        if (psInfo.Root) listitem.Selected = true;
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (channelInfo.TransType == ECrossSiteTransType.AllSite)
            {
                var siteIdList = await DataProvider.SiteDao.GetSiteIdListAsync();

                foreach (var psId in siteIdList)
                {
                    var psInfo = await DataProvider.SiteDao.GetAsync(psId);
                    var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                    if (psInfo.Root) listitem.Selected = true;
                    siteIdDropDownList.Items.Add(listitem);
                }
            }
        }

        public static async Task LoadChannelIdListBoxAsync(ListBox channelIdListBox, Site site, int psId, Channel channel, PermissionsImpl permissionsImpl)
        {
            channelIdListBox.Items.Clear();

            var isUseNodeNames = channel.TransType == ECrossSiteTransType.AllParentSite || channel.TransType == ECrossSiteTransType.AllSite;

            if (!isUseNodeNames)
            {
                var channelIdList = TranslateUtils.StringCollectionToIntList(channel.TransChannelIds);
                foreach (var theChannelId in channelIdList)
                {
                    var theNodeInfo = await ChannelManager.GetChannelAsync(psId, theChannelId);
                    if (theNodeInfo != null)
                    {
                        var listitem = new ListItem(theNodeInfo.ChannelName, theNodeInfo.Id.ToString());
                        channelIdListBox.Items.Add(listitem);
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(channel.TransChannelNames))
                {
                    var nodeNameArrayList = TranslateUtils.StringCollectionToStringList(channel.TransChannelNames);
                    var channelIdList = await ChannelManager.GetChannelIdListAsync(psId);
                    foreach (var nodeName in nodeNameArrayList)
                    {
                        foreach (var theChannelId in channelIdList)
                        {
                            var theNodeInfo = await ChannelManager.GetChannelAsync(psId, theChannelId);
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
                    await ChannelManager.AddListItemsForAddContentAsync(channelIdListBox.Items, await DataProvider.SiteDao.GetAsync(psId), false, permissionsImpl);
                }
            }
        }

        public static async Task<string> GetDescriptionAsync(int siteId, Channel channel)
        {
            var results = string.Empty;

            if (channel != null)
            {
                results = ECrossSiteTransTypeUtils.GetText(channel.TransType);

                if (channel.TransType == ECrossSiteTransType.AllParentSite || channel.TransType == ECrossSiteTransType.AllSite)
                {
                    if (!string.IsNullOrEmpty(channel.TransChannelNames))
                    {
                        results += $"({channel.TransChannelNames})";
                    }
                }
                else if (channel.TransType == ECrossSiteTransType.SelfSite || channel.TransType == ECrossSiteTransType.SpecifiedSite || channel.TransType == ECrossSiteTransType.ParentSite)
                {
                    Site site = null;

                    if (channel.TransType == ECrossSiteTransType.SelfSite)
                    {
                        site = await DataProvider.SiteDao.GetAsync(siteId);
                    }
                    else if (channel.TransType == ECrossSiteTransType.SpecifiedSite)
                    {
                        site = await DataProvider.SiteDao.GetAsync(channel.TransSiteId);
                    }
                    else
                    {
                        var parentSiteId = await DataProvider.SiteDao.GetParentSiteIdAsync(siteId);
                        if (parentSiteId != 0)
                        {
                            site = await DataProvider.SiteDao.GetAsync(parentSiteId);
                        }
                    }

                    if (site != null && !string.IsNullOrEmpty(channel.TransChannelIds))
                    {
                        var nodeNameBuilder = new StringBuilder();
                        var channelIdArrayList = TranslateUtils.StringCollectionToIntList(channel.TransChannelIds);
                        foreach (int channelId in channelIdArrayList)
                        {
                            var theNodeInfo = await ChannelManager.GetChannelAsync(site.Id, channelId);
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

        public static async Task TransContentInfoAsync(Site site, Channel channel, int contentId, Site targetSite, int targetChannelId)
        {
            var contentInfo = await DataProvider.ContentDao.GetAsync(site, channel, contentId);
            FileUtility.MoveFileByContentInfo(site, targetSite, contentInfo);
            contentInfo.SiteId = targetSite.Id;
            contentInfo.SourceId = channel.Id;
            contentInfo.ChannelId = targetChannelId;
            contentInfo.Checked = targetSite.IsCrossSiteTransChecked;
            contentInfo.CheckedLevel = 0;

            //复制
            if (Equals(channel.TransDoneType, ETranslateContentType.Copy))
            {
                contentInfo.TranslateContentType = ETranslateContentTypeUtils.GetValue(ETranslateContentType.Copy);
            }
            //引用地址
            else if (Equals(channel.TransDoneType, ETranslateContentType.Reference))
            {
                contentInfo.SiteId = targetSite.Id;
                contentInfo.SourceId = channel.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.TranslateContentType = ETranslateContentTypeUtils.GetValue(ETranslateContentType.Reference);
            }
            //引用内容
            else if (Equals(channel.TransDoneType, ETranslateContentType.ReferenceContent))
            {
                contentInfo.SiteId = targetSite.Id;
                contentInfo.SourceId = channel.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.TranslateContentType = ETranslateContentTypeUtils.GetValue(ETranslateContentType.ReferenceContent);
            }

            await DataProvider.ContentDao.InsertAsync(targetSite, channel, contentInfo);

            #region 复制资源
            //资源：图片，文件，视频
            if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.ImageUrl)))
            {
                //修改图片
                var sourceImageUrl = PathUtility.MapPath(site, contentInfo.Get<string>(ContentAttribute.ImageUrl));
                CopyReferenceFiles(targetSite, sourceImageUrl, site);

            }
            else if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl))))
            {
                var sourceImageUrls = TranslateUtils.StringCollectionToStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl)));

                foreach (string imageUrl in sourceImageUrls)
                {
                    var sourceImageUrl = PathUtility.MapPath(site, imageUrl);
                    CopyReferenceFiles(targetSite, sourceImageUrl, site);
                }
            }
            if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.FileUrl)))
            {
                //修改附件
                var sourceFileUrl = PathUtility.MapPath(site, contentInfo.Get<string>(ContentAttribute.FileUrl));
                CopyReferenceFiles(targetSite, sourceFileUrl, site);

            }
            else if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl))))
            {
                var sourceFileUrls = TranslateUtils.StringCollectionToStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl)));

                foreach (string fileUrl in sourceFileUrls)
                {
                    var sourceFileUrl = PathUtility.MapPath(site, fileUrl);
                    CopyReferenceFiles(targetSite, sourceFileUrl, site);
                }
            }
            #endregion
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