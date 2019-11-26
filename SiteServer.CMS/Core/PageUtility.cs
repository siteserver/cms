using System;
using System.Threading.Tasks;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.Context;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Enumerations;
using SiteServer.CMS.Model;
using SiteServer.Utils;

namespace SiteServer.CMS.Core
{
    public static class PageUtility
    {
        public static string GetSiteUrl(Site site, bool isLocal)
        {
            return GetSiteUrl(site, string.Empty, isLocal);
        }

        public static string GetSiteUrl(Site site, string requestPath, bool isLocal)
        {
            var url = isLocal
                ? GetLocalSiteUrl(site, requestPath)
                : GetRemoteSiteUrl(site, requestPath);

            return RemoveDefaultFileName(site, url);
        }

        public static async Task<string> GetSiteUrlByPhysicalPathAsync(Site site, string physicalPath, bool isLocal)
        {
            if (site == null)
            {
                var siteId = await PathUtility.GetCurrentSiteIdAsync();
                site = await DataProvider.SiteDao.GetAsync(siteId);
            }
            if (string.IsNullOrEmpty(physicalPath)) return site.WebUrl;

            var sitePath = PathUtility.GetSitePath(site);
            var requestPath = StringUtils.StartsWithIgnoreCase(physicalPath, sitePath)
                ? StringUtils.ReplaceStartsWithIgnoreCase(physicalPath, sitePath, string.Empty)
                : string.Empty;

            return GetSiteUrl(site, requestPath, isLocal);
        }

        private static string GetRemoteSiteUrl(Site site, string requestPath)
        {
            var url = site.WebUrl;

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

            var assetsUrl = PageUtils.Combine(site.WebUrl,
                site.AssetsDir);
            if (StringUtils.StartsWithIgnoreCase(url, assetsUrl))
            {
                url = StringUtils.ReplaceStartsWithIgnoreCase(url, assetsUrl, site.AssetsUrl);
            }

            return url;
        }

        private static string GetLocalSiteUrl(Site site, string requestPath)
        {
            var url = PageUtils.ParseNavigationUrl($"~/{site.SiteDir}");

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
            var indexTemplateId = await TemplateManager.GetIndexTemplateIdAsync(site.Id);
            var createdFileFullName = await TemplateManager.GetCreatedFileFullNameAsync(site.Id, indexTemplateId);

            var url = isLocal
                ? ApiRoutePreview.GetSiteUrl(site.Id)
                : ParseNavigationUrl(site, createdFileFullName, false);

            return RemoveDefaultFileName(site, url);
        }

        public static async Task<string> GetSpecialUrlAsync(Site site, int specialId, bool isLocal)
        {
            var specialUrl = await SpecialManager.GetSpecialUrlAsync(site, specialId);

            var url = isLocal
                ? ApiRoutePreview.GetSpecialUrl(site.Id, specialId)
                : ParseNavigationUrl(site, specialUrl, false);

            return RemoveDefaultFileName(site, url);
        }

        public static async Task<string> GetFileUrlAsync(Site site, int fileTemplateId, bool isLocal)
        {
            var createdFileFullName = await TemplateManager.GetCreatedFileFullNameAsync(site.Id, fileTemplateId);

            var url = isLocal
                ? ApiRoutePreview.GetFileUrl(site.Id, fileTemplateId)
                : ParseNavigationUrl(site, createdFileFullName, false);

            return RemoveDefaultFileName(site, url);
        }

        public static async Task<string> GetContentUrlAsync(Site site, Content content, bool isLocal)
        {
            return await GetContentUrlByIdAsync(site, content, isLocal);
        }

