using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Channels
{
    public partial class ChannelsController
    {
        [HttpGet, Route(RouteGet)]
        public async Task<ActionResult<ChannelResult>> Get(int siteId, int channelId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId,
                    Types.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(channelId);

            var linkTypes = _pathManager.GetLinkTypeSelects();
            var taxisTypes = new List<Select<string>>
            {
                new Select<string>(TaxisType.OrderByTaxisDesc),
                new Select<string>(TaxisType.OrderByTaxis),
                new Select<string>(TaxisType.OrderByAddDateDesc),
                new Select<string>(TaxisType.OrderByAddDate)
            };

            var styles = await GetInputStylesAsync(channel);
            var entity = new Entity(channel.ToDictionary());
            foreach (var style in styles)
            {
                if (style.InputType == InputType.Image ||
                    style.InputType == InputType.Video ||
                    style.InputType == InputType.File)
                {
                    var count = channel.Get(ColumnsManager.GetCountName(style.AttributeName), 0);
                    entity.Set(ColumnsManager.GetCountName(style.AttributeName), count);
                    for (var n = 0; n <= count; n++)
                    {
                        var extendName = ColumnsManager.GetExtendName(style.AttributeName, n);
                        entity.Set(extendName, channel.Get(extendName));
                    }
                }
                else if (style.InputType == InputType.CheckBox ||
                         style.InputType == InputType.SelectMultiple)
                {
                    var list = ListUtils.GetStringList(channel.Get(style.AttributeName,
                        string.Empty));
                    entity.Set(style.AttributeName, list);
                }
                else
                {
                    entity.Set(style.AttributeName, channel.Get(style.AttributeName));
                }
            }

            var isTemplateEditable =
                await _authManager.HasSitePermissionsAsync(siteId, Types.SitePermissions.Templates);

            return new ChannelResult
            {
                Entity = entity,
                LinkTypes = linkTypes,
                TaxisTypes = taxisTypes,
                Styles = styles,
                IsTemplateEditable = isTemplateEditable
            };
        }
    }
}