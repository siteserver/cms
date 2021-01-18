using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesEditorController
	{
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] TemplateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            Template template;
            if (request.TemplateId > 0)
            {
                template = await _templateRepository.GetAsync(request.TemplateId);
            }
            else
            {
                template = new Template
                {
                    TemplateType = request.TemplateType
                };
            }

            var settings = await GetSettingsAsync(template, site);
            var content = Constants.Html5Empty;
            if (template.Id > 0)
            {
                content = await _pathManager.GetTemplateContentAsync(site, template);
            }
            var channel = await _channelRepository.GetAsync(request.SiteId);
            var channels = new List<Channel>();
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var entity = await _channelRepository.GetAsync(summary.Id);
                channels.Add(entity);
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count
                };
            });

            var channelIds = new List<int>
            {
                request.SiteId
            };
            var channelId = 0;
            var contentId = 0;
            var contents = new List<KeyValuePair<int, string>>();
            if (template.Id > 0 && (template.TemplateType == TemplateType.ChannelTemplate ||
                                    template.TemplateType == TemplateType.ContentTemplate))
            {
                var templateChannelIds = _channelRepository.GetChannelIdsByTemplateId(
                    template.TemplateType == TemplateType.ChannelTemplate,
                    template.Id, channels);
                if (templateChannelIds.Count > 0)
                {
                    channelId = templateChannelIds.FirstOrDefault(x => x != request.SiteId);
                    if (channelId > 0)
                    {
                        var firstChannel = await _channelRepository.GetAsync(channelId);
                        channelIds = ListUtils.GetIntList(firstChannel.ParentsPath);
                        channelIds.Add(channelId);

                        if (template.TemplateType == TemplateType.ContentTemplate)
                        {
                            var contentIds = await _contentRepository.GetContentIdsCheckedAsync(site, firstChannel);
                            foreach (var id in contentIds.Take(30))
                            {
                                if (contentId == 0)
                                {
                                    contentId = id;
                                }
                                var entity = await _contentRepository.GetAsync(site, channel, id);
                                contents.Add(new KeyValuePair<int, string>(entity.Id, entity.Title));
                            }
                        }
                    }
                }
            }

            return new GetResult
            {
                Settings = settings,
                Content = content,
                Channels = cascade,
                ChannelIds = channelIds,
                ChannelId = channelId,
                Contents = contents,
                ContentId = contentId
            };
        }
    }
}
