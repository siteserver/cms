using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class MessageController
    {
        [HttpDelete, Route(Route)]
        public async Task<ActionResult<BoolResult>> Delete([FromBody] DeleteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            await _materialMessageRepository.DeleteAsync(request.Id);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
