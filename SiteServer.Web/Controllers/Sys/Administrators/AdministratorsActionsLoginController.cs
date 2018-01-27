using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Administrators;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Sys.Administrators
{
    [RoutePrefix("api")]
    public class AdministratorsActionsLoginController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsLogin.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var request = new Request();
                var account = request.GetPostString("account");
                var password = request.GetPostString("password");
                if (string.IsNullOrEmpty(account) || string.IsNullOrEmpty(password))
                {
                    return Unauthorized();
                }

                string userName;
                string errorMessage;
                if (!DataProvider.AdministratorDao.ValidateAccount(account, password, out userName, out errorMessage))
                {
                    LogUtils.AddAdminLog(userName, "后台管理员登录失败");
                    DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
                    return Unauthorized();
                }

                DataProvider.AdministratorDao.UpdateLastActivityDateAndCountOfLogin(userName);
                request.AdminLogin(userName);

                return Ok(new
                {
                    UserName = userName
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
