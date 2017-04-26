using System;
using System.Collections;
using System.Collections.Specialized;
using BaiRong.Core;
using BaiRong.Core.Net;
using BaiRong.Core.Model;
using SiteServer.CMS.Model;
using BaiRong.Core.AuxiliaryTable;
using System.Text.RegularExpressions;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.CMS.Core
{
    public class PathUtility
    {
        private PathUtility()
        {
        }

        public static string GetPublishmentSystemPath(PublishmentSystemInfo publishmentSystemInfo)
        {
            return PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, publishmentSystemInfo.PublishmentSystemDir);
        }

        public static string GetPublishmentSystemPath(int publishmentSystemId, params string[] paths)
        {
            return GetPublishmentSystemPath(PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId), paths);
        }

        public static string GetPublishmentSystemPath(PublishmentSystemInfo publishmentSystemInfo, params string[] paths)
        {
            var retval = GetPublishmentSystemPath(publishmentSystemInfo);
            if (paths == null || paths.Length <= 0) return retval;

            foreach (var t in paths)
            {
                var path = t?.Replace(PageUtils.SeparatorChar, PathUtils.SeparatorChar).Trim(PathUtils.SeparatorChar) ?? string.Empty;
                retval = PathUtils.Combine(retval, path);
            }
            return retval;
        }

        public static string GetIndexPageFilePath(PublishmentSystemInfo publishmentSystemInfo, string createFileFullName, bool isHeadquarters, int currentPageIndex)
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

            var filePath = MapPath(publishmentSystemInfo, createFileFullName);

            if (currentPageIndex != 0)
            {
                string appendix = $"_{currentPageIndex + 1}";
                var fileName = PathUtils.GetFileNameWithoutExtension(filePath) + appendix + PathUtils.GetExtension(filePath);
                filePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), fileName);
            }

            return filePath;
        }

        public static string GetBackupFilePath(PublishmentSystemInfo publishmentSystemInfo, EBackupType backupType)
        {
            var extention = ".zip";
            var siteName = publishmentSystemInfo.PublishmentSystemDir;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteName += "_";
            }
            if (backupType == EBackupType.Templates)
            {
                extention = ".xml";
            }
            return PathUtils.Combine(PathUtils.PhysicalSiteFilesPath, DirectoryUtils.SiteFiles.BackupFiles, publishmentSystemInfo.PublishmentSystemDir, DateTime.Now.ToString("yyyy-MM"), EBackupTypeUtils.GetValue(backupType) + "_" + siteName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + extention);
        }

        public static string GetUploadDirectoryPath(PublishmentSystemInfo publishmentSystemInfo, string fileExtension)
        {
            return GetUploadDirectoryPath(publishmentSystemInfo, DateTime.Now, fileExtension);
        }

        public static string GetUploadDirectoryPath(PublishmentSystemInfo publishmentSystemInfo, DateTime datetime, string fileExtension)
        {
            var uploadDateFormatString = publishmentSystemInfo.Additional.FileUploadDateFormatString;
            var uploadDirectoryName = publishmentSystemInfo.Additional.FileUploadDirectoryName;

            if (IsImageExtenstionAllowed(publishmentSystemInfo, fileExtension))
            {
                uploadDateFormatString = publishmentSystemInfo.Additional.ImageUploadDateFormatString;
                uploadDirectoryName = publishmentSystemInfo.Additional.ImageUploadDirectoryName;
            }
            else if (IsVideoExtenstionAllowed(publishmentSystemInfo, fileExtension))
            {
                uploadDateFormatString = publishmentSystemInfo.Additional.VideoUploadDateFormatString;
                uploadDirectoryName = publishmentSystemInfo.Additional.VideoUploadDirectoryName;
            }

            string directoryPath;
            var dateFormatType = EDateFormatTypeUtils.GetEnumType(uploadDateFormatString);
            var publishmentSystemPath = GetPublishmentSystemPath(publishmentSystemInfo);
            if (dateFormatType == EDateFormatType.Year)
            {
                directoryPath = PathUtils.Combine(publishmentSystemPath, uploadDirectoryName, datetime.Year.ToString());
            }
            else if (dateFormatType == EDateFormatType.Day)
            {
                directoryPath = PathUtils.Combine(publishmentSystemPath, uploadDirectoryName, datetime.Year.ToString(), datetime.Month.ToString(), datetime.Day.ToString());
            }
            else
            {
                directoryPath = PathUtils.Combine(publishmentSystemPath, uploadDirectoryName, datetime.Year.ToString(), datetime.Month.ToString());
            }
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            return directoryPath;
        }

        public static string GetUploadDirectoryPath(PublishmentSystemInfo publishmentSystemInfo, EUploadType uploadType)
        {
            return GetUploadDirectoryPath(publishmentSystemInfo, DateTime.Now, uploadType);
        }

        public static string GetUploadDirectoryPath(PublishmentSystemInfo publishmentSystemInfo, DateTime datetime, EUploadType uploadType)
        {
            var uploadDateFormatString = string.Empty;
            var uploadDirectoryName = string.Empty;

            if (uploadType == EUploadType.Image)
            {
                uploadDateFormatString = publishmentSystemInfo.Additional.ImageUploadDateFormatString;
                uploadDirectoryName = publishmentSystemInfo.Additional.ImageUploadDirectoryName;
            }
            else if (uploadType == EUploadType.Video)
            {
                uploadDateFormatString = publishmentSystemInfo.Additional.VideoUploadDateFormatString;
                uploadDirectoryName = publishmentSystemInfo.Additional.VideoUploadDirectoryName;
            }
            else if (uploadType == EUploadType.File)
            {
                uploadDateFormatString = publishmentSystemInfo.Additional.FileUploadDateFormatString;
                uploadDirectoryName = publishmentSystemInfo.Additional.FileUploadDirectoryName;
            }
            else if (uploadType == EUploadType.Special)
            {
                uploadDateFormatString = publishmentSystemInfo.Additional.FileUploadDateFormatString;
                uploadDirectoryName = "/Special";
            }
            else if (uploadType == EUploadType.AdvImage)
            {
                uploadDateFormatString = publishmentSystemInfo.Additional.FileUploadDateFormatString;
                uploadDirectoryName = "/AdvImage";
            }

            string directoryPath;
            var dateFormatType = EDateFormatTypeUtils.GetEnumType(uploadDateFormatString);
            var publishmentSystemPath = GetPublishmentSystemPath(publishmentSystemInfo);
            if (dateFormatType == EDateFormatType.Year)
            {
                directoryPath = PathUtils.Combine(publishmentSystemPath, uploadDirectoryName, datetime.Year.ToString());
            }
            else if (dateFormatType == EDateFormatType.Day)
            {
                directoryPath = PathUtils.Combine(publishmentSystemPath, uploadDirectoryName, datetime.Year.ToString(), datetime.Month.ToString(), datetime.Day.ToString());
            }
            else
            {
                directoryPath = PathUtils.Combine(publishmentSystemPath, uploadDirectoryName, datetime.Year.ToString(), datetime.Month.ToString());
            }
            DirectoryUtils.CreateDirectoryIfNotExists(directoryPath);
            return directoryPath;
        }

        public static string GetUploadFileName(PublishmentSystemInfo publishmentSystemInfo, string filePath)
        {
            return GetUploadFileName(publishmentSystemInfo, filePath, DateTime.Now);
        }

        public static string GetUploadFileName(PublishmentSystemInfo publishmentSystemInfo, string filePath, DateTime now)
        {
            var fileExtension = PathUtils.GetExtension(filePath);

            var isUploadChangeFileName = publishmentSystemInfo.Additional.IsFileUploadChangeFileName;
            if (IsImageExtenstionAllowed(publishmentSystemInfo, fileExtension))
            {
                isUploadChangeFileName = publishmentSystemInfo.Additional.IsImageUploadChangeFileName;
            }
            else if (IsVideoExtenstionAllowed(publishmentSystemInfo, fileExtension))
            {
                isUploadChangeFileName = publishmentSystemInfo.Additional.IsVideoUploadChangeFileName;
            }

            return GetUploadFileName(publishmentSystemInfo, filePath, now, isUploadChangeFileName);
        }

        public static string GetUploadFileName(PublishmentSystemInfo publishmentSystemInfo, string filePath, DateTime now, bool isUploadChangeFileName)
        {
            string retval;

            if (isUploadChangeFileName)
            {
                string strDateTime = $"{now.Day}{now.Hour}{now.Minute}{now.Second}{now.Millisecond}";
                retval = $"{strDateTime}{PathUtils.GetExtension(filePath)}";
            }
            else
            {
                retval = PathUtils.GetFileName(filePath);
            }

            retval = StringUtils.ReplaceIgnoreCase(retval, "as", string.Empty);
            retval = StringUtils.ReplaceIgnoreCase(retval, ";", string.Empty);
            return retval;
        }

        public static string GetUploadSpecialName(PublishmentSystemInfo publishmentSystemInfo, string filePath, DateTime now, bool isUploadChangeFileName)
        {
            string retval;

            if (isUploadChangeFileName)
            {
                string strDateTime = $"{now.Day}{now.Hour}{now.Minute}{now.Second}{now.Millisecond}";
                retval = $"{strDateTime}{PathUtils.GetExtension(filePath)}";
            }
            else
            {
                retval = PathUtils.GetFileName(filePath);
            }

            retval = StringUtils.ReplaceIgnoreCase(retval, "as", string.Empty);
            retval = StringUtils.ReplaceIgnoreCase(retval, ";", string.Empty);
            return retval;
        }

        public static string GetUploadAdvImageName(PublishmentSystemInfo publishmentSystemInfo, string filePath, DateTime now, bool isUploadChangeFileName)
        {
            string retval;

            if (isUploadChangeFileName)
            {
                string strDateTime = $"{now.Day}{now.Hour}{now.Minute}{now.Second}{now.Millisecond}";
                retval = $"{strDateTime}{PathUtils.GetExtension(filePath)}";
            }
            else
            {
                retval = PathUtils.GetFileName(filePath);
            }

            retval = StringUtils.ReplaceIgnoreCase(retval, "as", string.Empty);
            retval = StringUtils.ReplaceIgnoreCase(retval, ";", string.Empty);
            return retval;
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

            var pairList = PublishmentSystemManager.GetPublishmentSystemInfoKeyValuePairList();
            foreach (var pair in pairList)
            {
                var publishmentSystemInfo = pair.Value;
                if (publishmentSystemInfo?.IsHeadquarters != false) continue;

                if (StringUtils.Contains(directoryDir, publishmentSystemInfo.PublishmentSystemDir.ToLower()))
                {
                    siteDir = publishmentSystemInfo.PublishmentSystemDir;
                }
            }

            return PathUtils.GetDirectoryName(siteDir);
        }

        public static string GetCurrentSiteDir()
        {
            return GetSiteDir(PathUtils.GetCurrentPagePath());
        }

        public static int GetCurrentPublishmentSystemId()
        {
            int publishmentSystemId;
            var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdList();
            if (publishmentSystemIdList.Count == 1)
            {
                publishmentSystemId = publishmentSystemIdList[0];
            }
            else
            {
                var publishmentSystemDir = GetCurrentSiteDir();
                publishmentSystemId = !string.IsNullOrEmpty(publishmentSystemDir) ? DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByPublishmentSystemDir(publishmentSystemDir) : DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();

                if (publishmentSystemId == 0)
                {
                    publishmentSystemId = DataProvider.PublishmentSystemDao.GetPublishmentSystemIdByIsHeadquarters();
                }
            }
            return publishmentSystemId;
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

        public static string GetAdminDirectoryPath(string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, FileConfigManager.Instance.AdminDirectoryName, relatedPath);
        }

        public static string MapPath(PublishmentSystemInfo publishmentSystemInfo, string virtualPath)
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

            if (publishmentSystemInfo != null)
            {
                resolvedPath = publishmentSystemInfo.IsHeadquarters ? string.Concat("~", virtualPath.Substring(1)) : PageUtils.Combine(publishmentSystemInfo.PublishmentSystemDir, virtualPath.Substring(1));
            }
            return PathUtils.MapPath(resolvedPath);
        }

        public static string MapPath(PublishmentSystemInfo publishmentSystemInfo, string virtualPath, bool isCopyToSite)
        {
            if (!isCopyToSite) return MapPath(publishmentSystemInfo, virtualPath);

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

            if (publishmentSystemInfo != null)
            {
                resolvedPath = publishmentSystemInfo.IsHeadquarters ? string.Concat("~", virtualPath.Substring(1)) : PageUtils.Combine(publishmentSystemInfo.PublishmentSystemDir, virtualPath.Substring(1));
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
        public static string SaveImage(PublishmentSystemInfo publishmentSystemInfo, string content)
        {
            var originalImageSrcs = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var originalImageSrc in originalImageSrcs)
            {
                if (!PageUtils.IsProtocolUrl(originalImageSrc) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, WebConfigUtils.ApplicationPath) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, publishmentSystemInfo.PublishmentSystemUrl))
                    continue;
                var fileExtName = PageUtils.GetExtensionFromUrl(originalImageSrc);
                if (!EFileSystemTypeUtils.IsImageOrFlashOrPlayer(fileExtName)) continue;

                var fileName = GetUploadFileName(publishmentSystemInfo, originalImageSrc);
                var directoryPath = GetUploadDirectoryPath(publishmentSystemInfo, fileExtName);
                var filePath = PathUtils.Combine(directoryPath, fileName);

                try
                {
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        WebClientUtils.SaveRemoteFileToLocal(originalImageSrc, filePath);
                        if (EFileSystemTypeUtils.IsImage(PathUtils.GetExtension(fileName)))
                        {
                            FileUtility.AddWaterMark(publishmentSystemInfo, filePath);
                        }
                    }
                    var fileUrl = PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, filePath);
                    content = content.Replace(originalImageSrc, fileUrl);
                }
                catch
                {
                    // ignored
                }
            }
            return content;
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

        public static string GetCacheFilePath(string cacheFileName)
        {
            cacheFileName = PathUtils.RemoveParentPath(cacheFileName);
            return PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, cacheFileName);
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


        public static bool IsSystemFileForChangePublishmentSystemType(string fileName)
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
            if (StringUtils.EqualsIgnoreCase(fileName, "T_系统首页模板.htm")
               || StringUtils.EqualsIgnoreCase(fileName, "index.htm"))
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

            private const string ChannelId = "{@ChannelID}";
            public const string ChannelIndex = "{@ChannelIndex}";
            private const string Year = "{@Year}";
            private const string Month = "{@Month}";
            private const string Day = "{@Day}";
            private const string Hour = "{@Hour}";
            private const string Minute = "{@Minute}";
            private const string Second = "{@Second}";
            private const string Sequence = "{@Sequence}";

            //继承父级设置 20151113 sessionliang
            private const string ParentRule = "{@ParentRule}";
            private const string ChannelName = "{@ChannelName}";

            public static string DefaultRule = "/channels/{@ChannelID}.html";
            public static string DefaultDirectoryName = "/channels/";
            public static string DefaultRegexString = "/channels/(?<channelID>[^_]*)_?(?<pageIndex>[^_]*)";

            public static IDictionary GetDictionary(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
            {
                var dictionary = new ListDictionary
                {
                    {ChannelId, "栏目ID"},
                    {ChannelIndex, "栏目索引"},
                    {Year, "年份"},
                    {Month, "月份"},
                    {Day, "日期"},
                    {Hour, "小时"},
                    {Minute, "分钟"},
                    {Second, "秒钟"},
                    {Sequence, "顺序数"},
                    {ParentRule, "父级命名规则"},
                    {ChannelName, "栏目名称"}
                };

                //继承父级设置 20151113 sessionliang

                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, nodeId);

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(ETableStyle.Channel, DataProvider.NodeDao.TableName, relatedIdentities);
                foreach (var styleInfo in styleInfoList)
                {
                    if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Text))
                    {
                        dictionary.Add($@"{{@{styleInfo.AttributeName}}}", styleInfo.DisplayName);
                    }
                }

                return dictionary;
            }

            public static string Parse(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
            {
                var channelFilePathRule = GetChannelFilePathRule(publishmentSystemInfo, nodeId);
                var filePath = ParseChannelPath(publishmentSystemInfo, nodeId, channelFilePathRule);
                return filePath;
            }

            //递归处理
            private static string ParseChannelPath(PublishmentSystemInfo publishmentSystemInfo, int nodeId, string channelFilePathRule)
            {

                var filePath = channelFilePathRule.Trim();
                const string regex = "(?<element>{@[^}]+})";
                var elements = RegexUtils.GetContents("element", regex, filePath);
                NodeInfo nodeInfo = null;

                foreach (var element in elements)
                {
                    var value = string.Empty;
                    if (StringUtils.EqualsIgnoreCase(element, ChannelId))
                    {
                        value = nodeId.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ChannelIndex))
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        value = nodeInfo.NodeIndexName;
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Year))
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        value = nodeInfo.AddDate.Year.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Month))
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        value = nodeInfo.AddDate.Month.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Day))
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        value = nodeInfo.AddDate.Day.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Hour))
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        value = nodeInfo.AddDate.Hour.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Minute))
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        value = nodeInfo.AddDate.Minute.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Second))
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        value = nodeInfo.AddDate.Second.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Sequence))
                    {
                        value = DataProvider.NodeDao.GetSequence(publishmentSystemInfo.PublishmentSystemId, nodeId).ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ParentRule))//继承父级设置 20151113 sessionliang
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        var parentInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeInfo.ParentId);
                        if (parentInfo != null)
                        {
                            var parentRule = GetChannelFilePathRule(publishmentSystemInfo, parentInfo.NodeId);
                            value = DirectoryUtils.GetDirectoryPath(ParseChannelPath(publishmentSystemInfo, parentInfo.NodeId, parentRule)).Replace("\\", "/");
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ChannelName))//栏目名称 20151113 sessionliang
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        value = nodeInfo.NodeName;
                    }
                    else
                    {
                        if (nodeInfo == null) nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        var attributeName = element.Replace("{@", string.Empty).Replace("}", string.Empty);
                        value = nodeInfo.Additional.GetExtendedAttribute(attributeName);
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

            private const string ChannelId = "{@ChannelID}";
            private const string ChannelIndex = "{@ChannelIndex}";
            private const string ContentId = "{@ContentID}";
            private const string Year = "{@Year}";
            private const string Month = "{@Month}";
            private const string Day = "{@Day}";
            private const string Hour = "{@Hour}";
            private const string Minute = "{@Minute}";
            private const string Second = "{@Second}";
            private const string Sequence = "{@Sequence}";

            //继承父级设置 20151113 sessionliang
            private const string ParentRule = "{@ParentRule}";
            private const string ChannelName = "{@ChannelName}";

            public const string DefaultRule = "/contents/{@ChannelID}/{@ContentID}.html";
            public const string DefaultDirectoryName = "/contents/";
            public const string DefaultRegexString = "/contents/(?<channelID>[^/]*)/(?<contentID>[^/]*)_?(?<pageIndex>[^_]*)";

            public static IDictionary GetDictionary(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
            {
                var dictionary = new ListDictionary
                {
                    {ChannelId, "栏目ID"},
                    {ChannelIndex, "栏目索引"},
                    {ContentId, "内容ID"},
                    {Year, "年份"},
                    {Month, "月份"},
                    {Day, "日期"},
                    {Hour, "小时"},
                    {Minute, "分钟"},
                    {Second, "秒钟"},
                    {Sequence, "顺序数"},
                    {ParentRule, "父级命名规则"},
                    {ChannelName, "栏目名称"}
                };

                //继承父级设置 20151113 sessionliang

                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeId);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemInfo.PublishmentSystemId, nodeId);

                var styleInfoList = TableStyleManager.GetTableStyleInfoList(tableStyle, tableName, relatedIdentities);
                foreach (var styleInfo in styleInfoList)
                {
                    if (EInputTypeUtils.Equals(styleInfo.InputType, EInputType.Text))
                    {
                        dictionary.Add($@"{{@{styleInfo.AttributeName}}}", styleInfo.DisplayName);
                    }
                }

                return dictionary;
            }

            public static string Parse(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId)
            {
                var contentFilePathRule = GetContentFilePathRule(publishmentSystemInfo, nodeId);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeId);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
                var filePath = ParseContentPath(publishmentSystemInfo, nodeId, contentInfo, contentFilePathRule);
                return filePath;
            }

            public static string Parse(PublishmentSystemInfo publishmentSystemInfo, int nodeId, ContentInfo contentInfo)
            {
                var contentFilePathRule = GetContentFilePathRule(publishmentSystemInfo, nodeId);
                var filePath = ParseContentPath(publishmentSystemInfo, nodeId, contentInfo, contentFilePathRule);
                return filePath;
            }

            private static string ParseContentPath(PublishmentSystemInfo publishmentSystemInfo, int nodeId, ContentInfo contentInfo, string contentFilePathRule)
            {
                var filePath = contentFilePathRule.Trim();
                var regex = "(?<element>{@[^}]+})";
                var elements = RegexUtils.GetContents("element", regex, filePath);
                var addDate = DateTime.MinValue;
                var contentId = contentInfo.Id;
                foreach (var element in elements)
                {
                    var value = string.Empty;
                    if (StringUtils.EqualsIgnoreCase(element, ChannelId))
                    {
                        value = nodeId.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ChannelIndex))
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        if (nodeInfo != null)
                        {
                            value = nodeInfo.NodeIndexName;
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ContentId))
                    {
                        value = contentId.ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Sequence))
                    {
                        var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                        value = BaiRongDataProvider.ContentDao.GetSequence(tableName, nodeId, contentId).ToString();
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ParentRule))//继承父级设置 20151113 sessionliang
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        var parentInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeInfo.ParentId);
                        if (parentInfo != null)
                        {
                            var parentRule = GetContentFilePathRule(publishmentSystemInfo, parentInfo.NodeId);
                            value = DirectoryUtils.GetDirectoryPath(ParseContentPath(publishmentSystemInfo, parentInfo.NodeId, contentInfo, parentRule)).Replace("\\", "/");
                        }
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, ChannelName))//栏目名称 20151113 sessionliang
                    {
                        var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
                        value = nodeInfo.NodeName;
                    }
                    else if (StringUtils.EqualsIgnoreCase(element, Year) || StringUtils.EqualsIgnoreCase(element, Month) || StringUtils.EqualsIgnoreCase(element, Day) || StringUtils.EqualsIgnoreCase(element, Hour) || StringUtils.EqualsIgnoreCase(element, Minute) || StringUtils.EqualsIgnoreCase(element, Second))
                    {
                        if (addDate == DateTime.MinValue)
                        {
                            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                            addDate = BaiRongDataProvider.ContentDao.GetAddDate(tableName, contentId);
                        }

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
                        value = contentInfo.GetExtendedAttribute(attributeName);
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

        public static string GetChannelFilePathRule(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            var channelFilePathRule = GetChannelFilePathRule(publishmentSystemInfo.PublishmentSystemId, nodeId);
            if (string.IsNullOrEmpty(channelFilePathRule))
            {
                channelFilePathRule = publishmentSystemInfo.Additional.ChannelFilePathRule;

                if (string.IsNullOrEmpty(channelFilePathRule))
                {
                    channelFilePathRule = ChannelFilePathRules.DefaultRule;
                }
            }
            return channelFilePathRule;
        }

        private static string GetChannelFilePathRule(int publishmentSystemId, int nodeId)
        {
            if (nodeId == 0) return string.Empty;
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ChannelFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = GetChannelFilePathRule(publishmentSystemId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public static string GetContentFilePathRule(PublishmentSystemInfo publishmentSystemInfo, int nodeId)
        {
            var contentFilePathRule = GetContentFilePathRule(publishmentSystemInfo.PublishmentSystemId, nodeId);
            if (string.IsNullOrEmpty(contentFilePathRule))
            {
                contentFilePathRule = publishmentSystemInfo.Additional.ContentFilePathRule;

                if (string.IsNullOrEmpty(contentFilePathRule))
                {
                    contentFilePathRule = ContentFilePathRules.DefaultRule;
                }
            }
            return contentFilePathRule;
        }

        private static string GetContentFilePathRule(int publishmentSystemId, int nodeId)
        {
            if (nodeId == 0) return string.Empty;
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ContentFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = GetContentFilePathRule(publishmentSystemId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public static string GetChannelPageFilePath(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int currentPageIndex)
        {
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
            if (nodeInfo.NodeType == ENodeType.BackgroundPublishNode)
            {
                var templateInfo = TemplateManager.GetDefaultTemplateInfo(publishmentSystemInfo.PublishmentSystemId, ETemplateType.IndexPageTemplate);
                return GetIndexPageFilePath(publishmentSystemInfo, templateInfo.CreatedFileFullName, publishmentSystemInfo.IsHeadquarters, currentPageIndex);
            }
            var filePath = nodeInfo.FilePath;

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = ChannelFilePathRules.Parse(publishmentSystemInfo, nodeId);
            }

            filePath = MapPath(publishmentSystemInfo, filePath);// PathUtils.Combine(publishmentSystemPath, filePath);
            if (PathUtils.IsDirectoryPath(filePath))
            {
                filePath = PathUtils.Combine(filePath, nodeId + ".html");
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

        public static string GetContentPageFilePath(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, int currentPageIndex)
        {
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
            return GetContentPageFilePath(publishmentSystemInfo, nodeId, contentInfo, currentPageIndex);
        }

        public static string GetContentPageFilePath(PublishmentSystemInfo publishmentSystemInfo, int nodeId, ContentInfo contentInfo, int currentPageIndex)
        {
            var filePath = ContentFilePathRules.Parse(publishmentSystemInfo, nodeId, contentInfo);

            filePath = MapPath(publishmentSystemInfo, filePath);
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

        public static bool IsImageExtenstionAllowed(PublishmentSystemInfo publishmentSystemInfo, string fileExtention)
        {
            return PathUtils.IsFileExtenstionAllowed(publishmentSystemInfo.Additional.ImageUploadTypeCollection, fileExtention);
        }

        public static bool IsImageSizeAllowed(PublishmentSystemInfo publishmentSystemInfo, int contentLength)
        {
            return contentLength <= publishmentSystemInfo.Additional.ImageUploadTypeMaxSize * 1024;
        }

        public static bool IsVideoExtenstionAllowed(PublishmentSystemInfo publishmentSystemInfo, string fileExtention)
        {
            return PathUtils.IsFileExtenstionAllowed(publishmentSystemInfo.Additional.VideoUploadTypeCollection, fileExtention);
        }

        public static bool IsVideoSizeAllowed(PublishmentSystemInfo publishmentSystemInfo, int contentLength)
        {
            return contentLength <= publishmentSystemInfo.Additional.VideoUploadTypeMaxSize * 1024;
        }

        public static bool IsFileExtenstionAllowed(PublishmentSystemInfo publishmentSystemInfo, string fileExtention)
        {
            var typeCollection = publishmentSystemInfo.Additional.FileUploadTypeCollection + "," + publishmentSystemInfo.Additional.ImageUploadTypeCollection + "," + publishmentSystemInfo.Additional.VideoUploadTypeCollection;
            return PathUtils.IsFileExtenstionAllowed(typeCollection, fileExtention);
        }

        public static bool IsFileSizeAllowed(PublishmentSystemInfo publishmentSystemInfo, int contentLength)
        {
            return contentLength <= publishmentSystemInfo.Additional.FileUploadTypeMaxSize * 1024;
        }

        public static bool IsUploadExtenstionAllowed(EUploadType uploadType, PublishmentSystemInfo publishmentSystemInfo, string fileExtention)
        {
            if (uploadType == EUploadType.Image)
            {
                return IsImageExtenstionAllowed(publishmentSystemInfo, fileExtention);
            }
            else if (uploadType == EUploadType.Video)
            {
                return IsVideoExtenstionAllowed(publishmentSystemInfo, fileExtention);
            }
            else if (uploadType == EUploadType.File)
            {
                return IsFileExtenstionAllowed(publishmentSystemInfo, fileExtention);
            }
            return false;
        }

        public static bool IsUploadSizeAllowed(EUploadType uploadType, PublishmentSystemInfo publishmentSystemInfo, int contentLength)
        {
            switch (uploadType)
            {
                case EUploadType.Image:
                    return IsImageSizeAllowed(publishmentSystemInfo, contentLength);
                case EUploadType.Video:
                    return IsVideoSizeAllowed(publishmentSystemInfo, contentLength);
                case EUploadType.File:
                    return IsFileSizeAllowed(publishmentSystemInfo, contentLength);
            }
            return false;
        }
    }
}
