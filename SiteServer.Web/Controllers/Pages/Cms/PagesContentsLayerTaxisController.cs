using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms
{
    
    [RoutePrefix("pages/cms/contentsLayerTaxis")]
    public class PagesContentsLayerTaxisController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<IHttpActionResult> Submit()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var channelContentIds =
                    ChannelContentId.ParseMinContentInfoList(request.GetPostString("channelContentIds"));
                var isUp = request.GetPostBool("isUp");
                var taxis = request.GetPostInt("taxis");

                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSitePermissionsAsync(siteId,
                        Constants.SitePermissions.Contents) ||
                    !await request.AdminPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentEdit))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteRepository.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (ETaxisTypeUtils.Equals(channelInfo.DefaultTaxisType, ETaxisType.OrderByTaxis))
                {
                    isUp = !isUp;
                }

                if (isUp == false)
                {
                    channelContentIds.Reverse();
                }

                foreach (var channelContentId in channelContentIds)
                {
                    var contentChannelInfo = await ChannelManager.GetChannelAsync(siteId, channelContentId.ChannelId);
                    var tableName = await ChannelManager.GetTableNameAsync(site, contentChannelInfo);
                    var contentInfo = await DataProvider.ContentRepository.GetAsync(site, contentChannelInfo, channelContentId.Id);
                    if (contentInfo == null) continue;

                    var isTop = contentInfo.Top;
                    for (var i = 1; i <= taxis; i++)
                    {
                        if (isUp)
                        {
                            if (await DataProvider.ContentRepository.SetTaxisToUpAsync(tableName, channelContentId.ChannelId, channelContentId.Id, isTop) == false)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (await DataProvider.ContentRepository.SetTaxisToDownAsync(tableName, channelContentId.ChannelId, channelContentId.Id, isTop) == false)
                            {
                                break;
                            }
                        }
                    }
                }

                foreach (var distinctChannelId in channelContentIds.Select(x => x.ChannelId).Distinct())
                {
                    await CreateManager.TriggerContentChangedEventAsync(siteId, distinctChannelId);
                }

                await request.AddSiteLogAsync(siteId, channelId, 0, "对内容排序", string.Empty);

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
