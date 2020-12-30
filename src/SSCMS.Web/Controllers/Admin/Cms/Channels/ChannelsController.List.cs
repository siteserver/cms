using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ChannelsResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var max = 0;
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var groupNames = await _channelRepository.GetGroupNamesAsync(summary.Id);

                var channelPlugins = _pluginManager.GetPlugins(request.SiteId, summary.Id);
                var menus = new List<Menu>();
                foreach (var plugin in channelPlugins)
                {
                    var pluginMenus = plugin.GetMenus()
                        .Where(x => ListUtils.ContainsIgnoreCase(x.Type, Types.Resources.Channel)).ToList();
                    if (pluginMenus.Count == 0) continue;

                    menus.AddRange(pluginMenus);
                }

                max = Math.Max(max, menus.Count);

                var node = await _channelRepository.GetAsync(summary.Id);
                var imageUrl = string.Empty;
                if (!string.IsNullOrEmpty(node.ImageUrl))
                {
                    imageUrl = await _pathManager.ParseSiteUrlAsync(site, node.ImageUrl, true);
                }

                return new
                {
                    Channel = node,
                    Count = count,
                    ImageUrl = imageUrl,
                    GroupNames = groupNames,
                    Menus = menus
                };
            });

            var indexNames = await _channelRepository.GetChannelIndexNamesAsync(request.SiteId);
            var groupNameList = await _channelGroupRepository.GetGroupNamesAsync(request.SiteId);

            var channelTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ChannelTemplate);
            var contentTemplates = await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            var columnsManager = new ColumnsManager(_databaseManager, _pathManager);
            var columns = await columnsManager.GetChannelListColumnsAsync(site);

            var commandsWidth = 160 + 40 * max;
            var isTemplateEditable =
                await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.Templates);

            var linkTypes = _pathManager.GetLinkTypeSelects();
            var taxisTypes = new List<Select<string>>
            {
                new Select<string>(TaxisType.OrderByTaxisDesc),
                new Select<string>(TaxisType.OrderByTaxis),
                new Select<string>(TaxisType.OrderByAddDateDesc),
                new Select<string>(TaxisType.OrderByAddDate)
            };
            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);

            return new ChannelsResult
            {
                Channel = cascade,
                IndexNames = indexNames,
                GroupNames = groupNameList,
                ChannelTemplates = channelTemplates,
                ContentTemplates = contentTemplates,
                Columns = columns,
                CommandsWidth = commandsWidth,
                IsTemplateEditable = isTemplateEditable,
                LinkTypes = linkTypes,
                TaxisTypes = taxisTypes,
                SiteUrl = siteUrl
            };
        }
    }
}
