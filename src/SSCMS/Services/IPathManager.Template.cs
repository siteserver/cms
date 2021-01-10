using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IPathManager
    {
        Task<string> GetTemplateFilePathAsync(Site site, Template template);

        Task<string> GetTemplateContentAsync(Site site, Template template);

        Task WriteContentToTemplateFileAsync(Site site, Template template, string content, int adminId);

        Task<string> GetIncludeContentAsync(Site site, string file);

        Task WriteContentToIncludeFileAsync(Site site, string file, string content);

        string GetContentByFilePath(string filePath);
    }
}
