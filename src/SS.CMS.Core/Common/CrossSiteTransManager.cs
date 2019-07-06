using System.Text;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Common
{
    public class CrossSiteTransManager
    {
        private IPathManager _pathManager;
        private IFileManager _fileManager;
        private ISiteRepository _siteRepository;
        private IChannelRepository _channelRepository;

        public CrossSiteTransManager(IPathManager pathManager, IFileManager fileManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _pathManager = pathManager;
            _fileManager = fileManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        public async Task<bool> IsCrossSiteTransAsync(SiteInfo siteInfo, ChannelInfo channelInfo)
        {
            var isCrossSiteTrans = false;

            if (channelInfo != null)
            {
                var transType = ECrossSiteTransTypeUtils.GetEnumType(channelInfo.TransType);
                if (transType != ECrossSiteTransType.None)
                {
                    if (transType != ECrossSiteTransType.None)
                    {
                        if (transType == ECrossSiteTransType.AllParentSite)
                        {
                            var parentSiteId = await _siteRepository.GetParentSiteIdAsync(siteInfo.Id);
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
                            if (channelInfo.TransSiteId > 0)
                            {
                                var theSiteInfo = await _siteRepository.GetSiteInfoAsync(channelInfo.TransSiteId);
                                if (theSiteInfo != null)
                                {
                                    isCrossSiteTrans = true;
                                }
                            }
                        }
                        else if (transType == ECrossSiteTransType.ParentSite)
                        {
                            var parentSiteId = await _siteRepository.GetParentSiteIdAsync(siteInfo.Id);
                            if (parentSiteId != 0)
                            {
                                isCrossSiteTrans = true;
                            }
                        }
                    }
                }
            }

            return isCrossSiteTrans;
        }

        public bool IsAutomatic(ChannelInfo channelInfo)
        {
            var isAutomatic = false;

            if (channelInfo != null)
            {
                var transType = ECrossSiteTransTypeUtils.GetEnumType(channelInfo.TransType);

                if (transType == ECrossSiteTransType.SelfSite || transType == ECrossSiteTransType.ParentSite || transType == ECrossSiteTransType.SpecifiedSite)
                {
                    isAutomatic = channelInfo.IsTransAutomatic;
                }
            }

            return isAutomatic;
        }

        public async Task<string> GetDescriptionAsync(int siteId, ChannelInfo channelInfo)
        {
            var results = string.Empty;

            if (channelInfo != null)
            {
                var transType = ECrossSiteTransTypeUtils.GetEnumType(channelInfo.TransType);
                results = ECrossSiteTransTypeUtils.GetText(transType);

                if (transType == ECrossSiteTransType.AllParentSite || transType == ECrossSiteTransType.AllSite)
                {
                    if (!string.IsNullOrEmpty(channelInfo.TransChannelNames))
                    {
                        results += $"({channelInfo.TransChannelNames})";
                    }
                }
                else if (transType == ECrossSiteTransType.SelfSite || transType == ECrossSiteTransType.SpecifiedSite || transType == ECrossSiteTransType.ParentSite)
                {
                    SiteInfo siteInfo = null;

                    if (transType == ECrossSiteTransType.SelfSite)
                    {
                        siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
                    }
                    else if (transType == ECrossSiteTransType.SpecifiedSite)
                    {
                        siteInfo = await _siteRepository.GetSiteInfoAsync(channelInfo.TransSiteId);
                    }
                    else
                    {
                        var parentSiteId = await _siteRepository.GetParentSiteIdAsync(siteId);
                        if (parentSiteId != 0)
                        {
                            siteInfo = await _siteRepository.GetSiteInfoAsync(parentSiteId);
                        }
                    }

                    if (siteInfo != null && !string.IsNullOrEmpty(channelInfo.TransChannelIds))
                    {
                        var nodeNameBuilder = new StringBuilder();
                        var channelIdArrayList = TranslateUtils.StringCollectionToIntList(channelInfo.TransChannelIds);
                        foreach (int channelId in channelIdArrayList)
                        {
                            var theNodeInfo = await _channelRepository.GetChannelInfoAsync(channelId);
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

        public async Task TransContentInfoAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId, SiteInfo targetSiteInfo, int targetChannelId)
        {
            var targetChannelInfo = await _channelRepository.GetChannelInfoAsync(targetChannelId);

            var contentInfo = channelInfo.ContentRepository.GetContentInfo(contentId);
            _fileManager.MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);
            contentInfo.SiteId = targetSiteInfo.Id;
            contentInfo.SourceId = channelInfo.Id;
            contentInfo.ChannelId = targetChannelId;
            contentInfo.IsChecked = targetSiteInfo.IsCrossSiteTransChecked;
            contentInfo.CheckedLevel = 0;

            //复制
            if (Equals(channelInfo.TransDoneType, TranslateContentType.Copy))
            {
                contentInfo.Set(ContentAttribute.TranslateContentType, TranslateContentType.Copy.ToString());
            }
            //引用地址
            else if (Equals(channelInfo.TransDoneType, TranslateContentType.Reference))
            {
                contentInfo.SiteId = targetSiteInfo.Id;
                contentInfo.SourceId = channelInfo.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, TranslateContentType.Reference.ToString());
            }
            //引用内容
            else if (Equals(channelInfo.TransDoneType, TranslateContentType.ReferenceContent))
            {
                contentInfo.SiteId = targetSiteInfo.Id;
                contentInfo.SourceId = channelInfo.Id;
                contentInfo.ChannelId = targetChannelId;
                contentInfo.ReferenceId = contentId;
                contentInfo.Set(ContentAttribute.TranslateContentType, TranslateContentType.ReferenceContent.ToString());
            }

            if (targetChannelInfo != null)
            {
                await targetChannelInfo.ContentRepository.InsertAsync(targetSiteInfo, targetChannelInfo, contentInfo);

                #region 复制资源
                //资源：图片，文件，视频
                if (!string.IsNullOrEmpty(contentInfo.ImageUrl))
                {
                    //修改图片
                    var sourceImageUrl = _pathManager.MapPath(siteInfo, contentInfo.ImageUrl);
                    CopyReferenceFiles(targetSiteInfo, sourceImageUrl, siteInfo);
                }
                else if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl))))
                {
                    var sourceImageUrls = TranslateUtils.StringCollectionToStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl)));

                    foreach (string imageUrl in sourceImageUrls)
                    {
                        var sourceImageUrl = _pathManager.MapPath(siteInfo, imageUrl);
                        CopyReferenceFiles(targetSiteInfo, sourceImageUrl, siteInfo);
                    }
                }
                if (!string.IsNullOrEmpty(contentInfo.FileUrl))
                {
                    //修改附件
                    var sourceFileUrl = _pathManager.MapPath(siteInfo, contentInfo.FileUrl);
                    CopyReferenceFiles(targetSiteInfo, sourceFileUrl, siteInfo);

                }
                else if (!string.IsNullOrEmpty(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl))))
                {
                    var sourceFileUrls = TranslateUtils.StringCollectionToStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl)));

                    foreach (string fileUrl in sourceFileUrls)
                    {
                        var sourceFileUrl = _pathManager.MapPath(siteInfo, fileUrl);
                        CopyReferenceFiles(targetSiteInfo, sourceFileUrl, siteInfo);
                    }
                }
                #endregion
            }
        }

        private void CopyReferenceFiles(SiteInfo targetSiteInfo, string sourceUrl, SiteInfo sourceSiteInfo)
        {
            var targetUrl = StringUtils.ReplaceFirst(sourceSiteInfo.SiteDir, sourceUrl, targetSiteInfo.SiteDir);
            if (!FileUtils.IsFileExists(targetUrl))
            {
                FileUtils.CopyFile(sourceUrl, targetUrl, true);
            }
        }
    }
}