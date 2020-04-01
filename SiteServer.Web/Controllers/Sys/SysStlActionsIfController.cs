using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    [OpenApiIgnore]
    public class SysStlActionsIfController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsIf.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var dynamicInfo = DynamicInfo.GetDynamicInfo(request, request.UserInfo);
                var ifInfo = TranslateUtils.JsonDeserialize<DynamicInfo.IfInfo>(dynamicInfo.ElementValues);

                var isSuccess = false;
                var html = string.Empty;

                if (ifInfo != null)
                {
                    if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsUserLoggin))
                    {
                        isSuccess = request.IsUserLoggin;
                    }
                    else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsAdministratorLoggin))
                    {
                        isSuccess = request.IsAdminLoggin;
                    }
                    else if (StringUtils.EqualsIgnoreCase(ifInfo.Type, StlIf.TypeIsUserOrAdministratorLoggin))
                    {
                        isSuccess = request.IsUserLoggin || request.IsAdminLoggin;
                    }

                    var template = isSuccess ? dynamicInfo.SuccessTemplate : dynamicInfo.FailureTemplate;
                    html = StlDynamic.ParseDynamicContent(dynamicInfo, template);
                }

                return Ok(new
                {
                    Value = isSuccess,
                    Html = html
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
