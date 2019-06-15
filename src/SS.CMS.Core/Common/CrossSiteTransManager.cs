using System.Text;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Utils;

namespace SS.CMS.Core.Common
{
    public class CrossSiteTransManager
    {
        private IPathManager _pathManager;
        private IFileManager _fileManager;
        private ISiteRepository _siteRepository;

        public CrossSiteTransManager(IPathManager pathManager, IFileManager fileManager, ISiteRepository siteRepository)
        {
            _pathManager = pathManager;
            _fileManager = fileManager;
            _siteRepository = siteRepository;
        }

        public bool IsCrossSiteTrans(SiteInfo siteInfo, ChannelInfo channelInfo)
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
                            var parentSiteId = _siteRepository.GetParentSiteId(siteInfo.Id);
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
                                var theSiteInfo = _siteRepository.GetSiteInfo(channelInfo.TransSiteId);
                                if (theSiteInfo != null)
                                {
                                    isCrossSiteTrans = true;
                                }
                            }
                        }
                        else if (transType == ECrossSiteTransType.ParentSite)
                        {
                            var parentSiteId = _siteRepository.GetParentSiteId(siteInfo.Id);
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
                    isAutomatic = channelInfo.TransIsAutomatic;
                }
            }

            return isAutomatic;
        }

        public string GetDescription(int siteId, ChannelInfo channelInfo)
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
                        siteInfo = _siteRepository.GetSiteInfo(siteId);
                    }
                    else if (transType == ECrossSiteTransType.SpecifiedSite)
                    {
                        siteInfo = _siteRepository.GetSiteInfo(channelInfo.TransSiteId);
                    }
                    else
                    {
                        var parentSiteId = _siteRepository.GetParentSiteId(siteId);
                        if (parentSiteId != 0)
                        {
                            siteInfo = _siteRepository.GetSiteInfo(parentSiteId);
                        }
                    }

                    if (siteInfo != null && !string.IsNullOrEmpty(channelInfo.TransChannelIds))
                    {
                        var nodeNameBuilder = new StringBuilder();
                        var channelIdArrayList = TranslateUtils.StringCollectionToIntList(channelInfo.TransChannelIds);
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

        public void TransContentInfo(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId, SiteInfo targetSiteInfo, int targetChannelId)
        {
            var targetChannelInfo = ChannelManager.GetChannelInfo(targetSiteInfo.Id, targetChannelId);

            var contentInfo = channelInfo.ContentRepository.GetContentInfo(siteInfo, channelInfo, contentId);
            _fileManager.MoveFileByContentInfo(siteInfo, targetSiteInfo, contentInfo);
            contentInfo.SiteId = targetSiteInfo.Id;
            contentInfo.SourceId = channelInfo.Id;
            contentInfo.ChannelId = targetChannelId;
            contentInfo.Checked = targetSiteInfo.IsCrossSiteTransChecked;
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
                targetChannelInfo.ContentRepository.Insert(targetSiteInfo, targetChannelInfo, contentInfo);

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