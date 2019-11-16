using System;
using System.Threading.Tasks;
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
        public async Task<IHttpActionResult> Main()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();

                var dynamicInfo = DynamicInfo.GetDynamicInfo(request, request.User);
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
                    html = await StlDynamic.ParseDynamicContentAsync(dynamicInfo, template);
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
