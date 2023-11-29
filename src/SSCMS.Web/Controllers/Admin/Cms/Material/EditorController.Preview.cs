using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class EditorController
    {
        [HttpPost, Route(RoutePreview)]
        public async Task<ActionResult<BoolResult>> Preview([FromBody] PreviewRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                // string mediaId;
                // (success, mediaId, errorMessage) = await _wxManager.DraftAddAsync(token, request.MessageId);

                var mediaId = await _wxManager.PushMaterialAsync(token, MaterialType.Message, request.MessageId);
                if (string.IsNullOrEmpty(mediaId))
                {
                    return this.Error("操作失败，素材未能上传");
                }

                if (success)
                {
                    foreach (var wxName in ListUtils.GetStringList(request.WxNames, Constants.Newline))
                    {
                        (success, errorMessage) = await _wxManager.PreviewAsync(token, mediaId, StringUtils.Trim(wxName));

                        // var mediaId = await _wxManager.PushMaterialAsync(token, MaterialType.Message, request.MaterialId);
                        // if (string.IsNullOrEmpty(mediaId))
                        // {
                        //     return this.Error("操作失败，素材未能上传");
                        // }
                        // await _wxManager.PreviewSendAsync(token, MaterialType.Message, mediaId, wxName);
                    }
                }
            }

            if (!success)
            {
                return this.Error(errorMessage);
            }

            return new BoolResult
            {
                Value = success
            };
        }
    }
}
