using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormDataLayerReplyController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.FormList))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            var data = await _formDataRepository.GetAsync(request.DataId);
            if (data == null) return NotFound();

            data.ReplyContent = request.ReplyContent;
            await _formDataRepository.ReplyAsync(form, data);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
