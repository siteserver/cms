using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class EditorController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<CreateResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.MaterialMessage))
            {
                return Unauthorized();
            }

            var messageId = await _materialMessageRepository.InsertAsync(request.GroupId, string.Empty, request.Items);

            return new CreateResult
            {
                MessageId = messageId
            };
        }
    }
}
