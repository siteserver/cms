using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class VideoController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.MaterialVideo))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);

            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Video);
            var count = await _materialVideoRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _materialVideoRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items,
                SiteType = site.SiteType
            };
        }
    }
}