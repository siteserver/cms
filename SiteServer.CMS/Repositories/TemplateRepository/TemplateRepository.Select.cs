using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;

namespace SiteServer.CMS.Repositories
{
    public partial class TemplateRepository
    {
        private string GetListKey(int siteId, TemplateType templateType)
        {
            return Caching.GetListKey(_repository.TableName, templateType.GetValue(), siteId.ToString());
        }

        private string GetEntityKey(int templateId)
        {
            return Caching.GetEntityKey(_repository.TableName, templateId);
        }

        public async Task<List<TemplateSummary>> GetSummariesAsync(int siteId, TemplateType templateType)
        {
            return await _repository.GetAllAsync<TemplateSummary>(Q
                .Select(nameof(Template.Id), nameof(Template.TemplateName), nameof(Template.TemplateType), nameof(Template.Default))
                .Where(nameof(Template.SiteId), siteId)
                .Where(nameof(Template.TemplateType), templateType.GetValue())
                .OrderBy(nameof(Template.TemplateType), nameof(Template.RelatedFileName))
                .CachingGet(GetListKey(siteId, templateType))
            );
        }

        public async Task<Template> GetAsync(int templateId)
        {
            return await _repository.GetAsync(templateId, Q
                .CachingGet(GetEntityKey(templateId))
            );
        }
    }
}
