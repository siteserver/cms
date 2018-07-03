using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Http;
using SiteServer.CMS.Api.V1;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("api")]
    public class ContentsController : ApiController
    {
        private const string Route = "v1/contents/{siteId:int}/{channelId:int}";
        private const string RouteContent = "v1/contents/{siteId:int}/{channelId:int}/{id:int}";

        [HttpPost, Route(Route)]
        public IHttpActionResult Create(int siteId, int channelId, [FromBody] IContentInfo contentInfo)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeContents);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                if (contentInfo == null) return BadRequest("无法从body中获取内容实体");
                contentInfo.SiteId = siteId;
                contentInfo.ChannelId = channelId;

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                contentInfo.Id = DataProvider.ContentDao.Insert(tableName, siteInfo, contentInfo);

                return Ok(new OResponse(contentInfo));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpPut, Route(RouteContent)]
        public IHttpActionResult Update(int siteId, int channelId, int id, [FromBody] IContentInfo contentInfo)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeContents);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                if (contentInfo == null) return BadRequest("无法从body中获取内容实体");
                contentInfo.SiteId = siteId;
                contentInfo.ChannelId = channelId;
                contentInfo.Id = id;

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                if (!DataProvider.ContentDao.ApiIsExists(tableName, id)) return NotFound();

                DataProvider.ContentDao.Update(tableName, siteInfo, contentInfo);

                return Ok(new OResponse(contentInfo));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpDelete, Route(RouteContent)]
        public IHttpActionResult Delete(int siteId, int channelId, int id)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeContents);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, id);
                if (contentInfo == null) return NotFound();

                DataProvider.ContentDao.DeleteContent(tableName, siteInfo, channelId, id);

                return Ok(new OResponse(contentInfo));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(RouteContent)]
        public IHttpActionResult Get(int siteId, int channelId, int id)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeContents);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableName, id);
                if (contentInfo == null) return NotFound();

                return Ok(new OResponse(contentInfo));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }

        [HttpGet, Route(Route)]
        public IHttpActionResult List(int siteId, int channelId)
        {
            try
            {
                var oRequest = new ORequest(AccessTokenManager.ScopeContents);
                if (!oRequest.IsApiAuthorized) return Unauthorized();

                var siteInfo = SiteManager.GetSiteInfo(siteId);
                if (siteInfo == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = ChannelManager.GetChannelInfo(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var tableName = ChannelManager.GetTableName(siteInfo, channelInfo);

                var contentIdList = DataProvider.ContentDao.GetContentIdListCheckedByChannelId(tableName, siteId, channelId);
                var value = new List<IContentInfo>();
                foreach(var contentId in contentIdList)
                {
                    value.Add(DataProvider.ContentDao.GetContentInfo(tableName, contentId));
                }

                return Ok(new OResponse(value));
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
