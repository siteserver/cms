using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormListController
    {
        [HttpPost, Route(RouteDown)]
        public async Task<ActionResult<TaxisResult>> Down([FromBody] TaxisRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormList))
            {
                return Unauthorized();
            }

            await _formRepository.UpdateTaxisToDownAsync(request.SiteId, request.FormId);

            var forms = await _formRepository.GetFormsAsync(request.SiteId);

            return new TaxisResult
            {
                Forms = forms
            };
        }
    }
}
