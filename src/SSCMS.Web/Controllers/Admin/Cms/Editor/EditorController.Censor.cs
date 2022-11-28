using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;
using System;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPost, Route(RouteCensor)]
        public async Task<ActionResult<CensorResult>> Censor([FromBody] CensorRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId,
                    MenuUtils.ContentPermissions.Add, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            try
            {
                var results = await _censorManager.CensorTextAsync(request.Text);
                if (results.Success)
                {
                    return results;
                }
                else
                {
                    return this.Error(results.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                return this.Error(ex.Message);
            }
        }
    }
}
