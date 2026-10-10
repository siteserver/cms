using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class FormsController
    {
        [HttpPost, Route(RouteSendSms)]
        public async Task<ActionResult<BoolResult>> SendSms([FromQuery] FormRequest formRequest, [FromBody] SendSmsRequest request)
        {
            Form form = null;
            if (formRequest.FormId > 0)
            {
                form = await _formRepository.GetAsync(formRequest.SiteId, formRequest.FormId);
            }
            else if (!string.IsNullOrEmpty(formRequest.FormName))
            {
                form = await _formRepository.GetByTitleAsync(formRequest.SiteId, formRequest.FormName);
            }

            if (form == null) 
            {
                return this.Error(Constants.ErrorNotFound);
            }
            if (form.IsClosed)
            {
                return this.Error("对不起，表单已被禁用");
            }
            if (form.IsTimeout && (form.TimeToStart > DateTime.Now || form.TimeToEnd < DateTime.Now))
            {
                return this.Error("对不起，表单只允许在规定的时间内提交");
            }

            var isSmsEnabled = await _smsManager.IsSmsEnabledAsync();
            if (!isSmsEnabled || !form.IsSms)
            {
                return this.Error("对不起，表单短信验证功能已被禁用");
            }

            if (string.IsNullOrEmpty(request?.Mobile)) return this.Error("请输入手机号码");
            var codeKey = GetSmsCodeCacheKey(form.Id, request.Mobile);
            var (allowed, limitMessage) = SmsCodeGuard.TryBeginSend(_cacheManager, codeKey, PageUtils.GetIpAddress(Request));
            if (!allowed) return this.Error(limitMessage);

            var code = SmsCodeGuard.NewCode();
            var (success, _) =
                await _smsManager.SendSmsAsync(request.Mobile, SmsCodeType.Authentication, code);
            if (!success)
            {
                return this.Error(SmsCodeGuard.ErrorSendFailed);
            }

            SmsCodeGuard.SaveCode(_cacheManager, codeKey, code);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
