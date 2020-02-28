using System.Threading.Tasks;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Services
{
    public partial class PathManager
    {
        public string GetSiteFilesPath(params string[] paths)
        {
            var path = PathUtils.Combine(_settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName, PathUtils.Combine(paths));
            return path;
        }

        public string GetSiteFilesUrl(params string[] paths)
        {
            return GetWebUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.DirectoryName, PageUtils.Combine(paths)));
        }

        public string GetAdministratorUploadPath(int userId, params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.Administrators, PathUtils.Combine(userId.ToString(), PathUtils.Combine(paths)));
            return path;
        }

        public string GetAdministratorUploadUrl(int userId, params string[] paths)
        {
            return GetWebUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.Administrators,
                PageUtils.Combine(userId.ToString(), PageUtils.Combine(paths))));
        }

        public string GetUserUploadPath(int userId, params string[] paths)
        {
            var path = GetSiteFilesPath(DirectoryUtils.SiteFiles.Users, PathUtils.Combine(userId.ToString(), PathUtils.Combine(paths)));
            return path;
        }

        public string GetUserUploadUrl(int userId, params string[] paths)
        {
            return GetWebUrl(PageUtils.Combine(DirectoryUtils.SiteFiles.DirectoryName, DirectoryUtils.SiteFiles.Users,
                PageUtils.Combine(userId.ToString(), PageUtils.Combine(paths))));
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
    }
}