        public static async Task<string> GetContentUrlAsync(Site site, Channel channel, int contentId, bool isLocal)
        {
            var contentInfo = await ContentManager.GetContentInfoAsync(site, channel, contentId);
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
            if (referenceId > 0 && contentCurrent.Get<string>(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (ChannelManager.IsExistsAsync(site.Id, sourceId).GetAwaiter().GetResult() || await ChannelManager.IsExistsAsync(sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = await StlChannelCache.GetSiteIdAsync(targetChannelId);
                    var targetSite = await DataProvider.SiteDao.GetAsync(targetSiteId);
                    var targetChannelInfo = await ChannelManager.GetChannelAsync(targetSiteId, targetChannelId);

                    var contentInfo = await ContentManager.GetContentInfoAsync(targetSite, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.SiteId == targetSite.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSite, contentInfo, false);
                    }
                    var siteTmp = await DataProvider.SiteDao.GetAsync(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteTmp, contentInfo, false);
                }
                else
                {
                    var tableName = await ChannelManager.GetTableNameAsync(site, channelId);
                    channelId = StlContentCache.GetChannelId(tableName, referenceId);
                    linkUrl = StlContentCache.GetValue(tableName, referenceId, ContentAttribute.LinkUrl);
                    if (ChannelManager.IsExistsAsync(site.Id, channelId).GetAwaiter().GetResult())
                    {
                        return await GetContentUrlByIdAsync(site, channelId, referenceId, 0, 0, linkUrl, false);
                    }
                    var targetSiteId = await StlChannelCache.GetSiteIdAsync(channelId);
                    var targetSite = await DataProvider.SiteDao.GetAsync(targetSiteId);
                    return await GetContentUrlByIdAsync(targetSite, channelId, referenceId, 0, 0, linkUrl, false);
                }
            }

            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(site, linkUrl, false);
            }
            var contentUrl = await PathUtility.ContentFilePathRules.ParseAsync(site, channelId, contentCurrent);
            return GetSiteUrl(site, contentUrl, false);
        }

        private static async Task<string> GetContentUrlByIdAsync(Site site, int channelId, int contentId, int sourceId, int referenceId, string linkUrl, bool isLocal)
        {
            if (isLocal)
            {
                return ApiRoutePreview.GetContentUrl(site.Id, channelId, contentId);
            }

            var contentInfoCurrent = await ContentManager.GetContentInfoAsync(site, channelId, contentId);

            if (referenceId > 0 && contentInfoCurrent.Get<string>(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (ChannelManager.IsExistsAsync(site.Id, sourceId).GetAwaiter().GetResult() || await ChannelManager.IsExistsAsync(sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = await StlChannelCache.GetSiteIdAsync(targetChannelId);
                    var targetSite = await DataProvider.SiteDao.GetAsync(targetSiteId);
                    var targetChannelInfo = await ChannelManager.GetChannelAsync(targetSiteId, targetChannelId);

                    var contentInfo = await ContentManager.GetContentInfoAsync(targetSite, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.SiteId == targetSite.Id)
                    {
                        return await GetContentUrlByIdAsync(targetSite, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                    }
                    var siteTmp = await DataProvider.SiteDao.GetAsync(contentInfo.SiteId);
                    return await GetContentUrlByIdAsync(siteTmp, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.LinkUrl, false);
                }
                else
                {
                    var tableName = await ChannelManager.GetTableNameAsync(site, channelId);
                    channelId = StlContentCache.GetChannelId(tableName, referenceId);
                    linkUrl = StlContentCache.GetValue(tableName, referenceId, ContentAttribute.LinkUrl);
                    return await GetContentUrlByIdAsync(site, channelId, referenceId, 0, 0, linkUrl, false);
                }
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(site, linkUrl, false);
            }
            var contentUrl = await PathUtility.ContentFilePathRules.ParseAsync(site, channelId, contentId);
            return GetSiteUrl(site, contentUrl, false);
        }

        private static async Task<string> GetChannelUrlNotComputedAsync(Site site, int channelId, bool isLocal)
        {
            if (channelId == site.Id)
            {
                return await GetIndexPageUrlAsync(site, isLocal);
            }
            var linkUrl = string.Empty;
            var nodeInfo = await ChannelManager.GetChannelAsync(site.Id, channelId);
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
                        return GetSiteUrl(site, channelUrl, isLocal);
                    }
                    return ParseNavigationUrl(site, PathUtility.AddVirtualToPath(filePath), isLocal);
                }
            }

            return ParseNavigationUrl(site, linkUrl, isLocal);
        }

