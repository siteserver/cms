using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Core.Utils.PathRules;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        //根据发布系统属性判断是否为相对路径并返回解析后路径
        public async Task<string> ParseSiteUrlAsync(Site site, string virtualUrl, bool isLocal)
        {
            if (string.IsNullOrEmpty(virtualUrl)) return string.Empty;

            if (site != null && StringUtils.StartsWith(virtualUrl, "@"))
            {
                return await GetSiteUrlAsync(site, virtualUrl.Substring(1), isLocal);
            }

            return ParseUrl(virtualUrl);
        }

        public async Task<string> ParseSitePathAsync(Site site, string virtualPath)
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
            if (!virtualPath.StartsWith("@")) return ParsePath(resolvedPath);

            if (site != null)
            {
                return await GetSitePathAsync(site, virtualPath.Substring(1));
            }
            return ParsePath(resolvedPath);
        }

        public async Task<string> GetSiteUrlAsync(Site site, bool isLocal)
        {
            return await GetSiteUrlAsync(site, string.Empty, isLocal);
        }

        public async Task<string> GetSiteUrlAsync(Site site, string requestPath, bool isLocal)
        {
            var url = isLocal
                ? await GetLocalSiteUrlAsync(site, requestPath)
                : await GetRemoteSiteUrlAsync(site, requestPath);

            return RemoveDefaultFileName(site, url);
        }

        public async Task<string> GetSiteUrlByPhysicalPathAsync(Site site, string physicalPath, bool isLocal)
        {
            if (site == null || string.IsNullOrEmpty(physicalPath)) return await GetWebUrlAsync(site);

            var sitePath = await GetSitePathAsync(site);
            var requestPath = StringUtils.StartsWithIgnoreCase(physicalPath, sitePath)
                ? StringUtils.ReplaceStartsWithIgnoreCase(physicalPath, sitePath, string.Empty)
                : string.Empty;

            return await GetSiteUrlAsync(site, requestPath, isLocal);
        }

        public async Task<string> GetVirtualUrlByPhysicalPathAsync(Site site, string physicalPath)
        {
            if (site == null || string.IsNullOrEmpty(physicalPath)) return await GetWebUrlAsync(site);

            var sitePath = await GetSitePathAsync(site);
            var requestPath = StringUtils.StartsWithIgnoreCase(physicalPath, sitePath)
                ? StringUtils.ReplaceStartsWithIgnoreCase(physicalPath, sitePath, string.Empty)
                : string.Empty;

            return AddVirtualToUrl(requestPath);
        }

        public async Task<string> GetRemoteSiteUrlAsync(Site site, string requestPath)
        {
            var url = await GetWebUrlAsync(site);

            if (string.IsNullOrEmpty(url))
            {
                url = "/";
            }
            else
            {
                if (url != "/" && url.EndsWith("/"))
                {
                    url = url.Substring(0, url.Length - 1);
                }
            }

            if (string.IsNullOrEmpty(requestPath)) return url;

            requestPath = requestPath.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
            requestPath = PathUtils.RemovePathInvalidChar(requestPath);
            if (requestPath.StartsWith("/"))
            {
                requestPath = requestPath.Substring(1);
            }

            url = PageUtils.Combine(url, requestPath);

            if (!site.IsSeparatedAssets) return url;

            var assetsUrl = PageUtils.Combine(await GetWebUrlAsync(site),
                site.AssetsDir);
            if (StringUtils.StartsWithIgnoreCase(url, assetsUrl))
            {
                url = StringUtils.ReplaceStartsWithIgnoreCase(url, assetsUrl, await GetAssetsUrlAsync(site));
            }

            return url;
        }

        public async Task<string> GetLocalSiteUrlAsync(Site site, string requestPath = null)
        {
            string url;
            if (site.ParentId == 0)
            {
                url = ParseUrl($"~/{(site.Root ? string.Empty : site.SiteDir)}");
            }
            else
            {
                var parent = await _siteRepository.GetAsync(site.ParentId);
                url = await GetLocalSiteUrlAsync(parent, site.SiteDir);
            }

            if (string.IsNullOrEmpty(url))
            {
                url = "/";
            }
            else
            {
                if (url != "/" && url.EndsWith("/"))
                {
                    url = url.Substring(0, url.Length - 1);
                }
            }

            if (string.IsNullOrEmpty(requestPath)) return url;

            requestPath = requestPath.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
            requestPath = PathUtils.RemovePathInvalidChar(requestPath);
            if (requestPath.StartsWith("/"))
            {
                requestPath = requestPath.Substring(1);
            }

            url = PageUtils.Combine(url, requestPath);

            return url;
        }

        public string GetPreviewSiteUrl(int siteId)
        {
            var apiUrl = PageUtils.Combine("/", Constants.RoutePreview);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            return apiUrl;
        }

        public string GetPreviewChannelUrl(int siteId, int channelId)
        {
            var apiUrl = PageUtils.Combine("/", Constants.RoutePreviewChannel);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            return apiUrl;
        }

        public string GetPreviewContentUrl(int siteId, int channelId, int contentId, bool isPreview = false)
        {
            var apiUrl = PageUtils.Combine("/", Constants.RoutePreviewContent);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            if (isPreview)
            {
                apiUrl += "?isPreview=true";
            }
            return apiUrl;
        }

        public string GetPreviewFileUrl(int siteId, int fileTemplateId)
        {
            var apiUrl = PageUtils.Combine("/", Constants.RoutePreviewFile);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{fileTemplateId}", fileTemplateId.ToString());
            return apiUrl;
        }

        public string GetPreviewSpecialUrl(int siteId, int specialId)
        {
            var apiUrl = PageUtils.Combine("/", Constants.RoutePreviewSpecial);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{specialId}", specialId.ToString());
            return apiUrl;
        }

        // 得到发布系统首页地址
        public async Task<string> GetIndexPageUrlAsync(Site site, bool isLocal)
        {
            var indexTemplateId = await _templateRepository.GetIndexTemplateIdAsync(site.Id);
            var createdFileFullName = await _templateRepository.GetCreatedFileFullNameAsync(indexTemplateId);

            var url = isLocal
                ? GetPreviewSiteUrl(site.Id)
                : await ParseSiteUrlAsync(site, createdFileFullName, false);

            return RemoveDefaultFileName(site, url);
        }

        public async Task<string> GetFileUrlAsync(Site site, int fileTemplateId, bool isLocal)
        {
            var createdFileFullName = await _templateRepository.GetCreatedFileFullNameAsync(fileTemplateId);

            var url = isLocal
                ? GetPreviewFileUrl(site.Id, fileTemplateId)
                : await ParseSiteUrlAsync(site, createdFileFullName, false);

            return RemoveDefaultFileName(site, url);
        }

        public async Task<string> GetContentUrlAsync(Site site, Content content, bool isLocal)
        {
            return await GetContentUrlByIdAsync(site, content, isLocal);
        }

        public async Task<string> GetContentUrlAsync(Site site, Channel channel, int contentId, bool isLocal)
        {
            var contentInfo = await _contentRepository.GetAsync(site, channel, contentId);
            return await GetContentUrlByIdAsync(site, contentInfo, isLocal);
        }

        public async Task<string> GetContentUrlByIdAsync(Site site, Content contentCurrent, bool isLocal)
        {
            if (contentCurrent == null) return PageUtils.UnClickableUrl;

            if (isLocal)
            {
                return GetPreviewContentUrl(site.Id, contentCurrent.ChannelId,
                    contentCurrent.Id);
            }

            var sourceId = contentCurrent.SourceId;
            var referenceId = contentCurrent.ReferenceId;
            var linkUrl = contentCurrent.LinkUrl;
            var channelId = contentCurrent.ChannelId;
            if (referenceId > 0)
            {
                if (sourceId > 0 && await _channelRepository.IsExistsAsync(sourceId))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = await _channelRepository.GetSiteIdAsync(targetChannelId);
                    var targetSite = await _siteRepository.GetAsync(targetSiteId);
                    var targetChannelInfo = await _channelRepository.GetAsync(targetChannelId);

                    var contentInfo = await _contentRepository.GetAsync(targetSite, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnClickableUrl;
                    }
                    if (contentInfo.SiteId == targetSite.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSite, contentInfo, false);
                    }
                    var siteTmp = await _siteRepository.GetAsync(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteTmp, contentInfo, false);
                }
                else
                {
                    var reference = await _contentRepository.GetAsync(site, channelId, referenceId);

                    channelId = reference.ChannelId;
                    linkUrl = reference.LinkUrl;
                    if (await _channelRepository.IsExistsAsync(channelId))
                    {
                        return await GetContentUrlByIdAsync(site, channelId, referenceId, 0, 0, linkUrl, false);
                    }
                    var targetSiteId = await _channelRepository.GetSiteIdAsync(channelId);
                    var targetSite = await _siteRepository.GetAsync(targetSiteId);
                    return await GetContentUrlByIdAsync(targetSite, channelId, referenceId, 0, 0, linkUrl, false);
                }
            }

            if (!string.IsNullOrEmpty(linkUrl))
            {
                return await ParseSiteUrlAsync(site, linkUrl, false);
            }

            var rules = new ContentFilePathRules(this, _databaseManager);
            var contentUrl = await rules.ParseAsync(site, channelId, contentCurrent);
            return await GetSiteUrlAsync(site, contentUrl, false);
        }

        public async Task<string> GetContentUrlByIdAsync(Site site, int channelId, int contentId, int sourceId, int referenceId, string linkUrl, bool isLocal)
        {
            if (isLocal)
            {
                return GetPreviewContentUrl(site.Id, channelId, contentId);
            }

            if (referenceId > 0)
            {
                if (sourceId > 0 && await _channelRepository.IsExistsAsync(sourceId))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = await _channelRepository.GetSiteIdAsync(targetChannelId);
                    var targetSite = await _siteRepository.GetAsync(targetSiteId);
                    var targetChannelInfo = await _channelRepository.GetAsync(targetChannelId);

                    var contentInfo = await _contentRepository.GetAsync(targetSite, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnClickableUrl;
                    }
                    if (contentInfo.SiteId == targetSite.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSite, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                    }
                    var siteTmp = await _siteRepository.GetAsync(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteTmp, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                }

                var reference = await _contentRepository.GetAsync(site, channelId, referenceId);

                channelId = reference.ChannelId;
                linkUrl = reference.LinkUrl;
                return await GetContentUrlByIdAsync(site, channelId, referenceId, 0, 0, linkUrl, false);
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return await ParseSiteUrlAsync(site, linkUrl, false);
            }

            var rules = new ContentFilePathRules(this, _databaseManager);
            var contentUrl = await rules.ParseAsync(site, channelId, contentId);
            return await GetSiteUrlAsync(site, contentUrl, false);
        }

        public async Task<string> GetChannelUrlNotComputedAsync(Site site, int channelId, bool isLocal)
        {
            if (channelId == site.Id)
            {
                return await GetIndexPageUrlAsync(site, isLocal);
            }
            var linkUrl = string.Empty;
            var nodeInfo = await _channelRepository.GetAsync(channelId);
            if (nodeInfo != null)
            {
                linkUrl = nodeInfo.LinkUrl;
            }

            if (string.IsNullOrEmpty(linkUrl))
            {
                if (nodeInfo != null)
                {
                    var filePath = nodeInfo.FilePath;

                    if (string.IsNullOrEmpty(filePath))
                    {
                        var rules = new ChannelFilePathRules(this, _databaseManager);
                        var channelUrl = await rules.ParseAsync(site, channelId);
                        return await GetSiteUrlAsync(site, channelUrl, isLocal);
                    }
                    return await ParseSiteUrlAsync(site, AddVirtualToPath(filePath), isLocal);
                }
            }

            return await ParseSiteUrlAsync(site, linkUrl, isLocal);
        }

        //得到栏目经过计算后的连接地址
        public async Task<string> GetChannelUrlAsync(Site site, Channel channel, bool isLocal)
        {
            if (channel == null) return string.Empty;

            if (isLocal)
            {
                return GetPreviewChannelUrl(site.Id, channel.Id);
            }

            var url = string.Empty;

            if (channel.ParentId == 0)
            {
                url = await GetChannelUrlNotComputedAsync(site, channel.Id, false);
            }
            else
            {
                var linkType = channel.LinkType;
                if (linkType == LinkType.None)
                {
                    url = await GetChannelUrlNotComputedAsync(site, channel.Id, false);
                }
                else if (linkType == LinkType.NoLink)
                {
                    url = PageUtils.UnClickableUrl;
                }
                else
                {
                    if (linkType == LinkType.NoLinkIfContentNotExists)
                    {
                        var count = await _contentRepository.GetCountAsync(site, channel);
                        url = count == 0 ? PageUtils.UnClickableUrl : await GetChannelUrlNotComputedAsync(site, channel.Id, false);
                    }
                    else if (linkType == LinkType.LinkToOnlyOneContent)
                    {
                        var count = await _contentRepository.GetCountAsync(site, channel);
                        if (count == 1)
                        {
                            var tableName = _channelRepository.GetTableName(site, channel);
                            var contentId = _contentRepository.GetContentId(tableName, channel.Id, true, _databaseManager.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, false);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, false);
                        }
                    }
                    else if (linkType == LinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
                    {
                        var count = await _contentRepository.GetCountAsync(site, channel);
                        if (count == 0)
                        {
                            url = PageUtils.UnClickableUrl;
                        }
                        else if (count == 1)
                        {
                            var tableName = _channelRepository.GetTableName(site, channel);
                            var contentId = _contentRepository.GetContentId(tableName, channel.Id, true, _databaseManager.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, false);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, false);
                        }
                    }
                    else if (linkType == LinkType.LinkToFirstContent)
                    {
                        var count = await _contentRepository.GetCountAsync(site, channel);
                        if (count >= 1)
                        {
                            var tableName = _channelRepository.GetTableName(site, channel);
                            var contentId = _contentRepository.GetContentId(tableName, channel.Id, true, _databaseManager.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, false);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, false);
                        }
                    }
                    else if (linkType == LinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
                    {
                        var count = await _contentRepository.GetCountAsync(site, channel);
                        if (count >= 1)
                        {
                            var tableName = _channelRepository.GetTableName(site, channel);
                            var contentId = _contentRepository.GetContentId(tableName, channel.Id, true, _databaseManager.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, false);
                        }
                        else
                        {
                            url = PageUtils.UnClickableUrl;
                        }
                    }
                    else if (linkType == LinkType.NoLinkIfChannelNotExists)
                    {
                        url = channel.ChildrenCount == 0 ? PageUtils.UnClickableUrl : await GetChannelUrlNotComputedAsync(site, channel.Id, false);
                    }
                    else if (linkType == LinkType.LinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = await _channelRepository.GetChannelByLastAddDateAsyncTask(site.Id, channel.Id);
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(site, lastAddChannelInfo, false) : await GetChannelUrlNotComputedAsync(site, channel.Id, false);
                    }
                    else if (linkType == LinkType.LinkToFirstChannel)
                    {
                        var firstChannelInfo = await _channelRepository.GetChannelByTaxisAsync(site.Id, channel.Id);
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(site, firstChannelInfo, false) : await GetChannelUrlNotComputedAsync(site, channel.Id, false);
                    }
                    else if (linkType == LinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = await _channelRepository.GetChannelByLastAddDateAsyncTask(site.Id, channel.Id);
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(site, lastAddChannelInfo, false) : PageUtils.UnClickableUrl;
                    }
                    else if (linkType == LinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                    {
                        var firstChannelInfo = await _channelRepository.GetChannelByTaxisAsync(site.Id, channel.Id);
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(site, firstChannelInfo, false) : PageUtils.UnClickableUrl;
                    }
                }
            }

            return RemoveDefaultFileName(site, url);
        }

        public async Task<string> GetBaseUrlAsync(Site site, Template template, int channelId, int contentId)
        {
            var baseUrl = string.Empty;
            if (template.TemplateType == TemplateType.IndexPageTemplate)
            {
                baseUrl = await GetIndexPageUrlAsync(site, false);
            }
            else if (template.TemplateType == TemplateType.ChannelTemplate)
            {
                var channel = await _channelRepository.GetAsync(channelId);
                baseUrl = await GetChannelUrlAsync(site, channel, false);
            }
            else if (template.TemplateType == TemplateType.ContentTemplate)
            {
                var content = await _contentRepository.GetAsync(site, channelId, contentId);
                baseUrl = await GetContentUrlByIdAsync(site, content, false);
            }
            else if (template.TemplateType == TemplateType.FileTemplate)
            {
                baseUrl = await GetFileUrlAsync(site, template.Id, false);
            }

            return baseUrl;
        }

        public string RemoveDefaultFileName(Site site, string url)
        {
            if (!site.IsCreateUseDefaultFileName || string.IsNullOrEmpty(url)) return url;

            return url.EndsWith("/" + site.CreateDefaultFileName)
                ? url.Substring(0, url.Length - site.CreateDefaultFileName.Length)
                : url;
        }

        public async Task<string> GetInputChannelUrlAsync(Site site, Channel node, bool isLocal)
        {
            var channelUrl = await GetChannelUrlAsync(site, node, isLocal);
            if (string.IsNullOrEmpty(channelUrl)) return channelUrl;

            channelUrl = StringUtils.ReplaceStartsWith(channelUrl, await GetWebUrlAsync(site), string.Empty);
            channelUrl = channelUrl.Trim('/');
            if (channelUrl != PageUtils.UnClickableUrl)
            {
                channelUrl = "/" + channelUrl;
            }

            return channelUrl;
        }

        public string AddVirtualToUrl(string url)
        {
            var resolvedUrl = url;
            if (string.IsNullOrEmpty(url) || PageUtils.IsProtocolUrl(url)) return resolvedUrl;

            url = url.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);

            if (!url.StartsWith("@") && !url.StartsWith("~"))
            {
                resolvedUrl = PageUtils.Combine("@/", url);
            }
            return resolvedUrl;
        }

        public string GetVirtualUrl(Site site, string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            var relatedSiteUrl = ParseUrl($"~/{site.SiteDir}");
            var virtualUrl = StringUtils.ReplaceStartsWith(url, relatedSiteUrl, "@/");
            return StringUtils.ReplaceStartsWith(virtualUrl, "@//", "@/");
        }

        public bool IsVirtualUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            return url.StartsWith("~") || url.StartsWith("@");
        }

        public bool IsRelativeUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            return url.StartsWith("/");
        }

        public List<Select<string>> GetLinkTypeSelects()
        {
            return new List<Select<string>>
            {
                new Select<string>(LinkType.None),
                new Select<string>(LinkType.NoLinkIfContentNotExists),
                new Select<string>(LinkType.LinkToOnlyOneContent),
                new Select<string>(LinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent),
                new Select<string>(LinkType.LinkToFirstContent),
                new Select<string>(LinkType.NoLinkIfContentNotExistsAndLinkToFirstContent),
                new Select<string>(LinkType.NoLinkIfChannelNotExists),
                new Select<string>(LinkType.LinkToLastAddChannel),
                new Select<string>(LinkType.LinkToFirstChannel),
                new Select<string>(LinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel),
                new Select<string>(LinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel),
                new Select<string>(LinkType.NoLink)
            };
        }

        public async Task<string> GetSitePathAsync(Site site)
        {
            return await GetSitePathAsync(site, null);
        }

        public async Task<string> GetSitePathAsync(Site site, params string[] paths)
        {
            if (site == null)
            {
                return GetRootPath(paths);
            }

            string sitePath;
            if (site.ParentId == 0)
            {
                sitePath = PathUtils.Combine(_settingsManager.WebRootPath, site.Root ? string.Empty : site.SiteDir);
            }
            else
            {
                var parent = await _siteRepository.GetAsync(site.ParentId);
                sitePath = await GetSitePathAsync(parent, site.SiteDir);
            }

            if (paths == null || paths.Length <= 0) return sitePath;

            foreach (var t in paths)
            {
                var path = t?.Replace(PageUtils.SeparatorChar, PathUtils.SeparatorChar).Trim(PathUtils.SeparatorChar) ?? string.Empty;
                sitePath = PathUtils.Combine(sitePath, path);
            }
            return sitePath;
        }

        public async Task<string> GetSitePathAsync(int siteId, params string[] paths)
        {
            if (siteId == 0)
            {
                return GetRootPath(paths);
            }
            var site = await _siteRepository.GetAsync(siteId);
            return await GetSitePathAsync(site, paths);
        }

        public async Task<string> GetIndexPageFilePathAsync(Site site, string createFileFullName, bool root, int currentPageIndex)
        {
            if (string.IsNullOrEmpty(createFileFullName))
            {
                createFileFullName = "@/index.html";
            }
            if (root)
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

            var filePath = await ParseSitePathAsync(site, createFileFullName);

            if (currentPageIndex != 0)
            {
                string appendix = $"_{currentPageIndex + 1}";
                var fileName = PathUtils.GetFileNameWithoutExtension(filePath) + appendix + PathUtils.GetExtension(filePath);
                filePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), fileName);
            }

            return filePath;
        }

        public string GetBackupFilePath(Site site, BackupType backupType)
        {
            var extension = ".zip";
            var siteName = site.SiteDir;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteName += "_";
            }
            if (backupType == BackupType.Templates)
            {
                extension = ".xml";
            }
            return PathUtils.Combine(PhysicalSiteFilesPath, DirectoryUtils.SiteFiles.BackupFiles, site.SiteDir, DateTime.Now.ToString("yyyy-MM"), backupType.GetValue() + "_" + siteName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + extension);
        }

        public async Task<string> GetUploadDirectoryPathAsync(Site site, string fileExtension)
        {
            return await GetUploadDirectoryPathAsync(site, DateTime.Now, fileExtension);
        }

        public async Task<string> GetUploadDirectoryPathAsync(Site site, DateTime datetime, string fileExtension)
        {
            var uploadDateFormatString = site.FileUploadDateFormatString;
            var uploadDirectoryName = site.FileUploadDirectoryName;

            if (IsImageExtensionAllowed(site, fileExtension))
            {
                uploadDateFormatString = site.ImageUploadDateFormatString;
                uploadDirectoryName = site.ImageUploadDirectoryName;
            }
            else if (IsVideoExtensionAllowed(site, fileExtension))
            {
                uploadDateFormatString = site.VideoUploadDateFormatString;
                uploadDirectoryName = site.VideoUploadDirectoryName;
            }

            string directoryPath;
            var dateFormatType = uploadDateFormatString;
            var sitePath = await GetSitePathAsync(site);
            if (dateFormatType == DateFormatType.Year)
            {
                directoryPath = PathUtils.Combine(sitePath, uploadDirectoryName, datetime.Year.ToString());
            }
            else if (dateFormatType == DateFormatType.Day)
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

        public async Task<string> GetUploadDirectoryPathAsync(Site site, UploadType uploadType)
        {
            return await GetUploadDirectoryPathAsync(site, DateTime.Now, uploadType);
        }

        public async Task<string> GetUploadDirectoryPathAsync(Site site, DateTime datetime, UploadType uploadType)
        {
            var uploadDateFormatString = DateFormatType.Month;
            var uploadDirectoryName = string.Empty;

            if (uploadType == UploadType.Image)
            {
                uploadDateFormatString = site.ImageUploadDateFormatString;
                uploadDirectoryName = site.ImageUploadDirectoryName;
            }
            else if (uploadType == UploadType.Audio)
            {
                uploadDateFormatString = site.AudioUploadDateFormatString;
                uploadDirectoryName = site.AudioUploadDirectoryName;
            }
            else if (uploadType == UploadType.Video)
            {
                uploadDateFormatString = site.VideoUploadDateFormatString;
                uploadDirectoryName = site.VideoUploadDirectoryName;
            }
            else if (uploadType == UploadType.File)
            {
                uploadDateFormatString = site.FileUploadDateFormatString;
                uploadDirectoryName = site.FileUploadDirectoryName;
            }
            else if (uploadType == UploadType.Special)
            {
                uploadDateFormatString = site.FileUploadDateFormatString;
                uploadDirectoryName = "/special";
            }

            string directoryPath;
            var dateFormatType = uploadDateFormatString;
            var sitePath = await GetSitePathAsync(site);
            if (dateFormatType == DateFormatType.Year)
            {
                directoryPath = PathUtils.Combine(sitePath, uploadDirectoryName, datetime.Year.ToString());
            }
            else if (dateFormatType == DateFormatType.Day)
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

        public string GetUploadFileName(Site site, string filePath)
        {
            var fileExtension = PathUtils.GetExtension(filePath);

            var isUploadChangeFileName = site.IsFileUploadChangeFileName;
            if (IsImageExtensionAllowed(site, fileExtension))
            {
                isUploadChangeFileName = site.IsImageUploadChangeFileName;
            }
            else if (IsVideoExtensionAllowed(site, fileExtension))
            {
                isUploadChangeFileName = site.IsVideoUploadChangeFileName;
            }

            return PathUtils.GetUploadFileName(filePath, isUploadChangeFileName);
        }

        private bool Contains(string text, string inner)
        {
            return text?.IndexOf(inner, StringComparison.Ordinal) >= 0;
        }

        public async Task<Site> GetSiteAsync(string path)
        {
            var directoryPath = StringUtils.ToLower(DirectoryUtils.GetDirectoryPath(path)).Trim(' ', '/', '\\');
            var applicationPath = StringUtils.ToLower(_settingsManager.WebRootPath).Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty) return null;

            var siteList = await _siteRepository.GetSitesAsync();

            Site headquarter = null;
            foreach (var site in siteList)
            {
                if (site.Root)
                {
                    headquarter = site;
                }
                else
                {
                    if (Contains(directoryDir, StringUtils.ToLower(site.SiteDir)))
                    {
                        return site;
                    }
                }
            }

            return headquarter;
        }

        public async Task<string> GetSiteDirAsync(string path)
        {
            var siteDir = string.Empty;
            var directoryPath = StringUtils.ToLower(DirectoryUtils.GetDirectoryPath(path)).Trim(' ', '/', '\\');
            var applicationPath = StringUtils.ToLower(_settingsManager.WebRootPath).Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty)
            {
                return string.Empty;
            }

            var siteList = await _siteRepository.GetSitesAsync();
            foreach (var site in siteList)
            {
                if (site?.Root != false) continue;

                if (Contains(directoryDir, StringUtils.ToLower(site.SiteDir)))
                {
                    siteDir = site.SiteDir;
                }
            }

            return string.IsNullOrWhiteSpace(siteDir) ? siteDir : PathUtils.GetDirectoryName(siteDir, false);
        }

        public async Task<int> GetCurrentSiteIdAsync()
        {
            int siteId;
            var siteIdList = await _siteRepository.GetSiteIdsAsync();
            if (siteIdList.Any())
            {
                siteId = siteIdList.First();
            }
            else
            {
                siteId = await _siteRepository.GetIdByIsRootAsync();
            }
            return siteId;
        }

        public string AddVirtualToPath(string path)
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

        //将编辑器中图片上传至本机
        public async Task<string> SaveImageAsync(Site site, string content)
        {
            var originalImageSrcs = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var originalImageSrc in originalImageSrcs)
            {
                if (!PageUtils.IsProtocolUrl(originalImageSrc) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, "/") ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, await GetWebUrlAsync(site)))
                    continue;
                var fileExtName = PageUtils.GetExtensionFromUrl(originalImageSrc);
                if (!FileUtils.IsImageOrPlayer(fileExtName))
                {
                    fileExtName = ".png";
                }

                var fileName = GetUploadFileName(site, originalImageSrc);
                if (string.IsNullOrEmpty(fileName) || !StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(fileName), fileExtName))
                {
                    fileName = StringUtils.GetShortGuid(false) + fileExtName;
                }
                var directoryPath = await GetUploadDirectoryPathAsync(site, fileExtName);
                var filePath = PathUtils.Combine(directoryPath, fileName);

                try
                {
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        WebClientUtils.Download(originalImageSrc, filePath);
                        if (FileUtils.IsImage(PathUtils.GetExtension(fileName)))
                        {
                            await AddWaterMarkAsync(site, filePath);
                        }
                    }
                    var fileUrl = await GetSiteUrlByPhysicalPathAsync(site, filePath, true);
                    content = content.Replace(originalImageSrc, fileUrl);
                }
                catch
                {
                    // ignored
                }
            }
            return content;
        }

        public string GetTemporaryFilesPath(string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedPath);
        }

        public string GetSiteTemplatesPath(string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.SiteTemplates.DirectoryName, relatedPath);
        }

        public string GetSiteTemplateMetadataPath(string siteTemplatePath, string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(siteTemplatePath, DirectoryUtils.SiteFiles.SiteTemplates.SiteTemplateMetadata, relatedPath);
        }

        public async Task<string> GetChannelFilePathRuleAsync(Site site, int channelId)
        {
            var channelFilePathRule = await GetChannelFilePathRuleAsync(site.Id, channelId);
            if (string.IsNullOrEmpty(channelFilePathRule))
            {
                channelFilePathRule = site.ChannelFilePathRule;

                if (string.IsNullOrEmpty(channelFilePathRule))
                {
                    channelFilePathRule = ChannelFilePathRules.DefaultRule;
                }
            }
            return channelFilePathRule;
        }

        public async Task<string> GetChannelFilePathRuleAsync(int siteId, int channelId)
        {
            if (channelId == 0) return string.Empty;
            var nodeInfo = await _channelRepository.GetAsync(channelId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ChannelFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = await GetChannelFilePathRuleAsync(siteId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public async Task<string> GetContentFilePathRuleAsync(Site site, int channelId)
        {
            var contentFilePathRule = await GetContentFilePathRuleAsync(site.Id, channelId);
            if (string.IsNullOrEmpty(contentFilePathRule))
            {
                contentFilePathRule = site.ContentFilePathRule;

                if (string.IsNullOrEmpty(contentFilePathRule))
                {
                    contentFilePathRule = ContentFilePathRules.DefaultRule;
                }
            }
            return contentFilePathRule;
        }

        public async Task<string> GetContentFilePathRuleAsync(int siteId, int channelId)
        {
            if (channelId == 0) return string.Empty;
            var nodeInfo = await _channelRepository.GetAsync(channelId);
            if (nodeInfo == null) return string.Empty;

            var filePathRule = nodeInfo.ContentFilePathRule;
            if (string.IsNullOrEmpty(filePathRule) && nodeInfo.ParentId != 0)
            {
                filePathRule = await GetContentFilePathRuleAsync(siteId, nodeInfo.ParentId);
            }

            return filePathRule;
        }

        public async Task<string> GetChannelPageFilePathAsync(Site site, int channelId, int currentPageIndex)
        {
            var nodeInfo = await _channelRepository.GetAsync(channelId);
            if (nodeInfo.ParentId == 0)
            {
                var templateInfo = await _templateRepository.GetDefaultTemplateAsync(site.Id, TemplateType.IndexPageTemplate);
                return await GetIndexPageFilePathAsync(site, templateInfo.CreatedFileFullName, site.Root, currentPageIndex);
            }
            var filePath = nodeInfo.FilePath;

            if (string.IsNullOrEmpty(filePath))
            {
                var rules = new ChannelFilePathRules(this, _databaseManager);
                filePath = await rules.ParseAsync(site, channelId);
            }

            filePath = await ParseSitePathAsync(site, filePath);// PathUtils.Combine(sitePath, filePath);
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

        public async Task<string> GetContentPageFilePathAsync(Site site, int channelId, int contentId, int currentPageIndex)
        {
            var contentInfo = await _contentRepository.GetAsync(site, channelId, contentId);
            return await GetContentPageFilePathAsync(site, channelId, contentInfo, currentPageIndex);
        }

        public async Task<string> GetContentPageFilePathAsync(Site site, int channelId, Content content, int currentPageIndex)
        {
            var rules = new ContentFilePathRules(this, _databaseManager);
            var filePath = await rules.ParseAsync(site, channelId, content);

            filePath = await ParseSitePathAsync(site, filePath);
            if (PathUtils.IsDirectoryPath(filePath))
            {
                filePath = PathUtils.Combine(filePath, content.Id + ".html");
            }
            DirectoryUtils.CreateDirectoryIfNotExists(filePath);

            if (currentPageIndex == 0) return filePath;

            string appendix = $"_{currentPageIndex + 1}";
            var fileName = PathUtils.GetFileNameWithoutExtension(filePath) + appendix + PathUtils.GetExtension(filePath);
            filePath = PathUtils.Combine(DirectoryUtils.GetDirectoryPath(filePath), fileName);

            return filePath;
        }

        public bool IsImageExtensionAllowed(Site site, string fileExtension)
        {
            return PathUtils.IsFileExtensionAllowed(site.ImageUploadExtensions, fileExtension);
        }

        public bool IsImageSizeAllowed(Site site, long contentLength)
        {
            return contentLength <= site.ImageUploadTypeMaxSize * 1024;
        }

        public bool IsVideoExtensionAllowed(Site site, string fileExtension)
        {
            return PathUtils.IsFileExtensionAllowed(site.VideoUploadExtensions, fileExtension);
        }

        public bool IsVideoSizeAllowed(Site site, long contentLength)
        {
            return contentLength <= site.VideoUploadTypeMaxSize * 1024;
        }

        public bool IsAudioExtensionAllowed(Site site, string fileExtension)
        {
            return PathUtils.IsFileExtensionAllowed(site.AudioUploadExtensions, fileExtension);
        }

        public bool IsAudioSizeAllowed(Site site, long contentLength)
        {
            return contentLength <= site.AudioUploadTypeMaxSize * 1024;
        }

        public bool IsFileExtensionAllowed(Site site, string fileExtension)
        {
            var typeCollection = site.FileUploadExtensions + "," + site.ImageUploadExtensions + "," + site.VideoUploadExtensions + "," + site.AudioUploadExtensions;
            return PathUtils.IsFileExtensionAllowed(typeCollection, fileExtension);
        }

        public bool IsFileSizeAllowed(Site site, long contentLength)
        {
            return contentLength <= site.FileUploadTypeMaxSize * 1024;
        }

        public string GetBinDirectoryPath(string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.BinDirectoryName, relatedPath);
        }

        public string PhysicalSiteFilesPath => PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName);

        public async Task DeleteSiteFilesAsync(Site site)
        {
            if (site == null) return;

            var sitePath = await GetSitePathAsync(site);

            if (site.Root)
            {
                var filePaths = DirectoryUtils.GetFilePaths(sitePath);
                foreach (var filePath in filePaths)
                {
                    FileUtils.DeleteFileIfExists(filePath);
                }

                var siteDirList = await _databaseManager.SiteRepository.GetSiteDirsAsync(0);

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(sitePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                    if (!IsSystemDirectory(directoryName) && !ListUtils.ContainsIgnoreCase(siteDirList, directoryName))
                    {
                        DirectoryUtils.DeleteDirectoryIfExists(subDirectoryPath);
                    }
                }
            }
            else
            {
                DirectoryUtils.DeleteDirectoryIfExists(sitePath);
            }
        }

        public async Task ChangeParentSiteAsync(int oldParentSiteId, int newParentSiteId, int siteId, string siteDir)
        {
            if (oldParentSiteId == newParentSiteId) return;

            string oldPsPath;
            if (oldParentSiteId != 0)
            {
                var oldSite = await _databaseManager.SiteRepository.GetAsync(oldParentSiteId);

                oldPsPath = await GetSitePathAsync(oldSite, siteDir);
            }
            else
            {
                var site = await _databaseManager.SiteRepository.GetAsync(siteId);
                oldPsPath = await GetSitePathAsync(site);
            }

            string newPsPath;
            if (newParentSiteId != 0)
            {
                var newSite = await _databaseManager.SiteRepository.GetAsync(newParentSiteId);

                newPsPath = PathUtils.Combine(await GetSitePathAsync(newSite), siteDir);
            }
            else
            {
                newPsPath = PathUtils.Combine(_settingsManager.WebRootPath, siteDir);
            }

            if (DirectoryUtils.IsDirectoryExists(newPsPath))
            {
                throw new ArgumentException("发布系统修改失败，发布路径文件夹已存在！");
            }
            if (DirectoryUtils.IsDirectoryExists(oldPsPath))
            {
                DirectoryUtils.MoveDirectory(oldPsPath, newPsPath, false);
            }
            else
            {
                DirectoryUtils.CreateDirectoryIfNotExists(newPsPath);
            }
        }

        public async Task ChangeToRootAsync(Site site, bool isMoveFiles)
        {
            if (site.Root == false)
            {
                var sitePath = await GetSitePathAsync(site);

                await _databaseManager.SiteRepository.UpdateParentIdToZeroAsync(site.Id);

                site.Root = true;
                site.SiteDir = string.Empty;

                await _databaseManager.SiteRepository.UpdateAsync(site);
                if (isMoveFiles)
                {
                    DirectoryUtils.MoveDirectory(sitePath, _settingsManager.WebRootPath, false);
                    DirectoryUtils.DeleteDirectoryIfExists(sitePath);
                }
            }
        }

        public async Task ChangeToSubSiteAsync(Site site, string siteDir, IList<string> directories, IList<string> files)
        {
            if (site.Root)
            {
                site.Root = false;
                site.SiteDir = siteDir;

                await _databaseManager.SiteRepository.UpdateAsync(site);

                var psPath = PathUtils.Combine(_settingsManager.WebRootPath, siteDir);
                DirectoryUtils.CreateDirectoryIfNotExists(psPath);
                if (directories != null)
                {
                    foreach (var fileSystemName in directories)
                    {
                        var srcPath = PathUtils.Combine(_settingsManager.WebRootPath, fileSystemName);
                        if (DirectoryUtils.IsDirectoryExists(srcPath))
                        {
                            var destDirectoryPath = PathUtils.Combine(psPath, fileSystemName);
                            DirectoryUtils.CreateDirectoryIfNotExists(destDirectoryPath);
                            DirectoryUtils.MoveDirectory(srcPath, destDirectoryPath, false);
                            DirectoryUtils.DeleteDirectoryIfExists(srcPath);
                        }
                    }
                }

                if (files != null)
                {
                    foreach (var fileSystemName in files)
                    {
                        var srcPath = PathUtils.Combine(_settingsManager.WebRootPath, fileSystemName);
                        if (FileUtils.IsFileExists(srcPath))
                        {
                            FileUtils.CopyFile(srcPath, PathUtils.Combine(psPath, fileSystemName));
                            FileUtils.DeleteFileIfExists(srcPath);
                        }
                    }
                }
            }
        }

        public bool IsSystemDirectory(string directoryName)
        {
            return StringUtils.EqualsIgnoreCase(directoryName, Constants.AdminDirectory) ||
                   StringUtils.EqualsIgnoreCase(directoryName, Constants.HomeDirectory) ||
                   StringUtils.EqualsIgnoreCase(directoryName, DirectoryUtils.SiteFiles.DirectoryName);
        }

        public async Task AddWaterMarkAsync(Site site, string imagePath)
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
                                OldImageUtils.AddImageWaterMark(imagePath, await ParseSitePathAsync(site, site.WaterMarkImagePath), site.WaterMarkPosition, site.WaterMarkTransparency, site.WaterMarkMinWidth, site.WaterMarkMinHeight);
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(site.WaterMarkFormatString))
                            {
                                var now = DateTime.Now;
                                OldImageUtils.AddTextWaterMark(imagePath, string.Format(site.WaterMarkFormatString, DateUtils.GetDateString(now), DateUtils.GetTimeString(now)), site.WaterMarkFontName, site.WaterMarkFontSize, site.WaterMarkPosition, site.WaterMarkTransparency, site.WaterMarkMinWidth, site.WaterMarkMinHeight);
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

        public async Task MoveFileAsync(Site sourceSite, Site destSite, string relatedUrl)
        {
            if (!string.IsNullOrEmpty(relatedUrl))
            {
                var sourceFilePath = await ParseSitePathAsync(sourceSite, relatedUrl);
                var descFilePath = await ParseSitePathAsync(destSite, relatedUrl);
                if (FileUtils.IsFileExists(sourceFilePath))
                {
                    FileUtils.MoveFile(sourceFilePath, descFilePath, false);
                }
            }
        }

        public async Task MoveFileByContentAsync(Site sourceSite, Site destSite, Content content)
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
                    if (!fileUrls.Contains(url) && IsVirtualUrl(url))
                    {
                        fileUrls.Add(url);
                    }
                }

                foreach (var fileUrl in fileUrls)
                {
                    if (!string.IsNullOrEmpty(fileUrl) && IsVirtualUrl(fileUrl))
                    {
                        await MoveFileAsync(sourceSite, destSite, fileUrl);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
