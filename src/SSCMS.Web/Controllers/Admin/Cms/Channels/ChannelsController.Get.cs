using System.Collections.Generic;
using System.Threading.Tasks;
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

            var styles = new List<InputStyle>();
            foreach (var style in await _tableStyleRepository.GetChannelStylesAsync( channel))
            {
                styles.Add(new InputStyle(style));

                if (style.InputType == InputType.Image ||
                    style.InputType == InputType.Video ||
                    style.InputType == InputType.File)
                {
                    channel.Set(ColumnsManager.GetCountName(style.AttributeName),
                        channel.Get(ColumnsManager.GetCountName(style.AttributeName), 0));
                }
                else if (style.InputType == InputType.CheckBox ||
                         style.InputType == InputType.SelectMultiple)
                {
                    var list = ListUtils.GetStringList(channel.Get(style.AttributeName,
                        string.Empty));
                    channel.Set(style.AttributeName, list);
                }
            }

            return new ChannelResult
            {
                Channel = channel,
                LinkTypes = linkTypes,
                TaxisTypes = taxisTypes,
                Styles = styles
            };
        }
    }
}