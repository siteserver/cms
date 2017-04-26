using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsAddCountHitsController : ApiController
    {
        [HttpGet]
        [Route(ActionsAddContentHits.Route)]
        public void Main(int publishmentSystemId, int channelId, int contentId)
        {
            try
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);
                BaiRongDataProvider.ContentDao.AddHits(tableName, publishmentSystemInfo.Additional.IsCountHits, publishmentSystemInfo.Additional.IsCountHitsByDay, contentId);
            }
            catch
            {
                // ignored
            }

            HttpContext.Current.Response.End();
        }
    }
}
