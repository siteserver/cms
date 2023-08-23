using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormListController
    {
        [HttpPost, Route(RouteOrder)]
        public async Task<ActionResult<OrderResult>> Order([FromBody] OrderRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormList))
            {
                return Unauthorized();
            }

            for (int i = 0; i < request.Rows; i++)
            {
                if (request.IsUp)
                {
                     await _formRepository.UpdateTaxisToUpAsync(request.SiteId, request.FormId);
                }
                else
                {
                    await _formRepository.UpdateTaxisToDownAsync(request.SiteId, request.FormId);
                }
            }

            var forms = await _formRepository.GetFormsAsync(request.SiteId);

            return new OrderResult
            {
                Forms = forms
            };
        }
    }
}
