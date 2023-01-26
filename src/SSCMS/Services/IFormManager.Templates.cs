using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IFormManager
    {
        Task<string> GetTemplateDirectoryPathAsync(Site site, bool isSystem, string name);
        
        Task<string> GetTemplateHtmlAsync(Site site, bool isSystem, string name);

        Task SetTemplateHtmlAsync(Site site, string name, string html);

        Task DeleteTemplateAsync(Site site, string name);

        Task<List<FormTemplate>> GetFormTemplatesAsync(Site site);

        Task<FormTemplate> GetFormTemplateAsync(Site site, string name);

        Task CloneAsync(Site site, bool isSystemOriginal, string nameOriginal, string name);

        Task CloneAsync(Site site, bool isSystemOriginal, string nameOriginal, string name, string templateHtml);
    }
}
