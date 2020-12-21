using System;
using System.Collections.Specialized;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public string GetRootUrl(params string[] paths)
        {
            return PageUtils.Combine("/", PageUtils.Combine(paths));
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
            if (PageUtils.IsAbsoluteUrl(virtualUrl)) return virtualUrl;

            virtualUrl = virtualUrl.StartsWith("~") ? GetRootUrl(virtualUrl.Substring(1)) : virtualUrl;
            virtualUrl = virtualUrl.Replace(PathUtils.SeparatorChar, PageUtils.SeparatorChar);
            virtualUrl = virtualUrl.Replace(PageUtils.DoubleSeparator, PageUtils.Separator);
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

        public string GetSiteFilesUrl(Site site, params string[] paths)
        {
            return site == null
                ? GetSiteFilesUrl(paths)
                : GetApiHostUrl(site, DirectoryUtils.SiteFiles.DirectoryName, PageUtils.Combine(paths));
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

        public string DefaultAvatarUrl => GetSiteFilesUrl("assets/images/default_avatar.png");

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
                return PageUtils.IsAbsoluteUrl(imageUrl) ? imageUrl : GetUserUploadUrl(user.Id, imageUrl);
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

        public string GetDownloadApiUrl(Site site, int channelId, int contentId, string fileUrl)
        {
            var apiUrl = GetApiHostUrl(site, Constants.ApiPrefix);
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Constants.ApiStlPrefix, Constants.RouteStlActionsDownload), new NameValueCollection
            {
                {"siteId", site.Id.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"fileUrl", _settingsManager.Encrypt(fileUrl)}
            });
        }

        public string GetDownloadApiUrl(Site site, string fileUrl)
        {
            var apiUrl = GetApiHostUrl(site, Constants.ApiPrefix);
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Constants.ApiStlPrefix, Constants.RouteStlActionsDownload), new NameValueCollection
            {
                {"siteId", site.Id.ToString()},
                {"fileUrl", _settingsManager.Encrypt(fileUrl)}
            });
        }

        public string GetDownloadApiUrl(string filePath)
        {
            var apiUrl = PageUtils.GetLocalApiUrl(Constants.ApiStlPrefix, Constants.RouteStlActionsDownload);
            return PageUtils.AddQueryString(apiUrl, new NameValueCollection
            {
                {"filePath", _settingsManager.Encrypt(filePath)}
            });
        }

        public string GetDynamicApiUrl(Site site)
        {
            var apiUrl = GetApiHostUrl(site, Constants.ApiPrefix);
            return PageUtils.Combine(apiUrl, Constants.ApiStlPrefix, Constants.RouteStlActionsDynamic);
        }

        public string GetIfApiUrl(Site site)
        {
            var apiUrl = GetApiHostUrl(site, Constants.ApiPrefix);
            return PageUtils.Combine(apiUrl, Constants.ApiStlPrefix, Constants.RouteStlRouteActionsIf);
        }

        public string GetPageContentsApiUrl(Site site)
        {
            var apiUrl = GetApiHostUrl(site, Constants.ApiPrefix);
            return PageUtils.Combine(apiUrl, Constants.ApiStlPrefix, Constants.RouteStlActionsPageContents);
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

        public string GetTriggerApiUrl(int siteId, int channelId, int contentId,
            int fileTemplateId, bool isRedirect)
        {
            var apiUrl = PageUtils.GetLocalApiUrl(Constants.ApiStlPrefix);
            return PageUtils.AddQueryString(PageUtils.Combine(apiUrl, Constants.RouteStlActionsTrigger), new NameValueCollection
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
