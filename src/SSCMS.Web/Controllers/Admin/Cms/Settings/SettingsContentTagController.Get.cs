using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsContentTagController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsContentTag))
            {
                return Unauthorized();
            }

            var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);
            var pageTagNames = new List<string>();
            var total = tagNames.Count;
            if (total > 0)
            {
                var offset = request.PerPage * (request.Page - 1);
                var limit = request.PerPage;
                pageTagNames = tagNames.Skip(offset).Take(limit).ToList();
            }

            return new GetResult
            {
                Total = total,
                TagNames = pageTagNames
            };
        }
    }
}