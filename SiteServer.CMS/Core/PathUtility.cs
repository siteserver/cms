using System;
using System.Collections;
using System.Collections.Specialized;
using SiteServer.Utils;
using SiteServer.CMS.Model;
using System.Text.RegularExpressions;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.Plugin;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
    public static class PathUtility
    {
        public static string GetSitePath(SiteInfo siteInfo)
        {
            return PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, siteInfo.SiteDir);
        }

        public static string GetSitePath(int siteId, params string[] paths)
        {
            return GetSitePath(SiteManager.GetSiteInfo(siteId), paths);
        }

        public static string GetSitePath(SiteInfo siteInfo, params string[] paths)
        {
            var retval = GetSitePath(siteInfo);
            if (paths == null || paths.Length <= 0) return retval;

            foreach (var t in paths)
            {
                var path = t?.Replace(PageUtils.SeparatorChar, PathUtils.SeparatorChar).Trim(PathUtils.SeparatorChar) ?? string.Empty;
                retval = PathUtils.Combine(retval, path);
            }
            return retval;
        }

        public static string GetIndexPageFilePath(SiteInfo siteInfo, string createFileFullName, bool isHeadquarters, int currentPageIndex)
        {
            if (isHeadquarters)
            {
                if (createFileFullName.StartsWith("@"))
                {
                    createFileFullName = "~" + createFileFullName.Substring(1);
                }
                else if (!createFileFullName.StartsWith("~"))
                {
                    createFileFullName = "~" + createFileFullName;
                }
            }
            else
            {
                if (!createFileFullName.StartsWith("~") && !createFileFullName.StartsWith("@"))
                {
                    createFileFullName = "@" + createFileFullName;
                }
            }

            var filePath = MapPath(siteInfo, createFileFullName);

            if (currentPageIndex != 0)
            {
                string appendix = $"_{currentPageIndex + 1}";
                var fileName = PathUtils.GetFileNameWithoutExtension(filePath) + appendix + PathUtils.GetExtension(filePath);
                filePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), fileName);
            }

            return filePath;
        }

        public static string GetBackupFilePath(SiteInfo siteInfo, EBackupType backupType)
        {
            var extention = ".zip";
            var siteName = siteInfo.SiteDir;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteName += "_";
            }
            if (backupType == EBackupType.Templates)
            {
                extention = ".xml";
            }
            return PathUtils.Combine(PathUtils.PhysicalSiteFilesPath, DirectoryUtils.SiteFiles.BackupFiles, siteInfo.SiteDir, DateTime.Now.ToString("yyyy-MM"), EBackupTypeUtils.GetValue(backupType) + "_" + siteName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + extention);
        }

        public static string GetUploadDirectoryPath(SiteInfo siteInfo, string fileExtension)
        {
            return GetUploadDirectoryPath(siteInfo, DateTime.Now, fileExtension);
        }

        public static string GetUploadDirectoryPath(SiteInfo siteInfo, DateTime datetime, string fileExtension)
        {
            var uploadDateFormatString = siteInfo.Additional.FileUploadDateFormatString;
            var uploadDirectoryName = siteInfo.Additional.FileUploadDirectoryName;

            if (IsImageExtenstionAllowed(siteInfo, fileExtension))
            {
                uploadDateFormatString = siteInfo.Additional.ImageUploadDateFormatString;
                uploadDirectoryName = siteInfo.Additional.ImageUploadDirectoryName;
            }
            else if (IsVideoExtenstionAllowed(siteInfo, fileExtension))
            {
                uploadDateFormatString = siteInfo.Additional.VideoUploadDateFormatString;
                uploadDirectoryName = siteInfo.Additional.VideoUploadDirectoryName;
            }

            string directoryPath;
            var dateFormatType = EDateFormatTypeUtils.GetEnumType(uploadDateFormatString);
            var sitePath = GetSitePath(siteInfo);
            if (dateFormatType == EDateFormatType.Year)
            {
                directoryPath = PathUtils.Combine(sitePath, uploadDirectoryName, datetime.Year.ToString());
            }
            else if (dateFormatType == EDateFormatType.Day)
            {
                directoryPath = PathUtils.Combine(sitePath, uploadDirectoryName, datetime.Year.ToString(), datetime.Month.ToString(), datetime.Day.ToString());
            }
            else
            {
                directoryPath = PathUtils.Combine(sitePath, uploadDirectoryName, datetime.Year.ToString(), datetime.Month.ToString());
            }
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            return directoryPath;
        }

        public static string GetUploadDirectoryPath(SiteInfo siteInfo, EUploadType uploadType)
        {
            return GetUploadDirectoryPath(siteInfo, DateTime.Now, uploadType);
        }

        public static string GetUploadDirectoryPath(SiteInfo siteInfo, DateTime datetime, EUploadType uploadType)
        {
            var uploadDateFormatString = string.Empty;
            var uploadDirectoryName = string.Empty;

            if (uploadType == EUploadType.Image)
            {
                uploadDateFormatString = siteInfo.Additional.ImageUploadDateFormatString;
                uploadDirectoryName = siteInfo.Additional.ImageUploadDirectoryName;
            }
            else if (uploadType == EUploadType.Video)
            {
                uploadDateFormatString = siteInfo.Additional.VideoUploadDateFormatString;
                uploadDirectoryName = siteInfo.Additional.VideoUploadDirectoryName;
            }
            else if (uploadType == EUploadType.File)
            {
                uploadDateFormatString = siteInfo.Additional.FileUploadDateFormatString;
                uploadDirectoryName = siteInfo.Additional.FileUploadDirectoryName;
            }
            else if (uploadType == EUploadType.Special)
            {
                uploadDateFormatString = siteInfo.Additional.FileUploadDateFormatString;
                uploadDirectoryName = "/Special";
            }
            else if (uploadType == EUploadType.AdvImage)
            {
                uploadDateFormatString = siteInfo.Additional.FileUploadDateFormatString;
                uploadDirectoryName = "/AdvImage";
            }

            string directoryPath;
            var dateFormatType = EDateFormatTypeUtils.GetEnumType(uploadDateFormatString);
            var sitePath = GetSitePath(siteInfo);
            if (dateFormatType == EDateFormatType.Year)
            {
                directoryPath = PathUtils.Combine(sitePath, uploadDirectoryName, datetime.Year.ToString());
            }
            else if (dateFormatType == EDateFormatType.Day)
            {
                directoryPath = PathUtils.Combine(sitePath, uploadDirectoryName, datetime.Year.ToString(), datetime.Month.ToString(), datetime.Day.ToString());
            }
            else
            {
                directoryPath = PathUtils.Combine(sitePath, uploadDirectoryName, datetime.Year.ToString(), datetime.Month.ToString());
            }
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            return directoryPath;
        }

        public static string GetUploadFileName(SiteInfo siteInfo, string filePath)
        {
            var fileExtension = PathUtils.GetExtension(filePath);

            var isUploadChangeFileName = siteInfo.Additional.IsFileUploadChangeFileName;
            if (IsImageExtenstionAllowed(siteInfo, fileExtension))
            {
                isUploadChangeFileName = siteInfo.Additional.IsImageUploadChangeFileName;
            }
            else if (IsVideoExtenstionAllowed(siteInfo, fileExtension))
            {
                isUploadChangeFileName = siteInfo.Additional.IsVideoUploadChangeFileName;
            }

            return GetUploadFileName(siteInfo, filePath, isUploadChangeFileName);
        }

        public static string GetUploadFileName(SiteInfo siteInfo, string filePath, bool isUploadChangeFileName)
        {
            var retVal = isUploadChangeFileName
                ? $"{StringUtils.GetShortGuid(false)}{PathUtils.GetExtension(filePath)}"
                : PathUtils.GetFileName(filePath);

            retVal = StringUtils.ReplaceIgnoreCase(retVal, "as", string.Empty);
            retVal = StringUtils.ReplaceIgnoreCase(retVal, ";", string.Empty);
            return retVal;
        }

        public static string GetUploadSpecialName(SiteInfo siteInfo, string filePath, bool isUploadChangeFileName)
        {
            var retVal = isUploadChangeFileName ? $"{StringUtils.GetShortGuid(false)}{PathUtils.GetExtension(filePath)}" : PathUtils.GetFileName(filePath);

            retVal = StringUtils.ReplaceIgnoreCase(retVal, "as", string.Empty);
            retVal = StringUtils.ReplaceIgnoreCase(retVal, ";", string.Empty);
            return retVal;
        }

        public static SiteInfo GetSiteInfo(string path)
        {
            var directoryPath = DirectoryUtils.GetDirectoryPath(path).ToLower().Trim(' ', '/', '\\');
            var applicationPath = WebConfigUtils.PhysicalApplicationPath.ToLower().Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty) return null;

            var siteInfoList = SiteManager.GetSiteInfoList();

            SiteInfo headquarter = null;
            foreach (var siteInfo in siteInfoList)
            {
                if (siteInfo.IsRoot)
                {
                    headquarter = siteInfo;
                }
                else
                {
                    if (StringUtils.Contains(directoryDir, siteInfo.SiteDir.ToLower()))
                    {
                        return siteInfo;
                    }
                }
            }

            return headquarter;
        }

        public static string GetSiteDir(string path)
        {
            var siteDir = string.Empty;
            var directoryPath = DirectoryUtils.GetDirectoryPath(path).ToLower().Trim(' ', '/', '\\');
            var applicationPath = WebConfigUtils.PhysicalApplicationPath.ToLower().Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty)
            {
                return string.Empty;
            }

            var siteInfoList = SiteManager.GetSiteInfoList();
            foreach (var siteInfo in siteInfoList)
            {
                if (siteInfo?.IsRoot!= false) continue;

                if (StringUtils.Contains(directoryDir, siteInfo.SiteDir.ToLower()))
                {
                    siteDir = siteInfo.SiteDir;
                }
            }

            return string.IsNullOrWhiteSpace(siteDir) ? siteDir : PathUtils.GetDirectoryName(siteDir, false);
        }

        public static string GetCurrentSiteDir()
        {
            return GetSiteDir(PathUtils.GetCurrentPagePath());
        }

        public static int GetCurrentSiteId()
        {
            int siteId;
            var siteIdList = SiteManager.GetSiteIdList();
            if (siteIdList.Count == 1)
            {
                siteId = siteIdList[0];
            }
            else
            {
                var siteDir = GetCurrentSiteDir();
                siteId = !string.IsNullOrEmpty(siteDir) ? StlSiteCache.GetSiteIdBySiteDir(siteDir) : StlSiteCache.GetSiteIdByIsRoot();

                if (siteId == 0)
                {
                    siteId = StlSiteCache.GetSiteIdByIsRoot();
                }
            }
            return siteId;
        }

        public static string AddVirtualToPath(string path)
        {
            var resolvedPath = path;
            if (!string.IsNullOrEmpty(path))
            {
                path = path.Replace("../", string.Empty);
                if (!path.StartsWith("@") && !path.StartsWith("~"))
                {
                    resolvedPath = "@" + path;
                }
            }
            return resolvedPath;
        }

        public static string MapPath(SiteInfo siteInfo, string virtualPath)
        {
            var resolvedPath = virtualPath;
            if (string.IsNullOrEmpty(virtualPath))
            {
                virtualPath = "@";
            }
            if (!virtualPath.StartsWith("@") && !virtualPath.StartsWith("~"))
            {
                virtualPath = "@" + virtualPath;
            }
            if (!virtualPath.StartsWith("@")) return PathUtils.MapPath(resolvedPath);

            if (siteInfo != null)
            {
                resolvedPath = siteInfo.IsRoot ? string.Concat("~", virtualPath.Substring(1)) : PageUtils.Combine(siteInfo.SiteDir, virtualPath.Substring(1));
            }
            return PathUtils.MapPath(resolvedPath);
        }

        public static string MapPath(SiteInfo siteInfo, string virtualPath, bool isCopyToSite)
        {
            if (!isCopyToSite) return MapPath(siteInfo, virtualPath);

            var resolvedPath = virtualPath;
            if (string.IsNullOrEmpty(virtualPath))
            {
                virtualPath = "@";
            }
            if (!virtualPath.StartsWith("@") && !virtualPath.StartsWith("~"))
            {
                virtualPath = "@" + virtualPath;
            }
            if (!virtualPath.StartsWith("@")) return PathUtils.MapPath(resolvedPath);

            if (siteInfo != null)
            {
                resolvedPath = siteInfo.IsRoot ? string.Concat("~", virtualPath.Substring(1)) : PageUtils.Combine(siteInfo.SiteDir, virtualPath.Substring(1));
            }
            return PathUtils.MapPath(resolvedPath);
        }

        public static string MapPath(string directoryPath, string virtualPath)
        {
            var resolvedPath = virtualPath;
            if (string.IsNullOrEmpty(virtualPath))
            {
                virtualPath = "@";
            }
            if (!virtualPath.StartsWith("@") && !virtualPath.StartsWith("~"))
            {
                virtualPath = "@" + virtualPath;
            }
            if (virtualPath.StartsWith("@"))
            {
                if (string.IsNullOrEmpty(directoryPath))
                {
                    resolvedPath = string.Concat("~", virtualPath.Substring(1));
                }
                else
                {
                    return PageUtils.Combine(directoryPath, virtualPath.Substring(1));
                }
            }
            return PathUtils.MapPath(resolvedPath);
        }

        //将编辑器中图片上传至本机
        public static string SaveImage(SiteInfo siteInfo, string content)
        {
            var originalImageSrcs = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var originalImageSrc in originalImageSrcs)
            {
                if (!PageUtils.IsProtocolUrl(originalImageSrc) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, PageUtils.ApplicationPath) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, siteInfo.Additional.WebUrl))
                    continue;
                var fileExtName = PageUtils.GetExtensionFromUrl(originalImageSrc);
                if (!EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName)) continue;

                var fileName = GetUploadFileName(siteInfo, originalImageSrc);
                var directoryPath = GetUploadDirectoryPath(siteInfo, fileExtName);
                var filePath = PathUtils.Combine(directoryPath, fileName);

                try
                {
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        WebClientUtils.SaveRemoteFileToLocal(originalImageSrc, filePath);
                        if (EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                        {
                            FileUtility.AddWaterMark(siteInfo, filePath);
                        }
                    }
                    var fileUrl = PageUtility.GetSiteUrlByPhysicalPath(siteInfo, filePath, true);
                    content = content.Replace(originalImageSrc, fileUrl);
                }
                catch
                {
                    // ignored
                }
            }
            return content;
        }

        public static string GetTemporaryFilesPath(string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedPath);
        }

        public static string GetSiteTemplatesPath(string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteTemplates.DirectoryName, relatedPath);
        }

        public static string GetSiteTemplateMetadataPath(string siteTemplatePath, string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteTemplateMetadata, relatedPath);
        }

        public static bool IsSystemFile(string fileName)
        {
            if (StringUtils.EqualsIgnoreCase(fileName, "Web.config")
                || StringUtils.EqualsIgnoreCase(fileName, "Global.asax")
                || StringUtils.EqualsIgnoreCase(fileName, "robots.txt"))
            {
                return true;
            }
            return false;
        }


        public static bool IsSystemFileForChangeSiteType(string fileName)
        {
            if (StringUtils.EqualsIgnoreCase(fileName, "Web.config")
                || StringUtils.EqualsIgnoreCase(fileName, "Global.asax")
                || StringUtils.EqualsIgnoreCase(fileName, "robots.txt")
                || StringUtils.EqualsIgnoreCase(fileName, "packages.config")
                || StringUtils.EqualsIgnoreCase(fileName, "version.txt"))
            {
                return true;
            }
            return false;
        }


        public static bool IsWebSiteFile(string fileName)
        {
            if (StringUtils.EqualsIgnoreCase(fileName, "T_系统首页模板.html")
               || StringUtils.EqualsIgnoreCase(fileName, "index.html"))
            {
                return true;
            }
            return false;
        }

        public class ChannelFilePathRules
        {
            private ChannelFilePathRules()
            {
            }

            private const string ChannelId = "{@channelId}";
            private const string Year = "{@year}";
            private const string Month = "{@month}";
            private const string Day = "{@day}";
            private const string Hour = "{@hour}";
            private const string Minute = "{@minute}";
            private const string Second = "{@second}";
            private const string Sequence = "{@sequence}";
            private const string ParentRule = "{@parentRule}";
            private const string ChannelName = "{@channelName}";
            private const string LowerChannelName = "{@lowerChannelName}";
            private const string ChannelIndex = "{@channelIndex}";
            private const string LowerChannelIndex = "{@lowerChannelIndex}";

            public static string DefaultRule = "/channels/{@channelId}.html";
            public static string DefaultDirectoryName = "/channels/";
            public static string DefaultRegexString = "/channels/(?<channelId>[^_]*)_?(?<pageIndex>[^_]*)";

            public static IDictionary GetDictionary(SiteInfo siteInfo, int channelId)
            {
                var dictionary = new ListDictionary
                {
                    {ChannelId, "栏目ID"},
                    {Year, "年份"},
                    {Month, "月份"},
                    {Day, "日期"},
                    {Hour, "小时"},
                    {Minute, "分钟"},
                    {Second, "秒钟"},
                    {Sequence, "顺序数"},
                    {ParentRule, "父级命名规则"},
                    {ChannelName, "栏目名称"},
                    {LowerChannelName, "栏目名称(小写)"},
                    {ChannelIndex, "栏目索引"},
                    {LowerChannelIndex, "栏目索引(小写)"}
                };

                var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                var styleInfoList = TableStyleManager.GetChannelStyleInfoList(channelInfo);
                foreach (var styleInfo in styleInfoList)
                {
                    if (styleInfo.InputType == InputType.Text)
                    {
                        dictionary.Add($@"{{@{StringUtils.LowerFirst(styleInfo.AttributeName)}}}", styleInfo.DisplayName);
                        dictionary.Add($@"{{@lower{styleInfo.AttributeName}}}", styleInfo.DisplayName + "(小写)");
                    }
                }

                return dictionary;
            }

            public static string Parse(SiteInfo siteInfo, int channelId)
            {
                var channelFilePathRule = GetChannelFilePathRule(siteInfo, channelId);
                var filePath = ParseChannelPath(siteInfo, channelId, channelFilePathRule);
                return filePath;
            }

            //递归处理
            private static string ParseChannelPath(SiteInfo siteInfo, int channelId, string channelFilePathRule)
            {
                var filePath = channelFilePathRule.Trim();
                const string regex = "(?<element>{@[^}]+})";
                var elements = RegexUtils.GetContents("element", regex, filePath);
                ChannelInfo nodeInfo = null;

                foreach (var element in elements)
                {
                    var value = string.Empty;
                    
                    if (StringUtils.EqualsIgnoreCase(element, ChannelId))
                    {
                        value = channelId.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Year))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        value = nodeInfo.AddDate.Year.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Month))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        value = nodeInfo.AddDate.Month.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Day))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        value = nodeInfo.AddDate.Day.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Hour))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        value = nodeInfo.AddDate.Hour.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Minute))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        value = nodeInfo.AddDate.Minute.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Second))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        value = nodeInfo.AddDate.Second.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Sequence))
                    {
                        value = StlChannelCache.GetSequence(siteInfo.Id, channelId).ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ParentRule))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        var parentInfo = ChannelManager.GetChannelInfo(siteInfo.Id, nodeInfo.ParentId);
                        if (parentInfo != null)
                        {
                            var parentRule = GetChannelFilePathRule(siteInfo, parentInfo.Id);
                            value = DirectoryUtils.GetDirectoryPath(ParseChannelPath(siteInfo, parentInfo.Id, parentRule)).Replace("\\", "/");
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ChannelName))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        value = nodeInfo.ChannelName;
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, LowerChannelName))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        value = nodeInfo.ChannelName.ToLower();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, LowerChannelIndex))
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        value = nodeInfo.IndexName.ToLower();
                    }
                    else
                    {
                        if (nodeInfo == null) nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        var attributeName = element.Replace("{@", string.Empty).Replace("}", string.Empty);

                        var isLower = false;
                        if (StringUtils.StartsWithIgnoreCase(attributeName, "lower"))
                        {
                            isLower = true;
                            attributeName = attributeName.Substring(5);
                        }

                        value = nodeInfo.Additional.GetString(attributeName);

                        if (isLower)
                        {
                            value = value.ToLower();
                        }
                    }

                    filePath = filePath.Replace(element, value);
                }

                if (!filePath.Contains("//")) return filePath;

                filePath = Regex.Replace(filePath, @"(/)\1{2,}", "/");
                filePath = Regex.Replace(filePath, @"//", "/");
                return filePath;
            }
        }

        public class ContentFilePathRules
        {
            private ContentFilePathRules()
            {
            }

            private const string ChannelId = "{@channelId}";
            private const string ContentId = "{@contentId}";
            private const string Year = "{@year}";
            private const string Month = "{@month}";
            private const string Day = "{@day}";
            private const string Hour = "{@hour}";
            private const string Minute = "{@minute}";
            private const string Second = "{@second}";
            private const string Sequence = "{@sequence}";
            private const string ParentRule = "{@parentRule}";
            private const string ChannelName = "{@channelName}";
            private const string LowerChannelName = "{@lowerChannelName}";
            private const string ChannelIndex = "{@channelIndex}";
            private const string LowerChannelIndex = "{@lowerChannelIndex}";

            public const string DefaultRule = "/contents/{@channelId}/{@contentId}.html";
            public const string DefaultDirectoryName = "/contents/";
            public const string DefaultRegexString = "/contents/(?<channelId>[^/]*)/(?<contentId>[^/]*)_?(?<pageIndex>[^_]*)";

            public static IDictionary GetDictionary(SiteInfo siteInfo, int channelId)
            {
                var dictionary = new ListDictionary
                {
                    {ChannelId, "栏目ID"},
                    {ContentId, "内容ID"},
                    {Year, "年份"},
                    {Month, "月份"},
                    {Day, "日期"},
                    {Hour, "小时"},
                    {Minute, "分钟"},
                    {Second, "秒钟"},
                    {Sequence, "顺序数"},
                    {ParentRule, "父级命名规则"},
                    {ChannelName, "栏目名称"},
                    {LowerChannelName, "栏目名称(小写)"},
                    {ChannelIndex, "栏目索引"},
                    {LowerChannelIndex, "栏目索引(小写)"}
                };

                var channelInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                var styleInfoList = TableStyleManager.GetContentStyleInfoList(siteInfo, channelInfo);
                foreach (var styleInfo in styleInfoList)
                {
                    if (styleInfo.InputType == InputType.Text)
                    {
                        dictionary.Add($@"{{@{StringUtils.LowerFirst(styleInfo.AttributeName)}}}", styleInfo.DisplayName);
                        dictionary.Add($@"{{@lower{styleInfo.AttributeName}}}", styleInfo.DisplayName + "(小写)");
                    }
                }

                return dictionary;
            }

            public static string Parse(SiteInfo siteInfo, int channelId, int contentId)
            {
                var contentFilePathRule = GetContentFilePathRule(siteInfo, channelId);
                var contentInfo = ContentManager.GetContentInfo(siteInfo, channelId, contentId);
                var filePath = ParseContentPath(siteInfo, channelId, contentInfo, contentFilePathRule);
                return filePath;
            }

            public static string Parse(SiteInfo siteInfo, int channelId, IContentInfo contentInfo)
            {
                var contentFilePathRule = GetContentFilePathRule(siteInfo, channelId);
                var filePath = ParseContentPath(siteInfo, channelId, contentInfo, contentFilePathRule);
                return filePath;
            }

            private static string ParseContentPath(SiteInfo siteInfo, int channelId, IContentInfo contentInfo, string contentFilePathRule)
            {
                var filePath = contentFilePathRule.Trim();
                var regex = "(?<element>{@[^}]+})";
                var elements = RegexUtils.GetContents("element", regex, filePath);
                var addDate = contentInfo.AddDate;
                var contentId = contentInfo.Id;
                foreach (var element in elements)
                {
                    var value = string.Empty;

                    if (StringUtils.EqualsIgnoreCase(element, ChannelId))
                    {
                        value = channelId.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ContentId))
                    {
                        value = contentId.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Sequence))
                    {
                        var tableName = ChannelManager.GetTableName(siteInfo, channelId);
                        value = StlContentCache.GetSequence(tableName, channelId, contentId).ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ParentRule))//继承父级设置 20151113 sessionliang
                    {
                        var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        var parentInfo = ChannelManager.GetChannelInfo(siteInfo.Id, nodeInfo.ParentId);
                        if (parentInfo != null)
                        {
                            var parentRule = GetContentFilePathRule(siteInfo, parentInfo.Id);
                            value = DirectoryUtils.GetDirectoryPath(ParseContentPath(siteInfo, parentInfo.Id, contentInfo, parentRule)).Replace("\\", "/");
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ChannelName))
                    {
                        var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        if (nodeInfo != null)
                        {
                            value = nodeInfo.ChannelName;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, LowerChannelName))
                    {
                        var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        if (nodeInfo != null)
                        {
                            value = nodeInfo.ChannelName.ToLower();
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ChannelIndex))
                    {
                        var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        if (nodeInfo != null)
                        {
                            value = nodeInfo.IndexName;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, LowerChannelIndex))
                    {
                        var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
                        if (nodeInfo != null)
                        {
                            value = nodeInfo.IndexName.ToLower();
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Year) || StringUtils.EqualsIgnoreCase(element, Month) || StringUtils.EqualsIgnoreCase(element, Day) || StringUtils.EqualsIgnoreCase(element, Hour) || StringUtils.EqualsIgnoreCase(element, Minute) || StringUtils.EqualsIgnoreCase(element, Second))
                    {
                        if (StringUtils.EqualsIgnoreCase(element, Year))
                        {
                            value = addDate.Year.ToString();
                        }
                        else if (StringUtils.EqualsIgnoreCase(element, Month))
                        {
                            value = addDate.Month.ToString("D2");
                            //value = addDate.ToString("MM");
                        }
                        else if (StringUtils.EqualsIgnoreCase(element, Day))
                        {
                            value = addDate.Day.ToString("D2");
                            //value = addDate.ToString("dd");
                        }
                        else if (StringUtils.EqualsIgnoreCase(element, Hour))
                        {
                            value = addDate.Hour.ToString();
                        }
                        else if (StringUtils.EqualsIgnoreCase(element, Minute))
                        {
                            value = addDate.Minute.ToString();
                        }
                        else if (StringUtils.EqualsIgnoreCase(element, Second))
                        {
                            value = addDate.Second.ToString();
                        }
                    }
                    else
                    {
                        var attributeName = element.Replace("{@", string.Empty).Replace("}", string.Empty);

                        var isLower = false;
                        if (StringUtils.StartsWithIgnoreCase(attributeName, "lower"))
                        {
                            isLower = true;
                            attributeName = attributeName.Substring(5);
                        }

                        value = contentInfo.GetString(attributeName);
                        if (isLower)
                        {
                            value = value.ToLower();
                        }
                    }

                    value = StringUtils.HtmlDecode(value);

                    filePath = filePath.Replace(element, value);
                }

                if (filePath.Contains("//"))
                {
                    filePath = Regex.Replace(filePath, @"(/)\1{2,}", "/");
                    filePath = filePath.Replace("//", "/");
                }

                if (filePath.Contains("("))
                {
                    regex = @"(?<element>\([^\)]+\))";
                    elements = RegexUtils.GetContents("element", regex, filePath);
                    foreach (var element in elements)
                    {
                        if (!element.Contains("|")) continue;

                        var value = element.Replace("(", string.Empty).Replace(")", string.Empty);
                        var value1 = value.Split('|')[0];
                        var value2 = value.Split('|')[1];
                        value = value1 + value2;

                        if (!string.IsNullOrEmpty(value1) && !string.IsNullOrEmpty(value1))
                        {
                            value = value1;
                        }

                        filePath = filePath.Replace(element, value);
                    }
                }
                return filePath;
            }
        }

        public static string GetChannelFilePathRule(SiteInfo siteInfo, int channelId)
        {
            var channelFilePathRule = GetChannelFilePathRule(siteInfo.Id, channelId);
            if (string.IsNullOrEmpty(channelFilePathRule))
            {
                channelFilePathRule = siteInfo.Additional.ChannelFilePathRule;

                if (string.IsNullOrEmpty(channelFilePathRule))
                {
                    channelFilePathRule = ChannelFilePathRules.DefaultRule;
                }
            }
            return channelFilePathRule;
        }

        private static string GetChannelFilePathRule(int siteId, int channelId)
        {
            if (channelId == 0) return string.Empty;
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ChannelFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = GetChannelFilePathRule(siteId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public static string GetContentFilePathRule(SiteInfo siteInfo, int channelId)
        {
            var contentFilePathRule = GetContentFilePathRule(siteInfo.Id, channelId);
            if (string.IsNullOrEmpty(contentFilePathRule))
            {
                contentFilePathRule = siteInfo.Additional.ContentFilePathRule;

                if (string.IsNullOrEmpty(contentFilePathRule))
                {
                    contentFilePathRule = ContentFilePathRules.DefaultRule;
                }
            }
            return contentFilePathRule;
        }

        private static string GetContentFilePathRule(int siteId, int channelId)
        {
            if (channelId == 0) return string.Empty;
            var nodeInfo = ChannelManager.GetChannelInfo(siteId, channelId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ContentFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = GetContentFilePathRule(siteId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public static string GetChannelPageFilePath(SiteInfo siteInfo, int channelId, int currentPageIndex)
        {
            var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
            if (nodeInfo.ParentId == 0)
            {
                var templateInfo = TemplateManager.GetDefaultTemplateInfo(siteInfo.Id, TemplateType.IndexPageTemplate);
                return GetIndexPageFilePath(siteInfo, templateInfo.CreatedFileFullName, siteInfo.IsRoot, currentPageIndex);
            }
            var filePath = nodeInfo.FilePath;

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = ChannelFilePathRules.Parse(siteInfo, channelId);
            }

            filePath = MapPath(siteInfo, filePath);// PathUtils.Combine(sitePath, filePath);
            if (PathUtils.IsDirectoryPath(filePath))
            {
                filePath = PathUtils.Combine(filePath, channelId + ".html");
            }
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            if (currentPageIndex != 0)
            {
                string appendix = $"_{(currentPageIndex + 1)}";
                var fileName = PathUtils.GetFileNameWithoutExtension(filePath) + appendix + PathUtils.GetExtension(filePath);
                filePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), fileName);
            }

            return filePath;
        }

        public static string GetContentPageFilePath(SiteInfo siteInfo, int channelId, int contentId, int currentPageIndex)
        {
            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelId, contentId);
            return GetContentPageFilePath(siteInfo, channelId, contentInfo, currentPageIndex);
        }

        public static string GetContentPageFilePath(SiteInfo siteInfo, int channelId, ContentInfo contentInfo, int currentPageIndex)
        {
            var filePath = ContentFilePathRules.Parse(siteInfo, channelId, contentInfo);

            filePath = MapPath(siteInfo, filePath);
            if (PathUtils.IsDirectoryPath(filePath))
            {
                filePath = PathUtils.Combine(filePath, contentInfo.Id + ".html");
            }
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            if (currentPageIndex == 0) return filePath;

            string appendix = $"_{currentPageIndex + 1}";
            var fileName = PathUtils.GetFileNameWithoutExtension(filePath) + appendix + PathUtils.GetExtension(filePath);
            filePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), fileName);

            return filePath;
        }

        public static bool IsImageExtenstionAllowed(SiteInfo siteInfo, string fileExtention)
        {
            return PathUtils.IsFileExtenstionAllowed(siteInfo.Additional.ImageUploadTypeCollection, fileExtention);
        }

        public static bool IsImageSizeAllowed(SiteInfo siteInfo, int contentLength)
        {
            return contentLength <= siteInfo.Additional.ImageUploadTypeMaxSize * 1024;
        }

        public static bool IsVideoExtenstionAllowed(SiteInfo siteInfo, string fileExtention)
        {
            return PathUtils.IsFileExtenstionAllowed(siteInfo.Additional.VideoUploadTypeCollection, fileExtention);
        }

        public static bool IsVideoSizeAllowed(SiteInfo siteInfo, int contentLength)
        {
            return contentLength <= siteInfo.Additional.VideoUploadTypeMaxSize * 1024;
        }

        public static bool IsFileExtenstionAllowed(SiteInfo siteInfo, string fileExtention)
        {
            var typeCollection = siteInfo.Additional.FileUploadTypeCollection + "," + siteInfo.Additional.ImageUploadTypeCollection + "," + siteInfo.Additional.VideoUploadTypeCollection;
            return PathUtils.IsFileExtenstionAllowed(typeCollection, fileExtention);
        }

        public static bool IsFileSizeAllowed(SiteInfo siteInfo, int contentLength)
        {
            return contentLength <= siteInfo.Additional.FileUploadTypeMaxSize * 1024;
        }

        public static bool IsUploadExtenstionAllowed(EUploadType uploadType, SiteInfo siteInfo, string fileExtention)
        {
            if (uploadType == EUploadType.Image)
            {
                return IsImageExtenstionAllowed(siteInfo, fileExtention);
            }
            else if (uploadType == EUploadType.Video)
            {
                return IsVideoExtenstionAllowed(siteInfo, fileExtention);
            }
            else if (uploadType == EUploadType.File)
            {
                return IsFileExtenstionAllowed(siteInfo, fileExtention);
            }
            return false;
        }

        public static bool IsUploadSizeAllowed(EUploadType uploadType, SiteInfo siteInfo, int contentLength)
        {
            switch (uploadType)
            {
                case EUploadType.Image:
                    return IsImageSizeAllowed(siteInfo, contentLength);
                case EUploadType.Video:
                    return IsVideoSizeAllowed(siteInfo, contentLength);
                case EUploadType.File:
                    return IsFileSizeAllowed(siteInfo, contentLength);
            }
            return false;
        }
    }
}
