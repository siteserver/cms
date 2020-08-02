using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class ReplyAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.WxReply))
            {
                return Unauthorized();
            }

            if (request.RuleId > 0)
            {

            }
            else
            {

            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
