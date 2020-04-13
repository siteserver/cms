using System.Text;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public class CrossSiteTransManager
    {
        private readonly IDatabaseManager _databaseManager;

        public CrossSiteTransManager(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        public async Task<string> GetDescriptionAsync(int siteId, Channel channel)
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
                        site = await _databaseManager.SiteRepository.GetAsync(siteId);
                    }
                    else if (channel.TransType == TransType.SpecifiedSite)
                    {
                        site = await _databaseManager.SiteRepository.GetAsync(channel.TransSiteId);
                    }
                    else
                    {
                        var parentSiteId = await _databaseManager.SiteRepository.GetParentSiteIdAsync(siteId);
                        if (parentSiteId != 0)
                        {
                            site = await _databaseManager.SiteRepository.GetAsync(parentSiteId);
                        }
                    }

                    if (site != null && !string.IsNullOrEmpty(channel.TransChannelIds))
                    {
                        var nodeNameBuilder = new StringBuilder();
                        var channelIdArrayList = Utilities.GetIntList(channel.TransChannelIds);
                        foreach (int channelId in channelIdArrayList)
                        {
                            var theNodeInfo = await _databaseManager.ChannelRepository.GetAsync(channelId);
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

        public async Task TransContentInfoAsync(IPathManager pathManager, Site site, Channel channel, int contentId, Site targetSite, int targetChannelId)
        {
            var contentInfo = await _databaseManager.ContentRepository.GetAsync(site, channel, contentId);
            await pathManager.MoveFileByContentAsync(site, targetSite, contentInfo);
            contentInfo.SiteId = targetSite.Id;
            contentInfo.SourceId = channel.Id;
            contentInfo.ChannelId = targetChannelId;
            contentInfo.Checked = targetSite.IsCrossSiteTransChecked;
            contentInfo.CheckedLevel = 0;

            //复制
            if (Equals(channel.TransDoneType, TranslateContentType.Copy))
            {
                contentInfo.Set(ColumnsManager.TranslateContentType, TranslateContentType.Copy.GetValue());
            }
            //引用地址
            else if (Equals(channel.TransDoneType, TranslateContentType.Reference))
            {
                contentInfo.SiteId = targetSite.Id;
                contentInfo.SourceId = channel.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ColumnsManager.TranslateContentType, TranslateContentType.Reference.GetValue());
            }
            //引用内容
            else if (Equals(channel.TransDoneType, TranslateContentType.ReferenceContent))
            {
                contentInfo.SiteId = targetSite.Id;
                contentInfo.SourceId = channel.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ColumnsManager.TranslateContentType, TranslateContentType.ReferenceContent.GetValue());
            }

            await _databaseManager.ContentRepository.InsertAsync(targetSite, channel, contentInfo);

            #region 复制资源
            //资源：图片，文件，视频
            if (!string.IsNullOrEmpty(contentInfo.ImageUrl))
            {
                //修改图片
                var sourceImageUrl = await pathManager.MapPathAsync(site, contentInfo.ImageUrl);
                CopyReferenceFiles(targetSite, sourceImageUrl, site);

                var countName = ColumnsManager.GetCountName(nameof(Content.ImageUrl));
                var count = contentInfo.Get<int>(countName);
                for (var i = 1; i <= count; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(nameof(Content.ImageUrl), i);
                    var extend = contentInfo.Get<string>(extendName);
                    sourceImageUrl = await pathManager.MapPathAsync(site, extend);
                    CopyReferenceFiles(targetSite, sourceImageUrl, site);
                }
            }
            
            if (!string.IsNullOrEmpty(contentInfo.FileUrl))
            {
                //修改附件
                var sourceFileUrl = await pathManager.MapPathAsync(site, contentInfo.FileUrl);
                CopyReferenceFiles(targetSite, sourceFileUrl, site);

                var countName = ColumnsManager.GetCountName(nameof(Content.FileUrl));
                var count = contentInfo.Get<int>(countName);
                for (var i = 1; i <= count; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(nameof(Content.FileUrl), i);
                    var extend = contentInfo.Get<string>(extendName);
                    sourceFileUrl = await pathManager.MapPathAsync(site, extend);
                    CopyReferenceFiles(targetSite, sourceFileUrl, site);
                }
            }
            #endregion
        }

        private void CopyReferenceFiles(Site targetSite, string sourceUrl, Site sourceSite)
        {
            var targetUrl = StringUtils.ReplaceFirst(sourceSite.SiteDir, sourceUrl, targetSite.SiteDir);
            if (!FileUtils.IsFileExists(targetUrl))
            {
                FileUtils.CopyFile(sourceUrl, targetUrl, true);
            }
        }
    }
}