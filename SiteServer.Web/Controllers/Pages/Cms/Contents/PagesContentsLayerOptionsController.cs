using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Cms.Contents
{
    
    [RoutePrefix("pages/cms/contentsLayerOptions")]
    public class PagesContentsLayerOptionsController : ApiController
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

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ChannelEdit))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteDao.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var attributes = await ChannelManager.GetContentsColumnsAsync(site, channelInfo, true);

                return Ok(new
                {
                    Value = attributes,
                    channelInfo.IsAllContents,
                    channelInfo.IsSelfOnly
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

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ChannelEdit))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteDao.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                var attributeNames = request.GetPostString("attributeNames");
                var isAllContents = request.GetPostBool("isAllContents");
                var isSelfOnly = request.GetPostBool("isSelfOnly");

                channelInfo.ContentAttributesOfDisplay = attributeNames;
                channelInfo.IsAllContents = isAllContents;
                channelInfo.IsSelfOnly = isSelfOnly;

                await DataProvider.ChannelDao.UpdateAsync(channelInfo);

                await request.AddSiteLogAsync(siteId, "设置内容选项");

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
