using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface ISpecialRepository
    {
        Task<Special> DeleteSpecialAsync(Site site, int specialId);

        Task<Special> GetSpecialAsync(int siteId, int specialId);

        Task<string> GetTitleAsync(int siteId, int specialId);

        Task<List<Template>> GetTemplateListAsync(Site site, int specialId);

        Task<List<int>> GetAllSpecialIdListAsync(int siteId);

        Task<string> GetSpecialDirectoryPathAsync(Site site, string url);

        Task<string> GetSpecialUrlAsync(Site site, int specialId);

        string GetSpecialZipFilePath(string title, string directoryPath);

        Task<string> GetSpecialZipFileUrlAsync(Site site, Special special);

        string GetSpecialSrcDirectoryPath(string directoryPath);
    }
}