using System;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.CMS.StlParser.Utility;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsIfController : ApiController
    {
        [HttpPost, Route(ActionsIf.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");
                var channelId = body.GetPostInt("channelId");
                var contentId = body.GetPostInt("contentId");
                var templateId = body.GetPostInt("templateId");
                var ajaxDivId = PageUtils.FilterSqlAndXss(body.GetPostString("ajaxDivId"));
                var pageUrl = TranslateUtils.DecryptStringBySecretKey(body.GetPostString("pageUrl"));
                var testType = PageUtils.FilterSqlAndXss(body.GetPostString("testType"));
                var testValue = PageUtils.FilterSqlAndXss(body.GetPostString("testValue"));
                var testOperate = PageUtils.FilterSqlAndXss(body.GetPostString("testOperate"));
                var successTemplate = TranslateUtils.DecryptStringBySecretKey(body.GetPostString("successTemplate"));
                var failureTemplate = TranslateUtils.DecryptStringBySecretKey(body.GetPostString("failureTemplate"));

                var isSuccess = false;
                if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsUserLoggin))
                {
                    isSuccess = body.IsUserLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsAdministratorLoggin))
                {
                    isSuccess = body.IsAdministratorLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeIsUserOrAdministratorLoggin))
                {
                    isSuccess = body.IsUserLoggin || body.IsAdministratorLoggin;
                }
                else if (StringUtils.EqualsIgnoreCase(testType, StlIf.TypeUserGroup))
                {
                    if (body.IsUserLoggin && body.UserInfo.GroupId > 0)
                    {
                        var groupName = UserGroupManager.GetGroupName(body.UserInfo.GroupId);
                        if (!string.IsNullOrEmpty(groupName))
                        {
                            isSuccess = StlIf.TestTypeValue(testOperate, testValue, groupName);
                        }
                    }
                }

                return Ok(new
                {
                    Html = StlUtility.ParseDynamicContent(publishmentSystemId, channelId, contentId, templateId, false, isSuccess ? successTemplate : failureTemplate, pageUrl, 0, ajaxDivId, null, body.UserInfo)
                });
            }
            catch(Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}
