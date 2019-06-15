using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IPathManager
    {
        string GetSpecialDirectoryPath(SiteInfo siteInfo, string url);

        string GetSpecialZipFilePath(string directoryPath);

        string GetSpecialSrcDirectoryPath(string directoryPath);
    }
}