using System;
using System.Threading.Tasks;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Utils;
using SS.CMS.Utils.Enumerations;

namespace SS.CMS.Core.Services
{
    public partial class PathManager
    {
        public async Task<string> GetSitePathAsync(int siteId, string virtualPath)
        {
            var siteInfo = await _siteRepository.GetSiteAsync(siteId);
            return MapPath(siteInfo, virtualPath);
        }

        public string MapPath(Site siteInfo, string virtualPath)
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

        public string MapPath(Site siteInfo, string virtualPath, bool isCopyToSite)
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

        public string GetUploadFileName(Site siteInfo, string filePath)
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

        public string GetUploadFileName(Site siteInfo, string filePath, bool isUploadChangeFileName)
        {
            var retVal = isUploadChangeFileName
                ? $"{StringUtils.GetShortGuid(false)}{PathUtils.GetExtension(filePath)}"
                : PathUtils.GetFileName(filePath);

            retVal = StringUtils.ReplaceIgnoreCase(retVal, "as", string.Empty);
            retVal = StringUtils.ReplaceIgnoreCase(retVal, ";", string.Empty);
            return retVal;
        }

        public string GetUploadSpecialName(Site siteInfo, string filePath, bool isUploadChangeFileName)
        {
            var retVal = isUploadChangeFileName ? $"{StringUtils.GetShortGuid(false)}{PathUtils.GetExtension(filePath)}" : PathUtils.GetFileName(filePath);

            retVal = StringUtils.ReplaceIgnoreCase(retVal, "as", string.Empty);
            retVal = StringUtils.ReplaceIgnoreCase(retVal, ";", string.Empty);
            return retVal;
        }

