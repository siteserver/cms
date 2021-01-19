using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId,
                    MenuUtils.ContentPermissions.Add, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(site.Id);

            var allStyles = await _tableStyleRepository.GetContentStylesAsync(site, channel);
            var styles = allStyles
                .Where(style =>
                    !string.IsNullOrEmpty(style.DisplayName) &&
                    !ListUtils.ContainsIgnoreCase(ColumnsManager.MetadataAttributes.Value, style.AttributeName))
                .Select(x => new InputStyle(x)).ToList();
            var templates =
                await _templateRepository.GetTemplatesByTypeAsync(request.SiteId, TemplateType.ContentTemplate);

            var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, request.ChannelId);
            var checkedLevels = CheckManager.GetCheckedLevelOptions(site, userIsChecked, userCheckedLevel, true);

            Content content;
            if (request.ContentId > 0)
            {
                content = await _pathManager.DecodeContentAsync(site, channel, request.ContentId);
            }
            else
            {
                content = new Content
                {
                    Id = 0,
                    SiteId = site.Id,
                    ChannelId = channel.Id,
                    AddDate = DateTime.Now,
                    CheckedLevel = site.CheckContentDefaultLevel
                };
            }

            foreach (var style in styles)
            {
                if (style.InputType == InputType.CheckBox || style.InputType == InputType.SelectMultiple)
                {
                    if (request.ContentId == 0)
                    {
                        var value = style.Items != null
                            ? style.Items.Where(x => x.Selected).Select(x => x.Value).ToList()
                            : new List<string>();
                        content.Set(style.AttributeName, value);
                    }
                    else
                    {
                        var value = content.Get(style.AttributeName);
                        content.Set(style.AttributeName, ListUtils.ToList(value));
                    }
                }
                else if (style.InputType == InputType.Radio || style.InputType == InputType.SelectOne)
                {
                    if (request.ContentId == 0)
                    {
                        var item = style.Items?.FirstOrDefault(x => x.Selected);
                        var value = item != null ? item.Value : string.Empty;
                        content.Set(style.AttributeName, value);
                    }
                    else
                    {
                        var value = content.Get(style.AttributeName);
                        content.Set(style.AttributeName, StringUtils.ToString(value));
                    }
                }
            }

            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);
            var isCensorTextEnabled = await _censorManager.IsTextEnabledAsync();

            return new GetResult
            {
                Content = content,
                Site = site,
                SiteUrl = StringUtils.TrimEndSlash(siteUrl),
                Channel = channel,
                GroupNames = groupNames,
                TagNames = tagNames,
                Styles = styles,
                Templates = templates,
                CheckedLevels = checkedLevels,
                CheckedLevel = userCheckedLevel,
                IsCensorTextEnabled = isCensorTextEnabled
            };
        }
    }
}
