using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.CMS.Model;
using BaiRong.Core.Model.Attributes;
using SiteServer.CMS.Model.Enumerations;
using System;

namespace SiteServer.CMS.Core
{
    public class PageUtility
    {
        private PageUtility()
        {
        }

        public static string GetPublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo, string requestPath)
        {
            return GetPublishmentSystemUrl(publishmentSystemInfo, requestPath, false);
        }

        public static string GetPublishmentSystemUrl(PublishmentSystemInfo publishmentSystemInfo, string requestPath, bool isFromBackground)
        {
            var url = string.Empty;

            if (isFromBackground)
            {
                if (publishmentSystemInfo.Additional.IsMultiDeployment)
                {
                    url = publishmentSystemInfo.Additional.InnerUrl;
                }
            }
            if (string.IsNullOrEmpty(url))
            {
                url = publishmentSystemInfo.PublishmentSystemUrl;
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

            if (!string.IsNullOrEmpty(requestPath))
            {
                requestPath = PathUtils.RemovePathInvalidChar(requestPath);
                if (requestPath.StartsWith("/"))
                {
                    requestPath = requestPath.Substring(1);
                }

                url = PageUtils.Combine(url, requestPath);
            }
            return url;
        }

        /****获取编辑器中内容，解析@符号，添加了远程路径处理 20151103****/
        public static string GetPublishmentSystemUrlForEditorUploadFilePre(PublishmentSystemInfo publishmentSystemInfo, string requestPath, bool isFromBackground)
        {
            var url = string.Empty;

            if (isFromBackground)
            {
                if (publishmentSystemInfo.Additional.IsMultiDeployment)
                {
                    url = publishmentSystemInfo.Additional.InnerUrl;
                }
            }
            else if (requestPath.StartsWith("@/upload") || requestPath.StartsWith("/upload") || requestPath.StartsWith("@\\upload") || requestPath.StartsWith("\\upload"))
            {
                url = publishmentSystemInfo.Additional.EditorUploadFilePre;
            }
            if (string.IsNullOrEmpty(url))
            {
                url = publishmentSystemInfo.PublishmentSystemUrl;
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

            if (!string.IsNullOrEmpty(requestPath))
            {
                requestPath = PathUtils.RemovePathInvalidChar(requestPath);
                if (requestPath.StartsWith("/"))
                {
                    requestPath = requestPath.Substring(1);
                }

                url = PageUtils.Combine(url, requestPath);
            }
            return url;
        }

        //返回代码类似/dev/site/images/pic.jpg
        public static string GetPublishmentSystemUrlByPhysicalPath(PublishmentSystemInfo publishmentSystemInfo, string physicalPath)
        {
            if (publishmentSystemInfo == null)
            {
                var publishmentSystemId = PathUtility.GetCurrentPublishmentSystemId();
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            }
            if (!string.IsNullOrEmpty(physicalPath))
            {
                var publishmentSystemPath = PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
                string requestPath;
                if (physicalPath.StartsWith(publishmentSystemPath))
                {
                    requestPath = StringUtils.ReplaceStartsWith(physicalPath, publishmentSystemPath, string.Empty);
                }
                else
                {
                    requestPath = physicalPath.ToLower().Replace(publishmentSystemPath.ToLower(), string.Empty);
                }
                requestPath = requestPath.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
                return GetPublishmentSystemUrl(publishmentSystemInfo, requestPath);
            }
            return publishmentSystemInfo.PublishmentSystemUrl;
        }

        //level=0代表站点根目录，1代表下一级目标。。。返回代码类似../images/pic.jpg
        public static string GetPublishmentSystemUrlOfRelatedByPhysicalPath(PublishmentSystemInfo publishmentSystemInfo, string physicalPath, int level)
        {
            if (publishmentSystemInfo == null)
            {
                var publishmentSystemId = PathUtility.GetCurrentPublishmentSystemId();
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            }
            if (!string.IsNullOrEmpty(physicalPath))
            {
                var publishmentSystemPath = PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
                var requestPath = physicalPath.ToLower().Replace(publishmentSystemPath.ToLower(), string.Empty);
                requestPath = requestPath.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
                requestPath = requestPath.Trim(PageUtils.SeparatorChar);
                if (level > 0)
                {
                    for (var i = 0; i < level; i++)
                    {
                        requestPath = "../" + requestPath;
                    }
                }
                return requestPath;
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetPublishmentSystemVirtualUrlByPhysicalPath(PublishmentSystemInfo publishmentSystemInfo, string physicalPath)
        {
            if (publishmentSystemInfo == null)
            {
                var publishmentSystemId = PathUtility.GetCurrentPublishmentSystemId();
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            }
            if (!string.IsNullOrEmpty(physicalPath))
            {
                var publishmentSystemPath = PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
                var requestPath = physicalPath.ToLower().Replace(publishmentSystemPath.ToLower(), string.Empty);
                requestPath = requestPath.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
                return PageUtils.Combine("@", requestPath);
            }
            else
            {
                return string.Empty;
            }
        }

        public static string GetPublishmentSystemVirtualUrlByAbsoluteUrl(PublishmentSystemInfo publishmentSystemInfo, string absoluteUrl)
        {
            if (publishmentSystemInfo == null)
            {
                var publishmentSystemId = PathUtility.GetCurrentPublishmentSystemId();
                publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            }
            if (!string.IsNullOrEmpty(absoluteUrl))
            {
                if (PageUtils.IsProtocolUrl(absoluteUrl) || absoluteUrl.StartsWith("/"))
                {
                    absoluteUrl = absoluteUrl.ToLower();
                    var publishmentSystemUrl = GetPublishmentSystemUrl(publishmentSystemInfo, string.Empty).ToLower();

                    if (PageUtils.IsProtocolUrl(absoluteUrl))
                    {
                        publishmentSystemUrl = PageUtils.AddProtocolToUrl(publishmentSystemUrl);
                    }

                    absoluteUrl = StringUtils.ReplaceFirst(publishmentSystemUrl, absoluteUrl, string.Empty);
                }
                else if (absoluteUrl.StartsWith("."))
                {
                    absoluteUrl = absoluteUrl.Replace("../", string.Empty);
                    absoluteUrl = absoluteUrl.Replace("./", string.Empty);
                }
                return PageUtils.Combine("@", absoluteUrl);
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 从设置信息中的RootUrl中取得地址，并结合发布系统和相对地址，得到能够运行动态文件的地址。
        /// </summary>
        public static string GetRootUrlByPublishmentSystemId(PublishmentSystemInfo psInfo, string requestPath)
        {
            if (psInfo == null)
            {
                var publishmentSystemId = PathUtility.GetCurrentPublishmentSystemId();
                psInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            }
            //string url = ConfigUtils.Instance.ApplicationPath;
            var url = PageUtils.GetRootUrl(string.Empty);

            if (url.EndsWith("/"))
            {
                url = url.Substring(0, url.Length - 1);
            }
            url += "/" + psInfo.PublishmentSystemDir;

            if (requestPath != null && requestPath.Trim().Length > 0)
            {
                if (requestPath.StartsWith("/"))
                {
                    requestPath = requestPath.Substring(1);
                }
                if (requestPath.EndsWith("/"))
                {
                    requestPath = requestPath.Substring(0, requestPath.Length - 1);
                }
                url = url + "/" + requestPath;
            }
            return url;
        }

        //public static string GetSiteFilesUrl(PublishmentSystemInfo publishmentSystemInfo, string relatedUrl)
        //{
        //    return GetSiteFilesUrl(publishmentSystemInfo.Additional.ApiUrl, relatedUrl);
        //}

        //public static string GetSiteFilesUrl(string apiUrl, string relatedUrl)
        //{
        //    if (string.IsNullOrEmpty(apiUrl))
        //    {
        //        apiUrl = "/api";
        //    }
        //    apiUrl = apiUrl.Trim().ToLower();
        //    if (apiUrl == "/api")
        //    {
        //        apiUrl = "/";
        //    }
        //    else if (apiUrl.EndsWith("/api"))
        //    {
        //        apiUrl = apiUrl.Substring(0, apiUrl.LastIndexOf("/api", StringComparison.Ordinal));
        //    }
        //    else if (apiUrl.EndsWith("/api/"))
        //    {
        //        apiUrl = apiUrl.Substring(0, apiUrl.LastIndexOf("/api/", StringComparison.Ordinal));
        //    }
        //    if (string.IsNullOrEmpty(apiUrl))
        //    {
        //        apiUrl = "/";
        //    }
        //    return PageUtils.Combine(apiUrl, DirectoryUtils.SiteFiles.DirectoryName, relatedUrl);
        //}

        public static string GetSiteTemplatesUrl(string relatedUrl)
        {
            return PageUtils.Combine(WebConfigUtils.ApplicationPath, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteTemplates.DirectoryName, relatedUrl);
        }

        public static string GetSiteTemplateMetadataUrl(string siteTemplateUrl, string relatedUrl)
        {
            return PageUtils.Combine(siteTemplateUrl, DirectoryUtils.SiteTemplates.SiteTemplateMetadata, relatedUrl);
        }

        public static string GetIndexPageUrl(PublishmentSystemInfo publishmentSystemInfo)
        {
            return GetIndexPageUrl(publishmentSystemInfo, false);
        }

        // 得到发布系统首页地址
        public static string GetIndexPageUrl(PublishmentSystemInfo publishmentSystemInfo, bool isFromBackground)
        {
            if (!string.IsNullOrEmpty(publishmentSystemInfo.PublishmentSystemUrl) && !isFromBackground)
            {
                return publishmentSystemInfo.PublishmentSystemUrl;
            }

            var indexTemplateId = TemplateManager.GetIndexTempalteID(publishmentSystemInfo.PublishmentSystemId);
            var createdFileFullName = TemplateManager.GetCreatedFileFullName(publishmentSystemInfo.PublishmentSystemId, indexTemplateId);

            return ParseNavigationUrl(publishmentSystemInfo, createdFileFullName, isFromBackground);
        }

        public static string GetFileUrl(PublishmentSystemInfo publishmentSystemInfo, int fileTemplateId)
        {
            var createdFileFullName = TemplateManager.GetCreatedFileFullName(publishmentSystemInfo.PublishmentSystemId, fileTemplateId);
            return ParseNavigationUrl(publishmentSystemInfo, createdFileFullName);
        }

        public static string GetContentUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, int contentId, bool isFromBackground)
        {
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
            var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
            return GetContentUrlById(publishmentSystemInfo, contentInfo, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl), isFromBackground);
        }

        public static string GetContentUrl(PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo)
        {
            return GetContentUrlById(publishmentSystemInfo, contentInfo, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl), false);
        }

        public static string GetContentUrl(PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfo, bool isFromBackground)
        {
            return GetContentUrlById(publishmentSystemInfo, contentInfo, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl), isFromBackground);
        }

        /// <summary>
        /// 对GetContentUrlByID的优化
        /// 通过传入参数contentInfoCurrent，避免对ContentInfo查询太多
        /// </summary>
        private static string GetContentUrlById(PublishmentSystemInfo publishmentSystemInfo, ContentInfo contentInfoCurrent, int sourceId, int referenceId, string linkUrl, bool isFromBackground)
        {
            var nodeId = contentInfoCurrent.NodeId;
            if (referenceId > 0 && contentInfoCurrent.GetExtendedAttribute(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, sourceId) || NodeManager.IsExists(sourceId)))
                {
                    var targetNodeId = sourceId;
                    var targetPublishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(targetNodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                    var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                    var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, referenceId);
                    if (contentInfo == null || contentInfo.NodeId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.PublishmentSystemId == targetPublishmentSystemInfo.PublishmentSystemId)
                    {
                        return GetContentUrlById(targetPublishmentSystemInfo, contentInfo, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl), isFromBackground);
                    }
                    var publishmentSystemInfoTmp = PublishmentSystemManager.GetPublishmentSystemInfo(contentInfo.PublishmentSystemId);
                    return GetContentUrlById(publishmentSystemInfoTmp, contentInfo, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl), isFromBackground);
                }
                else
                {
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                    nodeId = BaiRongDataProvider.ContentDao.GetNodeId(tableName, referenceId);
                    linkUrl = BaiRongDataProvider.ContentDao.GetValue(tableName, referenceId, BackgroundContentAttribute.LinkUrl);
                    if (NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, nodeId))
                    {
                        return GetContentUrlById(publishmentSystemInfo, nodeId, referenceId, 0, 0, linkUrl, isFromBackground);
                    }
                    var targetPublishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(nodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    return GetContentUrlById(targetPublishmentSystemInfo, nodeId, referenceId, 0, 0, linkUrl, isFromBackground);
                }
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(publishmentSystemInfo, linkUrl, isFromBackground);
            }
            var contentUrl = PathUtility.ContentFilePathRules.Parse(publishmentSystemInfo, nodeId, contentInfoCurrent);
            return GetPublishmentSystemUrl(publishmentSystemInfo, contentUrl, isFromBackground);
        }


        private static string GetContentUrlById(PublishmentSystemInfo publishmentSystemInfo, int nodeId, int contentId, int sourceId, int referenceId, string linkUrl, bool isFromBackground)
        {
            var tableStyleCurrent = NodeManager.GetTableStyle(publishmentSystemInfo, nodeId);
            var tableNameCurrent = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
            var contentInfoCurrent = DataProvider.ContentDao.GetContentInfo(tableStyleCurrent, tableNameCurrent, contentId);

            if (referenceId > 0 && contentInfoCurrent.GetExtendedAttribute(ContentAttribute.TranslateContentType) != ETranslateContentType.ReferenceContent.ToString())
            {
                if (sourceId > 0 && (NodeManager.IsExists(publishmentSystemInfo.PublishmentSystemId, sourceId) || NodeManager.IsExists(sourceId)))
                {
                    var targetNodeId = sourceId;
                    var targetPublishmentSystemId = DataProvider.NodeDao.GetPublishmentSystemId(targetNodeId);
                    var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
                    var targetNodeInfo = NodeManager.GetNodeInfo(targetPublishmentSystemId, targetNodeId);

                    var tableStyle = NodeManager.GetTableStyle(targetPublishmentSystemInfo, targetNodeInfo);
                    var tableName = NodeManager.GetTableName(targetPublishmentSystemInfo, targetNodeInfo);
                    var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, referenceId);
                    if (contentInfo == null || contentInfo.NodeId <= 0)
                    {
                        return PageUtils.UnclickedUrl;
                    }
                    if (contentInfo.PublishmentSystemId == targetPublishmentSystemInfo.PublishmentSystemId)
                    {
                        return GetContentUrlById(targetPublishmentSystemInfo, contentInfo.NodeId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl), isFromBackground);
                    }
                    var publishmentSystemInfoTmp = PublishmentSystemManager.GetPublishmentSystemInfo(contentInfo.PublishmentSystemId);
                    return GetContentUrlById(publishmentSystemInfoTmp, contentInfo.NodeId, contentInfo.Id, contentInfo.SourceId, contentInfo.ReferenceId, contentInfo.GetExtendedAttribute(BackgroundContentAttribute.LinkUrl), isFromBackground);
                }
                else
                {
                    var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeId);
                    nodeId = BaiRongDataProvider.ContentDao.GetNodeId(tableName, referenceId);
                    linkUrl = BaiRongDataProvider.ContentDao.GetValue(tableName, referenceId, BackgroundContentAttribute.LinkUrl);
                    return GetContentUrlById(publishmentSystemInfo, nodeId, referenceId, 0, 0, linkUrl, isFromBackground);
                }
            }
            if (!string.IsNullOrEmpty(linkUrl))
            {
                return ParseNavigationUrl(publishmentSystemInfo, linkUrl, isFromBackground);
            }
            var contentUrl = PathUtility.ContentFilePathRules.Parse(publishmentSystemInfo, nodeId, contentId);
            return GetPublishmentSystemUrl(publishmentSystemInfo, contentUrl, isFromBackground);
        }

        private static string GetChannelUrlNotComputed(PublishmentSystemInfo publishmentSystemInfo, int nodeId, ENodeType nodeType, bool isFromBackground)
        {
            if (nodeType == ENodeType.BackgroundPublishNode)
            {
                return GetIndexPageUrl(publishmentSystemInfo, isFromBackground);
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
                        return GetPublishmentSystemUrl(publishmentSystemInfo, channelUrl, isFromBackground);
                    }
                    return ParseNavigationUrl(publishmentSystemInfo, PathUtility.AddVirtualToPath(filePath), isFromBackground);
                }
            }

            return ParseNavigationUrl(publishmentSystemInfo, linkUrl, isFromBackground);
        }

        public static string GetChannelUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo)
        {
            return GetChannelUrl(publishmentSystemInfo, nodeInfo, false);
        }

        //得到栏目经过计算后的连接地址
        public static string GetChannelUrl(PublishmentSystemInfo publishmentSystemInfo, NodeInfo nodeInfo, bool isFromBackground)
        {
            var url = string.Empty;
            if (nodeInfo != null)
            {
                if (nodeInfo.NodeType == ENodeType.BackgroundPublishNode)
                {
                    url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType, isFromBackground);
                }
                else
                {
                    if (nodeInfo.LinkType == ELinkType.LinkNoRelatedToChannelAndContent)
                    {
                        url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType, isFromBackground);
                    }
                    else if (nodeInfo.LinkType == ELinkType.NoLink)
                    {
                        url = PageUtils.UnclickedUrl;
                    }
                    else
                    {
                        if (nodeInfo.LinkType == ELinkType.NoLinkIfContentNotExists)
                        {
                            if (nodeInfo.ContentNum == 0)
                            {
                                url = PageUtils.UnclickedUrl;
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType, isFromBackground);
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.LinkToOnlyOneContent)
                        {
                            if (nodeInfo.ContentNum == 1)
                            {
                                var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, isFromBackground);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType, isFromBackground);
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
                                var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, isFromBackground);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType, isFromBackground);
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.LinkToFirstContent)
                        {
                            if (nodeInfo.ContentNum >= 1)
                            {
                                var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, isFromBackground);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType, isFromBackground);
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.NoLinkIfContentNotExistsAndLinkToFirstContent)
                        {
                            if (nodeInfo.ContentNum >= 1)
                            {
                                var contentId = StlCacheManager.FirstContentId.GetValue(publishmentSystemInfo, nodeInfo);
                                url = GetContentUrl(publishmentSystemInfo, nodeInfo, contentId, isFromBackground);
                            }
                            else
                            {
                                url = PageUtils.UnclickedUrl;
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.NoLinkIfChannelNotExists)
                        {
                            if (nodeInfo.ChildrenCount == 0)
                            {
                                url = PageUtils.UnclickedUrl;
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType, isFromBackground);
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.LinkToLastAddChannel)
                        {
                            var lastAddNodeInfo = DataProvider.NodeDao.GetNodeInfoByLastAddDate(nodeInfo.NodeId);
                            if (lastAddNodeInfo != null)
                            {
                                url = GetChannelUrl(publishmentSystemInfo, lastAddNodeInfo, isFromBackground);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType, isFromBackground);
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.LinkToFirstChannel)
                        {
                            var firstNodeInfo = DataProvider.NodeDao.GetNodeInfoByTaxis(nodeInfo.NodeId);
                            if (firstNodeInfo != null)
                            {
                                url = GetChannelUrl(publishmentSystemInfo, firstNodeInfo, isFromBackground);
                            }
                            else
                            {
                                url = GetChannelUrlNotComputed(publishmentSystemInfo, nodeInfo.NodeId, nodeInfo.NodeType, isFromBackground);
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToLastAddChannel)
                        {
                            var lastAddNodeInfo = DataProvider.NodeDao.GetNodeInfoByLastAddDate(nodeInfo.NodeId);
                            if (lastAddNodeInfo != null)
                            {
                                url = GetChannelUrl(publishmentSystemInfo, lastAddNodeInfo, isFromBackground);
                            }
                            else
                            {
                                url = PageUtils.UnclickedUrl;
                            }
                        }
                        else if (nodeInfo.LinkType == ELinkType.NoLinkIfChannelNotExistsAndLinkToFirstChannel)
                        {
                            var firstNodeInfo = DataProvider.NodeDao.GetNodeInfoByTaxis(nodeInfo.NodeId);
                            if (firstNodeInfo != null)
                            {
                                url = GetChannelUrl(publishmentSystemInfo, firstNodeInfo, isFromBackground);
                            }
                            else
                            {
                                url = PageUtils.UnclickedUrl;
                            }
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

            channelUrl = StringUtils.ReplaceStartsWith(channelUrl, publishmentSystemInfo.PublishmentSystemUrl, string.Empty);
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
            return ParseNavigationUrl(publishmentSystemInfo, url, false);
        }

        public static string ParseNavigationUrl(PublishmentSystemInfo publishmentSystemInfo, string url)
        {
            return ParseNavigationUrl(publishmentSystemInfo, url, false);
        }

        //根据发布系统属性判断是否为相对路径并返回解析后路径
        public static string ParseNavigationUrl(PublishmentSystemInfo publishmentSystemInfo, string url, bool isFromBackground)
        {
            if (publishmentSystemInfo != null)
            {
                if (!string.IsNullOrEmpty(url) && url.StartsWith("@"))
                {
                    var extensionName = PathUtils.GetExtension(url).ToLower();
                    //如果设置编辑器上传文件URL前缀,排除单页html,只允许file,image,video
                    if ((url.StartsWith("@/upload") || url.StartsWith("/upload") || url.StartsWith("@\\upload") || url.StartsWith("\\upload"))
                        && !string.IsNullOrEmpty(publishmentSystemInfo.Additional.EditorUploadFilePre)
                        && (PathUtility.IsImageExtenstionAllowed(publishmentSystemInfo, extensionName)
                        || PathUtility.IsFileExtenstionAllowed(publishmentSystemInfo, extensionName)
                        || PathUtility.IsVideoExtenstionAllowed(publishmentSystemInfo, extensionName)))
                    {
                        /****获取编辑器中内容，解析@符号，添加了远程路径处理 20151103****/
                        return GetPublishmentSystemUrlForEditorUploadFilePre(publishmentSystemInfo, url.Substring(1), isFromBackground);
                    }
                    return GetPublishmentSystemUrl(publishmentSystemInfo, url.Substring(1), isFromBackground);
                }
                return PageUtils.ParseNavigationUrl(url, WebConfigUtils.ApplicationPath);
            }
            return PageUtils.ParseNavigationUrl(url);
        }

        public static string GetVirtualUrl(PublishmentSystemInfo publishmentSystemInfo, string url)
        {
            var virtualUrl = StringUtils.ReplaceStartsWith(url, publishmentSystemInfo.PublishmentSystemUrl, "@/");
            return StringUtils.ReplaceStartsWith(virtualUrl, "@//", "@/");
        }

        public static bool IsVirtualUrl(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            return url.StartsWith("~") || url.StartsWith("@");
        }

        public static string GetUserFilesUrl(string apiUrl, string relatedUrl)
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
            return PageUtils.Combine(apiUrl, DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.UserFiles,
                relatedUrl);
        }

        public static string GetUserAvatarUrl(string apiUrl, UserInfo userInfo)
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
