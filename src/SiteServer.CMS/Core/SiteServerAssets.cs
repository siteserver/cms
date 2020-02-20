using SiteServer.Abstractions;

namespace SiteServer.CMS.Core
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
            return PathUtils.Combine(PathUtility.PhysicalSiteServerPath, DirectoryName, PathUtils.Combine(paths));
        }

        public static string GetIconUrl(string iconName)
        {
            return GetUrl(PageUtils.Combine(DirectoryIcons, iconName));
        }
    }
}
