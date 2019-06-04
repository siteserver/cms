using System;
using System.Web;
using System.Web.Http;
using SiteServer.API.Common;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.Sys
{
    public class SysStlActionsDynamicController : ControllerBase
    {
        [HttpPost, Route(ApiRouteActionsDynamic.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var request = GetRequest();

                var dynamicInfo = DynamicInfo.GetDynamicInfo(request, request.UserInfo);

                return Ok(new
                {
                    Value = true,
                    Html = StlDynamic.ParseDynamicContent(dynamicInfo, dynamicInfo.SuccessTemplate)
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
