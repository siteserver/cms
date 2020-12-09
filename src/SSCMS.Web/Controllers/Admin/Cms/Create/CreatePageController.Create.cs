using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    public partial class CreatePageController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] CreateRequest request)
        {
            var permission = string.Empty;
            if (request.Type == CreateType.Index)
            {
                permission = MenuUtils.SitePermissions.CreateIndex;
            }
            else if (request.Type == CreateType.Channel)
            {
                permission = MenuUtils.SitePermissions.CreateChannels;
            }
            else if (request.Type == CreateType.Content)
            {
                permission = MenuUtils.SitePermissions.CreateContents;
            }
            else if (request.Type == CreateType.File)
            {
                permission = MenuUtils.SitePermissions.CreateFiles;
            }
            else if (request.Type == CreateType.Special)
            {
                permission = MenuUtils.SitePermissions.CreateSpecials;
            }
            else if (request.Type == CreateType.All)
            {
                permission = MenuUtils.SitePermissions.CreateAll;
            }

            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, permission))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var selectedChannelIdList = new List<int>();

            if (request.IsAllChecked)
            {
                selectedChannelIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId);
            }
            else if (request.IsDescendent)
            {
                foreach (var channelId in request.ChannelIdList)
                {
                    selectedChannelIdList.Add(channelId);

                    var channelInfo = await _channelRepository.GetAsync(channelId);
                    if (channelInfo.ChildrenCount > 0)
                    {
                        var descendentIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId, channelId, ScopeType.Descendant);
                        foreach (var descendentId in descendentIdList)
                        {
                            if (selectedChannelIdList.Contains(descendentId)) continue;

                            selectedChannelIdList.Add(descendentId);
                        }
                    }
                }
            }
            else
            {
                selectedChannelIdList.AddRange(request.ChannelIdList);
            }

            var channelIdList = new List<int>();

            if (request.Scope == "1month" || request.Scope == "1day" || request.Scope == "2hours")
            {
                if (request.Scope == "1month")
                {
                    var lastEditList = await _contentRepository.GetChannelIdsCheckedByLastModifiedDateHourAsync(site, 720);
                    foreach (var channelId in lastEditList)
                    {
                        if (selectedChannelIdList.Contains(channelId))
                        {
                            channelIdList.Add(channelId);
                        }
                    }
                }
                else if (request.Scope == "1day")
                {
                    var lastEditList = await _contentRepository.GetChannelIdsCheckedByLastModifiedDateHourAsync(site, 24);
                    foreach (var channelId in lastEditList)
                    {
                        if (selectedChannelIdList.Contains(channelId))
                        {
                            channelIdList.Add(channelId);
                        }
                    }
                }
                else if (request.Scope == "2hours")
                {
                    var lastEditList = await _contentRepository.GetChannelIdsCheckedByLastModifiedDateHourAsync(site, 2);
                    foreach (var channelId in lastEditList)
                    {
                        if (selectedChannelIdList.Contains(channelId))
                        {
                            channelIdList.Add(channelId);
                        }
                    }
                }
            }
            else
            {
                channelIdList = selectedChannelIdList;
            }

            foreach (var channelId in channelIdList)
            {
                if (request.IsChannelPage)
                {
                    await _createManager.CreateChannelAsync(request.SiteId, channelId);
                }
                if (request.IsContentPage)
                {
                    await _createManager.CreateAllContentAsync(request.SiteId, channelId);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
