
namespace SS.CMS.Abstractions
{
    public partial interface IPathManager : IService
    {
        string ApplicationPath { get; }

        string GetRootUrl(string relatedUrl);

        string GetApiUrl(string route);

        string GetAdminUrl(string relatedUrl);

        string GetUploadFileName(string fileName);
    }
}
