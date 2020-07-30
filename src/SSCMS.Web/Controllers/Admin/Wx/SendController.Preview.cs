using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Extensions;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class SendController
    {
        [HttpPost, Route(RouteActionsPreview)]
        public async Task<ActionResult<BoolResult>> Preview([FromBody] PreviewRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (!success)
            {
                return this.Error(errorMessage);
            }

            if (request.MaterialType == MaterialType.Text)
            {
                foreach (var wxName in ListUtils.GetStringList(request.WxNames, Constants.Newline))
                {
                    await _wxManager.SendPreviewAsync(token, request.MaterialType, request.Text, wxName);
                }
            }
            else
            {
                var mediaId = await _wxManager.PushMaterialAsync(token, request.MaterialType, request.MaterialId);

                foreach (var wxName in ListUtils.GetStringList(request.WxNames, Constants.Newline))
                {
                    await _wxManager.SendPreviewAsync(token, request.MaterialType, mediaId, wxName);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
