using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerTagController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<IEnumerable<string>>>> Get([FromQuery] ChannelRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Types.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId, Types.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var tagNames = await _contentTagRepository.GetTagNamesAsync(request.SiteId);

            return new ObjectResult<IEnumerable<string>>
            {
                Value = tagNames
            };
        }
    }
}