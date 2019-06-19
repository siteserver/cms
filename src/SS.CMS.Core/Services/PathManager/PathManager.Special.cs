using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class PathManager
    {
        public string GetSpecialDirectoryPath(SiteInfo siteInfo, string url)
        {
            var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
            return MapPath(siteInfo, virtualPath);
        }

        public string GetSpecialZipFilePath(string directoryPath)
        {
            return PathUtils.Combine(directoryPath, "_src.zip");
        }

        public string GetSpecialSrcDirectoryPath(string directoryPath)
        {
            return PathUtils.Combine(directoryPath, "_src");
        }
    }
}