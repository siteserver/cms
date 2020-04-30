using System.Collections.Generic;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsCreateTriggerController : ControllerBase
    {
        private const string Route = "cms/settings/settingsCreateTrigger";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public SettingsCreateTriggerController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> List([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsCreateTrigger))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(request.SiteId);
            var cascade = await _channelRepository.GetCascadeAsync(site, channel, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);
                var entity = await _channelRepository.GetAsync(summary.Id);

                var changeNames = new List<string>();
                var channelIdList = Utilities.GetIntList(entity.CreateChannelIdsIfContentChanged);
                foreach (var channelId in channelIdList)
                {
                    if (await _channelRepository.IsExistsAsync(channelId))
                    {
                        changeNames.Add(await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, channelId));
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
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    AuthTypes.SitePermissions.SettingsCreateTrigger))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var channel = await _channelRepository.GetAsync(request.ChannelId);

            channel.IsCreateChannelIfContentChanged = request.IsCreateChannelIfContentChanged;
            channel.CreateChannelIdsIfContentChanged = Utilities.ToString(request.CreateChannelIdsIfContentChanged);

            await _channelRepository.UpdateAsync(channel);

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "设置栏目变动生成页面", $"栏目:{channel.ChannelName}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}