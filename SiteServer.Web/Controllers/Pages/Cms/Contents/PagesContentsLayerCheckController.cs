using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    [RoutePrefix("pages/cms/contents/contentsLayerCheck")]
    public partial class PagesContentsLayerCheckController : ApiController
    {
        private const string Route = "";
        private const string RouteOptions = "actions/options";

        private readonly ICreateManager _createManager;

        public PagesContentsLayerCheckController(ICreateManager createManager)
        {
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentCheckLevel1))
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

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissionsImpl, site, request.ChannelId);
            var checkedLevels = ElementUtils.GetSelects(CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true));

            return new GetResult
            {
                Contents = contents,
                CheckedLevels = checkedLevels,
                CheckedLevel = checkedLevel,
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
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentCheckLevel1))
            {
                return Request.Unauthorized<BoolResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<BoolResult>();

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                request.CheckedLevel = 0;
            }

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            summaries.Reverse();

            foreach (var summary in summaries)
            {
                var contentChannelInfo = await DataProvider.ChannelRepository.GetAsync(summary.ChannelId);
                var contentInfo = await DataProvider.ContentRepository.GetAsync(site, contentChannelInfo, summary.Id);
                if (contentInfo == null) continue;

                contentInfo.CheckAdminId = auth.AdminId;
                contentInfo.CheckDate = DateTime.Now;
                contentInfo.CheckReasons = request.Reasons;

                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = request.CheckedLevel;

                await DataProvider.ContentRepository.UpdateAsync(site, contentChannelInfo, contentInfo);

                await DataProvider.ContentCheckRepository.InsertAsync(new ContentCheck
                {
                    SiteId = request.SiteId,
                    ChannelId = contentInfo.ChannelId,
                    ContentId = contentInfo.Id,
                    AdminId = auth.AdminId,
                    Checked = isChecked,
                    CheckedLevel = request.CheckedLevel,
                    CheckDate = DateTime.Now,
                    Reasons = request.Reasons
                });

                if (request.IsTranslate)
                {
                    await ContentUtility.TranslateAsync(site, summary.ChannelId, summary.Id, request.TransSiteId, request.TransChannelId, TranslateContentType.Cut, _createManager);
                }
            }

            await auth.AddSiteLogAsync(request.SiteId, "批量审核内容");

            if (request.IsTranslate)
            {
                await _createManager.TriggerContentChangedEventAsync(request.TransSiteId, request.TransChannelId);
            }
            else
            {
                foreach (var summary in summaries)
                {
                    await _createManager.CreateContentAsync(request.SiteId, summary.ChannelId, summary.Id);
                }
            }

            foreach (var distinctChannelId in summaries.Select(x => x.ChannelId).Distinct())
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, distinctChannelId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
