using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Framework
{
	public static class SiteServerAssets
	{
        private const string DirectoryName = "assets";
        private const string DirectoryIcons = "icons";

        private static string GetUrl(string relatedUrl)
        {
            return PageUtils.Combine(PageUtils.GetAdminUrl(DirectoryName), relatedUrl);
        }

        public static string GetPath(params string[] paths)
        {
            return PathUtils.Combine(GlobalSettings.ContentRootPath, Constants.AdminRootDirectory, DirectoryName, PathUtils.Combine(paths));
        }

        public static string GetIconUrl(string iconName)
        {
            return GetUrl(PageUtils.Combine(DirectoryIcons, iconName));
        }
    }
}
