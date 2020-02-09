using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using Datory;
using Datory.Utils;
using SiteServer.CMS.DataCache;
using SiteServer.Abstractions;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.Core
{
    public static class CrossSiteTransUtility
    {
        public static async Task<bool> IsCrossSiteTransAsync(Site site, Channel channel)
        {
            var isCrossSiteTrans = false;

            if (channel != null && channel.TransType != TransType.None)
            {
                var transType = channel.TransType;
                if (transType != TransType.None)
                {
                    if (transType == TransType.AllParentSite)
                    {
                        var parentSiteId = await DataProvider.SiteRepository.GetParentSiteIdAsync(site.Id);
                        if (parentSiteId != 0)
                        {
                            isCrossSiteTrans = true;
                        }
                    }
                    else if (transType == TransType.SelfSite)
                    {
                        isCrossSiteTrans = true;
                    }
                    else if (transType == TransType.AllSite)
                    {
                        isCrossSiteTrans = true;
                    }
                    else if (transType == TransType.SpecifiedSite)
                    {
                        if (channel.TransSiteId > 0)
                        {
                            var theSite = await DataProvider.SiteRepository.GetAsync(channel.TransSiteId);
                            if (theSite != null)
                            {
                                isCrossSiteTrans = true;
                            }
                        }
                    }
                    else if (transType == TransType.ParentSite)
                    {
                        var parentSiteId = await DataProvider.SiteRepository.GetParentSiteIdAsync(site.Id);
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
                if (channel.TransType == TransType.SelfSite || channel.TransType == TransType.ParentSite || channel.TransType == TransType.SpecifiedSite)
                {
                    isAutomatic = channel.TransIsAutomatic;
                }
            }

            return isAutomatic;
        }

        public static async Task LoadSiteIdDropDownListAsync(DropDownList siteIdDropDownList, Site site, int channelId)
        {
            siteIdDropDownList.Items.Clear();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
            if (channelInfo.TransType == TransType.SelfSite || channelInfo.TransType == TransType.SpecifiedSite || channelInfo.TransType == TransType.ParentSite)
            {
                int theSiteId;
                if (channelInfo.TransType == TransType.SelfSite)
                {
                    theSiteId = site.Id;
                }
                else if (channelInfo.TransType == TransType.SpecifiedSite)
                {
                    theSiteId = channelInfo.TransSiteId;
                }
                else
                {
                    theSiteId = await DataProvider.SiteRepository.GetParentSiteIdAsync(site.Id);
                }
                if (theSiteId > 0)
                {
                    var theSite = await DataProvider.SiteRepository.GetAsync(theSiteId);
                    if (theSite != null)
                    {
                        var listitem = new ListItem(theSite.SiteName, theSite.Id.ToString());
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (channelInfo.TransType == TransType.AllParentSite)
            {
                var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync();

                var allParentSiteIdList = new List<int>();
                await DataProvider.SiteRepository.GetAllParentSiteIdListAsync(allParentSiteIdList, siteIdList, site.Id);

                foreach (var psId in siteIdList)
                {
                    if (psId == site.Id) continue;
                    var psInfo = await DataProvider.SiteRepository.GetAsync(psId);
                    var show = psInfo.Root || allParentSiteIdList.Contains(psInfo.Id);
                    if (show)
                    {
                        var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                        if (psInfo.Root) listitem.Selected = true;
                        siteIdDropDownList.Items.Add(listitem);
                    }
                }
            }
            else if (channelInfo.TransType == TransType.AllSite)
            {
                var siteIdList = await DataProvider.SiteRepository.GetSiteIdListAsync();

                foreach (var psId in siteIdList)
                {
                    var psInfo = await DataProvider.SiteRepository.GetAsync(psId);
                    var listitem = new ListItem(psInfo.SiteName, psId.ToString());
                    if (psInfo.Root) listitem.Selected = true;
                    siteIdDropDownList.Items.Add(listitem);
                }
            }
        }

        public static async Task LoadChannelIdListBoxAsync(ListBox channelIdListBox, Site site, int psId, Channel channel, PermissionsImpl permissionsImpl)
        {
            channelIdListBox.Items.Clear();

            var isUseNodeNames = channel.TransType == TransType.AllParentSite || channel.TransType == TransType.AllSite;

            if (!isUseNodeNames)
            {
                var channelIdList = Utilities.GetIntList(channel.TransChannelIds);
                foreach (var theChannelId in channelIdList)
                {
                    var theNodeInfo = await DataProvider.ChannelRepository.GetAsync(theChannelId);
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
                    var nodeNameArrayList = Utilities.GetStringList(channel.TransChannelNames);
                    var channelIdList = await DataProvider.ChannelRepository.GetChannelIdListAsync(psId);
                    foreach (var nodeName in nodeNameArrayList)
                    {
                        foreach (var theChannelId in channelIdList)
                        {
                            var theNodeInfo = await DataProvider.ChannelRepository.GetAsync(theChannelId);
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
                    await DataProvider.ChannelRepository.AddListItemsForAddContentAsync(channelIdListBox.Items, await DataProvider.SiteRepository.GetAsync(psId), false, permissionsImpl);
                }
            }
        }

        public static async Task<string> GetDescriptionAsync(int siteId, Channel channel)
        {
            var results = string.Empty;

            if (channel != null)
            {
                results = channel.TransType.GetDisplayName();

                if (channel.TransType == TransType.AllParentSite || channel.TransType == TransType.AllSite)
                {
                    if (!string.IsNullOrEmpty(channel.TransChannelNames))
                    {
                        results += $"({channel.TransChannelNames})";
                    }
                }
                else if (channel.TransType == TransType.SelfSite || channel.TransType == TransType.SpecifiedSite || channel.TransType == TransType.ParentSite)
                {
                    Site site = null;

                    if (channel.TransType == TransType.SelfSite)
                    {
                        site = await DataProvider.SiteRepository.GetAsync(siteId);
                    }
                    else if (channel.TransType == TransType.SpecifiedSite)
                    {
                        site = await DataProvider.SiteRepository.GetAsync(channel.TransSiteId);
                    }
                    else
                    {
                        var parentSiteId = await DataProvider.SiteRepository.GetParentSiteIdAsync(siteId);
                        if (parentSiteId != 0)
                        {
                            site = await DataProvider.SiteRepository.GetAsync(parentSiteId);
                        }
                    }

                    if (site != null && !string.IsNullOrEmpty(channel.TransChannelIds))
                    {
                        var nodeNameBuilder = new StringBuilder();
                        var channelIdArrayList = Utilities.GetIntList(channel.TransChannelIds);
                        foreach (int channelId in channelIdArrayList)
                        {
                            var theNodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
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
            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channel, contentId);
            FileUtility.MoveFileByContentInfo(site, targetSite, contentInfo);
            contentInfo.SiteId = targetSite.Id;
            contentInfo.SourceId = channel.Id;
            contentInfo.ChannelId = targetChannelId;
            contentInfo.Checked = targetSite.IsCrossSiteTransChecked;
            contentInfo.CheckedLevel = 0;

            //复制
            if (Equals(channel.TransDoneType, TranslateContentType.Copy))
            {
                contentInfo.TranslateContentType = TranslateContentType.Copy;
            }
            //引用地址
            else if (Equals(channel.TransDoneType, TranslateContentType.Reference))
            {
                contentInfo.SiteId = targetSite.Id;
                contentInfo.SourceId = channel.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.TranslateContentType = TranslateContentType.Reference;
            }
            //引用内容
            else if (Equals(channel.TransDoneType, TranslateContentType.ReferenceContent))
            {
                contentInfo.SiteId = targetSite.Id;
                contentInfo.SourceId = channel.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.TranslateContentType = TranslateContentType.ReferenceContent;
            }

            await DataProvider.ContentRepository.InsertAsync(targetSite, channel, contentInfo);

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
                var sourceImageUrls = Utilities.GetStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl)));

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
                var sourceFileUrls = Utilities.GetStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl)));

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