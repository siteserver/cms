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
        [HttpGet, Route(Constants.RoutePreviewFile)]
        public async Task<FileResult> GetFile([FromRoute] int siteId, [FromRoute] int fileTemplateId, [FromQuery] GetFileRequest request)
        {
            try
            {
                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, siteId, 0, 0, fileTemplateId, request.PageIndex));
                return response;
            }
            catch (Exception ex)
            {
                HttpContext.Response.Redirect(_pathManager.GetAdminUrl(ErrorController.Route) + "/?message=" + HttpUtility.UrlPathEncode(ex.Message));
            }

            return null;
        }
    }
}
