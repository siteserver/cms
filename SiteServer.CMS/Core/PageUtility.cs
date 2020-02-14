using System;
using System.Threading.Tasks;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Context;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.CMS.Core
{
    public static class PageUtility
    {
        public static async Task<string> GetSiteUrlAsync(Site site, bool isLocal)
        {
            return await GetSiteUrlAsync(site, string.Empty, isLocal);
        }

        public static async Task<string> GetSiteUrlAsync(Site site, string requestPath, bool isLocal)
        {
            var url = isLocal
                ? await GetLocalSiteUrlAsync(site, requestPath)
                : await GetRemoteSiteUrlAsync(site, requestPath);

            return RemoveDefaultFileName(site, url);
        }

        public static async Task<string> GetSiteUrlByPhysicalPathAsync(Site site, string physicalPath, bool isLocal)
        {
            if (site == null)
            {
                var siteId = await PathUtility.GetCurrentSiteIdAsync();
                site = await DataProvider.SiteRepository.GetAsync(siteId);
            }
            if (string.IsNullOrEmpty(physicalPath)) return await site.GetWebUrlAsync();

            var sitePath = await PathUtility.GetSitePathAsync(site);
            var requestPath = StringUtils.StartsWithIgnoreCase(physicalPath, sitePath)
                ? StringUtils.ReplaceStartsWithIgnoreCase(physicalPath, sitePath, string.Empty)
                : string.Empty;

            return await GetSiteUrlAsync(site, requestPath, isLocal);
        }

        private static async Task<string> GetRemoteSiteUrlAsync(Site site, string requestPath)
        {
            var url = await site.GetWebUrlAsync();

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

            var assetsUrl = PageUtils.Combine(await site.GetWebUrlAsync(),
                site.AssetsDir);
            if (StringUtils.StartsWithIgnoreCase(url, assetsUrl))
            {
                url = StringUtils.ReplaceStartsWithIgnoreCase(url, assetsUrl, await site.GetAssetsUrlAsync());
            }

            return url;
        }

        public static async Task<string> GetLocalSiteUrlAsync(Site site, string requestPath = null)
        {
            string url;
            if (site.ParentId == 0)
            {
                url = PageUtils.ParseNavigationUrl($"~/{(site.Root ? string.Empty : site.SiteDir)}");
            }
            else
            {
                var parent = await DataProvider.SiteRepository.GetAsync(site.ParentId);
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

        // 得到发布系统首页地址
        public static async Task<string> GetIndexPageUrlAsync(Site site, bool isLocal)
        {
            var indexTemplateId = await DataProvider.TemplateRepository.GetIndexTemplateIdAsync(site.Id);
            var createdFileFullName = await DataProvider.TemplateRepository.GetCreatedFileFullNameAsync(indexTemplateId);

            var url = isLocal
                ? ApiRoutePreview.GetSiteUrl(site.Id)
                : await ParseNavigationUrlAsync(site, createdFileFullName, false);

            return RemoveDefaultFileName(site, url);
        }

        public static async Task<string> GetSpecialUrlAsync(Site site, int specialId, bool isLocal)
        {
            var specialUrl = await DataProvider.SpecialRepository.GetSpecialUrlAsync(site, specialId);

            var url = isLocal
                ? ApiRoutePreview.GetSpecialUrl(site.Id, specialId)
                : await ParseNavigationUrlAsync(site, specialUrl, false);

            return RemoveDefaultFileName(site, url);
        }

        public static async Task<string> GetFileUrlAsync(Site site, int fileTemplateId, bool isLocal)
        {
            var createdFileFullName = await DataProvider.TemplateRepository.GetCreatedFileFullNameAsync(fileTemplateId);

            var url = isLocal
                ? ApiRoutePreview.GetFileUrl(site.Id, fileTemplateId)
                : await ParseNavigationUrlAsync(site, createdFileFullName, false);

            return RemoveDefaultFileName(site, url);
        }

        public static async Task<string> GetContentUrlAsync(Site site, Content content, bool isLocal)
        {
            return await GetContentUrlByIdAsync(site, content, isLocal);
        }

        public static async Task<string> GetContentUrlAsync(Site site, Channel channel, int contentId, bool isLocal)
        {
            var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channel, contentId);
            return await GetContentUrlByIdAsync(site, contentInfo, isLocal);
        }

        /// <summary>
        /// 对GetContentUrlByID的优化
        /// 通过传入参数contentInfoCurrent，避免对ContentInfo查询太多
        /// </summary>
        private static async Task<string> GetContentUrlByIdAsync(Site site, Content contentCurrent, bool isLocal)
        {
            if (contentCurrent == null) return PageUtils.UnclickedUrl;

            if (isLocal)
            {
                return ApiRoutePreview.GetContentUrl(site.Id, contentCurrent.ChannelId,
                    contentCurrent.Id);
            }

            var sourceId = contentCurrent.SourceId;
            var referenceId = contentCurrent.ReferenceId;
            var linkUrl = contentCurrent.Get<string>(ContentAttribute.LinkUrl);
            var channelId = contentCurrent.ChannelId;
            if (referenceId > 0 && TranslateContentType.ReferenceContent == contentCurrent.TranslateContentType)
            {
                if (sourceId > 0 && (DataProvider.ChannelRepository.IsExistsAsync(sourceId).GetAwaiter().GetResult() || await DataProvider.ChannelRepository.IsExistsAsync(sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = await DataProvider.ChannelRepository.GetSiteIdAsync(targetChannelId);
                    var targetSite = await DataProvider.SiteRepository.GetAsync(targetSiteId);
                    var targetChannelInfo = await DataProvider.ChannelRepository.GetAsync(targetChannelId);

                    var contentInfo = await DataProvider.ContentRepository.GetAsync(targetSite, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.SiteId == targetSite.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSite, contentInfo, false);
                    }
                    var siteTmp = await DataProvider.SiteRepository.GetAsync(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteTmp, contentInfo, false);
                }
                else
                {
                    var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelId);
                    channelId = DataProvider.ContentRepository.GetChannelId(tableName, referenceId);
                    linkUrl = await DataProvider.ContentRepository.GetValueAsync(tableName, referenceId, ContentAttribute.LinkUrl);
                    if (DataProvider.ChannelRepository.IsExistsAsync(channelId).GetAwaiter().GetResult())
                    {
                        return await GetContentUrlByIdAsync(site, channelId, referenceId, 0, 0, linkUrl, false);
                    }
                    var targetSiteId = await DataProvider.ChannelRepository.GetSiteIdAsync(channelId);
                    var targetSite = await DataProvider.SiteRepository.GetAsync(targetSiteId);
                    return await GetContentUrlByIdAsync(targetSite, channelId, referenceId, 0, 0, linkUrl, false);
                }
            }

            if (!string.IsNullOrEmpty(linkUrl))
            {
                return await ParseNavigationUrlAsync(site, linkUrl, false);
            }
            var contentUrl = await PathUtility.ContentFilePathRules.ParseAsync(site, channelId, contentCurrent);
            return await GetSiteUrlAsync(site, contentUrl, false);
        }

        private static async Task<string> GetContentUrlByIdAsync(Site site, int channelId, int contentId, int sourceId, int referenceId, string linkUrl, bool isLocal)
        {
            if (isLocal)
            {
                return ApiRoutePreview.GetContentUrl(site.Id, channelId, contentId);
            }

            var contentInfoCurrent = await DataProvider.ContentRepository.GetAsync(site, channelId, contentId);

            if (referenceId > 0 && TranslateContentType.ReferenceContent == contentInfoCurrent.TranslateContentType)
            {
                if (sourceId > 0 && (DataProvider.ChannelRepository.IsExistsAsync(sourceId).GetAwaiter().GetResult() || await DataProvider.ChannelRepository.IsExistsAsync(sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = await DataProvider.ChannelRepository.GetSiteIdAsync(targetChannelId);
                    var targetSite = await DataProvider.SiteRepository.GetAsync(targetSiteId);
                    var targetChannelInfo = await DataProvider.ChannelRepository.GetAsync(targetChannelId);

                    var contentInfo = await DataProvider.ContentRepository.GetAsync(targetSite, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.SiteId == targetSite.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSite, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                    }
                    var siteTmp = await DataProvider.SiteRepository.GetAsync(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteTmp, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                }
                else
                {
                    var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelId);
                    channelId = DataProvider.ContentRepository.GetChannelId(tableName, referenceId);
                    linkUrl = await DataProvider.ContentRepository.GetValueAsync(tableName, referenceId, ContentAttribute.LinkUrl);
                    return await GetContentUrlByIdAsync(site, channelId, referenceId, 0, 0, linkUrl, false);
                }
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return await ParseNavigationUrlAsync(site, linkUrl, false);
            }
            var contentUrl = await PathUtility.ContentFilePathRules.ParseAsync(site, channelId, contentId);
            return await GetSiteUrlAsync(site, contentUrl, false);
        }

        private static async Task<string> GetChannelUrlNotComputedAsync(Site site, int channelId, bool isLocal)
        {
            if (channelId == site.Id)
            {
                return await GetIndexPageUrlAsync(site, isLocal);
            }
            var linkUrl = string.Empty;
            var nodeInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
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
                        var channelUrl = PathUtility.ChannelFilePathRules.ParseAsync(site, channelId).GetAwaiter().GetResult();
                        return await GetSiteUrlAsync(site, channelUrl, isLocal);
                    }
                    return await ParseNavigationUrlAsync(site, PathUtility.AddVirtualToPath(filePath), isLocal);
                }
            }

            return await ParseNavigationUrlAsync(site, linkUrl, isLocal);
        }

        //得到栏目经过计算后的连接地址
        public static async Task<string> GetChannelUrlAsync(Site site, Channel channel, bool isLocal)
        {
            if (channel == null) return string.Empty;

            if (isLocal)
            {
                return ApiRoutePreview.GetChannelUrl(site.Id, channel.Id);
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
                    url = PageUtils.UnclickedUrl;
                }
                else
                {
                    if (linkType == LinkType.NoLinkIfContentNotExists)
                    {
                        var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                        url = count == 0 ? PageUtils.UnclickedUrl : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == LinkType.LinkToOnlyOneContent)
                    {
                        var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                        if (count == 1)
                        {
                            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
                            var contentId = DataProvider.ContentRepository.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                        }
                    }
                    else if (linkType == LinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
                    {
                        var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                        if (count == 0)
                        {
                            url = PageUtils.UnclickedUrl;
                        }
                        else if (count == 1)
                        {
                            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
                            var contentId = DataProvider.ContentRepository.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                        }
                    }
                    else if (linkType == LinkType.LinkToFirstContent)
                    {
                        var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                        if (count >= 1)
                        {
                            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
                            var contentId = DataProvider.ContentRepository.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                        }
                    }
                    else if (linkType == LinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
                    {
                        var count = await DataProvider.ContentRepository.GetCountAsync(site, channel);
                        if (count >= 1)
                        {
                            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
                            var contentId = DataProvider.ContentRepository.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(channel.DefaultTaxisType));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = PageUtils.UnclickedUrl;
                        }
                    }
                    else if (linkType == LinkType.NoLinkIfChannelNotExists)
                    {
                        url = channel.ChildrenCount == 0 ? PageUtils.UnclickedUrl : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == LinkType.LinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = DataProvider.ChannelRepository.GetChannelByLastAddDateAsyncTask(site.Id,  channel.Id).GetAwaiter().GetResult();
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(site, lastAddChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == LinkType.LinkToFirstChannel)
                    {
                        var firstChannelInfo = DataProvider.ChannelRepository.GetChannelByTaxisAsync(site.Id, channel.Id).GetAwaiter().GetResult();
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(site, firstChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == LinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = DataProvider.ChannelRepository.GetChannelByLastAddDateAsyncTask(site.Id, channel.Id).GetAwaiter().GetResult();
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(site, lastAddChannelInfo, isLocal) : PageUtils.UnclickedUrl;
                    }
                    else if (linkType == LinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                    {
                        var firstChannelInfo = DataProvider.ChannelRepository.GetChannelByTaxisAsync(site.Id, channel.Id).GetAwaiter().GetResult();
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(site, firstChannelInfo, isLocal) : PageUtils.UnclickedUrl;
                    }
                }
            }

            return RemoveDefaultFileName(site, url);
        }

        private static string RemoveDefaultFileName(Site site, string url)
        {
            if (!site.IsCreateUseDefaultFileName || string.IsNullOrEmpty(url)) return url;

            return url.EndsWith("/" + site.CreateDefaultFileName)
                ? url.Substring(0, url.Length - site.CreateDefaultFileName.Length)
                : url;
        }

        public static async Task<string> GetInputChannelUrlAsync(Site site, Channel node, bool isLocal)
        {
            var channelUrl = await GetChannelUrlAsync(site, node, isLocal);
            if (string.IsNullOrEmpty(channelUrl)) return channelUrl;

            channelUrl = StringUtils.ReplaceStartsWith(channelUrl, await site.GetWebUrlAsync(), string.Empty);
            channelUrl = channelUrl.Trim('/');
            if (channelUrl != PageUtils.UnclickedUrl)
            {
                channelUrl = "/" + channelUrl;
            }
            
            return channelUrl;
        }

        public static string AddVirtualToUrl(string url)
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
        public static async Task<string> ParseNavigationUrlAsync(Site site, string url, bool isLocal)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (site != null)
            {
                if (!string.IsNullOrEmpty(url) && url.StartsWith("@"))
                {
                    return await GetSiteUrlAsync(site, url.Substring(1), isLocal);
                }
                return PageUtils.ParseNavigationUrl(url);
            }
            return PageUtils.ParseNavigationUrl(url);
        }

        public static string GetVirtualUrl(Site site, string url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            var relatedSiteUrl = PageUtils.ParseNavigationUrl($"~/{site.SiteDir}");
            var virtualUrl = StringUtils.ReplaceStartsWith(url, relatedSiteUrl, "@/");
            return StringUtils.ReplaceStartsWith(virtualUrl, "@//", "@/");
        }

        public static bool IsVirtualUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            return url.StartsWith("~") || url.StartsWith("@");
        }

        public static bool IsRelativeUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            return url.StartsWith("/");
        }

        public static string GetSiteFilesUrl(string apiUrl, string relatedUrl)
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
    }
}