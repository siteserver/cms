using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsRecycleController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.ContentsRecycle))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            if (request.Action == Action.Delete)
            {
                var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
                foreach (var channelId in summaries.Select(x => Math.Abs(x.ChannelId)).Distinct())
                {
                    var contentIdList = summaries.Where(x => Math.Abs(x.ChannelId) == channelId).Select(x => x.Id).Distinct().ToList();

                    var tableName = site.TableName;
                    var channel = await _channelRepository.GetAsync(channelId);
                    if (channel != null)
                    {
                        tableName = _channelRepository.GetTableName(site, channel);
                    }

                    await _contentRepository.DeleteTrashAsync(site, channelId, tableName, contentIdList, _pluginManager);
                }

                await _authManager.AddSiteLogAsync(request.SiteId, "从回收站删除内容");
            }
            else if (request.Action == Action.DeleteAll)
            {
                await _contentRepository.DeleteTrashAsync(site, _pluginManager);
                await _authManager.AddSiteLogAsync(request.SiteId, "从回收站清空所有内容");
            }
            else if (request.Action == Action.Restore)
            {
                var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
                foreach (var channelId in summaries.Select(x => Math.Abs(x.ChannelId)).Distinct())
                {
                    var contentIdList = summaries.Where(x => Math.Abs(x.ChannelId) == channelId).Select(x => x.Id).Distinct().ToList();

                    var tableName = site.TableName;
                    var channel = await _channelRepository.GetAsync(channelId);
                    if (channel != null)
                    {
                        tableName = _channelRepository.GetTableName(site, channel);
                    }

                    await _contentRepository.RestoreTrashAsync(site, channelId, tableName, contentIdList, request.RestoreChannelId);
                }

                await _authManager.AddSiteLogAsync(request.SiteId, "从回收站还原内容");
            }
            else if (request.Action == Action.RestoreAll)
            {
                await _contentRepository.RestoreTrashAsync(site, request.RestoreChannelId);
                await _authManager.AddSiteLogAsync(request.SiteId, "从回收站还原所有内容");
            }

            return new BoolResult
            {
                Value = true
            };
        }

        public enum Action
        {
            Delete,
            DeleteAll,
            Restore,
            RestoreAll
        }

        public class DeleteRequest : SiteRequest
        {
            public Action Action { get; set; }
            public string ChannelContentIds { get; set; }
            public int RestoreChannelId { get; set; }
        }
    }
}
