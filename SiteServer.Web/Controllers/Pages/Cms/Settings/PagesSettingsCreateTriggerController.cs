using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Request;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsCreateTrigger")]
    public partial class PagesSettingsCreateTriggerController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<GetResult> List([FromUri] SiteRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<GetResult>("无法确定内容对应的站点");

            var channel = await DataProvider.ChannelRepository.GetAsync(request.SiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);
                var entity = await DataProvider.ChannelRepository.GetAsync(summary.Id);

                var changeNames = new List<string>();
                var channelIdList = Utilities.GetIntList(entity.CreateChannelIdsIfContentChanged);
                foreach (var channelId in channelIdList)
                {
                    if (await DataProvider.ChannelRepository.IsExistsAsync(channelId))
                    {
                        changeNames.Add(await DataProvider.ChannelRepository.GetChannelNameNavigationAsync(request.SiteId, channelId));
                    }
                }

                return new
                {
                    entity.IndexName,
                    Count = count,
                    ChangeNames = changeNames,
                    entity.IsCreateChannelIfContentChanged,
                    CreateChannelIdsIfContentChanged = channelIdList,
                };
            });

            return new GetResult
            {
                Channel = cascade
            };
        }

        [HttpPut, Route(Route)]
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<BoolResult>("无法确定内容对应的站点");

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);

            channel.IsCreateChannelIfContentChanged = request.IsCreateChannelIfContentChanged;
            channel.CreateChannelIdsIfContentChanged = Utilities.ToString(request.CreateChannelIdsIfContentChanged);

            await DataProvider.ChannelRepository.UpdateAsync(channel);

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "设置栏目变动生成页面", $"栏目:{channel.ChannelName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}