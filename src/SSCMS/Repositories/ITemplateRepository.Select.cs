using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Repositories
{
    public partial interface ITemplateRepository
    {
        Task<List<TemplateSummary>> GetSummariesAsync(int siteId);

        Task<Template> GetAsync(int templateId);
    }
}