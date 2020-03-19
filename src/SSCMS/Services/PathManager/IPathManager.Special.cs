using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IPathManager
    {
        Task<string> GetSpecialUrlAsync(Site site, int specialId, bool isLocal);

        Task<string> GetSpecialDirectoryPathAsync(Site site, string url);

        Task<string> GetSpecialUrlAsync(Site site, int specialId);

        string GetSpecialZipFilePath(string title, string directoryPath);

        Task<string> GetSpecialZipFileUrlAsync(Site site, Special special);

        string GetSpecialSrcDirectoryPath(string directoryPath);

        Task<List<Template>> GetSpecialTemplateListAsync(Site site, int specialId);

        Task<Special> DeleteSpecialAsync(Site site, int specialId);
    }
}
