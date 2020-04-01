using System;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.API.Controllers.Sys
{
    [OpenApiIgnore]
    public class SysStlActionsDynamicController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsDynamic.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var request = new AuthenticatedRequest();

                var dynamicInfo = DynamicInfo.GetDynamicInfo(request, request.UserInfo);

                return Ok(new
                {
                    Value = true,
                    Html = StlDynamic.ParseDynamicContent(dynamicInfo, dynamicInfo.SuccessTemplate)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
