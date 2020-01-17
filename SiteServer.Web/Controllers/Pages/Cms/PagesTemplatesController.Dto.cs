using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.Abstractions;
using SiteServer.API.Result;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
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
            var children = await ChannelManager.GetCascadeChildrenAsync(site, site.Id, func: async (siteInfo, channelInfo) =>
            {
                var dict = new Dictionary<string, object>
                {
                    ["channelTemplateId"] = channelInfo.ChannelTemplateId,
                    ["contentTemplateId"] = channelInfo.ContentTemplateId
                };
                return dict;
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
                    template.Set("channelIds", await ChannelManager.GetChannelIdListByTemplateIdAsync(site.Id, template.Id));
                    templates.Add(template);
                }
            }
            foreach (var template in allTemplates)
            {
                if (template.TemplateType == TemplateType.ContentTemplate)
                {
                    template.Set("channelIds", await ChannelManager.GetChannelIdListByTemplateIdAsync(site.Id, template.Id));
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
