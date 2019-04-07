using System;
using System.Web.Http;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Plugin;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsIfController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsIf.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var rest = Request.GetAuthenticatedRequest();

                var siteId = Request.GetPostInt("siteId");
                var channelId = Request.GetPostInt("channelId");
                var contentId = Request.GetPostInt("contentId");
                var templateId = Request.GetPostInt("templateId");
                var ajaxDivId = AttackUtils.FilterSqlAndXss(Request.GetPostString("ajaxDivId"));
                var pageUrl = TranslateUtils.DecryptStringBySecretKey(Request.GetPostString("pageUrl"));
                var testType = AttackUtils.FilterSqlAndXss(Request.GetPostString("testType"));
                //var testValue = PageUtils.FilterSqlAndXss(Request.GetPostString("testValue"));
                //var testOperate = PageUtils.FilterSqlAndXss(Request.GetPostString("testOperate"));
                var successTemplate = TranslateUtils.DecryptStringBySecretKey(Request.GetPostString("successTemplate"));
                var failureTemplate = TranslateUtils.DecryptStringBySecretKey(Request.GetPostString("failureTemplate"));

                var isSuccess = false;
                if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsUserLoggin))
                {
                    isSuccess = rest.IsUserLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsAdministratorLoggin))
                {
                    isSuccess = rest.IsAdminLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsUserOrAdministratorLoggin))
                {
                    isSuccess = rest.IsUserLoggin || rest.IsAdminLoggin;
                }

                var userInfo = UserManager.GetUserInfoByUserId(rest.UserId);

                return Ok(new
                {
                    Html = StlDynamic.ParseDynamicContent(siteId, channelId, contentId, templateId, false, isSuccess ? successTemplate : failureTemplate, pageUrl, 0, ajaxDivId, null, userInfo)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
