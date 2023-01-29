using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormTemplatesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormTemplates))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var forms = await _formRepository.GetFormsAsync(request.SiteId);
            var templates = await _formManager.GetFormTemplatesAsync(site);
            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);

            return new GetResult
            {
                Forms = forms,
                Templates = templates,
                SiteUrl = siteUrl,
            };
        }
    }
}
