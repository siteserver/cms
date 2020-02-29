using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerView")]
    public partial class ContentsLayerViewController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerViewController(IAuthManager authManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentView))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var content = await _contentRepository.GetAsync(site, channel, request.ContentId);
            if (content == null) return NotFound();

            content.Set(ContentAttribute.CheckState, CheckManager.GetCheckState(site, content));

            var channelName = await _channelRepository.GetChannelNameNavigationAsync(request.SiteId, request.ChannelId);

            var columnsManager = new ColumnsManager(_databaseManager, _pluginManager);
            var attributes = await columnsManager.GetContentListColumnsAsync(site, channel, ColumnsManager.PageType.Contents);

            return new GetResult
            {
                Content = content,
                ChannelName = channelName,
                Attributes = attributes
            };
        }
    }
}
