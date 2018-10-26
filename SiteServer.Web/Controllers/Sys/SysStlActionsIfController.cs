using System;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.StlParser.StlElement;
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
                var request = new RequestImpl();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentId = request.GetPostInt("contentId");
                var templateId = request.GetPostInt("templateId");
                var ajaxDivId = AttackUtils.FilterSqlAndXss(request.GetPostString("ajaxDivId"));
                var pageUrl = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("pageUrl"));
                var testType = AttackUtils.FilterSqlAndXss(request.GetPostString("testType"));
                //var testValue = PageUtils.FilterSqlAndXss(request.GetPostString("testValue"));
                //var testOperate = PageUtils.FilterSqlAndXss(request.GetPostString("testOperate"));
                var successTemplate = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("successTemplate"));
                var failureTemplate = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("failureTemplate"));

                var isSuccess = false;
                if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsUserLoggin))
                {
                    isSuccess = request.IsUserLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsAdministratorLoggin))
                {
                    isSuccess = request.IsAdminLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsUserOrAdministratorLoggin))
                {
                    isSuccess = request.IsUserLoggin || request.IsAdminLoggin;
                }

                return Ok(new
                {
                    Html = StlDynamic.ParseDynamicContent(siteId, channelId, contentId, templateId, false, isSuccess ? successTemplate : failureTemplate, pageUrl, 0, ajaxDivId, null, request.UserInfo)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
