using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SS.CMS.Abstractions
{
    public partial interface IPathManager
    {
        string ContentRootPath { get; }
        string WebRootPath { get; }

        string WebUrl { get; }

        string GetContentRootPath(params string[] paths);

        string GetAdminUrl(params string[] paths);

        string GetUploadFileName(string fileName);

        Task<string> GetWebUrlAsync(Site site);

        Task<string> GetAssetsUrlAsync(Site site);

        string MapPath(string virtualPath);

        Task UploadAsync(IFormFile file, string filePath);
    }
}
