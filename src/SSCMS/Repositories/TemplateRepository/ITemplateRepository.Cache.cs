using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface ITemplateRepository
    {
        Task<List<Template>> GetTemplateListByTypeAsync(int siteId, TemplateType templateType);

        Task<Template> GetTemplateByTemplateNameAsync(int siteId, TemplateType templateType, string templateName);

        Task<bool> ExistsAsync(int siteId, TemplateType templateType, string templateName);

        Task<Template> GetDefaultTemplateAsync(int siteId, TemplateType templateType);

        Task<int> GetDefaultTemplateIdAsync(int siteId, TemplateType templateType);

        Task<int> GetTemplateIdByTemplateNameAsync(int siteId, TemplateType templateType, string templateName);

        Task<List<string>> GetTemplateNameListAsync(int siteId, TemplateType templateType);

        Task<List<string>> GetRelatedFileNameListAsync(int siteId, TemplateType templateType);

        Task<List<int>> GetAllFileTemplateIdListAsync(int siteId);

        Task<string> GetCreatedFileFullNameAsync(int templateId);

        Task<string> GetTemplateNameAsync(int templateId);

        Task<Template> GetIndexPageTemplateAsync(int siteId);

        Task<Template> GetChannelTemplateAsync(int siteId, Channel channel);

        Task<Template> GetContentTemplateAsync(int siteId, Channel channel);

        Task<Template> GetFileTemplateAsync(int siteId, int fileTemplateId);

        Task<int> GetIndexTemplateIdAsync(int siteId);

        Task<string> GetImportTemplateNameAsync(int siteId, TemplateType templateType, string templateName);
    }
}