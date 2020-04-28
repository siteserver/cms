using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class ContentsLayerTaxisController : ControllerBase
    {
        private const string Route = "contentsLayerTaxis";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public ContentsLayerTaxisController(IAuthManager authManager, ICreateManager createManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            if (!await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, AuthTypes.SiteContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            if (channel.DefaultTaxisType == TaxisType.OrderByTaxis)
            {
                request.IsUp = !request.IsUp;
            }

            if (request.IsUp == false)
            {
                request.ContentIds.Reverse();
            }

            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await _contentRepository.GetAsync(site, channel, contentId);
                if (contentInfo == null) continue;

                var isTop = contentInfo.Top;
                for (var i = 1; i <= request.Taxis; i++)
                {
                    if (request.IsUp)
                    {
                        if (await _contentRepository.SetTaxisToUpAsync(site, channel, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (await _contentRepository.SetTaxisToDownAsync(site, channel, contentId, isTop) == false)
                        {
                            break;
                        }
                    }
                }
            }

            await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);

            await _authManager.AddSiteLogAsync(request.SiteId, request.ChannelId, 0, "对内容排序", string.Empty);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
