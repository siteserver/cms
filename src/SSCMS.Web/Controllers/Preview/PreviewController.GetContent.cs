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
        [HttpGet, Route(Constants.RoutePreviewContent)]
        public async Task<FileResult> GetContent([FromRoute] int siteId, [FromRoute] int channelId, [FromRoute] int contentId, [FromQuery] GetContentRequest request)
        {
            try
            {
                var response = await GetResponseMessageAsync(await VisualInfo.GetInstanceAsync(_pathManager, _databaseManager, siteId, channelId, contentId, 0, request.PageIndex, request.IsPreview));
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
