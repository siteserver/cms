using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class ImageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.MaterialImage))
            {
                return Unauthorized();
            }

            var isOpen = false;
            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);
            if (account.MpConnected)
            {
                isOpen = true;
            }

            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Image);
            var count = await _materialImageRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _materialImageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items,
                IsOpen = isOpen
            };
        }
    }
}
