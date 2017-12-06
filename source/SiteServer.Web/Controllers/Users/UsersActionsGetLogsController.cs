using System;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Controllers.Users;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersActionsGetLogsController : ApiController
    {
        [HttpPost, Route(ActionsGetLogs.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                if (!body.IsUserLoggin) return Unauthorized();

                var action = body.GetPostString("action");
                var totalNum = TranslateUtils.ToInt(body.GetPostString("totalNum"), 10);
                var list = BaiRongDataProvider.UserLogDao.List(body.UserName, totalNum, action);
                foreach (var logInfo in list)
                {
                    logInfo.Action = EUserActionTypeUtils.GetText(EUserActionTypeUtils.GetEnumType(logInfo.Action));
                }

                return Ok(list);
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
