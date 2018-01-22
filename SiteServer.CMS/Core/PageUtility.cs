using SiteServer.Utils;
using SiteServer.Utils.Model;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using System;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.CMS.Controllers.Preview;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.Plugin;

namespace SiteServer.CMS.Core
{
    public class PageUtility
    {
        private PageUtility()
        {
        }

        public const string InnerApiUrl = "/api";

        public static string OuterApiUrl => ConfigManager.SystemConfigInfo.ApiUrl;

        public static string GetPublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo, bool isLocal)
        {
            return GetPublishmentSystemUrl(publishmentSystemInfo, string.Empty, isLocal);
        }

        public static string GetPublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo, string requestPath, bool isLocal)
        {
            return isLocal
                ? GetLocalPublishmentSystemUrl(publishmentSystemInfo, requestPath)
                : GetRemotePublishmentSystemUrl(publishmentSystemInfo, requestPath);
        }

        public static string GetRemotePublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return GetRemotePublishmentSystemUrl(publishmentSystemInfo, string.Empty);
        }

        public static string GetRemotePublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo, string requestPath)
        {
            var url = publishmentSystemInfo.Additional.WebUrl;

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

            if (!publishmentSystemInfo.Additional.IsSeparatedAssets) return url;

            var assetsUrl = PageUtils.Combine(publishmentSystemInfo.Additional.WebUrl,
                publishmentSystemInfo.Additional.AssetsDir);
            if (StringUtils.StartsWithIgnoreCase(url, assetsUrl))
            {
                url = StringUtils.ReplaceStartsWithIgnoreCase(url, assetsUrl, publishmentSystemInfo.Additional.AssetsUrl);
            }

            return url;
        }

        public static string GetLocalPublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return GetLocalPublishmentSystemUrl(publishmentSystemInfo, string.Empty);
        }

        public static string GetLocalPublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo, string requestPath)
        {
            var url = PageUtils.ParseNavigationUrl($"~/{publishmentSystemInfo.PublishmentSystemDir}");

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

        public static string GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo publishmentSystemInfo, string physicalPath, bool isLocal)
        {
            if (publishmentSystemInfo == null)
            {
                var publishmentSystemId = PathUtility.GetCurrentPublishmentSystemId();
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            }
            if (string.IsNullOrEmpty(physicalPath)) return publishmentSystemInfo.Additional.WebUrl;

            var publishmentSystemPath = PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
            var requestPath = StringUtils.StartsWithIgnoreCase(physicalPath, publishmentSystemPath)
                ? StringUtils.ReplaceStartsWithIgnoreCase(physicalPath, publishmentSystemPath, string.Empty)
                : string.Empty;
            
            return GetPublishmentSystemUrl(publishmentSystemInfo, requestPath, isLocal);
        }

        // 得到发布系统首页地址
        public static string GetIndexPageUrl(PublishmentSystemInfo publishmentSystemInfo, bool isLocal)
        {
            var indexTemplateId = TemplateManager.GetIndexTempalteId(publishmentSystemInfo.PublishmentSystemId);
            var createdFileFullName = TemplateManager.GetCreatedFileFullName(publishmentSystemInfo.PublishmentSystemId, indexTemplateId);

            return isLocal
                ? PreviewApi.GetPublishmentSystemUrl(publishmentSystemInfo.PublishmentSystemId)
                : ParseNavigationUrl(publishmentSystemInfo, createdFileFullName, false);
        }

        public static string GetFileUrl(PublishmentSystemInfo publishmentSystemInfo, int fileTemplateId, bool isLocal)
        {
            var createdFileFullName = TemplateManager.GetCreatedFileFullName(publishmentSystemInfo.PublishmentSystemId, fileTemplateId);

            return isLocal
                ? PreviewApi.GetFileUrl(publishmentSystemInfo.PublishmentSystemId, fileTemplateId)
                : ParseNavigationUrl(publishmentSystemInfo, createdFileFullName, false);
        }

        public static string GetContentUrl(PublishmentSystemInfo publishmentSystemInfo, IContentInfo contentInfo, bool isLocal)
        {
            return GetContentUrlById(publishmentSystemInfo, contentInfo, isLocal);
        }

        public static string GetContentUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, int contentId, bool isLocal)
        {
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var contentInfo = Content.GetContentInfo(tableName, contentId);
            return GetContentUrlById(publishmentSystemInfo, contentInfo, isLocal);
        }

        /// <summary>
        /// 对GetContentUrlByID的优化
        /// 通过传入参数contentInfoCurrent，避免对ContentInfo查询太多
        /// </summary>
        private static string GetContentUrlById(PublishmentSystemInfo publishmentSystemInfo, IContentInfo contentInfoCurrent, bool isLocal)
        {
            if (contentInfoCurrent == null) return PageUtils.UnclickedUrl;

            if (isLocal)
            {
                return PreviewApi.GetContentUrl(publishmentSystemInfo.PublishmentSystemId, contentInfoCurrent.NodeId,
                    contentInfoCurrent.Id);
            }

            var sourceId = contentInfoCurrent.SourceId;
            var referenceId = contentInfoCurrent.ReferenceId;
            var linkUrl = contentInfoCurrent.GetString(ContentAttribute.LinkUrl);
            var nodeId = contentInfoCurrent.NodeId;
            if (referenceId > 0 && contentInfoCurrent.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, sourceId) || NodeManager.IsExists(sourceId)))
                {
                    var targetNodeId = sourceId;
                    var targetPublishmentSystemId = Node.GetPublishmentSystemId(targetNodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                    var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                    var contentInfo = Content.GetContentInfo(tableName, referenceId);
                    if (contentInfo == null || contentInfo.NodeId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.PublishmentSystemId == targetPublishmentSystemInfo.PublishmentSystemId)
                    {
                        return GetContentUrlById(targetPublishmentSystemInfo, contentInfo, false);
                    }
                    var publishmentSystemInfoTmp = PublishmentSystemManager.GetPublishmentSystemInfo(contentInfo.PublishmentSystemId);
                    return GetContentUrlById(publishmentSystemInfoTmp, contentInfo, false);
                }
                else
                {
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                    nodeId = Content.GetNodeId(tableName, referenceId);
                    linkUrl = Content.GetValue(tableName, referenceId, ContentAttribute.LinkUrl);
                    if (NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, nodeId))
                    {
                        return GetContentUrlById(publishmentSystemInfo, nodeId, referenceId, 0, 0, linkUrl, false);
                    }
                    var targetPublishmentSystemId = Node.GetPublishmentSystemId(nodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    return GetContentUrlById(targetPublishmentSystemInfo, nodeId, referenceId, 0, 0, linkUrl, false);
                }
            }

            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(publishmentSystemInfo, linkUrl, false);
            }
            var contentUrl = PathUtility.ContentFilePathRules.Parse(publishmentSystemInfo, nodeId, contentInfoCurrent);
            return GetPublishmentSystemUrl(publishmentSystemInfo, contentUrl, false);
        }

        private static string GetContentUrlById(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, int sourceId, int referenceId, string linkUrl, bool isLocal)
        {
            if (isLocal)
            {
                return PreviewApi.GetContentUrl(publishmentSystemInfo.PublishmentSystemId, nodeId, contentId);
            }

            var tableNameCurrent = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
            var contentInfoCurrent = Content.GetContentInfo(tableNameCurrent, contentId);

            if (referenceId > 0 && contentInfoCurrent.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, sourceId) || NodeManager.IsExists(sourceId)))
                {
                    var targetNodeId = sourceId;
                    var targetPublishmentSystemId = Node.GetPublishmentSystemId(targetNodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                    var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                    var contentInfo = Content.GetContentInfo(tableName, referenceId);
                    if (contentInfo == null || contentInfo.NodeId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.PublishmentSystemId == targetPublishmentSystemInfo.PublishmentSystemId)
                    {
                        return GetContentUrlById(targetPublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetString(ContentAttribute.LinkUrl), false);
                    }
                    var publishmentSystemInfoTmp = PublishmentSystemManager.GetPublishmentSystemInfo(contentInfo.PublishmentSystemId);
                    return GetContentUrlById(publishmentSystemInfoTmp, contentInfo.NodeId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetString(ContentAttribute.LinkUrl), false);
                }
                else
                {
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                    nodeId = Content.GetNodeId(tableName, referenceId);
                    linkUrl = Content.GetValue(tableName, referenceId, ContentAttribute.LinkUrl);
                    return GetContentUrlById(publishmentSystemInfo, nodeId, referenceId, 0, 0, linkUrl, false);
                }
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(publishmentSystemInfo, linkUrl, false);
            }
            var contentUrl = PathUtility.ContentFilePathRules.Parse(publishmentSystemInfo, nodeId, contentId);
            return GetPublishmentSystemUrl(publishmentSystemInfo, contentUrl, false);
        }

        private static string GetChannelUrlNotComputed(PublishmentSystemInfo publishmentSystemInfo, int nodeId, bool isLocal)
        {
            if (nodeId == publishmentSystemInfo.PublishmentSystemId)
            {
                return GetIndexPageUrl(publishmentSystemInfo, isLocal);
            }
            var linkUrl = string.Empty;
            var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemInfo.PublishmentSystemId, nodeId);
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
                        var channelUrl = PathUtility.ChannelFilePathRules.Parse(publishmentSystemInfo, nodeId);
                        return GetPublishmentSystemUrl(publishmentSystemInfo, channelUrl, isLocal);
                    }
                    return ParseNavigationUrl(publishmentSystemInfo, PathUtility.AddVirtualToPath(filePath), isLocal);
                }
            }

            return ParseNavigationUrl(publishmentSystemInfo, linkUrl, isLocal);
        }

        //得到栏目经过计算后的连接地址
        public static string GetChannelUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, bool isLocal)
        {
            if (isLocal)
            {
                return PreviewApi.GetChannelUrl(publishmentSystemInfo.PublishmentSystemId, nodeInfo.NodeId);
            }
            var url = string.Empty;
            if (nodeInfo != null)
            {
                if (nodeInfo.ParentId == 0)
                {
                    url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, false);
                }
                else
                {
                    var linkType = ELinkTypeUtils.GetEnumType(nodeInfo.LinkType);
                    if (linkType == ELinkType.None)
                    {
                        url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, false);
                    }
                    else if (linkType == ELinkType.NoLink)
                    {
                        url = PageUtils.UnclickedUrl;
                    }
                    else
                    {
                        if (linkType == ELinkType.NoLinkIfContentNotExists)
                        {
                            url = nodeInfo.ContentNum == 0 ? PageUtils.UnclickedUrl : GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, false);
                        }
                        else if (linkType == ELinkType.LinkToOnlyOneContent)
                        {
                            if (nodeInfo.ContentNum == 1)
                            {
                                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                                //var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                var contentId = Content.GetContentId(tableName, nodeInfo.NodeId, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(nodeInfo.Additional.DefaultTaxisType)));
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, false);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, false);
                            }
                        }
                        else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
                        {
                            if (nodeInfo.ContentNum == 0)
                            {
                                url = PageUtils.UnclickedUrl;
                            }
                            else if (nodeInfo.ContentNum == 1)
                            {
                                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                                var contentId = Content.GetContentId(tableName, nodeInfo.NodeId, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(nodeInfo.Additional.DefaultTaxisType)));
                                //var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, false);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, false);
                            }
                        }
                        else if (linkType == ELinkType.LinkToFirstContent)
                        {
                            if (nodeInfo.ContentNum >= 1)
                            {
                                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                                var contentId = Content.GetContentId(tableName, nodeInfo.NodeId, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(nodeInfo.Additional.DefaultTaxisType)));
                                //var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, false);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, false);
                            }
                        }
                        else if (linkType == ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
                        {
                            if (nodeInfo.ContentNum >= 1)
                            {
                                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                                var contentId = Content.GetContentId(tableName, nodeInfo.NodeId, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(nodeInfo.Additional.DefaultTaxisType)));
                                //var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, false);
                            }
                            else
                            {
                                url = PageUtils.UnclickedUrl;
                            }
                        }
                        else if (linkType == ELinkType.NoLinkIfChannelNotExists)
                        {
                            url = nodeInfo.ChildrenCount == 0 ? PageUtils.UnclickedUrl : GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, false);
                        }
                        else if (linkType == ELinkType.LinkToLastAddChannel)
                        {
                            var lastAddNodeInfo = Node.GetNodeInfoByLastAddDate(nodeInfo.NodeId);
                            url = lastAddNodeInfo != null ? GetChannelUrl(publishmentSystemInfo, lastAddNodeInfo, false) : GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, false);
                        }
                        else if (linkType == ELinkType.LinkToFirstChannel)
                        {
                            var firstNodeInfo = Node.GetNodeInfoByTaxis(nodeInfo.NodeId);
                            url = firstNodeInfo != null ? GetChannelUrl(publishmentSystemInfo, firstNodeInfo, false) : GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, false);
                        }
                        else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                        {
                            var lastAddNodeInfo = Node.GetNodeInfoByLastAddDate(nodeInfo.NodeId);
                            url = lastAddNodeInfo != null ? GetChannelUrl(publishmentSystemInfo, lastAddNodeInfo, false) : PageUtils.UnclickedUrl;
                        }
                        else if (linkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                        {
                            var firstNodeInfo = Node.GetNodeInfoByTaxis(nodeInfo.NodeId);
                            url = firstNodeInfo != null ? GetChannelUrl(publishmentSystemInfo, firstNodeInfo, false) : PageUtils.UnclickedUrl;
                        }
                    }
                }
            }
            return url;
        }

        public static string GetInputChannelUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, bool isLocal)
        {
            var channelUrl = GetChannelUrl(publishmentSystemInfo, nodeInfo, isLocal);
            if (string.IsNullOrEmpty(channelUrl)) return channelUrl;

            channelUrl = StringUtils.ReplaceStartsWith(channelUrl, publishmentSystemInfo.Additional.WebUrl, string.Empty);
            channelUrl = channelUrl.Trim('/');
            channelUrl = "/" + channelUrl;
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

        public static string ParseNavigationUrlAddPrefix(PublishmentSystemInfo publishmentSystemInfo, string url, bool isLocal)
        {
            if (string.IsNullOrEmpty(url)) return ParseNavigationUrl(publishmentSystemInfo, url, isLocal);

            if (!url.StartsWith("~/") && !url.StartsWith("@/"))
            {
                url = "@/" + url;
            }
            return ParseNavigationUrl(publishmentSystemInfo, url, isLocal);
        }

        public static string ParseNavigationUrl(int publishmentSystemId, string url, bool isLocal)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return ParseNavigationUrl(publishmentSystemInfo, url, isLocal);
        }

        //根据发布系统属性判断是否为相对路径并返回解析后路径
        public static string ParseNavigationUrl(PublishmentSystemInfo publishmentSystemInfo, string url, bool isLocal)
        {
            if (publishmentSystemInfo != null)
            {
                if (!string.IsNullOrEmpty(url) && url.StartsWith("@"))
                {
                    return GetPublishmentSystemUrl(publishmentSystemInfo, url.Substring(1), isLocal);
                }
                return PageUtils.ParseNavigationUrl(url);
            }
            return PageUtils.ParseNavigationUrl(url);
        }

        public static string GetVirtualUrl(PublishmentSystemInfo publishmentSystemInfo, string url)
        {
            var virtualUrl = StringUtils.ReplaceStartsWith(url, publishmentSystemInfo.Additional.WebUrl, "@/");
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

        public static string GetUserFilesUrl(string apiUrl, string relatedUrl)
        {
            return GetSiteFilesUrl(apiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.UserFiles, relatedUrl));
        }

        public static string GetUserAvatarUrl(string apiUrl, IUserInfo userInfo)
        {
            var imageUrl = userInfo?.AvatarUrl;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.IsProtocolUrl(imageUrl) ? imageUrl : GetUserFilesUrl(apiUrl, PageUtils.Combine(userInfo.UserName, imageUrl));
            }

            return SiteFilesAssets.GetUrl(apiUrl, "default_avatar.png");
        }
    }
}
