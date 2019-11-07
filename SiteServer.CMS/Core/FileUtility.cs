using System;
using SiteServer.Utils;
using System.Collections.Generic;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Db;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.Images;

namespace SiteServer.CMS.Core
{
    public static class FileUtility
    {
        public static void AddWaterMark(Site site, string imagePath)
        {
            try
            {
                var fileExtName = PathUtils.GetExtension(imagePath);
                if (EFileSystemTypeUtils.IsImage(fileExtName))
                {
                    if (site.Additional.IsWaterMark)
                    {
                        if (site.Additional.IsImageWaterMark)
                        {
                            if (!string.IsNullOrEmpty(site.Additional.WaterMarkImagePath))
                            {
                                ImageUtils.AddImageWaterMark(imagePath, PathUtility.MapPath(site, site.Additional.WaterMarkImagePath), site.Additional.WaterMarkPosition, site.Additional.WaterMarkTransparency, site.Additional.WaterMarkMinWidth, site.Additional.WaterMarkMinHeight);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(site.Additional.WaterMarkFormatString))
                            {
                                var now = DateTime.Now;
                                ImageUtils.AddTextWaterMark(imagePath, string.Format(site.Additional.WaterMarkFormatString, DateUtils.GetDateString(now), DateUtils.GetTimeString(now)), site.Additional.WaterMarkFontName, site.Additional.WaterMarkFontSize, site.Additional.WaterMarkPosition, site.Additional.WaterMarkTransparency, site.Additional.WaterMarkMinWidth, site.Additional.WaterMarkMinHeight);
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

        public static void MoveFile(Site sourceSite, Site destSite, string relatedUrl)
        {
            if (!string.IsNullOrEmpty(relatedUrl))
            {
                var sourceFilePath = PathUtility.MapPath(sourceSite, relatedUrl);
                var descFilePath = PathUtility.MapPath(destSite, relatedUrl);
                if (FileUtils.IsFileExists(sourceFilePath))
                {
                    FileUtils.MoveFile(sourceFilePath, descFilePath, false);
                }
            }
        }

        public static void MoveFileByContentInfo(Site sourceSite, Site destSite, ContentInfo contentInfo)
        {
            if (contentInfo == null || sourceSite.Id == destSite.Id) return;

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
                        MoveFile(sourceSite, destSite, fileUrl);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public static void MoveFileByVirtaulUrl(Site sourceSite, Site destSite, string fileVirtaulUrl)
        {
            if (string.IsNullOrEmpty(fileVirtaulUrl) || sourceSite.Id == destSite.Id) return;

            try
            {
                if (PageUtility.IsVirtualUrl(fileVirtaulUrl))
                {
                    MoveFile(sourceSite, destSite, fileVirtaulUrl);
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
