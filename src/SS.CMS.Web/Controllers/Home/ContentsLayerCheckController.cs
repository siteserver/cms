using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerCheck")]
    public partial class ContentsLayerCheckController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentCheckRepository _contentCheckRepository;

        public ContentsLayerCheckController(IAuthManager authManager, ICreateManager createManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentCheckRepository contentCheckRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentCheckRepository = contentCheckRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromBody] GetRequest request)
        {
            if (!await _authManager.IsUserAuthenticatedAsync() ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentCheckLevel1))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            var retVal = new List<IDictionary<string, object>>();
            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                if (contentInfo == null) continue;

                var dict = contentInfo.ToDictionary();
                dict["checkState"] =
                    CheckManager.GetCheckState(site, contentInfo);
                retVal.Add(dict);
            }

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, request.ChannelId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            var allChannels =
                await _channelRepository.GetChannelsAsync(request.SiteId, _authManager, Constants.ChannelPermissions.ContentAdd);

            return new GetResult
            {
                Value = retVal,
                CheckedLevels = checkedLevels,
                CheckedLevel = checkedLevel,
                AllChannels = allChannels
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            if (!await _authManager.IsUserAuthenticatedAsync() ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentCheckLevel1))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await _channelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                request.CheckedLevel = 0;
            }

            var adminId = await _authManager.GetAdminIdAsync();
            var contentInfoList = new List<Content>();
            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await _contentRepository.GetAsync(site, channelInfo, contentId);
                if (contentInfo == null) continue;

                contentInfo.Set(ColumnsManager.CheckAdminId, adminId);
                contentInfo.Set(ColumnsManager.CheckDate, DateTime.Now);
                contentInfo.Set(ColumnsManager.CheckReasons, request.Reasons);

                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = request.CheckedLevel;

                if (request.IsTranslate && request.TranslateChannelId > 0)
                {
                    var translateChannelInfo = await _channelRepository.GetAsync(request.TranslateChannelId);
                    contentInfo.ChannelId = translateChannelInfo.Id;
                    await _contentRepository.UpdateAsync(site, translateChannelInfo, contentInfo);
                }
                else
                {
                    await _contentRepository.UpdateAsync(site, channelInfo, contentInfo);
                }

                contentInfoList.Add(contentInfo);

                await _contentCheckRepository.InsertAsync(new ContentCheck
                {
                    SiteId = request.SiteId,
                    ChannelId = contentInfo.ChannelId,
                    ContentId = contentInfo.Id,
                    AdminId = adminId,
                    Checked = isChecked,
                    CheckedLevel = request.CheckedLevel,
                    CheckDate = DateTime.Now,
                    Reasons = request.Reasons
                });
            }

            await _authManager.AddSiteLogAsync(request.SiteId, "批量审核内容");

            foreach (var contentInfo in contentInfoList)
            {
                await _createManager.CreateContentAsync(request.SiteId, contentInfo.ChannelId, contentInfo.Id);
            }
            await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.ChannelId);
            if (request.IsTranslate && request.TranslateChannelId > 0)
            {
                await _createManager.TriggerContentChangedEventAsync(request.SiteId, request.TranslateChannelId);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
