using System;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Administrators;
using SiteServer.CMS.Core;
using BaiRong.Core.Text;

namespace SiteServer.API.Controllers.Administrators
{
    [RoutePrefix("api")]
    public class AdministratorsActionsLoginController : ApiController
    {
        [HttpPost, Route(ActionsLogin.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                var account = body.GetPostString("account");
                var password = body.GetPostString("password");
                if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
                {
                    return Unauthorized();
                }

                string userName;
                string errorMessage;
                if (!BaiRongDataProvider.AdministratorDao.ValidateAccount(account, password, out userName, out errorMessage))
                {
                    LogUtils.AddAdminLog(userName, "后台管理员登录失败");
                    BaiRongDataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                    return Unauthorized();
                }

                BaiRongDataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(userName);
                body.AdministratorLogin(userName);
                return Ok(new
                {
                    UserName = userName
                });
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
