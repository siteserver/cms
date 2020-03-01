using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Core;
using SS.CMS.Core.PathRules;

namespace SS.CMS.Services
{
    public partial class PathManager
    {
        // 系统根目录访问地址
        public string GetRootUrl(string relatedUrl)
        {
            return PageUtils.Combine(WebUrl, relatedUrl);
        }

        public string GetRootPath(params string[] paths)
        {
            var path = PathUtils.Combine(_settingsManager.WebRootPath, PathUtils.Combine(paths));
            return path;
        }

        public string GetAdminUrl(string relatedUrl)
        {
            return PageUtils.Combine(WebUrl, _settingsManager.AdminDirectory, relatedUrl);
        }

        public string GetHomeUrl(string relatedUrl)
        {
            return PageUtils.Combine(WebUrl, _settingsManager.HomeDirectory, relatedUrl);
        }

        public string GetTemporaryFilesUrl(string relatedUrl)
        {
            return PageUtils.Combine(WebUrl, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.TemporaryFiles, relatedUrl);
        }

        public string GetSiteTemplatesUrl(string relatedUrl)
        {
            return PageUtils.Combine(WebUrl, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteTemplates.DirectoryName, relatedUrl);
        }

        public string ParsePluginUrl(string pluginId, string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (PageUtils.IsProtocolUrl(url)) return url;

            if (StringUtils.StartsWith(url, "~/"))
            {
                return GetRootUrl(url.Substring(1));
            }

            if (StringUtils.StartsWith(url, "@/"))
            {
                return GetAdminUrl(url.Substring(1));
            }

            return GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, pluginId, url));
        }

        public string GetRootUrlByPhysicalPath(string physicalPath)
        {
            var requestPath = PathUtils.GetPathDifference(_settingsManager.WebRootPath, physicalPath);
            requestPath = requestPath.Replace(PathUtils.SeparatorChar, Constants.PageSeparatorChar);
            return GetRootUrl(requestPath);
        }

        public string ParseNavigationUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            url = url.StartsWith("~") ? PageUtils.Combine(WebUrl, url.Substring(1)) : url;
            url = url.Replace(PathUtils.SeparatorChar, Constants.PageSeparatorChar);
            return url;
        }

        //根据发布系统属性判断是否为相对路径并返回解析后路径
        public async Task<string> ParseNavigationUrlAsync(Site site, string url, bool isLocal)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (site != null)
            {
                if (!string.IsNullOrEmpty(url) && url.StartsWith("@"))
                {
                    return await GetSiteUrlAsync(site, url.Substring(1), isLocal);
                }
                return ParseNavigationUrl(url);
            }
            return ParseNavigationUrl(url);
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

            requestPath = requestPath.Replace(PathUtils.SeparatorChar, Constants.PageSeparatorChar);
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
                url = ParseNavigationUrl($"~/{(site.Root ? string.Empty : site.SiteDir)}");
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

            requestPath = requestPath.Replace(PathUtils.SeparatorChar, Constants.PageSeparatorChar);
            requestPath = PathUtils.RemovePathInvalidChar(requestPath);
            if (requestPath.StartsWith("/"))
            {
                requestPath = requestPath.Substring(1);
            }

            url = PageUtils.Combine(url, requestPath);

            return url;
        }

        public string GetLocalSiteUrl(int siteId)
        {
            var apiUrl = GetInnerApiUrl(Constants.RoutePreview);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            return apiUrl;
        }

        public string GetLocalChannelUrl(int siteId, int channelId)
        {
            var apiUrl = GetInnerApiUrl(Constants.RoutePreviewChannel);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            return apiUrl;
        }

        public string GetLocalContentUrl(int siteId, int channelId, int contentId)
        {
            var apiUrl = GetInnerApiUrl(Constants.RoutePreviewContent);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{channelId}", channelId.ToString());
            apiUrl = apiUrl.Replace("{contentId}", contentId.ToString());
            return apiUrl;
        }

        public string GetContentPreviewUrl(int siteId, int channelId, int contentId, int previewId)
        {
            if (contentId == 0)
            {
                contentId = previewId;
            }
            return $"{GetLocalContentUrl(siteId, channelId, contentId)}?isPreview=true&previewId={previewId}";
        }

        public string GetLocalFileUrl(int siteId, int fileTemplateId)
        {
            var apiUrl = GetInnerApiUrl(Constants.RoutePreviewFile);
            apiUrl = apiUrl.Replace("{siteId}", siteId.ToString());
            apiUrl = apiUrl.Replace("{fileTemplateId}", fileTemplateId.ToString());
            return apiUrl;
        }

        public string GetLocalSpecialUrl(int siteId, int specialId)
        {
            var apiUrl = GetInnerApiUrl(Constants.RoutePreviewSpecial);
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
                ? GetLocalSiteUrl(site.Id)
                : await ParseNavigationUrlAsync(site, createdFileFullName, false);

            return RemoveDefaultFileName(site, url);
        }

        public async Task<string> GetFileUrlAsync(Site site, int fileTemplateId, bool isLocal)
        {
            var createdFileFullName = await _templateRepository.GetCreatedFileFullNameAsync(fileTemplateId);

            var url = isLocal
                ? GetLocalFileUrl(site.Id, fileTemplateId)
                : await ParseNavigationUrlAsync(site, createdFileFullName, false);

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
                return GetLocalContentUrl(site.Id, contentCurrent.ChannelId,
                    contentCurrent.Id);
            }

            var sourceId = contentCurrent.SourceId;
            var referenceId = contentCurrent.ReferenceId;
            var linkUrl = contentCurrent.LinkUrl;
            var channelId = contentCurrent.ChannelId;
            if (referenceId > 0 && TranslateContentType.ReferenceContent == contentCurrent.TranslateContentType)
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
                return await ParseNavigationUrlAsync(site, linkUrl, false);
            }

            var rules = new ContentFilePathRules(this, _databaseManager);
            var contentUrl = await rules.ParseAsync(site, channelId, contentCurrent);
            return await GetSiteUrlAsync(site, contentUrl, false);
        }

        public async Task<string> GetContentUrlByIdAsync(Site site, int channelId, int contentId, int sourceId, int referenceId, string linkUrl, bool isLocal)
        {
            if (isLocal)
            {
                return GetLocalContentUrl(site.Id, channelId, contentId);
            }

            var contentInfoCurrent = await _contentRepository.GetAsync(site, channelId, contentId);

            if (referenceId > 0 && TranslateContentType.ReferenceContent == contentInfoCurrent.TranslateContentType)
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
                return await ParseNavigationUrlAsync(site, linkUrl, false);
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
                    return await ParseNavigationUrlAsync(site, AddVirtualToPath(filePath), isLocal);
                }
            }

            return await ParseNavigationUrlAsync(site, linkUrl, isLocal);
        }

        //得到栏目经过计算后的连接地址
        public async Task<string> GetChannelUrlAsync(Site site, Channel channel, bool isLocal)
        {
            if (channel == null) return string.Empty;

            if (isLocal)
            {
                return GetLocalChannelUrl(site.Id, channel.Id);
            }

            var url = string.Empty;

            if (channel.ParentId == 0)
            {
                url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
            }
            else
            {
                var linkType = channel.LinkType;
                if (linkType == LinkType.None)
                {
                    url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
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
                        url = count == 0 ? PageUtils.UnClickableUrl : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == LinkType.LinkToOnlyOneContent)
                    {
                        var count = await _contentRepository.GetCountAsync(site, channel);
                        if (count == 1)
                        {
                            var tableName = _channelRepository.GetTableName(site, channel);
                            var contentId = _contentRepository.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
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
                            var contentId = _contentRepository.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                        }
                    }
                    else if (linkType == LinkType.LinkToFirstContent)
                    {
                        var count = await _contentRepository.GetCountAsync(site, channel);
                        if (count >= 1)
                        {
                            var tableName = _channelRepository.GetTableName(site, channel);
                            var contentId = _contentRepository.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                        }
                    }
                    else if (linkType == LinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
                    {
                        var count = await _contentRepository.GetCountAsync(site, channel);
                        if (count >= 1)
                        {
                            var tableName = _channelRepository.GetTableName(site, channel);
                            var contentId = _contentRepository.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = PageUtils.UnClickableUrl;
                        }
                    }
                    else if (linkType == LinkType.NoLinkIfChannelNotExists)
                    {
                        url = channel.ChildrenCount == 0 ? PageUtils.UnClickableUrl : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == LinkType.LinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = await _channelRepository.GetChannelByLastAddDateAsyncTask(site.Id, channel.Id);
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(site, lastAddChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == LinkType.LinkToFirstChannel)
                    {
                        var firstChannelInfo = await _channelRepository.GetChannelByTaxisAsync(site.Id, channel.Id);
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(site, firstChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == LinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = await _channelRepository.GetChannelByLastAddDateAsyncTask(site.Id, channel.Id);
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(site, lastAddChannelInfo, isLocal) : PageUtils.UnClickableUrl;
                    }
                    else if (linkType == LinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                    {
                        var firstChannelInfo = await _channelRepository.GetChannelByTaxisAsync(site.Id, channel.Id);
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(site, firstChannelInfo, isLocal) : PageUtils.UnClickableUrl;
                    }
                }
            }

            return RemoveDefaultFileName(site, url);
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

            if (!url.StartsWith("@") && !url.StartsWith("~"))
            {
                resolvedUrl = PageUtils.Combine("@/", url);
            }
            return resolvedUrl;
        }

        public string GetVirtualUrl(Site site, string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            var relatedSiteUrl = ParseNavigationUrl($"~/{site.SiteDir}");
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

        public string GetSiteFilesUrl(string relatedUrl)
        {
            return PageUtils.Combine(WebUrl, DirectoryUtils.SiteFiles.DirectoryName, relatedUrl);
        }

        public string GetSiteFilesUrl(string apiUrl, string relatedUrl)
        {
            if (string.IsNullOrEmpty(apiUrl))
            {
                apiUrl = "/api";
            }
            apiUrl = apiUrl.Trim().ToLower();
            if (apiUrl == "/api")
            {
                apiUrl = "/";
            }
            else if (apiUrl.EndsWith("/api"))
            {
                apiUrl = apiUrl.Substring(0, apiUrl.LastIndexOf("/api", StringComparison.Ordinal));
            }
            else if (apiUrl.EndsWith("/api/"))
            {
                apiUrl = apiUrl.Substring(0, apiUrl.LastIndexOf("/api/", StringComparison.Ordinal));
            }
            if (string.IsNullOrEmpty(apiUrl))
            {
                apiUrl = "/";
            }
            return PageUtils.Combine(apiUrl, DirectoryUtils.SiteFiles.DirectoryName, relatedUrl);
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
                var path = t?.Replace(Constants.PageSeparatorChar, PathUtils.SeparatorChar).Trim(PathUtils.SeparatorChar) ?? string.Empty;
                sitePath = PathUtils.Combine(sitePath, path);
            }
            return sitePath;
        }

        public async Task<string> GetSitePathAsync(int siteId, params string[] paths)
        {
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

            var filePath = await MapPathAsync(site, createFileFullName);

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
            var extention = ".zip";
            var siteName = site.SiteDir;
            if (!string.IsNullOrEmpty(siteName))
            {
                siteName += "_";
            }
            if (backupType == BackupType.Templates)
            {
                extention = ".xml";
            }
            return PathUtils.Combine(PhysicalSiteFilesPath, DirectoryUtils.SiteFiles.BackupFiles, site.SiteDir, DateTime.Now.ToString("yyyy-MM"), backupType.GetValue() + "_" + siteName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + extention);
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

            return GetUploadFileName(filePath, isUploadChangeFileName);
        }

        public string GetUploadFileName(string filePath, bool isUploadChangeFileName)
        {
            if (isUploadChangeFileName)
            {
                return $"{StringUtils.GetShortGuid(false)}{PathUtils.GetExtension(filePath)}";
            }

            var fileName = PathUtils.GetFileNameWithoutExtension(filePath);
            fileName = PathUtils.GetSafeFilename(fileName);
            return $"{fileName}{PathUtils.GetExtension(filePath)}";
        }

        public async Task<Site> GetSiteAsync(string path)
        {
            var directoryPath = DirectoryUtils.GetDirectoryPath(path).ToLower().Trim(' ', '/', '\\');
            var applicationPath = _settingsManager.WebRootPath.ToLower().Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty) return null;

            var siteList = await _siteRepository.GetSiteListAsync();

            Site headquarter = null;
            foreach (var site in siteList)
            {
                if (site.Root)
                {
                    headquarter = site;
                }
                else
                {
                    if (StringUtils.Contains(directoryDir, site.SiteDir.ToLower()))
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
            var directoryPath = DirectoryUtils.GetDirectoryPath(path).ToLower().Trim(' ', '/', '\\');
            var applicationPath = _settingsManager.WebRootPath.ToLower().Trim(' ', '/', '\\');
            var directoryDir = StringUtils.ReplaceStartsWith(directoryPath, applicationPath, string.Empty).Trim(' ', '/', '\\');
            if (directoryDir == string.Empty)
            {
                return string.Empty;
            }

            var siteList = await _siteRepository.GetSiteListAsync();
            foreach (var site in siteList)
            {
                if (site?.Root != false) continue;

                if (StringUtils.Contains(directoryDir, site.SiteDir.ToLower()))
                {
                    siteDir = site.SiteDir;
                }
            }

            return string.IsNullOrWhiteSpace(siteDir) ? siteDir : PathUtils.GetDirectoryName(siteDir, false);
        }

        public async Task<int> GetCurrentSiteIdAsync()
        {
            int siteId;
            var siteIdList = await _siteRepository.GetSiteIdListAsync();
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

        public async Task<string> MapPathAsync(Site site, string virtualPath)
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
            if (!virtualPath.StartsWith("@")) return MapPath(resolvedPath);

            if (site != null)
            {
                return await GetSitePathAsync(site, virtualPath.Substring(1));
            }
            return MapPath(resolvedPath);
        }

        public async Task<string> MapPathAsync(Site site, string virtualPath, bool isCopyToSite)
        {
            if (!isCopyToSite) return await MapPathAsync(site, virtualPath);

            var resolvedPath = virtualPath;
            if (string.IsNullOrEmpty(virtualPath))
            {
                virtualPath = "@";
            }
            if (!virtualPath.StartsWith("@") && !virtualPath.StartsWith("~"))
            {
                virtualPath = "@" + virtualPath;
            }
            if (!virtualPath.StartsWith("@")) return MapPath(resolvedPath);

            if (site != null)
            {
                return await GetSitePathAsync(site, virtualPath.Substring(1));
            }
            return MapPath(resolvedPath);
        }

        public string MapPath(string directoryPath, string virtualPath)
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
            return MapPath(resolvedPath);
        }

        //将编辑器中图片上传至本机
        public async Task<string> SaveImageAsync(Site site, string content)
        {
            var originalImageSrcs = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var originalImageSrc in originalImageSrcs)
            {
                if (!PageUtils.IsProtocolUrl(originalImageSrc) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, WebUrl) ||
                    StringUtils.StartsWithIgnoreCase(originalImageSrc, await GetWebUrlAsync(site)))
                    continue;
                var fileExtName = PageUtils.GetExtensionFromUrl(originalImageSrc);
                if (!FileUtils.IsImageOrPlayer(fileExtName)) continue;

                var fileName = GetUploadFileName(site, originalImageSrc);
                var directoryPath = await GetUploadDirectoryPathAsync(site, fileExtName);
                var filePath = PathUtils.Combine(directoryPath, fileName);

                try
                {
                    if (!FileUtils.IsFileExists(filePath))
                    {
                        WebClientUtils.SaveRemoteFileToLocal(originalImageSrc, filePath);
                        if (FileUtils.IsImage(PathUtils.GetExtension(fileName)))
                        {
                            await FileUtility.AddWaterMarkAsync(this, site, filePath);
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
            return PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteTemplates.DirectoryName, relatedPath);
        }

        public string GetSiteTemplateMetadataPath(string siteTemplatePath, string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(siteTemplatePath, DirectoryUtils.SiteTemplates.SiteTemplateMetadata, relatedPath);
        }

        public bool IsSystemFile(string fileName)
        {
            if (StringUtils.EqualsIgnoreCase(fileName, "Web.config")
                || StringUtils.EqualsIgnoreCase(fileName, "Global.asax")
                || StringUtils.EqualsIgnoreCase(fileName, "robots.txt"))
            {
                return true;
            }
            return false;
        }

        public bool IsSystemFileForChangeSiteType(string fileName)
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

            filePath = await MapPathAsync(site, filePath);// PathUtils.Combine(sitePath, filePath);
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

            filePath = await MapPathAsync(site, filePath);
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

        public bool IsImageExtensionAllowed(Site site, string fileExtention)
        {
            return PathUtils.IsFileExtensionAllowed(site.ImageUploadTypeCollection, fileExtention);
        }

        public bool IsImageSizeAllowed(Site site, long contentLength)
        {
            return contentLength <= site.ImageUploadTypeMaxSize * 1024;
        }

        public bool IsVideoExtensionAllowed(Site site, string fileExtention)
        {
            return PathUtils.IsFileExtensionAllowed(site.VideoUploadTypeCollection, fileExtention);
        }

        public bool IsVideoSizeAllowed(Site site, int contentLength)
        {
            return contentLength <= site.VideoUploadTypeMaxSize * 1024;
        }

        public bool IsFileExtensionAllowed(Site site, string fileExtention)
        {
            var typeCollection = site.FileUploadTypeCollection + "," + site.ImageUploadTypeCollection + "," + site.VideoUploadTypeCollection;
            return PathUtils.IsFileExtensionAllowed(typeCollection, fileExtention);
        }

        public bool IsFileSizeAllowed(Site site, int contentLength)
        {
            return contentLength <= site.FileUploadTypeMaxSize * 1024;
        }

        public bool IsUploadExtensionAllowed(UploadType uploadType, Site site, string fileExtention)
        {
            if (uploadType == UploadType.Image)
            {
                return IsImageExtensionAllowed(site, fileExtention);
            }
            else if (uploadType == UploadType.Video)
            {
                return IsVideoExtensionAllowed(site, fileExtention);
            }
            else if (uploadType == UploadType.File)
            {
                return IsFileExtensionAllowed(site, fileExtention);
            }
            return false;
        }

        public bool IsUploadSizeAllowed(UploadType uploadType, Site site, int contentLength)
        {
            switch (uploadType)
            {
                case UploadType.Image:
                    return IsImageSizeAllowed(site, contentLength);
                case UploadType.Video:
                    return IsVideoSizeAllowed(site, contentLength);
                case UploadType.File:
                    return IsFileSizeAllowed(site, contentLength);
            }
            return false;
        }

        public string GetBinDirectoryPath(string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.Bin.DirectoryName, relatedPath);
        }

        public string GetAdminDirectoryPath(string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(_settingsManager.WebRootPath, _settingsManager.AdminDirectory, relatedPath);
        }

        public string GetHomeDirectoryPath(string relatedPath)
        {
            relatedPath = PathUtils.RemoveParentPath(relatedPath);
            return PathUtils.Combine(_settingsManager.WebRootPath, _settingsManager.HomeDirectory, relatedPath);
        }

        public string PhysicalSiteFilesPath => PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName);

        public string GetLibraryFilePath(string virtualUrl)
        {
            return PathUtils.Combine(_settingsManager.WebRootPath, virtualUrl);
        }

        public async Task DeleteSiteFilesAsync(Site site)
        {
            if (site == null) return;

            var sitePath = await GetSitePathAsync(site);

            if (site.Root)
            {
                var filePaths = DirectoryUtils.GetFilePaths(sitePath);
                foreach (var filePath in filePaths)
                {
                    var fileName = PathUtils.GetFileName(filePath);
                    if (!IsSystemFile(fileName))
                    {
                        FileUtils.DeleteFileIfExists(filePath);
                    }
                }

                var siteDirList = await _databaseManager.SiteRepository.GetSiteDirListAsync(0);

                var directoryPaths = DirectoryUtils.GetDirectoryPaths(sitePath);
                foreach (var subDirectoryPath in directoryPaths)
                {
                    var directoryName = PathUtils.GetDirectoryName(subDirectoryPath, false);
                    if (!IsSystemDirectory(directoryName) && !StringUtils.ContainsIgnoreCase(siteDirList, directoryName))
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
            if (StringUtils.EqualsIgnoreCase(directoryName, DirectoryUtils.AspnetClient.DirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, DirectoryUtils.Bin.DirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, DirectoryUtils.Home.DirectoryName)
                || StringUtils.EqualsIgnoreCase(directoryName, DirectoryUtils.SiteFiles.DirectoryName))
            {
                return true;
            }
            return false;
        }
    }
}
