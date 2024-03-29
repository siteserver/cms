﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Models;
using System.Collections.Generic;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerTaxisController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            //   var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            var summaries = new List<ChannelContentId>();
            var jsonFilePath = _pathManager.GetTemporaryFilesPath(request.FileName);
            if (FileUtils.IsFileExists(jsonFilePath))
            {
                var json = await FileUtils.ReadTextAsync(jsonFilePath);
                if (!string.IsNullOrEmpty(json))
                {
                    summaries = TranslateUtils.JsonDeserialize<List<ChannelContentId>>(json);
                }
                FileUtils.DeleteFileIfExists(jsonFilePath);
            }

            if (StringUtils.EqualsIgnoreCase(request.Type, "to"))
            {
                summaries.Reverse();
                foreach (var summary in summaries)
                {
                    var channel = await _channelRepository.GetAsync(summary.ChannelId);
                    var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                    await _contentRepository.SetTaxisAsync(site, channel, content.Id, content.Top, request.Value);
                }
            }
            else
            {
                var isUp = StringUtils.EqualsIgnoreCase(request.Type, "up");

                if (isUp == false)
                {
                    summaries.Reverse();
                }

                foreach (var summary in summaries)
                {
                    var channel = await _channelRepository.GetAsync(summary.ChannelId);
                    var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                    if (content == null) continue;

                    var isTop = content.Top;
                    for (var i = 1; i <= request.Value; i++)
                    {
                        if (isUp)
                        {
                            if (await _contentRepository.SetTaxisToUpAsync(site, channel, summary.Id, isTop) == false)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (await _contentRepository.SetTaxisToDownAsync(site, channel, summary.Id, isTop) == false)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "对内容排序", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}