using System;
using System.Collections.Specialized;
using System.Threading.Tasks;
using SS.CMS.Core.Models.Attributes;
using SS.CMS.Core.Common.Enums;
using SS.CMS.Enums;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;
using System.Linq;

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
            return PageUtils.Combine(Constants.ApiPrefix, route);
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

        public string GetWebUrl(Site siteInfo, params string[] paths)
        {
            var webUrl = siteInfo.IsSeparatedWeb
                ? siteInfo.SeparatedWebUrl
                : ParseNavigationUrl($"~/{siteInfo.SiteDir}");
            webUrl = PageUtils.Combine(webUrl, PageUtils.Combine(paths));
            return RemoveDefaultFileName(siteInfo, webUrl);
        }

        public string GetAssetsUrl(Site siteInfo, params string[] paths)
        {
            var assetsUrl = siteInfo.IsSeparatedAssets
                ? siteInfo.SeparatedAssetsUrl
                : PageUtils.Combine(GetWebUrl(siteInfo), siteInfo.AssetsDir);
            return PageUtils.Combine(assetsUrl, PageUtils.Combine(paths));
        }

        public string GetHomeUrl(Site siteInfo, params string[] paths)
        {
            var assetsUrl = siteInfo.IsSeparatedAssets
                ? siteInfo.SeparatedAssetsUrl
                : PageUtils.Combine(GetWebUrl(siteInfo), siteInfo.AssetsDir);
            return PageUtils.Combine(assetsUrl, PageUtils.Combine(paths));
        }

        public string GetSiteUrl(Site siteInfo, bool isLocal)
        {
            return GetSiteUrl(siteInfo, string.Empty, isLocal);
        }

        public string GetSiteUrl(Site siteInfo, string requestPath, bool isLocal)
        {
            var url = isLocal
                ? GetLocalSiteUrl(siteInfo, requestPath)
                : GetRemoteSiteUrl(siteInfo, requestPath);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public string GetSiteUrlByPhysicalPath(Site siteInfo, string physicalPath, bool isLocal)
        {
            if (string.IsNullOrEmpty(physicalPath)) return GetWebUrl(siteInfo);

            var sitePath = _pathManager.GetSitePath(siteInfo);
            var requestPath = StringUtils.StartsWithIgnoreCase(physicalPath, sitePath)
                ? StringUtils.ReplaceStartsWithIgnoreCase(physicalPath, sitePath, string.Empty)
                : string.Empty;

            return GetSiteUrl(siteInfo, requestPath, isLocal);
        }

        private string GetRemoteSiteUrl(Site siteInfo, string requestPath)
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

        private string GetLocalSiteUrl(Site siteInfo, string requestPath)
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
        public async Task<string> GetIndexPageUrlAsync(Site siteInfo, bool isLocal)
        {
            var indexTemplateId = await _templateRepository.GetIndexTemplateIdAsync(siteInfo.Id);
            var createdFileFullName = await _templateRepository.GetCreatedFileFullNameAsync(indexTemplateId);

            var url = isLocal
                ? GetPreviewSiteUrl(siteInfo.Id)
                : ParseNavigationUrl(siteInfo, createdFileFullName, false);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public async Task<string> GetFileUrlAsync(Site siteInfo, int fileTemplateId, bool isLocal)
        {
            var createdFileFullName = await _templateRepository.GetCreatedFileFullNameAsync(fileTemplateId);

            var url = isLocal
                ? GetPreviewFileUrl(siteInfo.Id, fileTemplateId)
                : ParseNavigationUrl(siteInfo, createdFileFullName, false);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public async Task<string> GetContentUrlAsync(Site siteInfo, Content contentInfo, bool isLocal)
        {
            return await GetContentUrlByIdAsync(siteInfo, contentInfo, isLocal);
        }

        public async Task<string> GetContentUrlAsync(Site siteInfo, Channel channelInfo, int contentId, bool isLocal)
        {
            var contentRepository = _channelRepository.GetContentRepository(siteInfo, channelInfo);
            var contentInfo = await contentRepository.GetContentInfoAsync(contentId);
            return await GetContentUrlByIdAsync(siteInfo, contentInfo, isLocal);
        }

        /// <summary>
        /// 对GetContentUrlByID的优化
        /// 通过传入参数contentInfoCurrent，避免对ContentInfo查询太多
        /// </summary>
        private async Task<string> GetContentUrlByIdAsync(Site siteInfo, Content contentInfoCurrent, bool isLocal)
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
                if (sourceId > 0 && (await _channelRepository.IsExistsAsync(sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = await _channelRepository.GetSiteIdAsync(targetChannelId);
                    var targetSiteInfo = await _siteRepository.GetSiteAsync(targetSiteId);
                    var targetChannelInfo = await _channelRepository.GetChannelAsync(targetChannelId);
                    var targetContentRepository = _channelRepository.GetContentRepository(targetSiteInfo, targetChannelInfo);

                    var contentInfo = await targetContentRepository.GetContentInfoAsync(referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnClickableUrl;
                    }
                    if (contentInfo.SiteId == targetSiteInfo.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSiteInfo, contentInfo, false);
                    }
                    var siteInfoTmp = await _siteRepository.GetSiteAsync(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteInfoTmp, contentInfo, false);
                }
                else
                {
                    var channelInfo = await _channelRepository.GetChannelAsync(channelId);
                    var contentRepository = _channelRepository.GetContentRepository(siteInfo, channelInfo);

                    channelId = await contentRepository.GetChannelIdAsync(referenceId);
                    linkUrl = await contentRepository.GetValueAsync<string>(referenceId, ContentAttribute.LinkUrl);
                    if (await _channelRepository.IsExistsAsync(channelId))
                    {
                        return await GetContentUrlByIdAsync(siteInfo, channelId, referenceId, 0, 0, linkUrl, false);
                    }
                    var targetSiteId = await _channelRepository.GetSiteIdAsync(channelId);
                    var targetSiteInfo = await _siteRepository.GetSiteAsync(targetSiteId);
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

        private async Task<string> GetContentUrlByIdAsync(Site siteInfo, int channelId, int contentId, int sourceId, int referenceId, string linkUrl, bool isLocal)
        {
            if (isLocal)
            {
                return GetPreviewContentUrl(siteInfo.Id, channelId, contentId);
            }

            var channelInfo = await _channelRepository.GetChannelAsync(channelId);
            var contentRepository = _channelRepository.GetContentRepository(siteInfo, channelInfo);

            var contentInfoCurrent = await contentRepository.GetContentInfoAsync(contentId);

            if (referenceId > 0 && contentInfoCurrent.TranslateContentType != TranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (await _channelRepository.IsExistsAsync(sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = await _channelRepository.GetSiteIdAsync(targetChannelId);
                    var targetSiteInfo = await _siteRepository.GetSiteAsync(targetSiteId);
                    var targetChannelInfo = await _channelRepository.GetChannelAsync(targetChannelId);
                    var targetContentRepository = _channelRepository.GetContentRepository(targetSiteInfo, targetChannelInfo);

                    var contentInfo = await targetContentRepository.GetContentInfoAsync(referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnClickableUrl;
                    }
                    if (contentInfo.SiteId == targetSiteInfo.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSiteInfo, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                    }
                    var siteInfoTmp = await _siteRepository.GetSiteAsync(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteInfoTmp, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                }
                else
                {
                    channelId = await contentRepository.GetChannelIdAsync(referenceId);
                    linkUrl = await contentRepository.GetValueAsync<string>(referenceId, ContentAttribute.LinkUrl);
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

        private async Task<string> GetChannelUrlNotComputedAsync(Site siteInfo, int channelId, bool isLocal)
        {
            if (channelId == siteInfo.Id)
            {
                return await GetIndexPageUrlAsync(siteInfo, isLocal);
            }
            var linkUrl = string.Empty;
            var nodeInfo = await _channelRepository.GetChannelAsync(channelId);
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
        public async Task<string> GetChannelUrlAsync(Site siteInfo, Channel channelInfo, bool isLocal)
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
                    var contentRepository = _channelRepository.GetContentRepository(siteInfo, channelInfo);

                    if (linkType == ELinkType.NoLinkIfContentNotExists)
                    {
                        var count = await contentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        url = count == 0 ? PageUtils.UnClickableUrl : await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                    }
                    else if (linkType == ELinkType.LinkToOnlyOneContent)
                    {
                        var count = await contentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        if (count == 1)
                        {
                            var contentId = await contentRepository.GetContentIdAsync(channelInfo.Id, TaxisType.Parse(channelInfo.DefaultTaxisType));
                            url = await GetContentUrlAsync(siteInfo, channelInfo, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
                    {
                        var count = await contentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        if (count == 0)
                        {
                            url = PageUtils.UnClickableUrl;
                        }
                        else if (count == 1)
                        {
                            var contentId = await contentRepository.GetContentIdAsync(channelInfo.Id, TaxisType.Parse(channelInfo.DefaultTaxisType));
                            url = await GetContentUrlAsync(siteInfo, channelInfo, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                        }
                    }
                    else if (linkType == ELinkType.LinkToFirstContent)
                    {
                        var count = await contentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        if (count >= 1)
                        {
                            var contentId = await contentRepository.GetContentIdAsync(channelInfo.Id, TaxisType.Parse(channelInfo.DefaultTaxisType));
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
                        var count = await contentRepository.GetCountAsync(siteInfo, channelInfo, true);
                        if (count >= 1)
                        {
                            var contentId = await contentRepository.GetContentIdAsync(channelInfo.Id, TaxisType.Parse(channelInfo.DefaultTaxisType));
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
                        var childrenIds = await _channelRepository.GetChildrenIdsAsync(channelInfo.SiteId, channelInfo.Id);
                        url = childrenIds.Count() == 0 ? PageUtils.UnClickableUrl : await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                    }
                    else if (linkType == ELinkType.LinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = await _channelRepository.GetChannelByLastAddDateAsync(channelInfo.Id);
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(siteInfo, lastAddChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                    }
                    else if (linkType == ELinkType.LinkToFirstChannel)
                    {
                        var firstChannelInfo = await _channelRepository.GetChannelByTaxisAsync(channelInfo.Id);
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(siteInfo, firstChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(siteInfo, channelInfo.Id, isLocal);
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = await _channelRepository.GetChannelByLastAddDateAsync(channelInfo.Id);
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(siteInfo, lastAddChannelInfo, isLocal) : PageUtils.UnClickableUrl;
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                    {
                        var firstChannelInfo = await _channelRepository.GetChannelByTaxisAsync(channelInfo.Id);
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(siteInfo, firstChannelInfo, isLocal) : PageUtils.UnClickableUrl;
                    }
                }
            }

            return RemoveDefaultFileName(siteInfo, url);
        }

        private string RemoveDefaultFileName(Site siteInfo, string url)
        {
            if (!siteInfo.IsCreateUseDefaultFileName || string.IsNullOrEmpty(url)) return url;

            return url.EndsWith("/" + siteInfo.CreateDefaultFileName)
                ? url.Substring(0, url.Length - siteInfo.CreateDefaultFileName.Length)
                : url;
        }

        public async Task<string> GetInputChannelUrlAsync(Site siteInfo, Channel nodeInfo, bool isLocal)
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
        public string ParseNavigationUrl(Site siteInfo, string url, bool isLocal)
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

        public string GetVirtualUrl(Site siteInfo, string url)
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

        public string GetUserAvatarUrl(User userInfo)
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