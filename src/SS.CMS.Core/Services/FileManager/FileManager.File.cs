using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using SS.CMS.Core.Common;
using SS.CMS.Core.Models;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Services
{
    public partial class FileManager
    {
        //将编辑器中图片上传至本机
        public string SaveImage(SiteInfo siteInfo, string content)
        {
            var originalImageSrcs = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var originalImageSrc in originalImageSrcs)
            {
                if (!PageUtils.IsProtocolUrl(originalImageSrc) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, Constants.ApplicationPath) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, _urlManager.GetWebUrl(siteInfo)))
                    continue;
                var fileExtName = PageUtils.GetExtensionFromUrl(originalImageSrc);
                if (!EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName)) continue;

                var fileName = _pathManager.GetUploadFileName(siteInfo, originalImageSrc);
                var directoryPath = _pathManager.GetUploadDirectoryPath(siteInfo, fileExtName);
                var filePath = PathUtils.Combine(directoryPath, fileName);

                try
                {
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        HttpClientUtils.SaveRemoteFileToLocal(originalImageSrc, filePath);
                        if (EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                        {
                            AddWaterMark(siteInfo, filePath);
                        }
                    }
                    var fileUrl = _urlManager.GetSiteUrlByPhysicalPath(siteInfo, filePath, true);
                    content = content.Replace(originalImageSrc, fileUrl);
                }
                catch
                {
                    // ignored
                }
            }
            return content;
        }

        public void AddWaterMark(SiteInfo siteInfo, string imagePath)
        {
            try
            {
                var fileExtName = PathUtils.GetExtension(imagePath);
                if (EFileSystemTypeUtils.IsImage(fileExtName))
                {
                    if (siteInfo.IsWaterMark)
                    {
                        if (siteInfo.IsImageWaterMark)
                        {
                            if (!string.IsNullOrEmpty(siteInfo.WaterMarkImagePath))
                            {
                                ImageUtils.AddImageWaterMark(imagePath, _pathManager.MapPath(siteInfo, siteInfo.WaterMarkImagePath), siteInfo.WaterMarkPosition, siteInfo.WaterMarkTransparency, siteInfo.WaterMarkMinWidth, siteInfo.WaterMarkMinHeight);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(siteInfo.WaterMarkFormatString))
                            {
                                var now = DateTime.Now;
                                ImageUtils.AddTextWaterMark(imagePath, string.Format(siteInfo.WaterMarkFormatString, DateUtils.GetDateString(now), DateUtils.GetTimeString(now)), siteInfo.WaterMarkFontName, siteInfo.WaterMarkFontSize, siteInfo.WaterMarkPosition, siteInfo.WaterMarkTransparency, siteInfo.WaterMarkMinWidth, siteInfo.WaterMarkMinHeight);
                            }
                        }
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public void MoveFile(SiteInfo sourceSiteInfo, SiteInfo destSiteInfo, string relatedUrl)
        {
            if (!string.IsNullOrEmpty(relatedUrl))
            {
                var sourceFilePath = _pathManager.MapPath(sourceSiteInfo, relatedUrl);
                var descFilePath = _pathManager.MapPath(destSiteInfo, relatedUrl);
                if (FileUtils.IsFileExists(sourceFilePath))
                {
                    FileUtils.MoveFile(sourceFilePath, descFilePath, false);
                }
            }
        }

        public void MoveFileByContentInfo(SiteInfo sourceSiteInfo, SiteInfo destSiteInfo, ContentInfo contentInfo)
        {
            if (contentInfo == null || sourceSiteInfo.Id == destSiteInfo.Id) return;

            try
            {
                var fileUrls = new List<string>
                {
                    contentInfo.ImageUrl,
                    contentInfo.VideoUrl,
                    contentInfo.FileUrl
                };

                foreach (var url in TranslateUtils.StringCollectionToStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl))))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in TranslateUtils.StringCollectionToStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.VideoUrl))))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in TranslateUtils.StringCollectionToStringList(contentInfo.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl))))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in RegexUtils.GetOriginalImageSrcs(contentInfo.Get<string>(ContentAttribute.Content)))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in RegexUtils.GetOriginalLinkHrefs(contentInfo.Get<string>(ContentAttribute.Content)))
                {
                    if (!fileUrls.Contains(url) && PageUtils.IsVirtualUrl(url))
                    {
                        fileUrls.Add(url);
                    }
                }

                foreach (var fileUrl in fileUrls)
                {
                    if (!string.IsNullOrEmpty(fileUrl) && _urlManager.IsVirtualUrl(fileUrl))
                    {
                        MoveFile(sourceSiteInfo, destSiteInfo, fileUrl);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public void MoveFileByVirtualUrl(SiteInfo sourceSiteInfo, SiteInfo destSiteInfo, string fileVirtualUrl)
        {
            if (string.IsNullOrEmpty(fileVirtualUrl) || sourceSiteInfo.Id == destSiteInfo.Id) return;

            try
            {
                if (_urlManager.IsVirtualUrl(fileVirtualUrl))
                {
                    MoveFile(sourceSiteInfo, destSiteInfo, fileVirtualUrl);
                }
            }
            catch
            {
                // ignored
            }
        }

        public void MoveFiles(int sourceSiteId, int targetSiteId, List<string> relatedUrls)
        {
            if (sourceSiteId == targetSiteId) return;

            var siteInfo = _siteRepository.GetSiteInfo(sourceSiteId);
            var targetSiteInfo = _siteRepository.GetSiteInfo(targetSiteId);
            if (siteInfo == null || targetSiteInfo == null) return;

            foreach (var relatedUrl in relatedUrls)
            {
                if (!string.IsNullOrEmpty(relatedUrl) && !PageUtils.IsProtocolUrl(relatedUrl))
                {
                    MoveFile(siteInfo, targetSiteInfo, relatedUrl);
                }
            }
        }

        public void AddWaterMark(int siteId, string filePath)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            AddWaterMark(siteInfo, filePath);
        }
    }
}
