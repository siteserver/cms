using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IPathManager
    {
        string ContentRootPath { get; }

        string WebRootPath { get; }

        string GetContentRootPath(params string[] paths);

        string GetAdminUrl(params string[] paths);

        string GetHomeUrl(params string[] paths);

        string GetApiHostUrl(Site site, params string[] paths);

        string GetUploadFileName(string fileName);

        Task<string> GetWebUrlAsync(Site site);

        Task<string> GetAssetsUrlAsync(Site site);

        string ParsePath(string virtualPath);

        Task UploadAsync(IFormFile file, string filePath);

        Task UploadAsync(byte[] bytes, string filePath);
    }
}
