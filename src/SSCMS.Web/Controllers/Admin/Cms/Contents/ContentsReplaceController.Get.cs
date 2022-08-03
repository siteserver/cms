using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Configuration;
using SSCMS.Utils;
using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsReplaceController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ContentsReplace))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                return new
                {
                    Count = count,
                    summary.IndexName
                };
            });

            var attributes = new List<Option<string>>();
            var styles = await _tableStyleRepository.GetContentStylesAsync(site, channel);
            var selectedAttributes = new List<string>();

            foreach (var style in styles)
            {
                if (string.IsNullOrEmpty(style.DisplayName)
                    || StringUtils.EqualsIgnoreCase(style.AttributeName, nameof(SSCMS.Models.Content.Id))
                    || StringUtils.EqualsIgnoreCase(style.AttributeName, nameof(SSCMS.Models.Content.LastModifiedDate))
                    || StringUtils.EqualsIgnoreCase(style.AttributeName, nameof(SSCMS.Models.Content.AdminId))
                    || StringUtils.EqualsIgnoreCase(style.AttributeName, nameof(SSCMS.Models.Content.UserId))
                    || StringUtils.EqualsIgnoreCase(style.AttributeName, nameof(SSCMS.Models.Content.SourceId))
                    || StringUtils.EqualsIgnoreCase(style.AttributeName, nameof(SSCMS.Models.Content.Hits))
                    || StringUtils.EqualsIgnoreCase(style.AttributeName, "CheckUserName")
                    || StringUtils.EqualsIgnoreCase(style.AttributeName, "CheckDate")
                    || StringUtils.EqualsIgnoreCase(style.AttributeName, "CheckReasons")) continue;

                var listItem = new Option<string>
                {
                    Value = style.AttributeName,
                    Label = style.DisplayName
                };
                if (ListUtils.ContainsIgnoreCase(selectedAttributes, style.AttributeName))
                {
                    listItem.Selected = true;
                }
                attributes.Add(listItem);
            }

            return new GetResult
            {
                Channels = cascade,
                Attributes = attributes
            };
        }
    }
}
