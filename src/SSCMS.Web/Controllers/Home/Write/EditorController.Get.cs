using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class EditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();

            if (siteIds.Count == 0)
            {
                return new GetResult
                {
                    Unauthorized = true
                };
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var channel = await _channelRepository.GetAsync(request.ChannelId);

            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(site.Id);

            var allStyles = await _tableStyleRepository.GetContentStylesAsync(site, channel);
            var styles = allStyles
                .Where(style =>
                    !string.IsNullOrEmpty(style.DisplayName) &&
                    !ListUtils.ContainsIgnoreCase(ColumnsManager.MetadataAttributes.Value, style.AttributeName))
                .Select(x => new InputStyle(x));

            var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, channel.Id);
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

            var relatedFields = new Dictionary<int, List<Cascade<int>>>();

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
                else if (style.InputType == InputType.Text || style.InputType == InputType.TextArea || style.InputType == InputType.TextEditor)
                {
                    if (request.ContentId == 0)
                    {
                        content.Set(style.AttributeName, string.Empty);
                    }
                }
                else if (style.InputType == InputType.SelectCascading)
                {
                    if (style.RelatedFieldId > 0)
                    {
                        var items = await _relatedFieldItemRepository.GetCascadesAsync(request.SiteId, style.RelatedFieldId, 0);
                        relatedFields[style.RelatedFieldId] = items;
                    }
                }
            }

            //await ContentUtility.TextEditorContentDecodeAsync(parseManager.PathManager, pageInfo.Site, content.Get<string>(ContentAttribute.Content), pageInfo.IsLocal);

            var siteUrl = await _pathManager.GetSiteUrlAsync(site, false);

            var settings = new Settings
            {
                IsCloudImages = await _cloudManager.IsImagesAsync(),
            };

            return new GetResult
            {
                Unauthorized = false,
                Content = content,
                Site = site,
                Channel = channel,
                GroupNames = groupNames,
                TagNames = tagNames,
                Styles = styles,
                RelatedFields = relatedFields,
                CheckedLevels = checkedLevels,
                SiteUrl = siteUrl,
                Settings = settings
            };
        }
    }
}