        public async Task<string> GetChannelFilePathRuleAsync(Site siteInfo, int channelId)
        {
            var channelFilePathRule = await GetChannelFilePathRuleAsync(siteInfo.Id, channelId);
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

        private async Task<string> GetChannelFilePathRuleAsync(int siteId, int channelId)
        {
            if (channelId == 0) return string.Empty;
            var nodeInfo = await _channelRepository.GetChannelAsync(channelId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ChannelFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = await GetChannelFilePathRuleAsync(siteId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public async Task<string> GetContentFilePathRuleAsync(Site siteInfo, int channelId)
        {
            var contentFilePathRule = await GetContentFilePathRuleAsync(siteInfo.Id, channelId);
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

        private async Task<string> GetContentFilePathRuleAsync(int siteId, int channelId)
        {
            if (channelId == 0) return string.Empty;
            var nodeInfo = await _channelRepository.GetChannelAsync(channelId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ContentFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = await GetContentFilePathRuleAsync(siteId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public async Task<string> GetChannelPageFilePathAsync(Site siteInfo, int channelId, int currentPageIndex)
        {
            var nodeInfo = await _channelRepository.GetChannelAsync(channelId);
            if (nodeInfo.ParentId == 0)
            {
                var templateInfo = await _templateRepository.GetDefaultTemplateInfoAsync(siteInfo.Id, TemplateType.IndexPageTemplate);
                return GetIndexPageFilePath(siteInfo, templateInfo.CreatedFileFullName, siteInfo.IsRoot, currentPageIndex);
            }
            var filePath = nodeInfo.FilePath;

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = await ChannelRulesParseAsync(siteInfo, channelId);
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

        public async Task<string> GetContentPageFilePathAsync(Site siteInfo, int channelId, int contentId, int currentPageIndex)
        {
            var channelInfo = await _channelRepository.GetChannelAsync(channelId);
            var contentRepository = _channelRepository.GetContentRepository(siteInfo, channelInfo);
            var contentInfo = await contentRepository.GetContentInfoAsync(contentId);
            return await GetContentPageFilePathAsync(siteInfo, channelId, contentInfo, currentPageIndex);
        }

        public async Task<string> GetContentPageFilePathAsync(Site siteInfo, int channelId, Content contentInfo, int currentPageIndex)
        {
            var filePath = await ContentRulesParseAsync(siteInfo, channelId, contentInfo);

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

        public bool IsImageExtensionAllowed(Site siteInfo, string fileExtention)
        {
            return PathUtils.IsFileExtensionAllowed(siteInfo.ImageUploadTypeCollection, fileExtention);
        }

        public bool IsImageSizeAllowed(Site siteInfo, int contentLength)
        {
            return contentLength <= siteInfo.ImageUploadTypeMaxSize * 1024;
        }

        public bool IsVideoExtensionAllowed(Site siteInfo, string fileExtention)
        {
            return PathUtils.IsFileExtensionAllowed(siteInfo.VideoUploadTypeCollection, fileExtention);
        }

        public bool IsVideoSizeAllowed(Site siteInfo, int contentLength)
        {
            return contentLength <= siteInfo.VideoUploadTypeMaxSize * 1024;
        }

        public bool IsFileExtensionAllowed(Site siteInfo, string fileExtention)
        {
            var typeCollection = siteInfo.FileUploadTypeCollection + "," + siteInfo.ImageUploadTypeCollection + "," + siteInfo.VideoUploadTypeCollection;
            return PathUtils.IsFileExtensionAllowed(typeCollection, fileExtention);
        }

        public bool IsFileSizeAllowed(Site siteInfo, int contentLength)
        {
            return contentLength <= siteInfo.FileUploadTypeMaxSize * 1024;
        }

        public bool IsUploadExtensionAllowed(UploadType uploadType, Site siteInfo, string fileExtention)
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

        public bool IsUploadSizeAllowed(UploadType uploadType, Site siteInfo, int contentLength)
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

        public string GetSitePath(Site siteInfo)
        {
            return PathUtils.Combine(_settingsManager.WebRootPath, siteInfo.SiteDir);
        }

        public async Task<string> GetSitePathAsync(int siteId, params string[] paths)
        {
            return GetSitePath(await _siteRepository.GetSiteAsync(siteId), paths);
        }

        public string GetSitePath(Site siteInfo, params string[] paths)
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

        public string GetIndexPageFilePath(Site siteInfo, string createFileFullName, bool isHeadquarters, int currentPageIndex)
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

        public string GetUploadDirectoryPath(Site siteInfo, string fileExtension)
        {
            return GetUploadDirectoryPath(siteInfo, DateTime.Now, fileExtension);
        }

        public string GetUploadDirectoryPath(Site siteInfo, DateTime datetime, string fileExtension)
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

        public string GetUploadDirectoryPath(Site siteInfo, UploadType uploadType)
        {
            return GetUploadDirectoryPath(siteInfo, DateTime.Now, uploadType);
        }

        public string GetUploadDirectoryPath(Site siteInfo, DateTime datetime, UploadType uploadType)
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

        public async Task<int> GetSiteIdByFilePathAsync(string path)
        {
            var siteInfo = await GetSiteInfoByFilePathAsync(path);
            return siteInfo?.Id ?? 0;
        }

        public async Task<string> GetSitePathAsync(int siteId)
        {
            if (siteId <= 0) return null;

            var siteInfo = await _siteRepository.GetSiteAsync(siteId);
            return siteInfo == null ? null : GetSitePath(siteInfo);
        }

        public async Task<string> GetUploadFilePathAsync(int siteId, string fileName)
        {
            var siteInfo = await _siteRepository.GetSiteAsync(siteId);
            var localDirectoryPath = GetUploadDirectoryPath(siteInfo, PathUtils.GetExtension(fileName));
            var localFileName = GetUploadFileName(siteInfo, fileName);
            return PathUtils.Combine(localDirectoryPath, localFileName);
        }

        public async Task<Site> GetSiteInfoByFilePathAsync(string path)
        {
            var directoryPath = DirectoryUtils.GetDirectoryPath(path).ToLower().Trim(' ', '/', '\\');
            var applicationPath = _settingsManager.WebRootPath.ToLower().Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty) return null;

            var siteInfoList = await _siteRepository.GetSiteListAsync();

            Site headquarter = null;
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

        public async Task<string> GetSiteDirByFilePathAsync(string path)
        {
            var siteDir = string.Empty;
            var directoryPath = DirectoryUtils.GetDirectoryPath(path).ToLower().Trim(' ', '/', '\\');
            var applicationPath = _settingsManager.WebRootPath.ToLower().Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty)
            {
                return string.Empty;
            }

            var siteInfoList = await _siteRepository.GetSiteListAsync();
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