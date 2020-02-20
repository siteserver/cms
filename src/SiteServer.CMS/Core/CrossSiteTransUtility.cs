using System.Text;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.Core
{
    public static class CrossSiteTransUtility
    {
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
            await FileUtility.MoveFileByContentAsync(site, targetSite, contentInfo);
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
                var sourceImageUrl = await PathUtility.MapPathAsync(site, contentInfo.Get<string>(ContentAttribute.ImageUrl));
                CopyReferenceFiles(targetSite, sourceImageUrl, site);

            }
            else if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl))))
            {
                var sourceImageUrls = Utilities.GetStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl)));

                foreach (string imageUrl in sourceImageUrls)
                {
                    var sourceImageUrl = await PathUtility.MapPathAsync(site, imageUrl);
                    CopyReferenceFiles(targetSite, sourceImageUrl, site);
                }
            }
            if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.FileUrl)))
            {
                //修改附件
                var sourceFileUrl = await PathUtility.MapPathAsync(site, contentInfo.Get<string>(ContentAttribute.FileUrl));
                CopyReferenceFiles(targetSite, sourceFileUrl, site);

            }
            else if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl))))
            {
                var sourceFileUrls = Utilities.GetStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl)));

                foreach (string fileUrl in sourceFileUrls)
                {
                    var sourceFileUrl = await PathUtility.MapPathAsync(site, fileUrl);
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