using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Templates
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

        private static async Task<ActionResult<GetResult>> GetResultAsync(Site site)
        {
            var channels = new List<Channel>();
            var children = await DataProvider.ChannelRepository.GetCascadeChildrenAsync(site, site.Id,
                async summary =>
                {
                    var entity = await DataProvider.ChannelRepository.GetAsync(summary.Id);
                    channels.Add(entity);
                    return new
                    {
                        entity.ChannelTemplateId,
                        entity.ContentTemplateId
                    };
                });

            var summaries = await DataProvider.TemplateRepository.GetSummariesAsync(site.Id);
            var templates = new List<Template>();
            foreach (var summary in summaries)
            {
                var original = await DataProvider.TemplateRepository.GetAsync(summary.Id);
                var template = original.Clone<Template>();

                template.Set("useCount", DataProvider.ChannelRepository.GetTemplateUseCount(site.Id, template.Id, template.TemplateType, template.Default, channels));

                if (template.TemplateType == TemplateType.IndexPageTemplate)
                {
                    template.Set("url", PageUtility.ParseNavigationUrlAsync(site, template.CreatedFileFullName, false));
                    templates.Add(template);
                }
                else if (template.TemplateType == TemplateType.ChannelTemplate)
                {
                    template.Set("channelIds", DataProvider.ChannelRepository.GetChannelIdListByTemplateId(true, template.Id, channels));
                    templates.Add(template);
                }
                else if (template.TemplateType == TemplateType.ContentTemplate)
                {
                    template.Set("channelIds", DataProvider.ChannelRepository.GetChannelIdListByTemplateId(false, template.Id, channels));
                    templates.Add(template);
                }
                else if (template.TemplateType == TemplateType.FileTemplate)
                {
                    template.Set("url", PageUtility.ParseNavigationUrlAsync(site, template.CreatedFileFullName, false));
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
