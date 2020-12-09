using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class EditorController
    {
        [HttpPost, Route(RouteActionsPreview)]
        public async Task<ActionResult<PreviewResult>> Preview([FromBody] PreviewRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                foreach (var wxName in ListUtils.GetStringList(request.WxNames, Constants.Newline))
                {
                    var mediaId = await _wxManager.PushMaterialAsync(token, MaterialType.Message, request.MaterialId);
                    if (string.IsNullOrEmpty(mediaId))
                    {
                        return this.Error("操作失败，素材未能上传");
                    }
                    await _wxManager.PreviewSendAsync(token, MaterialType.Message, mediaId, wxName);
                }
            }

            return new PreviewResult
            {
                Success = success,
                ErrorMessage = errorMessage
            };
        }
    }
}
