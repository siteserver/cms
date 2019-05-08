using System;
using System.Collections.Generic;
using System.Web.Http;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Caches.Content;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Database.Attributes;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [RoutePrefix("pages/cms/contentsLayerCheck")]
    public class PagesContentsLayerCheckController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var siteId = Request.GetQueryInt("siteId");
                var channelId = Request.GetQueryInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(Request.GetQueryString("contentIds"));

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentCheck))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<Dictionary<string, object>>();
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    var dict = new Dictionary<string, object>(contentInfo.ToDictionary())
                    {
                        {"title", WebUtils.GetContentTitle(siteInfo, contentInfo, string.Empty)},
                        {"checkState", CheckManager.GetCheckState(siteInfo, contentInfo)}
                    };
                    retVal.Add(dict);
                }

                var isChecked = CheckManager.GetUserCheckLevel(rest.AdminPermissions, siteInfo, siteId, out var checkedLevel);
                var checkedLevels = CheckManager.GetCheckedLevels(siteInfo, isChecked, checkedLevel, true);

                var allChannels =
                    ChannelManager.GetChannels(siteId, rest.AdminPermissions, ConfigManager.ChannelPermissions.ContentAdd);

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
        public IHttpActionResult Submit()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var siteId = Request.GetPostInt("siteId");
                var channelId = Request.GetPostInt("channelId");
                var contentIdList = TranslateUtils.StringCollectionToIntList(Request.GetPostString("contentIds"));
                var checkedLevel = Request.GetPostInt("checkedLevel");
                var isTranslate = Request.GetPostBool("isTranslate");
                var translateChannelId = Request.GetPostInt("translateChannelId");
                var reasons = Request.GetPostString("reasons");

                if (!rest.IsAdminLoggin ||
                    !rest.AdminPermissions.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentCheck))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var isChecked = checkedLevel >= siteInfo.CheckContentLevel;
                if (isChecked)
                {
                    checkedLevel = 0;
                }
                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                var contentInfoList = new List<ContentInfo>();
                foreach (var contentId in contentIdList)
                {
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    contentInfo.Set(ContentAttribute.CheckUserName, rest.AdminName);
                    contentInfo.Set(ContentAttribute.CheckDate, DateTime.Now);
                    contentInfo.Set(ContentAttribute.CheckReasons, reasons);

                    contentInfo.Checked = isChecked;
                    contentInfo.CheckedLevel = checkedLevel;

                    if (isTranslate && translateChannelId > 0)
                    {
                        var translateChannelInfo = ChannelManager.GetChannelInfo(siteId, translateChannelId);
                        contentInfo.ChannelId = translateChannelInfo.Id;
                        DataProvider.ContentRepository.Update(siteInfo, translateChannelInfo, contentInfo);
                    }
                    else
                    {
                        DataProvider.ContentRepository.Update(siteInfo, channelInfo, contentInfo);
                    }

                    contentInfoList.Add(contentInfo);

                    var checkInfo = new ContentCheckInfo
                    {
                        TableName = tableName,
                        SiteId = siteId,
                        ChannelId = contentInfo.ChannelId,
                        ContentId = contentInfo.Id,
                        UserName = rest.AdminName,
                        Checked = isChecked,
                        CheckedLevel = checkedLevel,
                        CheckDate = DateTime.Now,
                        Reasons = reasons
                    };

                    DataProvider.ContentCheck.Insert(checkInfo);
                }

                if (isTranslate && translateChannelId > 0)
                {
                    ContentManager.RemoveCache(tableName, channelId);
                    var translateTableName = ChannelManager.GetTableName(siteInfo, translateChannelId);
                    ContentManager.RemoveCache(translateTableName, translateChannelId);
                }

                LogUtils.AddSiteLog(siteId, rest.AdminName, "批量审核内容");

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
