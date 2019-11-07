using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.Model.Db;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    [OpenApiIgnore]
    [RoutePrefix("home/contentsLayerCheck")]
    public class HomeContentsLayerCheckController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(request.GetQueryString("contentIds"));

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentCheck))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<Dictionary<string, object>>();
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = ContentManager.GetContentInfo(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    var dict = contentInfo.ToDictionary();
                    dict["checkState"] =
                        CheckManager.GetCheckState(site, contentInfo);
                    retVal.Add(dict);
                }

                var isChecked = CheckManager.GetUserCheckLevel(request.AdminPermissionsImpl, site, channelId, out var checkedLevel);
                var checkedLevels = CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true);

                var allChannels =
                    ChannelManager.GetChannels(siteId, request.AdminPermissionsImpl, ConfigManager.ChannelPermissions.ContentAdd);

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
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(request.GetPostString("contentIds"));
                var checkedLevel = request.GetPostInt("checkedLevel");
                var isTranslate = request.GetPostBool("isTranslate");
                var translateChannelId = request.GetPostInt("translateChannelId");
                var reasons = request.GetPostString("reasons");

                if (!request.IsUserLoggin ||
                    !request.UserPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentCheck))
                {
                    return Unauthorized();
                }

                var site = await SiteManager.GetSiteAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var isChecked = checkedLevel >= site.Additional.CheckContentLevel;
                if (isChecked)
                {
                    checkedLevel = 0;
                }
                var tableName = ChannelManager.GetTableName(site, channelInfo);

                var contentInfoList = new List<ContentInfo>();
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = ContentManager.GetContentInfo(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    contentInfo.Set(ContentAttribute.CheckUserName, request.AdminName);
                    contentInfo.Set(ContentAttribute.CheckDate, DateTime.Now);
                    contentInfo.Set(ContentAttribute.CheckReasons, reasons);

                    contentInfo.IsChecked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;

                    if (isTranslate && translateChannelId > 0)
                    {
                        var translateChannelInfo = ChannelManager.GetChannelInfo(siteId, translateChannelId);
                        contentInfo.ChannelId = translateChannelInfo.Id;
                        DataProvider.ContentDao.Update(site, translateChannelInfo, contentInfo);
                    }
                    else
                    {
                        DataProvider.ContentDao.Update(site, channelInfo, contentInfo);
                    }

                    contentInfoList.Add(contentInfo);

                    var checkInfo = new ContentCheckInfo(0, tableName, siteId, contentInfo.ChannelId, contentInfo.Id, request.AdminName, isChecked, checkedLevel, DateTime.Now, reasons);
                    DataProvider.ContentCheckDao.Insert(checkInfo);
                }

                if (isTranslate && translateChannelId > 0)
                {
                    ContentManager.RemoveCache(tableName, channelId);
                    var translateTableName = ChannelManager.GetTableName(site, translateChannelId);
                    ContentManager.RemoveCache(translateTableName, translateChannelId);
                }

                await request.AddSiteLogAsync(siteId, "批量审核内容");

                foreach (var contentInfo in contentInfoList)
                {
                    CreateManager.CreateContent(siteId, contentInfo.ChannelId, contentInfo.Id);
                }
                CreateManager.TriggerContentChangedEvent(siteId, channelId);
                if (isTranslate && translateChannelId > 0)
                {
                    CreateManager.TriggerContentChangedEvent(siteId, translateChannelId);
                }

                return Ok(new
                {
                    Value = contentIdList
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
