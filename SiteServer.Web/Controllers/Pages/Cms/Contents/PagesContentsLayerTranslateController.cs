using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Dto.Result;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerTranslate")]
    public partial class PagesContentsLayerTranslateController : ApiController
    {
        private const string Route = "";
        private const string RouteOptions = "actions/options";

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var channel = await DataProvider.ChannelRepository.GetAsync(summary.ChannelId);
                var content = await DataProvider.ContentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                var pageContent = new Content(content.ToDictionary());
                pageContent.Set(ContentAttribute.CheckState, CheckManager.GetCheckState(site, content));
                contents.Add(pageContent);
            }

            var siteIdList = await auth.AdminPermissions.GetSiteIdListAsync();
            var transSites = await DataProvider.SiteRepository.GetSelectsAsync(siteIdList);

            return new GetResult
            {
                Contents = contents,
                TransSites = transSites
            };
        }

        [HttpPost, Route(RouteOptions)]
        public async Task<GetOptionsResult> GetOptions([FromBody]GetOptionsRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Request.Unauthorized<GetOptionsResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetOptionsResult>();

            var channelIdList = await auth.AdminPermissions.GetChannelIdListAsync(request.TransSiteId, Constants.ChannelPermissions.ContentAdd);

            var transChannels = await DataProvider.ChannelRepository.GetAsync(request.TransSiteId);
            var transSite = await DataProvider.SiteRepository.GetAsync(request.TransSiteId);
            var cascade = await DataProvider.ChannelRepository.GetCascadeAsync(transSite, transChannels, async summary =>
            {
                var count = await DataProvider.ContentRepository.GetCountAsync(site, summary);

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
        public async Task<BoolResult> Submit([FromBody] SubmitRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentTranslate))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            summaries.Reverse();

            foreach (var summary in summaries)
            {
                await ContentUtility.TranslateAsync(site, summary.ChannelId, summary.Id, request.TransSiteId, request.TransChannelId, TranslateContentType.Cut);
            }

            await auth.AddSiteLogAsync(request.SiteId, request.ChannelId, "转移内容", string.Empty);

            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                await CreateManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            await CreateManager.TriggerContentChangedEventAsync(request.TransSiteId, request.TransChannelId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
