using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface ITemplateRepository : IRepository
    {
        Task<int> InsertAsync(TemplateInfo templateInfo, string templateContent, int userId);

        Task UpdateAsync(SiteInfo siteInfo, TemplateInfo templateInfo, string templateContent, int userId);

        Task SetDefaultAsync(int siteId, int templateId);

        Task DeleteAsync(int siteId, int templateId);

        Task<string> GetImportTemplateNameAsync(int siteId, string templateName);

        Task<Dictionary<TemplateType, int>> GetCountDictionaryAsync(int siteId);

        Task<IEnumerable<TemplateInfo>> GetTemplateInfoListByTypeAsync(int siteId, TemplateType type);

        Task<IEnumerable<TemplateInfo>> GetTemplateInfoListOfFileAsync(int siteId);

        Task<IEnumerable<TemplateInfo>> GetTemplateInfoListBySiteIdAsync(int siteId);

        Task<IEnumerable<string>> GetTemplateNameListAsync(int siteId, TemplateType templateType);

        Task<IEnumerable<string>> GetLowerRelatedFileNameListAsync(int siteId, TemplateType templateType);

        Task CreateDefaultTemplateInfoAsync(int siteId, int userId);

        Task<string> GetCreatedFileFullNameAsync(int templateId);

        Task<string> GetTemplateNameAsync(int templateId);

        Task<TemplateInfo> GetTemplateInfoByTemplateNameAsync(int siteId, TemplateType templateType, string templateName);

        Task<TemplateInfo> GetDefaultTemplateInfoAsync(int siteId, TemplateType templateType);

        Task<int> GetDefaultTemplateIdAsync(int siteId, TemplateType templateType);

        Task<int> GetTemplateIdByTemplateNameAsync(int siteId, TemplateType templateType, string templateName);

        string GetTemplateFilePath(SiteInfo siteInfo, TemplateInfo templateInfo);

        Task<TemplateInfo> GetIndexPageTemplateInfoAsync(int siteId);

        Task<TemplateInfo> GetChannelTemplateInfoAsync(int siteId, int channelId);

        Task<TemplateInfo> GetContentTemplateInfoAsync(int siteId, int channelId);

        Task<TemplateInfo> GetFileTemplateInfoAsync(int siteId, int fileTemplateId);

        Task WriteContentToTemplateFileAsync(SiteInfo siteInfo, TemplateInfo templateInfo, string content, int userId);

        Task<int> GetIndexTemplateIdAsync(int siteId);

        Task<int> GetChannelTemplateIdAsync(int siteId, int channelId);

        Task<int> GetContentTemplateIdAsync(int siteId, int channelId);

        Task<string> GetTemplateContentAsync(SiteInfo siteInfo, TemplateInfo templateInfo);

        Task<string> GetContentByFilePathAsync(string filePath);

        Task<IEnumerable<int>> GetAllFileTemplateIdListAsync(int siteId);
    }
}
