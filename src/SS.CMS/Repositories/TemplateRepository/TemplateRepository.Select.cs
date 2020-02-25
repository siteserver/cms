using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using SS.CMS.Abstractions;
using SS.CMS;
using SS.CMS.Core;

namespace SS.CMS.Repositories
{
    public partial class TemplateRepository
    {
        private string GetListKey(int siteId)
        {
            return Caching.GetListKey(_repository.TableName, siteId);
        }

        private string GetEntityKey(int templateId)
        {
            return Caching.GetEntityKey(_repository.TableName, templateId);
        }

        public async Task<List<TemplateSummary>> GetSummariesAsync(int siteId)
        {
            return await _repository.GetAllAsync<TemplateSummary>(Q
                .Select(nameof(Template.Id), nameof(Template.TemplateName), nameof(Template.TemplateType), nameof(Template.Default))
                .Where(nameof(Template.SiteId), siteId)
                .OrderBy(nameof(Template.TemplateType), nameof(Template.RelatedFileName))
                .CachingGet(GetListKey(siteId))
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
