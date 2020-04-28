using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ContentsLayerColumnsController : ControllerBase
    {
        private const string Route = "contentsLayerColumns";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IOldPluginManager _pluginManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public ContentsLayerColumnsController(IAuthManager authManager, IDatabaseManager databaseManager, IOldPluginManager pluginManager, IPathManager pathManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]ChannelRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.SiteChannelPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager, _pathManager);
            var attributes = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            return new GetResult
            {
                Attributes = attributes
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            if (!await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,  AuthTypes.SiteChannelPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            channelInfo.ListColumns = request.AttributeNames;

            await _channelRepository.UpdateAsync(channelInfo);

            await _authManager.AddSiteLogAsync(request.SiteId, "设置内容显示项", $"显示项:{request.AttributeNames}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
