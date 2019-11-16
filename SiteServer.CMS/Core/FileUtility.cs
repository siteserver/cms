using System;
using SiteServer.Utils;
using System.Collections.Generic;
using SiteServer.CMS.Context;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Context.Images;
using SiteServer.CMS.Model;

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
                    if (site.IsWaterMark)
                    {
                        if (site.IsImageWaterMark)
                        {
                            if (!string.IsNullOrEmpty(site.WaterMarkImagePath))
                            {
                                ImageUtils.AddImageWaterMark(imagePath, PathUtility.MapPath(site, site.WaterMarkImagePath), site.WaterMarkPosition, site.WaterMarkTransparency, site.WaterMarkMinWidth, site.WaterMarkMinHeight);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(site.WaterMarkFormatString))
                            {
                                var now = DateTime.Now;
                                ImageUtils.AddTextWaterMark(imagePath, string.Format(site.WaterMarkFormatString, DateUtils.GetDateString(now), DateUtils.GetTimeString(now)), site.WaterMarkFontName, site.WaterMarkFontSize, site.WaterMarkPosition, site.WaterMarkTransparency, site.WaterMarkMinWidth, site.WaterMarkMinHeight);
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

        public static void MoveFileByContentInfo(Site sourceSite, Site destSite, Content content)
        {
            if (content == null || sourceSite.Id == destSite.Id) return;

            try
            {
                var fileUrls = new List<string>
                {
                    content.Get<string>(ContentAttribute.ImageUrl),
                    content.Get<string>(ContentAttribute.VideoUrl),
                    content.Get<string>(ContentAttribute.FileUrl)
                };

                foreach (var url in TranslateUtils.StringCollectionToStringList(content.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.ImageUrl))))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in TranslateUtils.StringCollectionToStringList(content.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.VideoUrl))))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in TranslateUtils.StringCollectionToStringList(content.Get<string>(ContentAttribute.GetExtendAttributeName(ContentAttribute.FileUrl))))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in RegexUtils.GetOriginalImageSrcs(content.Get<string>(ContentAttribute.Content)))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in RegexUtils.GetOriginalLinkHrefs(content.Get<string>(ContentAttribute.Content)))
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
