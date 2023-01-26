using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Repositories;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormStylesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] FormRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormList))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            var styles = await _formRepository.GetTableStylesAsync(form.Id);

            var inputTypes = ListUtils.GetSelects<InputType>();

            return new GetResult
            {
                InputTypes = inputTypes,
                TableName = FormDataRepository.TABLE_NAME,
                RelatedIdentities = ListUtils.ToString(_formRepository.GetRelatedIdentities(form.Id)),
                Styles = styles,
            };
        }
    }
}
