using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SS.CMS.Abstractions
{
    public partial interface IPathManager : IService
    {
        string ContentRootPath { get; }
        string WebRootPath { get; }

        string WebUrl { get; }

        string GetWebUrl(params string[] paths);

        string GetWebPath(params string[] paths);

        string GetAdminUrl(params string[] paths);

        string GetUploadFileName(string fileName);

        Task<string> GetWebUrlAsync(Site site);

        Task<string> GetAssetsUrlAsync(Site site);

        string MapPath(string virtualPath);

        Task UploadAsync(IFormFile file, string filePath);
    }
}
