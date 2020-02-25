using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsRecycleController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ContentsRecycle))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            if (request.Action == Action.Delete)
            {
                var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
                foreach (var channelId in summaries.Select(x => Math.Abs(x.ChannelId)).Distinct())
                {
                    var contentIdList = summaries.Where(x => Math.Abs(x.ChannelId) == channelId).Select(x => x.Id).Distinct().ToList();

                    var tableName = site.TableName;
                    var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
                    if (channel != null)
                    {
                        tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
                    }

                    await DataProvider.ContentRepository.RecycleDeleteAsync(site, channelId, tableName, contentIdList);
                }

                await auth.AddSiteLogAsync(request.SiteId, "从回收站删除内容");
            }
            else if (request.Action == Action.DeleteAll)
            {
                await DataProvider.ContentRepository.RecycleDeleteAllAsync(site);
                await auth.AddSiteLogAsync(request.SiteId, "从回收站清空所有内容");
            }
            else if (request.Action == Action.Restore)
            {
                var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
                foreach (var channelId in summaries.Select(x => Math.Abs(x.ChannelId)).Distinct())
                {
                    var contentIdList = summaries.Where(x => Math.Abs(x.ChannelId) == channelId).Select(x => x.Id).Distinct().ToList();

                    var tableName = site.TableName;
                    var channel = await DataProvider.ChannelRepository.GetAsync(channelId);
                    if (channel != null)
                    {
                        tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
                    }

                    await DataProvider.ContentRepository.RecycleRestoreAsync(site, channelId, tableName, contentIdList, request.RestoreChannelId);
                }

                await auth.AddSiteLogAsync(request.SiteId, "从回收站还原内容");
            }
            else if (request.Action == Action.RestoreAll)
            {
                await DataProvider.ContentRepository.RecycleRestoreAllAsync(site, request.RestoreChannelId);
                await auth.AddSiteLogAsync(request.SiteId, "从回收站还原所有内容");
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
