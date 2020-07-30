using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Wx
{
    public partial class SendLayerMessageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.WxSend))
            {
                return Unauthorized();
            }

            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Article);
            var count = await _materialMessageRepository.GetCountAsync(request.GroupId, request.Keyword);
            var messages = await _materialMessageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Messages = messages
            };
        }
    }
}
