using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using System;
using SiteServer.CMS.Api.Preview;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.DataCache.Stl;
using SiteServer.CMS.Model.Attributes;
using SiteServer.Plugin;
using SiteServer.Utils;
using SiteServer.Utils.Enumerations;

namespace SiteServer.CMS.Core
{
    public static class PageUtility
    {
        public static string GetSiteUrl(SiteInfo siteInfo, bool isLocal)
        {
            return GetSiteUrl(siteInfo, string.Empty, isLocal);
        }

        public static string GetSiteUrl(SiteInfo siteInfo, string requestPath, bool isLocal)
        {
            var url = isLocal
                ? GetLocalSiteUrl(siteInfo, requestPath)
                : GetRemoteSiteUrl(siteInfo, requestPath);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public static string GetSiteUrlByPhysicalPath(SiteInfo siteInfo, string physicalPath, bool isLocal)
        {
            if (siteInfo == null)
            {
                var siteId = PathUtility.GetCurrentSiteId();
                siteInfo = SiteManager.GetSiteInfo(siteId);
            }
            if (string.IsNullOrEmpty(physicalPath)) return siteInfo.Additional.WebUrl;

            var sitePath = PathUtility.GetSitePath(siteInfo);
            var requestPath = StringUtils.StartsWithIgnoreCase(physicalPath, sitePath)
                ? StringUtils.ReplaceStartsWithIgnoreCase(physicalPath, sitePath, string.Empty)
                : string.Empty;

            return GetSiteUrl(siteInfo, requestPath, isLocal);
        }

        private static string GetRemoteSiteUrl(SiteInfo siteInfo, string requestPath)
        {
            var url = siteInfo.Additional.WebUrl;

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

            if (!siteInfo.Additional.IsSeparatedAssets) return url;

            var assetsUrl = PageUtils.Combine(siteInfo.Additional.WebUrl,
                siteInfo.Additional.AssetsDir);
            if (StringUtils.StartsWithIgnoreCase(url, assetsUrl))
            {
                url = StringUtils.ReplaceStartsWithIgnoreCase(url, assetsUrl, siteInfo.Additional.AssetsUrl);
            }

            return url;
        }

        private static string GetLocalSiteUrl(SiteInfo siteInfo, string requestPath)
        {
            var url = PageUtils.ParseNavigationUrl($"~/{siteInfo.SiteDir}");

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
        public static string GetIndexPageUrl(SiteInfo siteInfo, bool isLocal)
        {
            var indexTemplateId = TemplateManager.GetIndexTempalteId(siteInfo.Id);
            var createdFileFullName = TemplateManager.GetCreatedFileFullName(siteInfo.Id, indexTemplateId);

            var url = isLocal
                ? ApiRoutePreview.GetSiteUrl(siteInfo.Id)
                : ParseNavigationUrl(siteInfo, createdFileFullName, false);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public static string GetSpecialUrl(SiteInfo siteInfo, int specialId, bool isLocal)
        {
            var specialUrl = SpecialManager.GetSpecialUrl(siteInfo, specialId);

            var url = isLocal
                ? ApiRoutePreview.GetSpecialUrl(siteInfo.Id, specialId)
                : ParseNavigationUrl(siteInfo, specialUrl, false);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public static string GetFileUrl(SiteInfo siteInfo, int fileTemplateId, bool isLocal)
        {
            var createdFileFullName = TemplateManager.GetCreatedFileFullName(siteInfo.Id, fileTemplateId);

            var url = isLocal
                ? ApiRoutePreview.GetFileUrl(siteInfo.Id, fileTemplateId)
                : ParseNavigationUrl(siteInfo, createdFileFullName, false);

            return RemoveDefaultFileName(siteInfo, url);
        }

        public static string GetContentUrl(SiteInfo siteInfo, IContentInfo contentInfo, bool isLocal)
        {
            return GetContentUrlById(siteInfo, contentInfo, isLocal);
        }

        public static string GetContentUrl(SiteInfo siteInfo, ChannelInfo channelInfo, int contentId, bool isLocal)
        {
            var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
            return GetContentUrlById(siteInfo, contentInfo, isLocal);
        }

        /// <summary>
        /// 对GetContentUrlByID的优化
        /// 通过传入参数contentInfoCurrent，避免对ContentInfo查询太多
        /// </summary>
        private static string GetContentUrlById(SiteInfo siteInfo, IContentInfo contentInfoCurrent, bool isLocal)
        {
            if (contentInfoCurrent == null) return PageUtils.UnclickedUrl;

            if (isLocal)
            {
                return ApiRoutePreview.GetContentUrl(siteInfo.Id, contentInfoCurrent.ChannelId,
                    contentInfoCurrent.Id);
            }

            var sourceId = contentInfoCurrent.SourceId;
            var referenceId = contentInfoCurrent.ReferenceId;
            var linkUrl = contentInfoCurrent.GetString(ContentAttribute.LinkUrl);
            var channelId = contentInfoCurrent.ChannelId;
            if (referenceId > 0 && contentInfoCurrent.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (ChannelManager.IsExists(siteInfo.Id, sourceId) || ChannelManager.IsExists(sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = StlChannelCache.GetSiteId(targetChannelId);
                    var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
                    var targetChannelInfo = ChannelManager.GetChannelInfo(targetSiteId, targetChannelId);

                    var contentInfo = ContentManager.GetContentInfo(targetSiteInfo, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.SiteId == targetSiteInfo.Id)
                    {
                        return GetContentUrlById(targetSiteInfo, contentInfo, false);
                    }
                    var siteInfoTmp = SiteManager.GetSiteInfo(contentInfo.SiteId);
                    return GetContentUrlById(siteInfoTmp, contentInfo, false);
                }
                else
                {
                    var tableName = ChannelManager.GetTableName(siteInfo, channelId);
                    channelId = StlContentCache.GetChannelId(tableName, referenceId);
                    linkUrl = StlContentCache.GetValue(tableName, referenceId, ContentAttribute.LinkUrl);
                    if (ChannelManager.IsExists(siteInfo.Id, channelId))
                    {
                        return GetContentUrlById(siteInfo, channelId, referenceId, 0, 0, linkUrl, false);
                    }
                    var targetSiteId = StlChannelCache.GetSiteId(channelId);
                    var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
                    return GetContentUrlById(targetSiteInfo, channelId, referenceId, 0, 0, linkUrl, false);
                }
            }

            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(siteInfo, linkUrl, false);
            }
            var contentUrl = PathUtility.ContentFilePathRules.Parse(siteInfo, channelId, contentInfoCurrent);
            return GetSiteUrl(siteInfo, contentUrl, false);
        }

        private static string GetContentUrlById(SiteInfo siteInfo, int channelId, int contentId, int sourceId, int referenceId, string linkUrl, bool isLocal)
        {
            if (isLocal)
            {
                return ApiRoutePreview.GetContentUrl(siteInfo.Id, channelId, contentId);
            }

            var contentInfoCurrent = ContentManager.GetContentInfo(siteInfo, channelId, contentId);

            if (referenceId > 0 && contentInfoCurrent.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (ChannelManager.IsExists(siteInfo.Id, sourceId) || ChannelManager.IsExists(sourceId)))
                {
                    var targetChannelId = sourceId;
                    var targetSiteId = StlChannelCache.GetSiteId(targetChannelId);
                    var targetSiteInfo = SiteManager.GetSiteInfo(targetSiteId);
                    var targetChannelInfo = ChannelManager.GetChannelInfo(targetSiteId, targetChannelId);

                    var contentInfo = ContentManager.GetContentInfo(targetSiteInfo, targetChannelInfo, referenceId);
                    if (contentInfo == null || contentInfo.ChannelId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.SiteId == targetSiteInfo.Id)
                    {
                        return GetContentUrlById(targetSiteInfo, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetString(ContentAttribute.LinkUrl), false);
                    }
                    var siteInfoTmp = SiteManager.GetSiteInfo(contentInfo.SiteId);
                    return GetContentUrlById(siteInfoTmp, contentInfo.ChannelId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetString(ContentAttribute.LinkUrl), false);
                }
                else
                {
                    var tableName = ChannelManager.GetTableName(siteInfo, channelId);
                    channelId = StlContentCache.GetChannelId(tableName, referenceId);
                    linkUrl = StlContentCache.GetValue(tableName, referenceId, ContentAttribute.LinkUrl);
                    return GetContentUrlById(siteInfo, channelId, referenceId, 0, 0, linkUrl, false);
                }
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(siteInfo, linkUrl, false);
            }
            var contentUrl = PathUtility.ContentFilePathRules.Parse(siteInfo, channelId, contentId);
            return GetSiteUrl(siteInfo, contentUrl, false);
        }

        private static string GetChannelUrlNotComputed(SiteInfo siteInfo, int channelId, bool isLocal)
        {
            if (channelId == siteInfo.Id)
            {
                return GetIndexPageUrl(siteInfo, isLocal);
            }
            var linkUrl = string.Empty;
            var nodeInfo = ChannelManager.GetChannelInfo(siteInfo.Id, channelId);
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
                        var channelUrl = PathUtility.ChannelFilePathRules.Parse(siteInfo, channelId);
                        return GetSiteUrl(siteInfo, channelUrl, isLocal);
                    }
                    return ParseNavigationUrl(siteInfo, PathUtility.AddVirtualToPath(filePath), isLocal);
                }
            }

            return ParseNavigationUrl(siteInfo, linkUrl, isLocal);
        }

