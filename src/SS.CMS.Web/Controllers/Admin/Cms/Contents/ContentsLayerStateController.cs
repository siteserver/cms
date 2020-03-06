using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    [Route("admin/cms/contents/contentsLayerState")]
    public partial class ContentsLayerStateController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentCheckRepository _contentCheckRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public ContentsLayerStateController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentCheckRepository contentCheckRepository, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentCheckRepository = contentCheckRepository;
            _administratorRepository = administratorRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentView))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var content = await _contentRepository.GetAsync(site, channel, request.ContentId);
            if (content == null) return NotFound();

            var contentChecks = await _contentCheckRepository.GetCheckListAsync(content.SiteId, content.ChannelId, request.ContentId);
            contentChecks.ForEach(async x =>
            {
                x.Set("State", CheckManager.GetCheckState(site, x.Checked, x.CheckedLevel));
                x.Set("AdminName", await _administratorRepository.GetDisplayAsync(x.AdminId));
            });

            return new GetResult
            {
                ContentChecks = contentChecks,
                Content = content,
                State = CheckManager.GetCheckState(site, content)
            };
        }
    }
}
