using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.DataCache.Content;
using SiteServer.CMS.Model.Attributes;
using SiteServer.CMS.StlParser.Model;

namespace SiteServer.API.Controllers.Pages.Cms
{
    [OpenApiIgnore]
    [RoutePrefix("pages/cms/contentsLayerDelete")]
    public class PagesContentsLayerDeleteController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public IHttpActionResult GetConfig()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var siteId = request.GetQueryInt("siteId");
                var channelId = request.GetQueryInt("channelId");
                var channelContentIds =
                    MinContentInfo.ParseMinContentInfoList(request.GetQueryString("channelContentIds"));

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentDelete))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var retVal = new List<Dictionary<string, object>>();
                foreach (var channelContentId in channelContentIds)
                {
                    var contentChannelInfo = ChannelManager.GetChannelInfo(siteId, channelContentId.ChannelId);
                    var contentInfo = ContentManager.GetContentInfo(siteInfo, contentChannelInfo, channelContentId.Id);
                    if (contentInfo == null) continue;

                    var dict = contentInfo.ToDictionary();
                    dict["title"] = WebUtils.GetContentTitle(siteInfo, contentInfo, string.Empty);
                    dict["checkState"] =
                        CheckManager.GetCheckState(siteInfo, contentInfo);
                    retVal.Add(dict);
                }

                return Ok(new
                {
                    Value = retVal
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
                var request = new AuthenticatedRequest();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var channelContentIds =
                    MinContentInfo.ParseMinContentInfoList(request.GetPostString("channelContentIds"));
                var isRetainFiles = request.GetPostBool("isRetainFiles");

                if (!request.IsAdminLoggin ||
                    !request.AdminPermissionsImpl.HasChannelPermissions(siteId, channelId,
                        ConfigManager.ChannelPermissions.ContentDelete))
                {
                    return Unauthorized();
                }

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (!isRetainFiles)
                {
                    foreach (var channelContentId in channelContentIds)
                    {
                        DeleteManager.DeleteContent(siteInfo, channelContentId.ChannelId, channelContentId.Id);
                    }
                }

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                if (channelContentIds.Count == 1)
                {
                    var channelContentId = channelContentIds[0];
                    var contentTitle = DataProvider.ContentDao.GetValue(tableName, channelContentId.Id, ContentAttribute.Title);
                    request.AddSiteLog(siteId, channelContentId.ChannelId, channelContentId.Id, "删除内容",
                        $"栏目:{ChannelManager.GetChannelNameNavigation(siteId, channelContentId.ChannelId)},内容标题:{contentTitle}");
                }
                else
                {
                    request.AddSiteLog(siteId, "批量删除内容",
                        $"栏目:{ChannelManager.GetChannelNameNavigation(siteId, channelId)},内容条数:{channelContentIds.Count}");
                }

                foreach (var distinctChannelId in channelContentIds.Select(x => x.ChannelId).Distinct())
                {
                    var contentIdList = channelContentIds.Where(x => x.ChannelId == distinctChannelId)
                        .Select(x => x.Id).ToList();
                    DataProvider.ContentDao.UpdateTrashContents(siteId, distinctChannelId, tableName, contentIdList);

                    CreateManager.TriggerContentChangedEvent(siteId, distinctChannelId);
                }

                return Ok(new
                {
                    Value = true
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
