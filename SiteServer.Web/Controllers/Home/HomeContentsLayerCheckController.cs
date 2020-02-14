using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using Datory.Utils;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Home
{
    
    [RoutePrefix("home/contentsLayerCheck")]
    public class HomeContentsLayerCheckController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var contentIdList = Utilities.GetIntList(request.GetQueryString("contentIds"));

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentCheckLevel1))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<IDictionary<string, object>>();
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    var dict = contentInfo.ToDictionary();
                    dict["checkState"] =
                        CheckManager.GetCheckState(site, contentInfo);
                    retVal.Add(dict);
                }

                var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(request.AdminPermissionsImpl, site, channelId);
                var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

                var allChannels =
                    await DataProvider.ChannelRepository.GetChannelsAsync(siteId, request.AdminPermissionsImpl, Constants.ChannelPermissions.ContentAdd);

                return Ok(new
                {
                    Value = retVal,
                    CheckedLevels = checkedLevels,
                    CheckedLevel = checkedLevel,
                    AllChannels = allChannels
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentIdList = Utilities.GetIntList(request.GetPostString("contentIds"));
                var checkedLevel = request.GetPostInt("checkedLevel");
                var isTranslate = request.GetPostBool("isTranslate");
                var translateChannelId = request.GetPostInt("translateChannelId");
                var reasons = request.GetPostString("reasons");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentCheckLevel1))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await DataProvider.ChannelRepository.GetAsync(channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var isChecked = checkedLevel >= site.CheckContentLevel;
                if (isChecked)
                {
                    checkedLevel = 0;
                }
                var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channelInfo);

                var contentInfoList = new List<Content>();
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    contentInfo.CheckAdminId = request.AdminId;
                    contentInfo.CheckDate = DateTime.Now;
                    contentInfo.CheckReasons = reasons;

                    contentInfo.Checked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;

                    if (isTranslate && translateChannelId > 0)
                    {
                        var translateChannelInfo = await DataProvider.ChannelRepository.GetAsync(translateChannelId);
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
                        SiteId = siteId,
                        ChannelId = contentInfo.ChannelId,
                        ContentId = contentInfo.Id,
                        AdminId = request.AdminId,
                        Checked = isChecked,
                        CheckedLevel = checkedLevel,
                        CheckDate = DateTime.Now,
                        Reasons = reasons
                    });
                }

                await request.AddSiteLogAsync(siteId, "批量审核内容");

                foreach (var contentInfo in contentInfoList)
                {
                    await CreateManager.CreateContentAsync(siteId, contentInfo.ChannelId, contentInfo.Id);
                }
                await CreateManager.TriggerContentChangedEventAsync(siteId, channelId);
                if (isTranslate && translateChannelId > 0)
                {
                    await CreateManager.TriggerContentChangedEventAsync(siteId, translateChannelId);
                }

                return Ok(new
                {
                    Value = contentIdList
                });
            }
            catch (Exception ex)
            {
                await LogUtils.AddErrorLogAsync(ex);
                return InternalServerError(ex);
            }
        }
    }
}
