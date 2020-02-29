using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Contents
{
    [Route("admin/cms/contents/contentsLayerCopy")]
    public partial class ContentsLayerCopyController : ControllerBase
    {
        private const string Route = "";
        private const string RouteOptions = "actions/options";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerCopyController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                var pageContent = new Content(content.ToDictionary());
                pageContent.Set(ContentAttribute.CheckState, CheckManager.GetCheckState(site, content));
                contents.Add(pageContent);
            }

            var siteIdList = await auth.AdminPermissions.GetSiteIdListAsync();
            var transSites = await _siteRepository.GetSelectsAsync(siteIdList);

            return new GetResult
            {
                Contents = contents,
                TransSites = transSites
            };
        }

        [HttpPost, Route(RouteOptions)]
        public async Task<ActionResult<GetOptionsResult>> GetOptions([FromBody]GetOptionsRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelIdList = await auth.AdminPermissions.GetChannelIdListAsync(request.TransSiteId, Constants.ChannelPermissions.ContentAdd);

            var transChannels = await _channelRepository.GetAsync(request.TransSiteId);
            var transSite = await _siteRepository.GetAsync(request.TransSiteId);
            var cascade = await _channelRepository.GetCascadeAsync(transSite, transChannels, async summary =>
            {
                var count = await _contentRepository.GetCountAsync(site, summary);

                return new
                {
                    Disabled = !channelIdList.Contains(summary.Id),
                    summary.IndexName,
                    Count = count
                };
            });

            return new GetOptionsResult
            {
                TransChannels = cascade
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            summaries.Reverse();

            foreach (var summary in summaries)
            {
                await ContentUtility.TranslateAsync(_pathManager, _databaseManager, _pluginManager, site, summary.ChannelId, summary.Id, request.TransSiteId, request.TransChannelId, request.CopyType, _createManager);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, "复制内容", string.Empty);

            await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
