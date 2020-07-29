using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Open
{
    public partial class SendLayerImageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.OpenSend))
            {
                return Unauthorized();
            }

            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Article);
            var count = await _materialImageRepository.GetCountAsync(request.GroupId, request.Keyword);
            var images = await _materialImageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Images = images
            };
        }
    }
}
