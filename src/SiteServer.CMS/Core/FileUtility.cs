using System;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using System.Collections.Generic;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Utils.Enumerations;
using SiteServer.Utils.Images;
using System.Net;

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

        public static string GetRemoteFileSource(string url, ECharset charset, string cookieString)
        {
            try
            {
                string retval;
                var uri = new Uri(PageUtilsEx.AddProtocolToUrl(url.Trim()));
                var hwReq = (HttpWebRequest)WebRequest.Create(uri);
                if (!string.IsNullOrEmpty(cookieString))
                {
                    hwReq.Headers.Add("Cookie", cookieString);
                }
                var hwRes = (HttpWebResponse)hwReq.GetResponse();
                hwReq.Method = "Get";
                //hwReq.ContentType = "text/html";
                hwReq.KeepAlive = false;

                using (var reader = new System.IO.StreamReader(hwRes.GetResponseStream(), ECharsetUtils.GetEncoding(charset)))
                {
                    retval = reader.ReadToEnd();
                }

                return retval;
            }
            catch
            {
                throw new Exception($"页面地址“{url}”无法访问！");
            }
        }

        public static string GetRemoteFileSource(string url, ECharset charset)
        {
            return GetRemoteFileSource(url, charset, string.Empty);
        }

        public static bool SaveRemoteFileToLocal(string remoteUrl, string localFileName)
        {
            try
            {
                var myWebClient = new WebClient();
                myWebClient.DownloadFile(remoteUrl, localFileName);
            }
            catch
            {
                throw new Exception($"页面地址“{remoteUrl}”无法访问！");
            }
            return true;
        }
    }
}
