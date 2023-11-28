using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ImageController
    {
        [HttpPost, Route(RouteDelete)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialImage))
            {
                return Unauthorized();
            }

            if (request.Id > 0)
            {
                await _materialImageRepository.DeleteAsync(request.Id);
            }
            else if (request.DataIds != null && request.DataIds.Count > 0)
            {
                foreach (var dataId in request.DataIds)
                {
                    await _materialImageRepository.DeleteAsync(dataId);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
