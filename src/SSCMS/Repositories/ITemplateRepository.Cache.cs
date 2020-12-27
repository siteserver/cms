using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface ITemplateRepository
    {
        Task<List<Template>> GetTemplatesByTypeAsync(int siteId, TemplateType templateType);

        Task<Template> GetTemplateByTemplateNameAsync(int siteId, TemplateType templateType, string templateName);

        Task<bool> ExistsAsync(int siteId, TemplateType templateType, string templateName);

        Task<Template> GetDefaultTemplateAsync(int siteId, TemplateType templateType);

        Task<int> GetDefaultTemplateIdAsync(int siteId, TemplateType templateType);

        Task<int> GetTemplateIdByTemplateNameAsync(int siteId, TemplateType templateType, string templateName);

        Task<List<string>> GetTemplateNamesAsync(int siteId, TemplateType templateType);

        Task<List<string>> GetRelatedFileNamesAsync(int siteId, TemplateType templateType);

        Task<List<int>> GetAllFileTemplateIdsAsync(int siteId);

        Task<string> GetCreatedFileFullNameAsync(int templateId);

        Task<string> GetTemplateNameAsync(int templateId);

        Task<Template> GetIndexPageTemplateAsync(int siteId);

        Task<Template> GetChannelTemplateAsync(int siteId, Channel channel);

        Task<Template> GetContentTemplateAsync(int siteId, Channel channel, int contentTemplateId);

        Task<Template> GetFileTemplateAsync(int siteId, int fileTemplateId);

        Task<int> GetIndexTemplateIdAsync(int siteId);

        Task<string> GetImportTemplateNameAsync(int siteId, TemplateType templateType, string templateName);
    }
}