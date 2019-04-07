using System;
using System.Web.Http;
using SiteServer.CMS.Apis;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.CMS.Fx;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Pages.Connect
{
    [RoutePrefix("pages/connect/index")]
    public class PagesIndexController : ApiController
    {
        private const string RouteActionsLogin = "actions/login";
        private const string RouteActionsConnect = "actions/connect";

        [HttpPost, Route(RouteActionsLogin)]
        public IHttpActionResult Login()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var account = Request.GetPostString("account");
                var password = Request.GetPostString("password");

                AdministratorInfo adminInfo;

                if (!DataProvider.Administrator.Validate(account, password, true, out var userName, out var errorMessage))
                {
                    adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                    if (adminInfo != null)
                    {
                        DataProvider.Administrator.UpdateLastActivityDateAndCountOfFailedLogin(adminInfo); // 记录最后登录时间、失败次数+1
                    }
                    return BadRequest(errorMessage);
                }

                adminInfo = AdminManager.GetAdminInfoByUserName(userName);
                DataProvider.Administrator.UpdateLastActivityDateAndCountOfLogin(adminInfo); // 记录最后登录时间、失败次数清零
                if (!AdminManager.IsSuperAdmin(userName))
                {
                    return BadRequest($"管理员{userName}不是超级管理员，请使用超级管理员账号登录");
                }


                var isNightlyUpdate = WebConfigUtils.IsNightlyUpdate;
                var apiPrefix = WebConfigUtils.ApiPrefix;
                var adminDirectory = WebConfigUtils.AdminDirectory;
                var homeDirectory = WebConfigUtils.HomeDirectory;
                var secretKey = WebConfigUtils.SecretKey;
                var cmsVersion = SystemManager.Version;
                var pluginVersion = SystemManager.PluginVersion;
                var apiVersion = SystemManager.ApiVersion;
                var adminName = adminInfo.UserName;
                var adminToken = AdminApi.Instance.GetAccessToken(adminInfo.Id, adminInfo.UserName, TimeSpan.FromDays(3650));

                return Ok(new
                {
                    Value = true,
                    IsNightlyUpdate = isNightlyUpdate,
                    ApiPrefix = apiPrefix,
                    AdminDirectory = adminDirectory,
                    HomeDirectory = homeDirectory,
                    SecretKey = secretKey,
                    CmsVersion = cmsVersion,
                    PluginVersion = pluginVersion,
                    ApiVersion = apiVersion,
                    AdminName = adminName,
                    AdminToken = adminToken
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost, Route(RouteActionsConnect)]
        public IHttpActionResult Connect()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var repositoryOwner = Request.GetPostString("repositoryOwner");
                var repositoryName = Request.GetPostString("repositoryName");
                var repositoryToken = Request.GetPostString("repositoryToken");

                ConfigManager.Instance.RepositoryOwner = repositoryOwner;
                ConfigManager.Instance.RepositoryName = repositoryName;
                ConfigManager.Instance.RepositoryToken = repositoryToken;
                DataProvider.Config.Update(ConfigManager.Instance);

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
