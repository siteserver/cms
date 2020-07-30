using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Wx;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class SendController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.WxSend))
            {
                return Unauthorized();
            }

            IEnumerable<WxUserTag> tags = null;
            MaterialMessage message = null;

            //var success = true;
            //var errorMessage = string.Empty;
            var (success, token, errorMessage) = await _wxManager.GetAccessTokenAsync(request.SiteId);
            if (success)
            {
                var list = await _wxManager.GetUserTags(token);
                tags = list.Select(tag =>
                    new WxUserTag
                    {
                        Id = tag.Id,
                        Name = tag.Name,
                        Count = tag.Count
                    });
                if (request.MessageId > 0)
                {
                    message = await _materialMessageRepository.GetAsync(request.MessageId);
                }
            }

            return new GetResult
            {
                Success = success,
                ErrorMessage = errorMessage,
                Tags = tags,
                Message = message
            };
        }
    }
}
