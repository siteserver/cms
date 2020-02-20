using System;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.API.Context;
using SiteServer.CMS.Api.Sys.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.Model;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.API.Controllers.Sys
{
    
    public class SysStlActionsDynamicController : ApiController
    {
        [HttpPost, Route(ApiRouteActionsDynamic.Route)]
        public async Task<IHttpActionResult> Main()
        {
            try
            {
                var request = await AuthenticatedRequest.GetAuthAsync();

                var dynamicInfo = DynamicInfo.GetDynamicInfo(request.GetPostString("value"), request.GetPostInt("page"), request.User, Request.RequestUri.PathAndQuery);

                return Ok(new
                {
                    Value = true,
                    Html = await StlDynamic.ParseDynamicContentAsync(dynamicInfo, dynamicInfo.SuccessTemplate)
                });
            }
            catch(Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
