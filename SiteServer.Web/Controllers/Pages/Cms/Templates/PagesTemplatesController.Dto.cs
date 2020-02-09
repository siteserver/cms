using System.Collections.Generic;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.Crmf;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Templates
{
    public partial class PagesTemplatesController
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

        private static async Task<GetResult> GetResultAsync(Site site)
        {
            var children = await DataProvider.ChannelRepository.GetCascadeChildrenAsync(site, site.Id,
                async summary =>
                {
                    var entity = await DataProvider.ChannelRepository.GetAsync(summary.Id);
                    return new
                    {
                        entity.ChannelTemplateId,
                        entity.ContentTemplateId
                    };
                });

            var allTemplates = await DataProvider.TemplateRepository.GetAllAsync(site.Id);
            var templates = new List<Template>();
            foreach (var template in allTemplates)
            {
                template.Set("useCount", DataProvider.ChannelRepository.GetTemplateUseCount(site.Id, template.Id, template.TemplateType, template.Default));
                if (template.TemplateType == TemplateType.IndexPageTemplate)
                {
                    template.Set("url", PageUtility.ParseNavigationUrl(site, template.CreatedFileFullName, false));
                    templates.Add(template);
                }
            }
            foreach (var template in allTemplates)
            {
                if (template.TemplateType == TemplateType.ChannelTemplate)
                {
                    template.Set("channelIds", await DataProvider.ChannelRepository.GetChannelIdListByTemplateIdAsync(site.Id, true, template.Id));
                    templates.Add(template);
                }
            }
            foreach (var template in allTemplates)
            {
                if (template.TemplateType == TemplateType.ContentTemplate)
                {
                    template.Set("channelIds", await DataProvider.ChannelRepository.GetChannelIdListByTemplateIdAsync(site.Id, false, template.Id));
                    templates.Add(template);
                }
            }
            foreach (var template in allTemplates)
            {
                if (template.TemplateType == TemplateType.FileTemplate)
                {
                    template.Set("url", PageUtility.ParseNavigationUrl(site, template.CreatedFileFullName, false));
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
