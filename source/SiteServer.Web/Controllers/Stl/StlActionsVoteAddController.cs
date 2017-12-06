using System;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.StlTemplates;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsVoteAddController : ApiController
    {
        [HttpPost, Route(ActionsVoteAdd.Route)]
        public void Main(int publishmentSystemId, int nodeId, int contentId)
        {
            var body = new RequestBody();

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            try
            {
                var contentInfo = DataProvider.VoteContentDao.GetContentInfo(publishmentSystemInfo, contentId);
                if ((contentInfo.EndDate - DateTime.Now).Seconds <= 0)
                {
                    throw new Exception("对不起，投票已经结束");
                }
                var cookieName = DataProvider.VoteOperationDao.GetCookieName(publishmentSystemId, nodeId, contentId);
                if (CookieUtils.IsExists(cookieName))
                {
                    throw new Exception("对不起，不能重复投票");
                }

                var optionIdArrayList = TranslateUtils.StringCollectionToIntList(HttpContext.Current.Request.Form["voteOption_" + contentId]);
                foreach (int optionId in optionIdArrayList)
                {
                    DataProvider.VoteOptionDao.AddVoteNum(optionId);
                }
                DataProvider.VoteOperationDao.Insert(new VoteOperationInfo(0, publishmentSystemId, nodeId, contentId, PageUtils.GetIpAddress(), body.UserName, DateTime.Now));

                HttpContext.Current.Response.Write(VoteTemplate.GetCallbackScript(publishmentSystemInfo, nodeId, contentId, true, string.Empty));
                CookieUtils.SetCookie(cookieName, true.ToString(), DateTime.MaxValue);
            }
            catch (Exception ex)
            {
                //HttpContext.Current.Response.Write(VoteTemplate.GetCallbackScript(publishmentSystemInfo, nodeId, contentId, false, ex.Message));
                HttpContext.Current.Response.Write(VoteTemplate.GetCallbackScript(publishmentSystemInfo, nodeId, contentId, false, "程序出错。"));
            }

            HttpContext.Current.Response.End();
        }
    }
}
