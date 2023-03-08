using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormSettingsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var formPermission = MenuUtils.GetFormPermission(request.FormId);
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, formPermission))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            form.IsClosed = request.IsClosed;
            form.Title = request.Title;
            form.Description = request.Description;
            form.SuccessMessage = request.SuccessMessage;
            form.SuccessCallback = request.SuccessCallback;
            form.IsReply = request.IsReply;
            form.IsReplyListAll = request.IsReplyListAll;
            form.PageSize = request.PageSize;
            form.IsTimeout = request.IsTimeout;
            form.TimeToStart = request.TimeToStart;
            form.TimeToEnd = request.TimeToEnd;
            form.IsCaptcha = request.IsCaptcha;
            form.IsSms = request.IsSms;
            form.IsAdministratorSmsNotify = request.IsAdministratorSmsNotify;
            form.AdministratorSmsNotifyTplId = request.AdministratorSmsNotifyTplId;
            form.AdministratorSmsNotifyKeys = request.AdministratorSmsNotifyKeys;
            form.AdministratorSmsNotifyMobile = request.AdministratorSmsNotifyMobile;
            form.IsAdministratorMailNotify = request.IsAdministratorMailNotify;
            form.AdministratorMailTitle = request.AdministratorMailTitle;
            form.AdministratorMailNotifyAddress = request.AdministratorMailNotifyAddress;
            form.IsUserSmsNotify = request.IsUserSmsNotify;
            form.UserSmsNotifyTplId = request.UserSmsNotifyTplId;
            form.UserSmsNotifyKeys = request.UserSmsNotifyKeys;
            form.UserSmsNotifyMobileName = request.UserSmsNotifyMobileName;

            await _formRepository.UpdateAsync(form);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
