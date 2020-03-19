using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesController
    {
        public class GetResult
        {
            public List<Cascade<int>> Channels { get; set; }
            public IEnumerable<Template> Templates { get; set; }
        }

        public class TemplateRequest
        {
            public int SiteId { get; set; }
            public int TemplateId { get; set; }
        }

        private async Task<ActionResult<GetResult>> GetResultAsync(Site site)
        {
            var channels = new List<Channel>();
            var children = await _channelRepository.GetCascadeChildrenAsync(site, site.Id,
                async summary =>
                {
                    var entity = await _channelRepository.GetAsync(summary.Id);
                    channels.Add(entity);
                    return new
                    {
                        entity.ChannelTemplateId,
                        entity.ContentTemplateId
                    };
                });

            var summaries = await _templateRepository.GetSummariesAsync(site.Id);
            var templates = new List<Template>();
            foreach (var summary in summaries)
            {
                var original = await _templateRepository.GetAsync(summary.Id);
                var template = original.Clone<Template>();

                template.Set("useCount", _channelRepository.GetTemplateUseCount(site.Id, template.Id, template.TemplateType, template.DefaultTemplate, channels));

                if (template.TemplateType == TemplateType.IndexPageTemplate)
                {
                    template.Set("url", _pathManager.ParseNavigationUrlAsync(site, template.CreatedFileFullName, false));
                    templates.Add(template);
                }
                else if (template.TemplateType == TemplateType.ChannelTemplate)
                {
                    template.Set("channelIds", _channelRepository.GetChannelIdListByTemplateId(true, template.Id, channels));
                    templates.Add(template);
                }
                else if (template.TemplateType == TemplateType.ContentTemplate)
                {
                    template.Set("channelIds", _channelRepository.GetChannelIdListByTemplateId(false, template.Id, channels));
                    templates.Add(template);
                }
                else if (template.TemplateType == TemplateType.FileTemplate)
                {
                    template.Set("url", _pathManager.ParseNavigationUrlAsync(site, template.CreatedFileFullName, false));
                    templates.Add(template);
                }
            }

            return new GetResult
            {
                Channels = children,
                Templates = templates
            };
        }
    }
}
