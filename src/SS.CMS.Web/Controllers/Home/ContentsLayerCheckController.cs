using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Home
{
    [Route("home/contentsLayerCheck")]
    public partial class ContentsLayerCheckController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;

        public ContentsLayerCheckController(IAuthManager authManager, ICreateManager createManager)
        {
            _authManager = authManager;
            _createManager = createManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromBody] GetRequest request)
        {
            var auth = await _authManager.GetUserAsync();
            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentCheckLevel1))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            var retVal = new List<IDictionary<string, object>>();
            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                if (contentInfo == null) continue;

                var dict = contentInfo.ToDictionary();
                dict["checkState"] =
                    CheckManager.GetCheckState(site, contentInfo);
                retVal.Add(dict);
            }

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissions, site, request.ChannelId);
            var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

            var allChannels =
                await DataProvider.ChannelRepository.GetChannelsAsync(request.SiteId, auth.AdminPermissions, Constants.ChannelPermissions.ContentAdd);

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
            var auth = await _authManager.GetUserAsync();

            if (!auth.IsUserLoggin ||
                !await auth.UserPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentCheckLevel1))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channelInfo = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channelInfo == null) return NotFound();

            var isChecked = request.CheckedLevel >= site.CheckContentLevel;
            if (isChecked)
            {
                request.CheckedLevel = 0;
            }

            var contentInfoList = new List<Content>();
            foreach (var contentId in request.ContentIds)
            {
                var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                if (contentInfo == null) continue;

                contentInfo.CheckAdminId = auth.AdminId;
                contentInfo.CheckDate = DateTime.Now;
                contentInfo.CheckReasons = request.Reasons;

                contentInfo.Checked = isChecked;
                contentInfo.CheckedLevel = request.CheckedLevel;

                if (request.IsTranslate && request.TranslateChannelId > 0)
                {
                    var translateChannelInfo = await DataProvider.ChannelRepository.GetAsync(request.TranslateChannelId);
                    contentInfo.ChannelId = translateChannelInfo.Id;
                    await DataProvider.ContentRepository.UpdateAsync(site, translateChannelInfo, contentInfo);
                }
                else
                {
                    await DataProvider.ContentRepository.UpdateAsync(site, channelInfo, contentInfo);
                }

                contentInfoList.Add(contentInfo);

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
            }

            await auth.AddSiteLogAsync(request.SiteId, "批量审核内容");

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
