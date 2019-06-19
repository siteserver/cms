using System;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Services
{
    public partial class PathManager
    {
        public string GetSitePath(int siteId, string virtualPath)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            return MapPath(siteInfo, virtualPath);
        }

        public string MapPath(SiteInfo siteInfo, string virtualPath)
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
            if (!virtualPath.StartsWith("@")) return MapWebRootPath(resolvedPath);

            if (siteInfo != null)
            {
                resolvedPath = siteInfo.IsRoot ? string.Concat("~", virtualPath.Substring(1)) : PageUtils.Combine(siteInfo.SiteDir, virtualPath.Substring(1));
            }
            return MapWebRootPath(resolvedPath);
        }

        public string MapPath(SiteInfo siteInfo, string virtualPath, bool isCopyToSite)
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
            if (!virtualPath.StartsWith("@")) return MapWebRootPath(resolvedPath);

            if (siteInfo != null)
            {
                resolvedPath = siteInfo.IsRoot ? string.Concat("~", virtualPath.Substring(1)) : PageUtils.Combine(siteInfo.SiteDir, virtualPath.Substring(1));
            }
            return MapWebRootPath(resolvedPath);
        }

        public string GetUploadFileName(SiteInfo siteInfo, string filePath)
        {
            var fileExtension = PathUtils.GetExtension(filePath);

            var isUploadChangeFileName = siteInfo.IsFileUploadChangeFileName;
            if (IsImageExtensionAllowed(siteInfo, fileExtension))
            {
                isUploadChangeFileName = siteInfo.IsImageUploadChangeFileName;
            }
            else if (IsVideoExtensionAllowed(siteInfo, fileExtension))
            {
                isUploadChangeFileName = siteInfo.IsVideoUploadChangeFileName;
            }

            return GetUploadFileName(siteInfo, filePath, isUploadChangeFileName);
        }

        public string GetUploadFileName(SiteInfo siteInfo, string filePath, bool isUploadChangeFileName)
        {
            var retVal = isUploadChangeFileName
                ? $"{StringUtils.GetShortGuid(false)}{PathUtils.GetExtension(filePath)}"
                : PathUtils.GetFileName(filePath);

            retVal = StringUtils.ReplaceIgnoreCase(retVal, "as", string.Empty);
            retVal = StringUtils.ReplaceIgnoreCase(retVal, ";", string.Empty);
            return retVal;
        }

        public string GetUploadSpecialName(SiteInfo siteInfo, string filePath, bool isUploadChangeFileName)
        {
            var retVal = isUploadChangeFileName ? $"{StringUtils.GetShortGuid(false)}{PathUtils.GetExtension(filePath)}" : PathUtils.GetFileName(filePath);

            retVal = StringUtils.ReplaceIgnoreCase(retVal, "as", string.Empty);
            retVal = StringUtils.ReplaceIgnoreCase(retVal, ";", string.Empty);
            return retVal;
        }

        public string GetChannelFilePathRule(SiteInfo siteInfo, int channelId)
        {
            var channelFilePathRule = GetChannelFilePathRule(siteInfo.Id, channelId);
            if (string.IsNullOrEmpty(channelFilePathRule))
            {
                channelFilePathRule = siteInfo.ChannelFilePathRule;

                if (string.IsNullOrEmpty(channelFilePathRule))
                {
                    channelFilePathRule = ChannelRulesDefaultRule;
                }
            }
            return channelFilePathRule;
        }

        private string GetChannelFilePathRule(int siteId, int channelId)
        {
            if (channelId == 0) return string.Empty;
            var nodeInfo = _channelRepository.GetChannelInfo(siteId, channelId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ChannelFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = GetChannelFilePathRule(siteId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public string GetContentFilePathRule(SiteInfo siteInfo, int channelId)
        {
            var contentFilePathRule = GetContentFilePathRule(siteInfo.Id, channelId);
            if (string.IsNullOrEmpty(contentFilePathRule))
            {
                contentFilePathRule = siteInfo.ContentFilePathRule;

                if (string.IsNullOrEmpty(contentFilePathRule))
                {
                    contentFilePathRule = ContentRulesDefaultRule;
                }
            }
            return contentFilePathRule;
        }

        private string GetContentFilePathRule(int siteId, int channelId)
        {
            if (channelId == 0) return string.Empty;
            var nodeInfo = _channelRepository.GetChannelInfo(siteId, channelId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ContentFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = GetContentFilePathRule(siteId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public string GetChannelPageFilePath(SiteInfo siteInfo, int channelId, int currentPageIndex)
        {
            var nodeInfo = _channelRepository.GetChannelInfo(siteInfo.Id, channelId);
            if (nodeInfo.ParentId == 0)
            {
                var templateInfo = _templateRepository.GetDefaultTemplateInfo(siteInfo.Id, TemplateType.IndexPageTemplate);
                return GetIndexPageFilePath(siteInfo, templateInfo.CreatedFileFullName, siteInfo.IsRoot, currentPageIndex);
            }
            var filePath = nodeInfo.FilePath;

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = ChannelRulesParse(siteInfo, channelId);
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

        public string GetContentPageFilePath(SiteInfo siteInfo, int channelId, int contentId, int currentPageIndex)
        {
            var channelInfo = _channelRepository.GetChannelInfo(siteInfo.Id, channelId);
            var contentInfo = channelInfo.ContentRepository.GetContentInfo(siteInfo, channelInfo, contentId);
            return GetContentPageFilePath(siteInfo, channelId, contentInfo, currentPageIndex);
        }

        public string GetContentPageFilePath(SiteInfo siteInfo, int channelId, ContentInfo contentInfo, int currentPageIndex)
        {
            var filePath = ContentRulesParse(siteInfo, channelId, contentInfo);

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

        public bool IsImageExtensionAllowed(SiteInfo siteInfo, string fileExtention)
        {
            return PathUtils.IsFileExtensionAllowed(siteInfo.ImageUploadTypeCollection, fileExtention);
        }

        public bool IsImageSizeAllowed(SiteInfo siteInfo, int contentLength)
        {
            return contentLength <= siteInfo.ImageUploadTypeMaxSize * 1024;
        }

        public bool IsVideoExtensionAllowed(SiteInfo siteInfo, string fileExtention)
        {
            return PathUtils.IsFileExtensionAllowed(siteInfo.VideoUploadTypeCollection, fileExtention);
        }

        public bool IsVideoSizeAllowed(SiteInfo siteInfo, int contentLength)
        {
            return contentLength <= siteInfo.VideoUploadTypeMaxSize * 1024;
        }

        public bool IsFileExtensionAllowed(SiteInfo siteInfo, string fileExtention)
        {
            var typeCollection = siteInfo.FileUploadTypeCollection + "," + siteInfo.ImageUploadTypeCollection + "," + siteInfo.VideoUploadTypeCollection;
            return PathUtils.IsFileExtensionAllowed(typeCollection, fileExtention);
        }

        public bool IsFileSizeAllowed(SiteInfo siteInfo, int contentLength)
        {
            return contentLength <= siteInfo.FileUploadTypeMaxSize * 1024;
        }

        public bool IsUploadExtensionAllowed(UploadType uploadType, SiteInfo siteInfo, string fileExtention)
        {
            if (uploadType == UploadType.Image)
            {
                return IsImageExtensionAllowed(siteInfo, fileExtention);
            }
            else if (uploadType == UploadType.Video)
            {
                return IsVideoExtensionAllowed(siteInfo, fileExtention);
            }
            else if (uploadType == UploadType.File)
            {
                return IsFileExtensionAllowed(siteInfo, fileExtention);
            }
            return false;
        }

        public bool IsUploadSizeAllowed(UploadType uploadType, SiteInfo siteInfo, int contentLength)
        {
            if (uploadType == UploadType.Image)
            {
                return IsImageSizeAllowed(siteInfo, contentLength);
            }

            if (uploadType == UploadType.Video)
            {
                return IsVideoSizeAllowed(siteInfo, contentLength);
            }

            if (uploadType == UploadType.File)
            {
                return IsFileSizeAllowed(siteInfo, contentLength);
            }

            return false;
        }

        public string GetSitePath(SiteInfo siteInfo)
        {
            return PathUtils.Combine(_settingsManager.WebRootPath, siteInfo.SiteDir);
        }

        public string GetSitePath(int siteId, params string[] paths)
        {
            return GetSitePath(_siteRepository.GetSiteInfo(siteId), paths);
        }

        public string GetSitePath(SiteInfo siteInfo, params string[] paths)
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

        public string GetIndexPageFilePath(SiteInfo siteInfo, string createFileFullName, bool isHeadquarters, int currentPageIndex)
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

        public string GetUploadDirectoryPath(SiteInfo siteInfo, string fileExtension)
        {
            return GetUploadDirectoryPath(siteInfo, DateTime.Now, fileExtension);
        }

        public string GetUploadDirectoryPath(SiteInfo siteInfo, DateTime datetime, string fileExtension)
        {
            var uploadDateFormatString = siteInfo.FileUploadDateFormatString;
            var uploadDirectoryName = siteInfo.FileUploadDirectoryName;

            if (IsImageExtensionAllowed(siteInfo, fileExtension))
            {
                uploadDateFormatString = siteInfo.ImageUploadDateFormatString;
                uploadDirectoryName = siteInfo.ImageUploadDirectoryName;
            }
            else if (IsVideoExtensionAllowed(siteInfo, fileExtension))
            {
                uploadDateFormatString = siteInfo.VideoUploadDateFormatString;
                uploadDirectoryName = siteInfo.VideoUploadDirectoryName;
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

        public string GetUploadDirectoryPath(SiteInfo siteInfo, UploadType uploadType)
        {
            return GetUploadDirectoryPath(siteInfo, DateTime.Now, uploadType);
        }

        public string GetUploadDirectoryPath(SiteInfo siteInfo, DateTime datetime, UploadType uploadType)
        {
            var uploadDateFormatString = string.Empty;
            var uploadDirectoryName = string.Empty;

            if (uploadType == UploadType.Image)
            {
                uploadDateFormatString = siteInfo.ImageUploadDateFormatString;
                uploadDirectoryName = siteInfo.ImageUploadDirectoryName;
            }
            else if (uploadType == UploadType.Video)
            {
                uploadDateFormatString = siteInfo.VideoUploadDateFormatString;
                uploadDirectoryName = siteInfo.VideoUploadDirectoryName;
            }
            else if (uploadType == UploadType.File)
            {
                uploadDateFormatString = siteInfo.FileUploadDateFormatString;
                uploadDirectoryName = siteInfo.FileUploadDirectoryName;
            }
            else if (uploadType == UploadType.Special)
            {
                uploadDateFormatString = siteInfo.FileUploadDateFormatString;
                uploadDirectoryName = "/Special";
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

        public int GetSiteIdByFilePath(string path)
        {
            var siteInfo = GetSiteInfoByFilePath(path);
            return siteInfo?.Id ?? 0;
        }

        public string GetSitePath(int siteId)
        {
            if (siteId <= 0) return null;

            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            return siteInfo == null ? null : GetSitePath(siteInfo);
        }

        public string GetUploadFilePath(int siteId, string fileName)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            var localDirectoryPath = GetUploadDirectoryPath(siteInfo, PathUtils.GetExtension(fileName));
            var localFileName = GetUploadFileName(siteInfo, fileName);
            return PathUtils.Combine(localDirectoryPath, localFileName);
        }

        public SiteInfo GetSiteInfoByFilePath(string path)
        {
            var directoryPath = DirectoryUtils.GetDirectoryPath(path).ToLower().Trim(' ', '/', '\\');
            var applicationPath = _settingsManager.WebRootPath.ToLower().Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty) return null;

            var siteInfoList = _siteRepository.GetSiteInfoList();

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

        public string GetSiteDirByFilePath(string path)
        {
            var siteDir = string.Empty;
            var directoryPath = DirectoryUtils.GetDirectoryPath(path).ToLower().Trim(' ', '/', '\\');
            var applicationPath = _settingsManager.WebRootPath.ToLower().Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty)
            {
                return string.Empty;
            }

            var siteInfoList = _siteRepository.GetSiteInfoList();
            foreach (var siteInfo in siteInfoList)
            {
                if (siteInfo?.IsRoot != false) continue;

                if (StringUtils.Contains(directoryDir, siteInfo.SiteDir.ToLower()))
                {
                    siteDir = siteInfo.SiteDir;
                }
            }

            return string.IsNullOrWhiteSpace(siteDir) ? siteDir : PathUtils.GetDirectoryName(siteDir, false);
        }
    }
}