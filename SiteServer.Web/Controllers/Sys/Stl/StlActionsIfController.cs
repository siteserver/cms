using System;
using System.Web.Http;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsIfController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsIf.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var request = new AuthRequest();

                var siteId = request.GetPostInt("siteId");
                var channelId = request.GetPostInt("channelId");
                var contentId = request.GetPostInt("contentId");
                var templateId = request.GetPostInt("templateId");
                var ajaxDivId = PageUtils.FilterSqlAndXss(request.GetPostString("ajaxDivId"));
                var pageUrl = TranslateUtils.DecryptStringBySecretKey(request.GetPostString("pageUrl"));
                var testType = PageUtils.FilterSqlAndXss(request.GetPostString("testType"));
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
