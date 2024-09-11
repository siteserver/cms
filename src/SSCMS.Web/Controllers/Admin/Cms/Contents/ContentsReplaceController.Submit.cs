using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;
using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsReplaceController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ContentsReplace))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var channelIds = new List<int>();
            foreach (var channelId in request.ChannelIds)
            {
                if (!channelIds.Contains(channelId))
                {
                    channelIds.Add(channelId);
                }
                if (request.IsDescendant)
                {
                    var descendantChannelIds = await _channelRepository.GetChannelIdsAsync(request.SiteId, channelId, Enums.ScopeType.Descendant);
                    foreach (var descendantChannelId in descendantChannelIds)
                    {
                        if (!channelIds.Contains(descendantChannelId))
                        {
                            channelIds.Add(descendantChannelId);
                        }
                    }
                }
            }

            foreach (var channelId in channelIds)
            {
                var channel = await _channelRepository.GetAsync(channelId);
                var contentIdList = await _contentRepository.GetContentIdsAsync(site, channel);

                foreach (var contentId in contentIdList)
                {
                    var content = await _contentRepository.GetAsync(site, channel, contentId);
                    var isReplaced = false;

                    foreach (var attributeName in request.AttributeNames)
                    {
                        if (attributeName == nameof(Models.Content.TagNames))
                        {
                            var originalTagNames = content.TagNames;
                            if (originalTagNames != null && originalTagNames.Count > 0)
                            {
                                var tagNames = new List<string>();
                                foreach (var tagName in originalTagNames)
                                {
                                    string value;
                                    if (request.IsRegex)
                                    {
                                        value = RegexUtils.Replace(request.Replace, tagName, request.To);
                                    }
                                    else
                                    {
                                        value = tagName.Replace(request.Replace, request.To, !request.IsCaseSensitive, null);
                                    }
                                    if (tagName != value)
                                    {
                                        isReplaced = true;
                                    }
                                    tagNames.Add(value);
                                }
                                content.TagNames = tagNames;
                            }
                        }
                        else
                        {
                            var originalValue = content.Get<string>(attributeName);
                            if (string.IsNullOrEmpty(originalValue))
                            {
                                originalValue = string.Empty;
                            }
                            string value;
                            if (request.IsRegex)
                            {
                                value = RegexUtils.Replace(request.Replace, originalValue, request.To);
                            }
                            else
                            {
                                value = originalValue.Replace(request.Replace, request.To, !request.IsCaseSensitive, null);
                            }
                            if (originalValue != value)
                            {
                                isReplaced = true;
                                content.Set(attributeName, value);
                            }
                        }
                    }
                    if (isReplaced)
                    {
                        await _contentRepository.UpdateAsync(site, channel, content);
                    }
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
