using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class MessageController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<BoolResult>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            await _materialMessageRepository.UpdateAsync(request.Id, request.GroupId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
