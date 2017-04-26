using System;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsAddTrackerCountController : ApiController
    {
        [HttpGet]
        [Route(ActionsAddTrackerCount.Route)]
        public void Main(int publishmentSystemId, int channelId, int contentId)
        {
            var request = HttpContext.Current.Request;
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var isFirstAccess = TranslateUtils.ToBool(request.QueryString["isFirstAccess"]);
            var location = request.QueryString["location"];
            var referrer = request.QueryString["referrer"];
            var lastAccessDateTime = request.QueryString["lastAccessDateTime"];

            if (publishmentSystemInfo != null && publishmentSystemInfo.Additional.IsTracker)
            {
                var ipAddress = request.ServerVariables["REMOTE_ADDR"];
                var operatingSystem = request.Browser.Platform;
                var browser = request.Browser.Browser + " " + request.Browser.MajorVersion;

                var trackingInfo = new TrackingInfo
                {
                    PublishmentSystemId = publishmentSystemId,
                    TrackerType = isFirstAccess ? ETrackerType.Site : ETrackerType.Page,
                    LastAccessDateTime = !string.IsNullOrEmpty(lastAccessDateTime) ? Converter.ToDateTime(lastAccessDateTime) : DateTime.Now,
                    PageUrl = location,
                    PageNodeId = channelId,
                    PageContentId = contentId,
                    Referrer = referrer,
                    IpAddress = ipAddress,
                    OperatingSystem = operatingSystem,
                    Browser = browser,
                    AccessDateTime = DateTime.Now
                };

                try
                {
                    DataProvider.TrackingDao.Insert(trackingInfo);
                }
                catch
                {
                    // ignored
                }
            }

            HttpContext.Current.Response.End();
        }
    }
}
