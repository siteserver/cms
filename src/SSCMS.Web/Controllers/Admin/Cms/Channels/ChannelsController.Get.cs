using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
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
        [HttpGet, Route(RouteGet)]
        public async Task<ActionResult<GetResult>> Get(int siteId, int channelId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId,
                    MenuUtils.SitePermissions.Channels))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(siteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channel = await _channelRepository.GetAsync(channelId);
            var templates = await _templateRepository.GetSummariesAsync(siteId);
            if (!templates.Exists(x => x.Id == channel.ChannelTemplateId))
            {
                var templateId = await _templateRepository.GetDefaultTemplateIdAsync(siteId, TemplateType.ChannelTemplate);
                channel.ChannelTemplateId = templateId;
                await _channelRepository.UpdateAsync(channel);
            }
            if (!templates.Exists(x => x.Id == channel.ContentTemplateId))
            {
                var templateId = await _templateRepository.GetDefaultTemplateIdAsync(siteId, TemplateType.ContentTemplate);
                channel.ContentTemplateId = templateId;
                await _channelRepository.UpdateAsync(channel);
            }

            var styles = await GetStylesAsync(channel);
            var entity = new Entity(channel.ToDictionary());
            var relatedFields = new Dictionary<int, List<Cascade<int>>>();

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
                else if (style.InputType == InputType.CheckBox || style.InputType == InputType.SelectMultiple)
                {
                    var value = entity.Get(style.AttributeName);
                    entity.Set(style.AttributeName, ListUtils.ToList(value));
                }
                // else if (style.InputType == InputType.CheckBox ||
                //          style.InputType == InputType.SelectMultiple)
                // {
                //     var list = ListUtils.GetStringList(channel.Get(style.AttributeName,
                //         string.Empty));
                //     entity.Set(style.AttributeName, list);
                // }
                else if (style.InputType == InputType.TextEditor)
                {
                    var value = channel.Get(style.AttributeName, string.Empty);
                    value = await _pathManager.DecodeTextEditorAsync(site, value, true);
                    value = UEditorUtils.TranslateToHtml(value);

                    entity.Set(style.AttributeName, value);
                }
                else if (style.InputType == InputType.SelectCascading)
                {
                    if (style.RelatedFieldId > 0)
                    {
                        var items = await _relatedFieldItemRepository.GetCascadesAsync(siteId, style.RelatedFieldId, 0);
                        relatedFields[style.RelatedFieldId] = items;
                    }
                    entity.Set(style.AttributeName, channel.Get(style.AttributeName));
                }
                else
                {
                    entity.Set(style.AttributeName, channel.Get(style.AttributeName));
                }
            }

            var filePath = channel.FilePath;
            var channelFilePathRule = channel.ChannelFilePathRule;
            var contentFilePathRule = channel.ContentFilePathRule;

            var linkTo = new LinkTo
            {
                ChannelIds = new List<int> {
                  siteId,
                },
                ContentId = 0,
                ContentTitle = string.Empty
            };
            if (channel.LinkType == Enums.LinkType.LinkToChannel)
            {
                linkTo.ChannelIds = ListUtils.GetIntList(channel.LinkUrl);
            }
            else if (channel.LinkType == Enums.LinkType.LinkToContent)
            {
                if (!string.IsNullOrEmpty(channel.LinkUrl) && channel.LinkUrl.IndexOf('_') != -1)
                {
                    var arr = channel.LinkUrl.Split('_');
                    if (arr.Length == 2)
                    {
                        var channelIds = ListUtils.GetIntList(arr[0]);
                        var linkContentId = TranslateUtils.ToInt(arr[1]);
                        var linkChannelId = channelIds.Count > 0 ? channelIds[channelIds.Count - 1] : 0;
                        var linkToContent = await _contentRepository.GetAsync(site.Id, linkChannelId, linkContentId);
                        if (linkToContent != null)
                        {
                            linkTo.ChannelIds = channelIds;
                            linkTo.ContentId = linkContentId;
                            linkTo.ContentTitle = linkToContent.Title;
                        }
                    }
                }
            }

            return new GetResult
            {
                Entity = entity,
                Styles = styles,
                RelatedFields = relatedFields,
                FilePath = filePath,
                ChannelFilePathRule = channelFilePathRule,
                ContentFilePathRule = contentFilePathRule,
                LinkTo = linkTo,
            };
        }
    }
}