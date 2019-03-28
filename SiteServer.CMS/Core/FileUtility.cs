using System;
using SiteServer.Utils;
using System.Collections.Generic;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Models;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.Images;

namespace SiteServer.CMS.Core
{
    public static class FileUtility
    {
        public static void AddWaterMark(SiteInfo siteInfo, string imagePath)
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
                                ImageUtils.AddImageWaterMark(imagePath, PathUtility.MapPath(siteInfo, siteInfo.WaterMarkImagePath), siteInfo.WaterMarkPosition, siteInfo.WaterMarkTransparency, siteInfo.WaterMarkMinWidth, siteInfo.WaterMarkMinHeight);
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

        public static void MoveFile(SiteInfo sourceSiteInfo, SiteInfo destSiteInfo, string relatedUrl)
        {
            if (!string.IsNullOrEmpty(relatedUrl))
            {
                var sourceFilePath = PathUtility.MapPath(sourceSiteInfo, relatedUrl);
                var descFilePath = PathUtility.MapPath(destSiteInfo, relatedUrl);
                if (FileUtils.IsFileExists(sourceFilePath))
                {
                    FileUtils.MoveFile(sourceFilePath, descFilePath, false);
                }
            }
        }

        public static void MoveFileByContentInfo(SiteInfo sourceSiteInfo, SiteInfo destSiteInfo, ContentInfo contentInfo)
        {
            if (contentInfo == null || sourceSiteInfo.Id == destSiteInfo.Id) return;

            try
            {
                var fileUrls = new List<string>
                {
                    contentInfo.Get<string>(ContentAttribute.ImageUrl),
                    contentInfo.Get<string>(ContentAttribute.VideoUrl),
                    contentInfo.Get<string>(ContentAttribute.FileUrl)
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
                    if (!string.IsNullOrEmpty(fileUrl) && PageUtility.IsVirtualUrl(fileUrl))
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

        public static void MoveFileByVirtaulUrl(SiteInfo sourceSiteInfo, SiteInfo destSiteInfo, string fileVirtaulUrl)
        {
            if (string.IsNullOrEmpty(fileVirtaulUrl) || sourceSiteInfo.Id == destSiteInfo.Id) return;

            try
            {
                if (PageUtility.IsVirtualUrl(fileVirtaulUrl))
                {
                    MoveFile(sourceSiteInfo, destSiteInfo, fileVirtaulUrl);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
