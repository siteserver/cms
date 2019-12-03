
namespace SiteServer.Abstractions
{
    public partial interface IPathManager
    {
        string GetSpecialDirectoryPath(Site siteInfo, string url);

        string GetSpecialZipFilePath(string directoryPath);

        string GetSpecialSrcDirectoryPath(string directoryPath);
    }
}