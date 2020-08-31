using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Web.Controllers.Admin;

namespace SSCMS.Web.Controllers.Preview
{
    public partial class PreviewController
    {
        [HttpGet, Route(Constants.RoutePreview)]
        public async Task<FileResult> Get([FromRoute] int siteId, [FromQuery] GetRequest request)
        {
            try
            {
                return await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, siteId, 0, 0, 0, request.PageIndex));
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }
    }
}
