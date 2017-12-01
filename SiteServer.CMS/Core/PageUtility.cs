using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Model;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using System;
using System.Collections.Specialized;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.StlParser.Cache;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Core
{
    public class PageUtility
    {
        private PageUtility()
        {
        }

        public static string GetPreviewPublishmentSystemUrlxxx(int publishmentSystemId)
        {
            return GetPreviewUrlxxx(publishmentSystemId, 0, 0, 0, 0);
        }

        public static string GetPreviewChannelUrlxxx(int publishmentSystemId, int channelId)
        {
            return GetPreviewUrlxxx(publishmentSystemId, channelId, 0, 0, 0);
        }

        public static string GetPreviewContentUrlxxx(int publishmentSystemId, int channelId, int contentId)
        {
            return GetPreviewUrlxxx(publishmentSystemId, channelId, channelId, 0, 0);
        }

        public static string GetPreviewFileUrlxxx(int publishmentSystemId, int fileTemplateId)
        {
            return GetPreviewUrlxxx(publishmentSystemId, 0, 0, fileTemplateId, 0);
        }

        public static string GetPreviewUrlxxx(int publishmentSystemId, int channelId, int contentId, int fileTemplateId, int pageIndex)
        {
            var queryString = new NameValueCollection
            {
                {"s", publishmentSystemId.ToString()}
            };
            if (channelId > 0)
            {
                queryString.Add("n", channelId.ToString());
            }
            if (contentId > 0)
            {
                queryString.Add("c", contentId.ToString());
            }
            if (fileTemplateId > 0)
            {
                queryString.Add("f", fileTemplateId.ToString());
            }
            if (pageIndex > 0)
            {
                queryString.Add("p", pageIndex.ToString());
            }

            return PageUtils.GetSiteServerUrl("PagePreview", queryString);
        }

        public static string GetPublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return GetPublishmentSystemUrl(publishmentSystemInfo, string.Empty);
        }

        public static string GetPublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo, string requestPath)
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

        public static string GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo publishmentSystemInfo, string physicalPath)
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
            
            return GetPublishmentSystemUrl(publishmentSystemInfo, requestPath);
        }

        // 得到发布系统首页地址
        public static string GetIndexPageUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            var indexTemplateId = TemplateManager.GetIndexTempalteId(publishmentSystemInfo.PublishmentSystemId);
            var createdFileFullName = TemplateManager.GetCreatedFileFullName(publishmentSystemInfo.PublishmentSystemId, indexTemplateId);

            return ParseNavigationUrl(publishmentSystemInfo, createdFileFullName);
        }

        public static string GetFileUrl(PublishmentSystemInfo publishmentSystemInfo, int fileTemplateId)
        {
            var createdFileFullName = TemplateManager.GetCreatedFileFullName(publishmentSystemInfo.PublishmentSystemId, fileTemplateId);
            return ParseNavigationUrl(publishmentSystemInfo, createdFileFullName);
        }

        public static string GetContentUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, int contentId)
        {
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var contentInfo = Content.GetContentInfo(tableStyle, tableName, contentId);
            return GetContentUrlById(publishmentSystemInfo, contentInfo);
        }

        public static string GetContentUrl(PublishmentSystemInfo publishmentSystemInfo, IContentInfo contentInfo)
        {
            return GetContentUrlById(publishmentSystemInfo, contentInfo);
        }

        /// <summary>
        /// 对GetContentUrlByID的优化
        /// 通过传入参数contentInfoCurrent，避免对ContentInfo查询太多
        /// </summary>
        private static string GetContentUrlById(PublishmentSystemInfo publishmentSystemInfo, IContentInfo contentInfoCurrent)
        {
            if (contentInfoCurrent == null) return PageUtils.UnclickedUrl;
            var sourceId = contentInfoCurrent.SourceId;
            var referenceId = contentInfoCurrent.ReferenceId;
            var linkUrl = contentInfoCurrent.GetString(BackgroundContentAttribute.LinkUrl);
            var nodeId = contentInfoCurrent.NodeId;
            if (referenceId > 0 && contentInfoCurrent.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, sourceId) || NodeManager.IsExists(sourceId)))
                {
                    var targetNodeId = sourceId;
                    var targetPublishmentSystemId = Node.GetPublishmentSystemId(targetNodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                    var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                    var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                    var contentInfo = Content.GetContentInfo(tableStyle, tableName, referenceId);
                    if (contentInfo == null || contentInfo.NodeId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.PublishmentSystemId == targetPublishmentSystemInfo.PublishmentSystemId)
                    {
                        return GetContentUrlById(targetPublishmentSystemInfo, contentInfo);
                    }
                    var publishmentSystemInfoTmp = PublishmentSystemManager.GetPublishmentSystemInfo(contentInfo.PublishmentSystemId);
                    return GetContentUrlById(publishmentSystemInfoTmp, contentInfo);
                }
                else
                {
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                    nodeId = Content.GetNodeId(tableName, referenceId);
                    linkUrl = Content.GetValue(tableName, referenceId, BackgroundContentAttribute.LinkUrl);
                    if (NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, nodeId))
                    {
                        return GetContentUrlById(publishmentSystemInfo, nodeId, referenceId, 0, 0, linkUrl);
                    }
                    var targetPublishmentSystemId = Node.GetPublishmentSystemId(nodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    return GetContentUrlById(targetPublishmentSystemInfo, nodeId, referenceId, 0, 0, linkUrl);
                }
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(publishmentSystemInfo, linkUrl);
            }
            var contentUrl = PathUtility.ContentFilePathRules.Parse(publishmentSystemInfo, nodeId, contentInfoCurrent);
            return GetPublishmentSystemUrl(publishmentSystemInfo, contentUrl);
        }

        private static string GetContentUrlById(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, int sourceId, int referenceId, string linkUrl)
        {
            var tableStyleCurrent = NodeManager.GetTableStyle(publishmentSystemInfo, nodeId);
            var tableNameCurrent = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
            var contentInfoCurrent = Content.GetContentInfo(tableStyleCurrent, tableNameCurrent, contentId);

            if (referenceId > 0 && contentInfoCurrent.GetString(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, sourceId) || NodeManager.IsExists(sourceId)))
                {
                    var targetNodeId = sourceId;
                    var targetPublishmentSystemId = Node.GetPublishmentSystemId(targetNodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                    var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                    var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                    var contentInfo = Content.GetContentInfo(tableStyle, tableName, referenceId);
                    if (contentInfo == null || contentInfo.NodeId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.PublishmentSystemId == targetPublishmentSystemInfo.PublishmentSystemId)
                    {
                        return GetContentUrlById(targetPublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetString(BackgroundContentAttribute.LinkUrl));
                    }
                    var publishmentSystemInfoTmp = PublishmentSystemManager.GetPublishmentSystemInfo(contentInfo.PublishmentSystemId);
                    return GetContentUrlById(publishmentSystemInfoTmp, contentInfo.NodeId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetString(BackgroundContentAttribute.LinkUrl));
                }
                else
                {
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                    nodeId = Content.GetNodeId(tableName, referenceId);
                    linkUrl = Content.GetValue(tableName, referenceId, BackgroundContentAttribute.LinkUrl);
                    return GetContentUrlById(publishmentSystemInfo, nodeId, referenceId, 0, 0, linkUrl);
                }
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(publishmentSystemInfo, linkUrl);
            }
            var contentUrl = PathUtility.ContentFilePathRules.Parse(publishmentSystemInfo, nodeId, contentId);
            return GetPublishmentSystemUrl(publishmentSystemInfo, contentUrl);
        }

        private static string GetChannelUrlNotComputed(PublishmentSystemInfo publishmentSystemInfo, int nodeId, ENodeType nodeType)
        {
            if (nodeType == ENodeType.BackgroundPublishNode)
            {
                return GetIndexPageUrl(publishmentSystemInfo);
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
                        return GetPublishmentSystemUrl(publishmentSystemInfo, channelUrl);
                    }
                    return ParseNavigationUrl(publishmentSystemInfo, PathUtility.AddVirtualToPath(filePath));
                }
            }

            return ParseNavigationUrl(publishmentSystemInfo, linkUrl);
        }

        //得到栏目经过计算后的连接地址
        public static string GetChannelUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var url = string.Empty;
            if (nodeInfo != null)
            {
                if (nodeInfo.NodeType == ENodeType.BackgroundPublishNode)
                {
                    url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType);
                }
                else
                {
                    if (nodeInfo.LinkType == ELinkType.LinkNoRelatedToChannelAndContent)
                    {
                        url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType);
                    }
                    else if (nodeInfo.LinkType == ELinkType.NoLink)
                    {
                        url = PageUtils.UnclickedUrl;
                    }
                    else
                    {
                        if (nodeInfo.LinkType == ELinkType.NoLinkIfContentNotExists)
                        {
                            url = nodeInfo.ContentNum == 0 ? PageUtils.UnclickedUrl : GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType);
                        }
                        else if (nodeInfo.LinkType == ELinkType.LinkToOnlyOneContent)
                        {
                            if (nodeInfo.ContentNum == 1)
                            {
                                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                                //var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                var contentId = Content.GetContentId(tableName, nodeInfo.NodeId, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(nodeInfo.Additional.DefaultTaxisType)));
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType);
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.NoLinkIfContentNotExistsAndLinkToOnlyOneContent)
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
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType);
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.LinkToFirstContent)
                        {
                            if (nodeInfo.ContentNum >= 1)
                            {
                                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                                var contentId = Content.GetContentId(tableName, nodeInfo.NodeId, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(nodeInfo.Additional.DefaultTaxisType)));
                                //var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType);
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
                        {
                            if (nodeInfo.ContentNum >= 1)
                            {
                                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                                var contentId = Content.GetContentId(tableName, nodeInfo.NodeId, ETaxisTypeUtils.GetContentOrderByString(ETaxisTypeUtils.GetEnumType(nodeInfo.Additional.DefaultTaxisType)));
                                //var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId);
                            }
                            else
                            {
                                url = PageUtils.UnclickedUrl;
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.NoLinkIfChannelNotExists)
                        {
                            url = nodeInfo.ChildrenCount == 0 ? PageUtils.UnclickedUrl : GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType);
                        }
                        else if (nodeInfo.LinkType == ELinkType.LinkToLastAddChannel)
                        {
                            var lastAddNodeInfo = Node.GetNodeInfoByLastAddDate(nodeInfo.NodeId);
                            url = lastAddNodeInfo != null ? GetChannelUrl(publishmentSystemInfo, lastAddNodeInfo) : GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType);
                        }
                        else if (nodeInfo.LinkType == ELinkType.LinkToFirstChannel)
                        {
                            var firstNodeInfo = Node.GetNodeInfoByTaxis(nodeInfo.NodeId);
                            url = firstNodeInfo != null ? GetChannelUrl(publishmentSystemInfo, firstNodeInfo) : GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType);
                        }
                        else if (nodeInfo.LinkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                        {
                            var lastAddNodeInfo = Node.GetNodeInfoByLastAddDate(nodeInfo.NodeId);
                            url = lastAddNodeInfo != null ? GetChannelUrl(publishmentSystemInfo, lastAddNodeInfo) : PageUtils.UnclickedUrl;
                        }
                        else if (nodeInfo.LinkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                        {
                            var firstNodeInfo = Node.GetNodeInfoByTaxis(nodeInfo.NodeId);
                            url = firstNodeInfo != null ? GetChannelUrl(publishmentSystemInfo, firstNodeInfo) : PageUtils.UnclickedUrl;
                        }
                    }
                }
            }
            return url;
        }

        public static string GetInputChannelUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            var channelUrl = GetChannelUrl(publishmentSystemInfo, nodeInfo);
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

        public static string ParseNavigationUrlAddPrefix(PublishmentSystemInfo publishmentSystemInfo, string url)
        {
            if (string.IsNullOrEmpty(url)) return ParseNavigationUrl(publishmentSystemInfo, url);

            if (!url.StartsWith("~/") && !url.StartsWith("@/"))
            {
                url = "@/" + url;
            }
            return ParseNavigationUrl(publishmentSystemInfo, url);
        }

        public static string ParseNavigationUrl(int publishmentSystemId, string url)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return ParseNavigationUrl(publishmentSystemInfo, url);
        }

        //根据发布系统属性判断是否为相对路径并返回解析后路径
        public static string ParseNavigationUrl(PublishmentSystemInfo publishmentSystemInfo, string url)
        {
            if (publishmentSystemInfo != null)
            {
                if (!string.IsNullOrEmpty(url) && url.StartsWith("@"))
                {
                    return GetPublishmentSystemUrl(publishmentSystemInfo, url.Substring(1));
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
