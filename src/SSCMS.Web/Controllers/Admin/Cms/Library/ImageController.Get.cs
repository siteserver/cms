using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Library
{
    public partial class ImageController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                AuthTypes.SitePermissions.LibraryImage))
            {
                return Unauthorized();
            }

            var isOpen = false;
            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);
            if (account.WxConnected)
            {
                isOpen = true;
            }

            var groups = await _libraryGroupRepository.GetAllAsync(LibraryType.Image);
            var count = await _libraryImageRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _libraryImageRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

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
