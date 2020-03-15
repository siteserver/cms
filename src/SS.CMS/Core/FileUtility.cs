using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Datory.Utils;
using SS.CMS.Abstractions;
using SS.CMS.Core.Images;

namespace SS.CMS.Core
{
    public static class FileUtility
    {
        public static async Task AddWaterMarkAsync(IPathManager pathManager, Site site, string imagePath)
        {
            try
            {
                var fileExtName = PathUtils.GetExtension(imagePath);
                if (FileUtils.IsImage(fileExtName))
                {
                    if (site.IsWaterMark)
                    {
                        if (site.IsImageWaterMark)
                        {
                            if (!string.IsNullOrEmpty(site.WaterMarkImagePath))
                            {
                                ImageUtils.AddImageWaterMark(imagePath, await pathManager.MapPathAsync(site, site.WaterMarkImagePath), site.WaterMarkPosition, site.WaterMarkTransparency, site.WaterMarkMinWidth, site.WaterMarkMinHeight);
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

        public static async Task MoveFileAsync(IPathManager pathManager, Site sourceSite, Site destSite, string relatedUrl)
        {
            if (!string.IsNullOrEmpty(relatedUrl))
            {
                var sourceFilePath = await pathManager.MapPathAsync(sourceSite, relatedUrl);
                var descFilePath = await pathManager.MapPathAsync(destSite, relatedUrl);
                if (FileUtils.IsFileExists(sourceFilePath))
                {
                    FileUtils.MoveFile(sourceFilePath, descFilePath, false);
                }
            }
        }

        public static async Task MoveFileByContentAsync(IPathManager pathManager, Site sourceSite, Site destSite, Content content)
        {
            if (content == null || sourceSite.Id == destSite.Id) return;

            try
            {
                var fileUrls = new List<string>
                {
                    content.ImageUrl,
                    content.VideoUrl,
                    content.FileUrl
                };

                var countName = ColumnsManager.GetCountName(nameof(Content.ImageUrl));
                var count = content.Get<int>(countName);
                for (var i = 1; i <= count; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(nameof(Content.ImageUrl), i);
                    var extend = content.Get<string>(extendName);
                    if (!fileUrls.Contains(extend))
                    {
                        fileUrls.Add(extend);
                    }
                }

                countName = ColumnsManager.GetCountName(nameof(Content.VideoUrl));
                count = content.Get<int>(countName);
                for (var i = 1; i <= count; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(nameof(Content.VideoUrl), i);
                    var extend = content.Get<string>(extendName);
                    if (!fileUrls.Contains(extend))
                    {
                        fileUrls.Add(extend);
                    }
                }

                countName = ColumnsManager.GetCountName(nameof(Content.FileUrl));
                count = content.Get<int>(countName);
                for (var i = 1; i <= count; i++)
                {
                    var extendName = ColumnsManager.GetExtendName(nameof(Content.FileUrl), i);
                    var extend = content.Get<string>(extendName);
                    if (!fileUrls.Contains(extend))
                    {
                        fileUrls.Add(extend);
                    }
                }

                foreach (var url in RegexUtils.GetOriginalImageSrcs(content.Body))
                {
                    if (!fileUrls.Contains(url))
                    {
                        fileUrls.Add(url);
                    }
                }
                foreach (var url in RegexUtils.GetOriginalLinkHrefs(content.Body))
                {
                    if (!fileUrls.Contains(url) && pathManager.IsVirtualUrl(url))
                    {
                        fileUrls.Add(url);
                    }
                }

                foreach (var fileUrl in fileUrls)
                {
                    if (!string.IsNullOrEmpty(fileUrl) && pathManager.IsVirtualUrl(fileUrl))
                    {
                        await MoveFileAsync(pathManager, sourceSite, destSite, fileUrl);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public static async Task MoveFileByVirtaulUrlAsync(IPathManager pathManager, Site sourceSite, Site destSite, string fileVirtaulUrl)
        {
            if (string.IsNullOrEmpty(fileVirtaulUrl) || sourceSite.Id == destSite.Id) return;

            try
            {
                if (pathManager.IsVirtualUrl(fileVirtaulUrl))
                {
                    await MoveFileAsync(pathManager, sourceSite, destSite, fileVirtaulUrl);
                }
            }
            catch
            {
                // ignored
            }
        }

        public static FileSystemInfoExtendCollection GetFileSystemInfoExtendCollection(string rootPath)
        {
            FileSystemInfoExtendCollection retval = null;
            if (Directory.Exists(rootPath))
            {
                var currentRoot = new DirectoryInfo(rootPath);
                var files = currentRoot.GetFileSystemInfos();
                retval = new FileSystemInfoExtendCollection(files);
            }

            return retval;
        }
    }
}
