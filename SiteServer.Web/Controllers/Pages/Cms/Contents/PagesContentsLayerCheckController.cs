using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    
    [RoutePrefix("pages/cms/contentsLayerCheck")]
    public class PagesContentsLayerCheckController : ApiController
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
                var channelContentIds =
                    MinContentInfo.ParseMinContentInfoList(request.GetQueryString("channelContentIds"));

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentCheck))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<IDictionary<string, object>>();
                foreach (var channelContentId in channelContentIds)
                {
                    var contentChannelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, contentChannelInfo, channelContentId.Id);
                    if (contentInfo == null) continue;

                    var dict = contentInfo.ToDictionary();
                    dict["title"] = WebUtils.GetContentTitle(site, contentInfo, string.Empty);
                    dict["checkState"] =
                        CheckManager.GetCheckState(site, contentInfo);
                    retVal.Add(dict);
                }

                var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(request.AdminPermissionsImpl, site, channelId);
                var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

                var allChannels =
                    await ChannelManager.GetChannelsAsync(siteId, request.AdminPermissionsImpl, Constants.ChannelPermissions.ContentAdd);

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
                var channelContentIds =
                    MinContentInfo.ParseMinContentInfoList(request.GetPostString("channelContentIds"));
                var checkedLevel = request.GetPostInt("checkedLevel");
                var isTranslate = request.GetPostBool("isTranslate");
                var translateChannelId = request.GetPostInt("translateChannelId");
                var reasons = request.GetPostString("reasons");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentCheck))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var isChecked = checkedLevel >= site.CheckContentLevel;
                if (isChecked)
                {
                    checkedLevel = 0;
                }
                var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);

                var contentInfoList = new List<Content>();
                foreach (var channelContentId in channelContentIds)
                {
                    var contentChannelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, contentChannelInfo, channelContentId.Id);
                    if (contentInfo == null) continue;

                    contentInfo.CheckUserName = request.AdminName;
                    contentInfo.CheckDate = DateTime.Now;
                    contentInfo.CheckReasons = reasons;

                    contentInfo.Checked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;

                    if (isTranslate && translateChannelId > 0)
                    {
                        var translateChannelInfo = await ChannelManager.GetChannelAsync(siteId, translateChannelId);
                        contentInfo.ChannelId = translateChannelInfo.Id;
                        await DataProvider.ContentRepository.UpdateAsync(site, translateChannelInfo, contentInfo);
                    }
                    else
                    {
                        await DataProvider.ContentRepository.UpdateAsync(site, contentChannelInfo, contentInfo);
                    }

                    contentInfoList.Add(contentInfo);

                    var checkInfo = new ContentCheck
                    {
                        TableName = tableName,
                        SiteId = siteId,
                        ChannelId = contentInfo.ChannelId,
                        ContentId = contentInfo.Id,
                        UserName = request.AdminName,
                        IsChecked = isChecked.ToString(),
                        CheckedLevel = checkedLevel,
                        CheckDate = DateTime.Now,
                        Reasons = reasons
                    };

                    await DataProvider.ContentCheckRepository.InsertAsync(checkInfo);
                }

                await request.AddSiteLogAsync(siteId, "批量审核内容");

                foreach (var contentInfo in contentInfoList)
                {
                    await CreateManager.CreateContentAsync(siteId, contentInfo.ChannelId, contentInfo.Id);
                }

                foreach (var distinctChannelId in channelContentIds.Select(x => x.ChannelId).Distinct())
                {
                    await CreateManager.TriggerContentChangedEventAsync(siteId, distinctChannelId);
                }

                if (isTranslate && translateChannelId > 0)
                {
                    await CreateManager.TriggerContentChangedEventAsync(siteId, translateChannelId);
                }

                return Ok(new
                {
                    Value = true
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
