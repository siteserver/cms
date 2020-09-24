using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialController
    {
        [HttpGet, Route(RouteId)]
        public async Task<ActionResult<GetSpecialResult>> GetSpecial(int siteId, int specialId)
        {
            if (!await _authManager.HasSitePermissionsAsync(siteId,
                Types.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            Special special = null;
            if (specialId > 0)
            {
                special = await _specialRepository.GetSpecialAsync(siteId, specialId);
            }

            return new GetSpecialResult
            {
                Special = special,
                Guid = StringUtils.GetShortGuid(false),
            };
        }
    }
}