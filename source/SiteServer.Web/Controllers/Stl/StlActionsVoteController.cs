using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsVoteController : ApiController
    {
        [HttpPost, Route(ActionsVote.Route)]
        public IHttpActionResult Main(int publishmentSystemId, int channelId, int contentId)
        {
            try
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

                return Ok(VoteTemplate.GetJsonString(publishmentSystemInfo, channelId, contentId, true, string.Empty));
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