        //得到栏目经过计算后的连接地址
        public static async Task<string> GetChannelUrlAsync(Site site, Channel channel, bool isLocal)
        {
            if (channel == null) return string.Empty;

            var url = string.Empty;
            
            if (channel.ParentId == 0)
            {
                url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
            }
            else
            {
                var linkType = ELinkTypeUtils.GetEnumType(channel.LinkType);
                if (linkType == ELinkType.None)
                {
                    url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                }
                else if (linkType == ELinkType.NoLink)
                {
                    url = PageUtils.UnclickedUrl;
                }
                else
                {
                    if (linkType == ELinkType.NoLinkIfContentNotExists)
                    {
                        var count = await ContentManager.GetCountAsync(site, channel, true);
                        url = count == 0 ? PageUtils.UnclickedUrl : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == ELinkType.LinkToOnlyOneContent)
                    {
                        var count = await ContentManager.GetCountAsync(site, channel, true);
                        if (count == 1)
                        {
                            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
                            var contentId = StlContentCache.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channel.DefaultTaxisType)));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
                    {
                        var count = await ContentManager.GetCountAsync(site, channel, true);
                        if (count == 0)
                        {
                            url = PageUtils.UnclickedUrl;
                        }
                        else if (count == 1)
                        {
                            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
                            var contentId = StlContentCache.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channel.DefaultTaxisType)));
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                        }
                    }
                    else if (linkType == ELinkType.LinkToFirstContent)
                    {
                        var count = await ContentManager.GetCountAsync(site, channel, true);
                        if (count >= 1)
                        {
                            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
                            var contentId = StlContentCache.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channel.DefaultTaxisType)));
                            //var contentId = StlCacheManager.FirstContentId.GetValue(site, node);
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
                    {
                        var count = await ContentManager.GetCountAsync(site, channel, true);
                        if (count >= 1)
                        {
                            var tableName = await ChannelManager.GetTableNameAsync(site, channel);
                            var contentId = StlContentCache.GetContentId(tableName, channel.Id, true, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channel.DefaultTaxisType)));
                            //var contentId = StlCacheManager.FirstContentId.GetValue(site, node);
                            url = await GetContentUrlAsync(site, channel, contentId, isLocal);
                        }
                        else
                        {
                            url = PageUtils.UnclickedUrl;
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExists)
                    {
                        url = channel.ChildrenCount == 0 ? PageUtils.UnclickedUrl : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == ELinkType.LinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = StlChannelCache.GetChannelInfoByLastAddDateAsync(channel.Id).GetAwaiter().GetResult();
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(site, lastAddChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == ELinkType.LinkToFirstChannel)
                    {
                        var firstChannelInfo = StlChannelCache.GetChannelInfoByTaxisAsync(channel.Id).GetAwaiter().GetResult();
                        url = firstChannelInfo != null ? await GetChannelUrlAsync(site, firstChannelInfo, isLocal) : await GetChannelUrlNotComputedAsync(site, channel.Id, isLocal);
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = StlChannelCache.GetChannelInfoByLastAddDateAsync(channel.Id).GetAwaiter().GetResult();
                        url = lastAddChannelInfo != null ? await GetChannelUrlAsync(site, lastAddChannelInfo, isLocal) : PageUtils.UnclickedUrl;
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                    {
                        var firstChannelInfo = StlChannelCache.GetChannelInfoByTaxisAsync(channel.Id).GetAwaiter().GetResult();
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

            channelUrl = StringUtils.ReplaceStartsWith(channelUrl, site.WebUrl, string.Empty);
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
        public static string ParseNavigationUrl(Site site, string url, bool isLocal)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (site != null)
            {
                if (!string.IsNullOrEmpty(url) && url.StartsWith("@"))
                {
                    return GetSiteUrl(site, url.Substring(1), isLocal);
                }
                return PageUtils.ParseNavigationUrl(url);
            }
            return PageUtils.ParseNavigationUrl(url);
        }

        public static string GetVirtualUrl(Site site, string url)
        {
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