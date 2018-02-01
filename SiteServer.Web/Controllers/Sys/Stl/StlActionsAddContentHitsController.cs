using System.Web;
using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Stl;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsAddCountHitsController : ApiController
    {
        [HttpGet]
        [Route(ApiRouteActionsAddContentHits.Route)]
        public void Main(int siteId, int channelId, int contentId)
        {
            try
            {
                var siteInfo = SiteManager.GetSiteInfo(siteId);
                var tableName = ChannelManager.GetTableName(siteInfo, channelId);
                DataProvider.ContentDao.AddHits(tableName, siteInfo.Additional.IsCountHits, siteInfo.Additional.IsCountHitsByDay, contentId);
            }
            catch
            {
                // ignored
            }

            HttpContext.Current.Response.End();
        }
    }
}
