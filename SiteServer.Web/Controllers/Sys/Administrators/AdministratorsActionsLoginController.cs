using System;
using System.Web.Http;
using SiteServer.CMS.Controllers.Sys.Administrators;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Model;

namespace SiteServer.API.Controllers.Sys.Administrators
{
    [RoutePrefix("api")]
    public class AdministratorsActionsLoginController : ApiController
    {
        [HttpPost, Route(ActionsLogin.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var context = new RequestContext();
                var account = context.GetPostString("account");
                var password = context.GetPostString("password");
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
                context.AdminLogin(userName);

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
