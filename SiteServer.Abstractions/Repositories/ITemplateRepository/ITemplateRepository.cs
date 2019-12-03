using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public partial interface ITemplateRepository : IRepository
    {
        Task<int> InsertAsync(Template templateInfo, string templateContent, int userId);

        Task UpdateAsync(Site siteInfo, Template templateInfo, string templateContent, int userId);

        Task SetDefaultAsync(int siteId, int templateId);

        Task DeleteAsync(int siteId, int templateId);

        Task<string> GetImportTemplateNameAsync(int siteId, string templateName);

        Task<Dictionary<TemplateType, int>> GetCountDictionaryAsync(int siteId);

        Task<IEnumerable<Template>> GetTemplateInfoListByTypeAsync(int siteId, TemplateType type);

        Task<IEnumerable<Template>> GetTemplateInfoListOfFileAsync(int siteId);

        Task<IEnumerable<Template>> GetTemplateInfoListBySiteIdAsync(int siteId);

        Task<IEnumerable<string>> GetTemplateNameListAsync(int siteId, TemplateType templateType);

        Task<IEnumerable<string>> GetLowerRelatedFileNameListAsync(int siteId, TemplateType templateType);

        Task CreateDefaultTemplateInfoAsync(int siteId, int userId);

        Task<string> GetCreatedFileFullNameAsync(int templateId);

        Task<string> GetTemplateNameAsync(int templateId);

        Task<Template> GetTemplateInfoByTemplateNameAsync(int siteId, TemplateType templateType, string templateName);

        Task<Template> GetDefaultTemplateInfoAsync(int siteId, TemplateType templateType);

        Task<int> GetDefaultTemplateIdAsync(int siteId, TemplateType templateType);

        Task<int> GetTemplateIdByTemplateNameAsync(int siteId, TemplateType templateType, string templateName);

        string GetTemplateFilePath(Site siteInfo, Template templateInfo);

        Task<Template> GetIndexPageTemplateInfoAsync(int siteId);

        Task<Template> GetChannelTemplateInfoAsync(int siteId, int channelId);

        Task<Template> GetContentTemplateInfoAsync(int siteId, int channelId);

        Task<Template> GetFileTemplateInfoAsync(int siteId, int fileTemplateId);

        Task WriteContentToTemplateFileAsync(Site siteInfo, Template templateInfo, string content, int userId);

        Task<int> GetIndexTemplateIdAsync(int siteId);

        Task<int> GetChannelTemplateIdAsync(int siteId, int channelId);

        Task<int> GetContentTemplateIdAsync(int siteId, int channelId);

        Task<string> GetTemplateContentAsync(Site siteInfo, Template templateInfo);

        Task<string> GetContentByFilePathAsync(string filePath);

        Task<IEnumerable<int>> GetAllFileTemplateIdListAsync(int siteId);
    }
}
