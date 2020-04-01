using System;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using System.Collections.Generic;
using SiteServer.CMS.Model.Attributes;
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
                    if (siteInfo.Additional.IsWaterMark)
                    {
                        if (siteInfo.Additional.IsImageWaterMark)
                        {
                            if (!string.IsNullOrEmpty(siteInfo.Additional.WaterMarkImagePath))
                            {
                                ImageUtils.AddImageWaterMark(imagePath, PathUtility.MapPath(siteInfo, siteInfo.Additional.WaterMarkImagePath), siteInfo.Additional.WaterMarkPosition, siteInfo.Additional.WaterMarkTransparency, siteInfo.Additional.WaterMarkMinWidth, siteInfo.Additional.WaterMarkMinHeight);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(siteInfo.Additional.WaterMarkFormatString))
                            {
                                var now = DateTime.Now;
                                ImageUtils.AddTextWaterMark(imagePath, string.Format(siteInfo.Additional.WaterMarkFormatString, DateUtils.GetDateString(now), DateUtils.GetTimeString(now)), siteInfo.Additional.WaterMarkFontName, siteInfo.Additional.WaterMarkFontSize, siteInfo.Additional.WaterMarkPosition, siteInfo.Additional.WaterMarkTransparency, siteInfo.Additional.WaterMarkMinWidth, siteInfo.Additional.WaterMarkMinHeight);
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
