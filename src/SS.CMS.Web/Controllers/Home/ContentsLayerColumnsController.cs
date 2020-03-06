using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerColumns")]
    public partial class ContentsLayerColumnsController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;

        public ContentsLayerColumnsController(IAuthManager authManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]ChannelRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ChannelEdit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager);
            var attributes = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            return new GetResult
            {
                Attributes = attributes
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId,  Constants.ChannelPermissions.ChannelEdit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            channelInfo.ListColumns = request.AttributeNames;

            await _channelRepository.UpdateAsync(channelInfo);

            await auth.AddSiteLogAsync(request.SiteId, "设置内容显示项", $"显示项:{request.AttributeNames}");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
