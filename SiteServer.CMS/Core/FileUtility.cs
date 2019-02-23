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
                    if (siteInfo.Extend.IsWaterMark)
                    {
                        if (siteInfo.Extend.IsImageWaterMark)
                        {
                            if (!string.IsNullOrEmpty(siteInfo.Extend.WaterMarkImagePath))
                            {
                                ImageUtils.AddImageWaterMark(imagePath, PathUtility.MapPath(siteInfo, siteInfo.Extend.WaterMarkImagePath), siteInfo.Extend.WaterMarkPosition, siteInfo.Extend.WaterMarkTransparency, siteInfo.Extend.WaterMarkMinWidth, siteInfo.Extend.WaterMarkMinHeight);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(siteInfo.Extend.WaterMarkFormatString))
                            {
                                var now = DateTime.Now;
                                ImageUtils.AddTextWaterMark(imagePath, string.Format(siteInfo.Extend.WaterMarkFormatString, DateUtils.GetDateString(now), DateUtils.GetTimeString(now)), siteInfo.Extend.WaterMarkFontName, siteInfo.Extend.WaterMarkFontSize, siteInfo.Extend.WaterMarkPosition, siteInfo.Extend.WaterMarkTransparency, siteInfo.Extend.WaterMarkMinWidth, siteInfo.Extend.WaterMarkMinHeight);
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
                    contentInfo.GetString(BackgroundContentAttribute.ImageUrl),
                    contentInfo.GetString(BackgroundContentAttribute.VideoUrl),
                    contentInfo.GetString(BackgroundContentAttribute.FileUrl)
                };

                foreach (var url in TranslateUtils.StringCollectionToStringList(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl))))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in TranslateUtils.StringCollectionToStringList(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.VideoUrl))))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in TranslateUtils.StringCollectionToStringList(contentInfo.GetString(ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.FileUrl))))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in RegexUtils.GetOriginalImageSrcs(contentInfo.GetString(BackgroundContentAttribute.Content)))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in RegexUtils.GetOriginalLinkHrefs(contentInfo.GetString(BackgroundContentAttribute.Content)))
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
