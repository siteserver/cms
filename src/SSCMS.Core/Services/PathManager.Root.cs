using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SSCMS.Core.StlParser.StlElement;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public string GetRootUrl(params string[] paths)
        {
            return PageUtils.Combine(_settingsManager.ApiHost, PageUtils.Combine(paths));
        }

        public string GetRootUrlByPath(string physicalPath)
        {
            var requestPath = PathUtils.GetPathDifference(_settingsManager.WebRootPath, physicalPath);
            requestPath = requestPath.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
            return GetRootUrl(requestPath);
        }

        public string GetTemporaryFilesUrl(params string[] paths)
        {
            return GetSiteFilesUrl(DirectoryUtils.SiteFiles.TemporaryFiles, PageUtils.Combine(paths));
        }

        public string GetSiteTemplatesUrl(string relatedUrl)
        {
            return GetRootUrl(DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.SiteTemplates.DirectoryName, relatedUrl);
        }

        public string ParseUrl(string virtualUrl)
        {
            if (string.IsNullOrEmpty(virtualUrl)) return string.Empty;

            virtualUrl = virtualUrl.StartsWith("~") ? GetRootUrl(virtualUrl.Substring(1)) : virtualUrl;
            virtualUrl = virtualUrl.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
            virtualUrl = virtualUrl.Replace(PageUtils.DoubleSeparator, PageUtils.SingleSeparator);
            return virtualUrl;
        }

        public string ParsePath(string virtualPath)
        {
            virtualPath = PathUtils.RemovePathInvalidChar(virtualPath);
            if (!string.IsNullOrEmpty(virtualPath))
            {
                if (virtualPath.StartsWith("~"))
                {
                    virtualPath = virtualPath.Substring(1);
                }
                virtualPath = PageUtils.Combine("~", virtualPath);
            }
            else
            {
                virtualPath = "~/";
            }
            var rootPath = WebRootPath;

            virtualPath = !string.IsNullOrEmpty(virtualPath) ? virtualPath.Substring(2) : string.Empty;
            var retVal = PathUtils.Combine(rootPath, virtualPath) ?? string.Empty;

            return retVal.Replace(PageUtils.SeparatorChar, PathUtils.SeparatorChar);
        }

        public string ParsePath(string directoryPath, string virtualPath)
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
            return ParsePath(resolvedPath);
        }

        public string GetSiteFilesUrl(params string[] paths)
        {
            return GetRootUrl(DirectoryUtils.SiteFiles.DirectoryName, PageUtils.Combine(paths));
        }

        public string GetAdministratorUploadUrl(int userId, params string[] paths)
        {
            return GetSiteFilesUrl(DirectoryUtils.SiteFiles.Administrators,
                PageUtils.Combine(userId.ToString(), PageUtils.Combine(paths)));
        }

        public string GetUserUploadUrl(int userId, params string[] paths)
        {
            return GetSiteFilesUrl(DirectoryUtils.SiteFiles.Users,
                PageUtils.Combine(userId.ToString(), PageUtils.Combine(paths)));
        }

        public string GetHomeUploadUrl(params string[] paths)
        {
            return GetSiteFilesUrl(DirectoryUtils.SiteFiles.Home, PageUtils.Combine(paths));
        }

        public string DefaultAvatarUrl => GetHomeUploadUrl("default_avatar.png");

        public string GetUserUploadFileName(string filePath)
        {
            var dt = DateTime.Now;
            return $"{dt.Day}{dt.Hour}{dt.Minute}{dt.Second}{dt.Millisecond}{PathUtils.GetExtension(filePath)}";
        }

        public string GetUserUploadUrl(int userId, string relatedUrl)
        {
            return GetHomeUploadUrl(userId.ToString(), relatedUrl);
        }

        public string GetUserAvatarUrl(User user)
        {
            var imageUrl = user?.AvatarUrl;

            if (!string.IsNullOrEmpty(imageUrl))
            {
                return PageUtils.IsProtocolUrl(imageUrl) ? imageUrl : GetUserUploadUrl(user.Id, imageUrl);
            }

            return DefaultAvatarUrl;
        }

        public string GetRootPath(params string[] paths)
        {
            return PathUtils.Combine(_settingsManager.WebRootPath, PathUtils.Combine(paths));
        }

        public string GetContentRootPath(params string[] paths)
        {
            return PathUtils.Combine(_settingsManager.ContentRootPath, PathUtils.Combine(paths));
        }

        public string GetSiteFilesPath(params string[] paths)
        {
            var path = PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName, PathUtils.Combine(paths));
            return path;
        }

        public string GetAdministratorUploadPath(int userId, params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.Administrators, PathUtils.Combine(userId.ToString(), PathUtils.Combine(paths)));
            return path;
        }

        public string GetUserUploadPath(int userId, params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.Users, PathUtils.Combine(userId.ToString(), PathUtils.Combine(paths)));
            return path;
        }

        public string GetHomeUploadPath(params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.Home, PathUtils.Combine(paths));
            return path;
        }

        public string GetTemporaryFilesPath(params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.TemporaryFiles, PathUtils.Combine(paths));
            return path;
        }

        public string GetUserUploadPath(int userId, string relatedPath)
        {
            return GetHomeUploadPath(userId.ToString(), relatedPath);
        }

        public string GetApiUrl(params string[] paths)
        {
            return PageUtils.Combine(_settingsManager.ApiHost, Constants.ApiPrefix, PathUtils.Combine(paths));
        }

        public string GetDownloadApiUrl(int siteId, int channelId, int contentId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(_settingsManager.ApiHost, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsDownload), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileUrl", _settingsManager.Encrypt(fileUrl)}
            });
        }

        public string GetDownloadApiUrl(int siteId, string fileUrl)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(_settingsManager.ApiHost, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsDownload), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"fileUrl", _settingsManager.Encrypt(fileUrl)}
            });
        }

        public string GetDownloadApiUrl(bool isInner, string filePath)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(_settingsManager.ApiHost, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsDownload), new NameValueCollection
            {
                {"filePath", _settingsManager.Encrypt(filePath)}
            });
        }

        public string GetDynamicApiUrl()
        {
            return PageUtils.Combine(_settingsManager.ApiHost, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsDynamic);
        }

        public string GetIfApiUrl()
        {
            return PageUtils.Combine(_settingsManager.ApiHost, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlRouteActionsIf);
        }

        public string GetPageContentsApiUrl()
        {
            return PageUtils.Combine(_settingsManager.ApiHost, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsPageContents);
        }

        public string GetPageContentsApiParameters(int siteId, int pageChannelId, int templateId, int totalNum, int pageCount,
            int currentPageIndex, string stlPageContentsElement)
        {
            return $@"
{{
    siteId: {siteId},
    pageChannelId: {pageChannelId},
    templateId: {templateId},
    totalNum: {totalNum},
    pageCount: {pageCount},
    currentPageIndex: {currentPageIndex},
    stlPageContentsElement: '{_settingsManager.Encrypt(stlPageContentsElement)}'
}}";
        }

        public string GetSearchApiUrl()
        {
            return PageUtils.Combine(_settingsManager.ApiHost, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsSearch);
        }

        public string GetSearchApiParameters(bool isAllSites, string siteName, string siteDir, string siteIds, string channelIndex, string channelName, string channelIds, string type, string word, string dateAttribute, string dateFrom, string dateTo, string since, int pageNum, bool isHighlight, int siteId, string ajaxDivId, string template)
        {
            return $@"
{{
    {StringUtils.ToLower(StlSearch.IsAllSites)}: {StringUtils.ToLower(isAllSites.ToString())},
    {StringUtils.ToLower(StlSearch.SiteName)}: '{siteName}',
    {StringUtils.ToLower(StlSearch.SiteDir)}: '{siteDir}',
    {StringUtils.ToLower(StlSearch.SiteIds)}: '{siteIds}',
    {StringUtils.ToLower(StlSearch.ChannelIndex)}: '{channelIndex}',
    {StringUtils.ToLower(StlSearch.ChannelName)}: '{channelName}',
    {StringUtils.ToLower(StlSearch.ChannelIds)}: '{channelIds}',
    {StringUtils.ToLower(StlSearch.Type)}: '{type}',
    {StringUtils.ToLower(StlSearch.Word)}: '{word}',
    {StringUtils.ToLower(StlSearch.DateAttribute)}: '{dateAttribute}',
    {StringUtils.ToLower(StlSearch.DateFrom)}: '{dateFrom}',
    {StringUtils.ToLower(StlSearch.DateTo)}: '{dateTo}',
    {StringUtils.ToLower(StlSearch.Since)}: '{since}',
    {StringUtils.ToLower(StlSearch.PageNum)}: {pageNum},
    {StringUtils.ToLower(StlSearch.IsHighlight)}: {StringUtils.ToLower(isHighlight.ToString())},
    siteid: '{siteId}',
    ajaxdivid: '{ajaxDivId}',
    template: '{_settingsManager.Encrypt(template)}',
}}";
        }

        public List<string> GetSearchExcludeAttributeNames => new List<string>
        {
            StringUtils.ToLower(StlSearch.IsAllSites),
            StringUtils.ToLower(StlSearch.SiteName),
            StringUtils.ToLower(StlSearch.SiteDir),
            StringUtils.ToLower(StlSearch.SiteIds),
            StringUtils.ToLower(StlSearch.ChannelIndex),
            StringUtils.ToLower(StlSearch.ChannelName),
            StringUtils.ToLower(StlSearch.ChannelIds),
            StringUtils.ToLower(StlSearch.Type),
            StringUtils.ToLower(StlSearch.Word),
            StringUtils.ToLower(StlSearch.DateAttribute),
            StringUtils.ToLower(StlSearch.DateFrom),
            StringUtils.ToLower(StlSearch.DateTo),
            StringUtils.ToLower(StlSearch.Since),
            StringUtils.ToLower(StlSearch.PageNum),
            StringUtils.ToLower(StlSearch.IsHighlight),
            "siteid",
            "ajaxdivid",
            "template",
        };

        public string GetTriggerApiUrl(int siteId, int channelId, int contentId,
            int fileTemplateId, bool isRedirect)
        {
            return PageUtils.AddQueryString(PageUtils.Combine(_settingsManager.ApiHost, Constants.ApiPrefix, Constants.ApiStlPrefix, Constants.RouteStlActionsTrigger), new NameValueCollection
            {
                {"siteId", siteId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileTemplateId", fileTemplateId.ToString()},
                {"isRedirect", isRedirect.ToString()}
            });
        }
    }
}