        //得到栏目经过计算后的连接地址
        public static string GetChannelUrl(SiteInfo siteInfo, ChannelInfo channelInfo, bool isLocal)
        {
            if (channelInfo == null) return string.Empty;

            if (isLocal)
            {
                return ApiRoutePreview.GetChannelUrl(siteInfo.Id, channelInfo.Id);
            }

            var url = string.Empty;
            
            if (channelInfo.ParentId == 0)
            {
                url = GetChannelUrlNotComputed(siteInfo, channelInfo.Id, false);
            }
            else
            {
                var linkType = ELinkTypeUtils.GetEnumType(channelInfo.LinkType);
                if (linkType == ELinkType.None)
                {
                    url = GetChannelUrlNotComputed(siteInfo, channelInfo.Id, false);
                }
                else if (linkType == ELinkType.NoLink)
                {
                    url = PageUtils.UnclickedUrl;
                }
                else
                {
                    if (linkType == ELinkType.NoLinkIfContentNotExists)
                    {
                        var count = ContentManager.GetCount(siteInfo, channelInfo, true);
                        url = count == 0 ? PageUtils.UnclickedUrl : GetChannelUrlNotComputed(siteInfo, channelInfo.Id, false);
                    }
                    else if (linkType == ELinkType.LinkToOnlyOneContent)
                    {
                        var count = ContentManager.GetCount(siteInfo, channelInfo, true);
                        if (count == 1)
                        {
                            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                            var contentId = StlContentCache.GetContentId(tableName, channelInfo.Id, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType)));
                            url = GetContentUrl(siteInfo, channelInfo, contentId, false);
                        }
                        else
                        {
                            url = GetChannelUrlNotComputed(siteInfo, channelInfo.Id, false);
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
                    {
                        var count = ContentManager.GetCount(siteInfo, channelInfo, true);
                        if (count == 0)
                        {
                            url = PageUtils.UnclickedUrl;
                        }
                        else if (count == 1)
                        {
                            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                            var contentId = StlContentCache.GetContentId(tableName, channelInfo.Id, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType)));
                            url = GetContentUrl(siteInfo, channelInfo, contentId, false);
                        }
                        else
                        {
                            url = GetChannelUrlNotComputed(siteInfo, channelInfo.Id, false);
                        }
                    }
                    else if (linkType == ELinkType.LinkToFirstContent)
                    {
                        var count = ContentManager.GetCount(siteInfo, channelInfo, true);
                        if (count >= 1)
                        {
                            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                            var contentId = StlContentCache.GetContentId(tableName, channelInfo.Id, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType)));
                            //var contentId = StlCacheManager.FirstContentId.GetValue(siteInfo, nodeInfo);
                            url = GetContentUrl(siteInfo, channelInfo, contentId, false);
                        }
                        else
                        {
                            url = GetChannelUrlNotComputed(siteInfo, channelInfo.Id, false);
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
                    {
                        var count = ContentManager.GetCount(siteInfo, channelInfo, true);
                        if (count >= 1)
                        {
                            var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);
                            var contentId = StlContentCache.GetContentId(tableName, channelInfo.Id, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(channelInfo.Additional.DefaultTaxisType)));
                            //var contentId = StlCacheManager.FirstContentId.GetValue(siteInfo, nodeInfo);
                            url = GetContentUrl(siteInfo, channelInfo, contentId, false);
                        }
                        else
                        {
                            url = PageUtils.UnclickedUrl;
                        }
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExists)
                    {
                        url = channelInfo.ChildrenCount == 0 ? PageUtils.UnclickedUrl : GetChannelUrlNotComputed(siteInfo, channelInfo.Id, false);
                    }
                    else if (linkType == ELinkType.LinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = StlChannelCache.GetChannelInfoByLastAddDate(channelInfo.Id);
                        url = lastAddChannelInfo != null ? GetChannelUrl(siteInfo, lastAddChannelInfo, false) : GetChannelUrlNotComputed(siteInfo, channelInfo.Id, false);
                    }
                    else if (linkType == ELinkType.LinkToFirstChannel)
                    {
                        var firstChannelInfo = StlChannelCache.GetChannelInfoByTaxis(channelInfo.Id);
                        url = firstChannelInfo != null ? GetChannelUrl(siteInfo, firstChannelInfo, false) : GetChannelUrlNotComputed(siteInfo, channelInfo.Id, false);
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                    {
                        var lastAddChannelInfo = StlChannelCache.GetChannelInfoByLastAddDate(channelInfo.Id);
                        url = lastAddChannelInfo != null ? GetChannelUrl(siteInfo, lastAddChannelInfo, false) : PageUtils.UnclickedUrl;
                    }
                    else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                    {
                        var firstChannelInfo = StlChannelCache.GetChannelInfoByTaxis(channelInfo.Id);
                        url = firstChannelInfo != null ? GetChannelUrl(siteInfo, firstChannelInfo, false) : PageUtils.UnclickedUrl;
                    }
                }
            }

            return RemoveDefaultFileName(siteInfo, url);
        }

        private static string RemoveDefaultFileName(SiteInfo siteInfo, string url)
        {
            if (!siteInfo.Additional.IsCreateUseDefaultFileName || string.IsNullOrEmpty(url)) return url;

            return url.EndsWith("/" + siteInfo.Additional.CreateDefaultFileName)
                ? url.Substring(0, url.Length - siteInfo.Additional.CreateDefaultFileName.Length)
                : url;
        }

        public static string GetInputChannelUrl(SiteInfo siteInfo, ChannelInfo nodeInfo, bool isLocal)
        {
            var channelUrl = GetChannelUrl(siteInfo, nodeInfo, isLocal);
            if (string.IsNullOrEmpty(channelUrl)) return channelUrl;

            channelUrl = StringUtils.ReplaceStartsWith(channelUrl, siteInfo.Additional.WebUrl, string.Empty);
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
        public static string ParseNavigationUrl(SiteInfo siteInfo, string url, bool isLocal)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            if (siteInfo != null)
            {
                if (!string.IsNullOrEmpty(url) && url.StartsWith("@"))
                {
                    return GetSiteUrl(siteInfo, url.Substring(1), isLocal);
                }
                return PageUtils.ParseNavigationUrl(url);
            }
            return PageUtils.ParseNavigationUrl(url);
        }

        public static string GetVirtualUrl(SiteInfo siteInfo, string url)
        {
            var relatedSiteUrl = PageUtils.ParseNavigationUrl($"~/{siteInfo.SiteDir}");
            var virtualUrl = StringUtils.ReplaceStartsWith(url, relatedSiteUrl, "@/");
            return StringUtils.ReplaceStartsWith(virtualUrl, "@//", "@/");
        }

        public static bool IsVirtualUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            return url.StartsWith("~") || url.StartsWith("@");
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