using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Enumerations;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Home
{
    
    [RoutePrefix("home/contentsLayerTaxis")]
    public class HomeContentsLayerTaxisController : ApiController
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
                var contentIdList = TranslateUtils.StringCollectionToIntList(request.GetPostString("contentIds"));
                var isUp = request.GetPostBool("isUp");
                var taxis = request.GetPostInt("taxis");

                if (!request.IsUserLoggin ||
                    !await request.UserPermissionsImpl.HasChannelPermissionsAsync(siteId, channelId,
                        Constants.ChannelPermissions.ContentEdit))
                {
                    return Unauthorized();
                }

                var site = await DataProvider.SiteDao.GetAsync(siteId);
                if (site == null) return BadRequest("无法确定内容对应的站点");

                var channelInfo = await ChannelManager.GetChannelAsync(siteId, channelId);
                if (channelInfo == null) return BadRequest("无法确定内容对应的栏目");

                if (ETaxisTypeUtils.Equals(channelInfo.DefaultTaxisType, ETaxisType.OrderByTaxis))
                {
                    isUp = !isUp;
                }

                if (isUp == false)
                {
                    contentIdList.Reverse();
                }

                var tableName = await ChannelManager.GetTableNameAsync(site, channelInfo);

                foreach (var contentId in contentIdList)
                {
                    var contentInfo = await DataProvider.ContentDao.GetAsync(site, channelInfo, contentId);
                    if (contentInfo == null) continue;

                    var isTop = contentInfo.Top;
                    for (var i = 1; i <= taxis; i++)
                    {
                        if (isUp)
                        {
                            if (await DataProvider.ContentDao.SetTaxisToUpAsync(tableName, channelId, contentId, isTop) == false)
                            {
                                break;
                            }
                        }
                        else
                        {
                            if (await DataProvider.ContentDao.SetTaxisToDownAsync(tableName, channelId, contentId, isTop) == false)
                            {
                                break;
                            }
                        }
                    }
                }

                await CreateManager.TriggerContentChangedEventAsync(siteId, channelId);

                await request.AddSiteLogAsync(siteId, channelId, 0, "对内容排序", string.Empty);

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
