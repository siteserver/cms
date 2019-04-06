using System;
using System.Web.Http;
using SiteServer.CMS.Core.RestRoutes.Sys.Stl;
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
                var rest = new Rest(Request);

                var siteId = rest.GetPostInt("siteId");
                var channelId = rest.GetPostInt("channelId");
                var contentId = rest.GetPostInt("contentId");
                var templateId = rest.GetPostInt("templateId");
                var ajaxDivId = AttackUtils.FilterSqlAndXss(rest.GetPostString("ajaxDivId"));
                var pageUrl = TranslateUtils.DecryptStringBySecretKey(rest.GetPostString("pageUrl"));
                var testType = AttackUtils.FilterSqlAndXss(rest.GetPostString("testType"));
                //var testValue = PageUtils.FilterSqlAndXss(rest.GetPostString("testValue"));
                //var testOperate = PageUtils.FilterSqlAndXss(rest.GetPostString("testOperate"));
                var successTemplate = TranslateUtils.DecryptStringBySecretKey(rest.GetPostString("successTemplate"));
                var failureTemplate = TranslateUtils.DecryptStringBySecretKey(rest.GetPostString("failureTemplate"));

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

                return Ok(new
                {
                    Html = StlDynamic.ParseDynamicContent(siteId, channelId, contentId, templateId, false, isSuccess ? successTemplate : failureTemplate, pageUrl, 0, ajaxDivId, null, rest.UserInfo)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
