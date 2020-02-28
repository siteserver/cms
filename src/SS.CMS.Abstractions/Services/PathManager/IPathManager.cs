
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPathManager : IService
    {
        string ApplicationPath { get; }

        string GetWebUrl(params string[] paths);

        string GetWebPath(params string[] paths);

        string GetApiUrl(string route);

        string GetAdminPath(params string[] paths);

        string GetAdminUrl(params string[] paths);

        string GetUploadFileName(string fileName);

        Task<string> GetWebUrlAsync(Site site);

        Task<string> GetAssetsUrlAsync(Site site);

        string GetApiUrl(Config config);
    }
}
