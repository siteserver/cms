using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormSettingsController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] FormRequest request)
        {
            var formPermission = MenuUtils.GetFormPermission(request.FormId);
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, formPermission))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            var styles = await _formRepository.GetTableStylesAsync(form.Id);

            var attributeNames = _formRepository.GetAllAttributeNames(styles);
            attributeNames.Remove(nameof(FormData.IsReplied));
            attributeNames.Remove(nameof(FormData.ReplyDate));
            attributeNames.Remove(nameof(FormData.ReplyContent));
            var administratorSmsNotifyKeys = ListUtils.GetStringList(form.AdministratorSmsNotifyKeys);
            var userSmsNotifyKeys = ListUtils.GetStringList(form.UserSmsNotifyKeys);

            var isSmsEnabled = await _smsManager.IsSmsEnabledAsync();
            var isMailEnabled = await _mailManager.IsMailEnabledAsync();

            if (string.IsNullOrEmpty(form.SuccessMessage))
            {
                form.SuccessMessage = "表单提交成功！";
            }

            return new GetResult
            {
                Form = form,
                Styles = styles,
                AttributeNames = attributeNames,
                AdministratorSmsNotifyKeys = administratorSmsNotifyKeys,
                UserSmsNotifyKeys = userSmsNotifyKeys,
                IsSmsEnabled = isSmsEnabled,
                IsMailEnabled = isMailEnabled
            };
        }
    }
}
