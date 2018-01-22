using System;
using System.Web.Http;
using SiteServer.Utils;
using SiteServer.CMS.Controllers.Sys.Stl;
using SiteServer.CMS.Plugin;
using SiteServer.CMS.Plugin.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.API.Controllers.Sys.Stl
{
    [RoutePrefix("api")]
    public class StlActionsIfController : ApiController
    {
        [HttpPost, Route(ActionsIf.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var context = new RequestContext();

                var publishmentSystemId = context.GetPostInt("publishmentSystemId");
                var channelId = context.GetPostInt("channelId");
                var contentId = context.GetPostInt("contentId");
                var templateId = context.GetPostInt("templateId");
                var ajaxDivId = PageUtils.FilterSqlAndXss(context.GetPostString("ajaxDivId"));
                var pageUrl = TranslateUtils.DecryptStringBySecretKey(context.GetPostString("pageUrl"));
                var testType = PageUtils.FilterSqlAndXss(context.GetPostString("testType"));
                var testValue = PageUtils.FilterSqlAndXss(context.GetPostString("testValue"));
                var testOperate = PageUtils.FilterSqlAndXss(context.GetPostString("testOperate"));
                var successTemplate = TranslateUtils.DecryptStringBySecretKey(context.GetPostString("successTemplate"));
                var failureTemplate = TranslateUtils.DecryptStringBySecretKey(context.GetPostString("failureTemplate"));

                var isSuccess = false;
                if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsUserLoggin))
                {
                    isSuccess = context.IsUserLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsAdministratorLoggin))
                {
                    isSuccess = context.IsAdminLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsUserOrAdministratorLoggin))
                {
                    isSuccess = context.IsUserLoggin || context.IsAdminLoggin;
                }

                return Ok(new
                {
                    Html = StlUtility.ParseDynamicContent(publishmentSystemId, channelId, contentId, templateId, false, isSuccess ? successTemplate : failureTemplate, pageUrl, 0, ajaxDivId, null, context.UserInfo)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
