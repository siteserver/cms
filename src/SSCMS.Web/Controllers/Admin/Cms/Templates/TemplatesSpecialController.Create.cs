using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;
using SSCMS.Configuration;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    public partial class TemplatesSpecialController
    {
        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] SpecialIdRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.Specials))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            await _createManager.CreateSpecialAsync(request.SiteId, request.SpecialId);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}