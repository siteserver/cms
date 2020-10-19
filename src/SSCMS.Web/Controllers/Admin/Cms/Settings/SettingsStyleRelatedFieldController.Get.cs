using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsStyleRelatedFieldController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<IEnumerable<RelatedField>>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.SettingsStyleRelatedField))
            {
                return Unauthorized();
            }

            return await _relatedFieldRepository.GetRelatedFieldsAsync(request.SiteId);
        }
    }
}
