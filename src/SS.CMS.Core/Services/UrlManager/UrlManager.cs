using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Models.Enumerations;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class UrlManager : IUrlManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ISpecialRepository _specialRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public UrlManager(ISettingsManager settingsManager, IPathManager pathManager, IPluginManager pluginManager, IConfigRepository configRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, ISpecialRepository specialRepository, ITemplateRepository templateRepository, IErrorLogRepository errorLogRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _specialRepository = specialRepository;
            _templateRepository = templateRepository;
            _errorLogRepository = errorLogRepository;
        }

        public string GetRootUrl(string relatedUrl)
        {
            return PageUtils.Combine(Constants.ApplicationPath, relatedUrl);
        }

        public string GetApiUrl(string route)
        {
            return PageUtils.Combine(Constants.ApiUrl, route);
        }

        public async Task<string> GetSystemDefaultPageUrlAsync(int siteId)
        {
            string pageUrl = null;

            foreach (var service in await _pluginManager.GetServicesAsync())
            {
                if (service.SystemDefaultPageUrl == null) continue;

                try
                {
                    pageUrl = GetMenuUrl(service.PluginId, service.SystemDefaultPageUrl, siteId, 0, 0);
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(service.PluginId, ex);
                }
            }

            return pageUrl;
        }

        public async Task<string> GetHomeDefaultPageUrlAsync()
        {
            string pageUrl = null;

            foreach (var service in await _pluginManager.GetServicesAsync())
            {
                if (service.HomeDefaultPageUrl == null) continue;

                try
                {
                    pageUrl = GetMenuUrl(service.PluginId, service.HomeDefaultPageUrl, 0, 0, 0);
                }
                catch (Exception ex)
                {
                    await _errorLogRepository.AddErrorLogAsync(service.PluginId, ex);
                }
            }

            return pageUrl;
        }

        public string GetMenuUrl(string pluginId, string href, int siteId, int channelId, int contentId)
        {
            if (PageUtils.IsAbsoluteUrl(href))
            {
                return href;
            }

            var url = PageUtils.AddQueryStringIfNotExists(ParsePluginUrl(pluginId, href), new NameValueCollection
            {
                {"v", StringUtils.GetRandomInt(1, 1000).ToString()},
                {"pluginId", pluginId}
            });
            if (siteId > 0)
            {
                url = PageUtils.AddQueryStringIfNotExists(url, new NameValueCollection
                {
                    {"siteId", siteId.ToString()}
                });
            }
            if (channelId > 0)
            {
                url = PageUtils.AddQueryStringIfNotExists(url, new NameValueCollection
                {
                    {"channelId", channelId.ToString()}
                });
            }
            if (contentId > 0)
            {
                url = PageUtils.AddQueryStringIfNotExists(url, new NameValueCollection
                {
                    {"contentId", contentId.ToString()}
                });
            }
            return url;
        }

        //public string GetMenuContentHref(string pluginId, string href, int siteId, int channelId, int contentId, string returnUrl)
        //{
        //    if (PageUtils.IsAbsoluteUrl(href))
        //    {
        //        return href;
        //    }
        //    return PageUtils.AddQueryStringIfNotExists(PageUtils.ParsePluginUrl(pluginId, href), new NameValueCollection
        //    {
        //        {"siteId", siteId.ToString()},
        //        {"channelId", channelId.ToString()},
        //        {"contentId", contentId.ToString()},
        //        {"apiUrl", ApiManager.ApiUrl},
        //        {"returnUrl", returnUrl},
        //        {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
        //    });
        //}

        //public string GetMenuContentHrefPrefix(string pluginId, string href)
        //{
        //    if (PageUtils.IsAbsoluteUrl(href))
        //    {
        //        return href;
        //    }
        //    return PageUtils.AddQueryStringIfNotExists(PageUtils.ParsePluginUrl(pluginId, href), new NameValueCollection
        //    {
        //        {"apiUrl", ApiManager.ApiUrl},
        //        {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
        //    });
        //}

        public string GetWebUrl(SiteInfo siteInfo, params string[] paths)
        {
            var webUrl = siteInfo.IsSeparatedWeb
                ? siteInfo.SeparatedWebUrl
                : ParseNavigationUrl($"~/{siteInfo.SiteDir}");
            webUrl = PageUtils.Combine(webUrl, PageUtils.Combine(paths));
            return RemoveDefaultFileName(siteInfo, webUrl);
        }

        public string GetAssetsUrl(SiteInfo siteInfo, params string[] paths)
        {
            var assetsUrl = siteInfo.IsSeparatedAssets
                ? siteInfo.SeparatedAssetsUrl
                : PageUtils.Combine(GetWebUrl(siteInfo), siteInfo.AssetsDir);
            return PageUtils.Combine(assetsUrl, PageUtils.Combine(paths));
        }

        public string GetHomeUrl(SiteInfo siteInfo, params string[] paths)
        {
            var assetsUrl = siteInfo.IsSeparatedAssets
                ? siteInfo.SeparatedAssetsUrl
                : PageUtils.Combine(GetWebUrl(siteInfo), siteInfo.AssetsDir);
            return PageUtils.Combine(assetsUrl, PageUtils.Combine(paths));
        }

        public string GetSiteUrl(SiteInfo siteInfo, bool isLocal)
        {
            return GetSiteUrl(siteInfo, string.Empty, isLocal);
        }

        public string GetSiteUrl(SiteInfo siteInfo, string requestPath, bool isLocal)
        {
            var url = isLocal
                ? GetLocalSiteUrl(siteInfo, requestPath)
                : GetRemoteSiteUrl(siteInfo, requestPath);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public string GetSiteUrlByPhysicalPath(SiteInfo siteInfo, string physicalPath, bool isLocal)
        {
            if (string.IsNullOrEmpty(physicalPath)) return GetWebUrl(siteInfo);

            var sitePath = _pathManager.GetSitePath(siteInfo);
            var requestPath = StringUtils.StartsWithIgnoreCase(physicalPath, sitePath)
                ? StringUtils.ReplaceStartsWithIgnoreCase(physicalPath, sitePath, string.Empty)
                : string.Empty;

            return GetSiteUrl(siteInfo, requestPath, isLocal);
        }

        private string GetRemoteSiteUrl(SiteInfo siteInfo, string requestPath)
        {
            var url = GetWebUrl(siteInfo);

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

            if (!siteInfo.IsSeparatedAssets) return url;

            var assetsUrl = PageUtils.Combine(GetWebUrl(siteInfo),
                siteInfo.AssetsDir);
            if (StringUtils.StartsWithIgnoreCase(url, assetsUrl))
            {
                url = StringUtils.ReplaceStartsWithIgnoreCase(url, assetsUrl, GetAssetsUrl(siteInfo));
            }

            return url;
        }

        private string GetLocalSiteUrl(SiteInfo siteInfo, string requestPath)
        {
            var url = ParseNavigationUrl($"~/{siteInfo.SiteDir}");

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

        // 得到发布系统首页地址
        public string GetIndexPageUrl(SiteInfo siteInfo, bool isLocal)
        {
            var indexTemplateId = _templateRepository.GetIndexTemplateId(siteInfo.Id);
            var createdFileFullName = _templateRepository.GetCreatedFileFullName(siteInfo.Id, indexTemplateId);

            var url = isLocal
                ? GetPreviewSiteUrl(siteInfo.Id)
                : ParseNavigationUrl(siteInfo, createdFileFullName, false);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public string GetFileUrl(SiteInfo siteInfo, int fileTemplateId, bool isLocal)
        {
            var createdFileFullName = _templateRepository.GetCreatedFileFullName(siteInfo.Id, fileTemplateId);

            var url = isLocal
                ? GetPreviewFileUrl(siteInfo.Id, fileTemplateId)
                : ParseNavigationUrl(siteInfo, createdFileFullName, false);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public async Task<string> GetContentUrlAsync(SiteInfo siteInfo, ContentInfo contentInfo, bool isLocal)
        {
            return await GetContentUrlByIdAsync(siteInfo, contentInfo, isLocal);
        }

        public async Task<string> GetContentUrlAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId, bool isLocal)
        {
            var contentInfo = channelInfo.ContentRepository.GetContentInfo(siteInfo, channelInfo, contentId);
            return await GetContentUrlByIdAsync(siteInfo, contentInfo, isLocal);
        }

        /// <summary>
        /// 对GetContentUrlByID的优化
        /// 通过传入参数contentInfoCurrent，避免对ContentInfo查询太多
        /// </summary>
        private async Task<string> GetContentUrlByIdAsync(SiteInfo siteInfo, ContentInfo contentInfoCurrent, bool isLocal)
        {
            if (contentInfoCurrent == null) return PageUtils.UnClickableUrl;

            if (isLocal)
            {
                return GetPreviewContentUrl(siteInfo.Id, contentInfoCurrent.ChannelId,
                    contentInfoCurrent.Id);
            }

            var sourceId = contentInfoCurrent.SourceId;
            var referenceId = contentInfoCurrent.ReferenceId;
            var linkUrl = contentInfoCurrent.LinkUrl;
            var channelId = contentInfoCurrent.ChannelId;
            if (referenceId > 0 && contentInfoCurrent.TranslateContentType != TranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (await _channelRepository.IsExistsAsync(siteInfo.Id, sourceId) || await _channelRepository.IsExistsAsync(_siteRepository, sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = _channelRepository.StlGetSiteId(targetChannelId);
                    var targetSiteInfo = _siteRepository.GetSiteInfo(targetSiteId);
                    var targetChannelInfo = await _channelRepository.GetChannelInfoAsync(targetSiteId, targetChannelId);

                    var contentInfo = targetChannelInfo.ContentRepository.GetContentInfo(targetSiteInfo, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnClickableUrl;
                    }
                    if (contentInfo.SiteId == targetSiteInfo.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSiteInfo, contentInfo, false);
                    }
                    var siteInfoTmp = _siteRepository.GetSiteInfo(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteInfoTmp, contentInfo, false);
                }
                else
                {
                    var channelInfo = await _channelRepository.GetChannelInfoAsync(siteInfo.Id, channelId);
                    channelId = channelInfo.ContentRepository.StlGetChannelId(channelInfo, referenceId);
                    linkUrl = channelInfo.ContentRepository.StlGetValue(channelInfo, referenceId, ContentAttribute.LinkUrl);
                    if (await _channelRepository.IsExistsAsync(siteInfo.Id, channelId))
                    {
                        return await GetContentUrlByIdAsync(siteInfo, channelId, referenceId, 0, 0, linkUrl, false);
                    }
                    var targetSiteId = _channelRepository.StlGetSiteId(channelId);
                    var targetSiteInfo = _siteRepository.GetSiteInfo(targetSiteId);
                    return await GetContentUrlByIdAsync(targetSiteInfo, channelId, referenceId, 0, 0, linkUrl, false);
                }
            }

            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(siteInfo, linkUrl, false);
            }
            var contentUrl = await _pathManager.ContentRulesParseAsync(siteInfo, channelId, contentInfoCurrent);
            return GetSiteUrl(siteInfo, contentUrl, false);
        }

        private async Task<string> GetContentUrlByIdAsync(SiteInfo siteInfo, int channelId, int contentId, int sourceId, int referenceId, string linkUrl, bool isLocal)
        {
            if (isLocal)
            {
                return GetPreviewContentUrl(siteInfo.Id, channelId, contentId);
            }

            var channelInfo = await _channelRepository.GetChannelInfoAsync(siteInfo.Id, channelId);
            var contentInfoCurrent = channelInfo.ContentRepository.GetContentInfo(siteInfo, channelInfo, contentId);

            if (referenceId > 0 && contentInfoCurrent.TranslateContentType != TranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (await _channelRepository.IsExistsAsync(siteInfo.Id, sourceId) || await _channelRepository.IsExistsAsync(_siteRepository, sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = _channelRepository.StlGetSiteId(targetChannelId);
                    var targetSiteInfo = _siteRepository.GetSiteInfo(targetSiteId);
                    var targetChannelInfo = await _channelRepository.GetChannelInfoAsync(targetSiteId, targetChannelId);

                    var contentInfo = targetChannelInfo.ContentRepository.GetContentInfo(targetSiteInfo, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnClickableUrl;
                    }
                    if (contentInfo.SiteId == targetSiteInfo.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSiteInfo, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                    }
                    var siteInfoTmp = _siteRepository.GetSiteInfo(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteInfoTmp, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                }
                else
                {
                    channelId = channelInfo.ContentRepository.StlGetChannelId(channelInfo, referenceId);
                    linkUrl = channelInfo.ContentRepository.StlGetValue(channelInfo, referenceId, ContentAttribute.LinkUrl);
                    return await GetContentUrlByIdAsync(siteInfo, channelId, referenceId, 0, 0, linkUrl, false);
                }
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(siteInfo, linkUrl, false);
            }
            var contentUrl = await _pathManager.ContentRulesParseAsync(siteInfo, channelId, contentId);
            return GetSiteUrl(siteInfo, contentUrl, false);
        }

        private async Task<string> GetChannelUrlNotComputedAsync(SiteInfo siteInfo, int channelId, bool isLocal)
        {
            if (channelId == siteInfo.Id)
            {
                return GetIndexPageUrl(siteInfo, isLocal);
            }
            var linkUrl = string.Empty;
            var nodeInfo = await _channelRepository.GetChannelInfoAsync(siteInfo.Id, channelId);
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
                        var channelUrl = await _pathManager.ChannelRulesParseAsync(siteInfo, channelId);
                        return GetSiteUrl(siteInfo, channelUrl, isLocal);
                    }
                    return ParseNavigationUrl(siteInfo, _pathManager.AddVirtualToPath(filePath), isLocal);
                }
            }

            return ParseNavigationUrl(siteInfo, linkUrl, isLocal);
        }

        //得到栏目经过计算后的连接地址
        public async Task<string> GetChannelUrlAsync(SiteInfo siteInfo, ChannelInfo channelInfo, bool isLocal)
        {
            if (channelInfo == null) return string.Empty;

            var url = string.Empty;

            if (channelInfo.ParentId == 0)
            {
                url = await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
            }
            else
            {
                var linkType = ELinkTypeUtils.GetEnumType(channelInfo.LinkType);
                if (linkType == ELinkType.None)
                {
                    url = await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                }
                else if (linkType == ELinkType.NoLink)
                {
                    url = PageUtils.UnClickableUrl;
                }
                else
                {
                    if (linkType == ELinkType.NoLinkIfContentNotExists)
                    {
                        var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        url = count == 0 ? PageUtils.UnClickableUrl : await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                    }
                    else if (linkType == ELinkType.LinkToOnlyOneContent)
                    {
                        var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        if (count == 1)
                        {
                            var contentId = channelInfo.ContentRepository.StlGetContentId(channelInfo, TaxisType.Parse(channelInfo.DefaultTaxisType));
                            url = await GetContentUrlAsync(siteInfo, channelInfo, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
                    {
                        var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        if (count == 0)
                        {
                            url = PageUtils.UnClickableUrl;
                        }
                        else if (count == 1)
                        {
                            var contentId = channelInfo.ContentRepository.StlGetContentId(channelInfo, TaxisType.Parse(channelInfo.DefaultTaxisType));
                            url = await GetContentUrlAsync(siteInfo, channelInfo, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                        }
                    }
                    else if (linkType == ELinkType.LinkToFirstContent)
                    {
                        var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        if (count >= 1)
                        {
                            var contentId = channelInfo.ContentRepository.StlGetContentId(channelInfo, TaxisType.Parse(channelInfo.DefaultTaxisType));
                            //var contentId = StlCacheManager.FirstContentId.GetValue(siteInfo, nodeInfo);
                            url = await GetContentUrlAsync(siteInfo, channelInfo, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
                    {
                        var count = await channelInfo.ContentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        if (count >= 1)
                        {
                            var contentId = channelInfo.ContentRepository.StlGetContentId(channelInfo, TaxisType.Parse(channelInfo.DefaultTaxisType));
                            //var contentId = StlCacheManager.FirstContentId.GetValue(siteInfo, nodeInfo);
                            url = await GetContentUrlAsync(siteInfo, channelInfo, contentId, isLocal);
                        }
                        else
                        {
                            url = PageUtils.UnClickableUrl;
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExists)
                    {
                        url = channelInfo.ChildrenCount == 0 ? PageUtils.UnClickableUrl : await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                    }
                    else if (linkType == ELinkType.LinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = _channelRepository.StlGetChannelInfoByLastAddDate(channelInfo.Id);
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(siteInfo, lastAddChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                    }
                    else if (linkType == ELinkType.LinkToFirstChannel)
                    {
                        var firstChannelInfo = _channelRepository.StlGetChannelInfoByTaxis(channelInfo.Id);
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(siteInfo, firstChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = _channelRepository.StlGetChannelInfoByLastAddDate(channelInfo.Id);
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(siteInfo, lastAddChannelInfo, isLocal) : PageUtils.UnClickableUrl;
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                    {
                        var firstChannelInfo = _channelRepository.StlGetChannelInfoByTaxis(channelInfo.Id);
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(siteInfo, firstChannelInfo, isLocal) : PageUtils.UnClickableUrl;
                    }
                }
            }

            return RemoveDefaultFileName(siteInfo, url);
        }

        private string RemoveDefaultFileName(SiteInfo siteInfo, string url)
        {
            if (!siteInfo.IsCreateUseDefaultFileName || string.IsNullOrEmpty(url)) return url;

            return url.EndsWith("/" + siteInfo.CreateDefaultFileName)
                ? url.Substring(0, url.Length - siteInfo.CreateDefaultFileName.Length)
                : url;
        }

        public async Task<string> GetInputChannelUrlAsync(SiteInfo siteInfo, ChannelInfo nodeInfo, bool isLocal)
        {
            var channelUrl = await GetChannelUrlAsync(siteInfo, nodeInfo, isLocal);
            if (string.IsNullOrEmpty(channelUrl)) return channelUrl;

            channelUrl = StringUtils.ReplaceStartsWith(channelUrl, GetWebUrl(siteInfo), string.Empty);
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

        //根据发布系统属性判断是否为相对路径并返回解析后路径
        public string ParseNavigationUrl(SiteInfo siteInfo, string url, bool isLocal)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (siteInfo != null)
            {
                if (!string.IsNullOrEmpty(url) && url.StartsWith("@"))
                {
                    return GetSiteUrl(siteInfo, url.Substring(1), isLocal);
                }
                return ParseNavigationUrl(url);
            }
            return ParseNavigationUrl(url);
        }

        public string GetVirtualUrl(SiteInfo siteInfo, string url)
        {
            var relatedSiteUrl = ParseNavigationUrl($"~/{siteInfo.SiteDir}");
            var virtualUrl = StringUtils.ReplaceStartsWith(url, relatedSiteUrl, "@/");
            return StringUtils.ReplaceStartsWith(virtualUrl, "@//", "@/");
        }

        public bool IsVirtualUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            return url.StartsWith("~") || url.StartsWith("@");
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

        public string GetAdministratorUploadUrl(params string[] paths)
        {
            return GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Administrators, PageUtils.Combine(paths)));
        }

        public string GetAdministratorUploadUrl(int userId, string relatedUrl)
        {
            return GetAdministratorUploadUrl(userId.ToString(), relatedUrl);
        }

        public string GetHomeUploadUrl(params string[] paths)
        {
            return GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Home, PageUtils.Combine(paths)));
        }

        public string DefaultAvatarUrl => GetHomeUploadUrl("default_avatar.png");

        public string GetUserUploadUrl(int userId, string relatedUrl)
        {
            return GetHomeUploadUrl(userId.ToString(), relatedUrl);
        }

        public string GetUserAvatarUrl(UserInfo userInfo)
        {
            var imageUrl = userInfo?.AvatarUrl;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.IsProtocolUrl(imageUrl) ? imageUrl : GetUserUploadUrl(userInfo.Id, imageUrl);
            }

            return DefaultAvatarUrl;
        }
    }
}