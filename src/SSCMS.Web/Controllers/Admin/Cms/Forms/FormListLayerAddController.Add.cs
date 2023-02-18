using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Repositories;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormListLayerAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Add([FromBody] AddRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormList))
            {
                return Unauthorized();
            }

            var form = new Form
            {
                SiteId = request.SiteId,
                Title = request.Title,
                Description = request.Description,
                ListAttributeNames = FormRepository.DefaultListAttributeNames
            };

            await _formRepository.InsertAsync(form);

            await _formRepository.CreateDefaultStylesAsync(form);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
