using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ImageController
    {
        [HttpPost, Route(RoutePull)]
        public async Task<ActionResult<BoolResult>> Pull([FromBody] PullRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialImage))
            {
                return Unauthorized();
            }

            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);

            if (success)
            {
                await _wxManager.PullMaterialAsync(token, MaterialType.Image, request.GroupId);
            }
            else
            {
                return this.Error(errorMessage);
            }

            return new BoolResult
            {
                Value = success,
            };
        }
    }
}
