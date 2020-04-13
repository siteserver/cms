using System;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PathManager
    {
        public string GetSiteFilesPath(params string[] paths)
        {
            var path = PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFilesDirectoryName, PathUtils.Combine(paths));
            return path;
        }

        public string GetSiteFilesUrl(params string[] paths)
        {
            return GetWebRootUrl(DirectoryUtils.SiteFilesDirectoryName, PageUtils.Combine(paths));
        }

        public string GetAdministratorUploadPath(int userId, params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.Administrators, PathUtils.Combine(userId.ToString(), PathUtils.Combine(paths)));
            return path;
        }

        public string GetAdministratorUploadUrl(int userId, params string[] paths)
        {
            return GetWebRootUrl(DirectoryUtils.SiteFilesDirectoryName, DirectoryUtils.SiteFiles.Administrators,
                PageUtils.Combine(userId.ToString(), PageUtils.Combine(paths)));
        }

        public string GetUserUploadPath(int userId, params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.Users, PathUtils.Combine(userId.ToString(), PathUtils.Combine(paths)));
            return path;
        }

        public string GetUserUploadUrl(int userId, params string[] paths)
        {
            return GetWebRootUrl(DirectoryUtils.SiteFilesDirectoryName, DirectoryUtils.SiteFiles.Users,
                PageUtils.Combine(userId.ToString(), PageUtils.Combine(paths)));
        }

        public string GetHomeUploadPath(params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.Home, PathUtils.Combine(paths));
            return path;
        }

        public string GetHomeUploadUrl(params string[] paths)
        {
            return GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.Home, PageUtils.Combine(paths)));
        }

        public string GetTemporaryFilesPath(params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.TemporaryFiles, PathUtils.Combine(paths));
            return path;
        }

        public string GetTemporaryFilesUrl(params string[] paths)
        {
            return GetSiteFilesUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.TemporaryFiles, PageUtils.Combine(paths)));
        }

        public string DefaultAvatarUrl => GetHomeUploadUrl("default_avatar.png");

        public string GetUserUploadPath(int userId, string relatedPath)
        {
            return GetHomeUploadPath(userId.ToString(), relatedPath);
        }

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
    }
}
