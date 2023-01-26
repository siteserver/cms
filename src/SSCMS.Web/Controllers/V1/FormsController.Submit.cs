using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class FormsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<FormData>> Submit([FromQuery] SubmitRequest request, [FromBody] FormData formData)
        {
            formData.SiteId = request.SiteId;
            formData.ChannelId = request.ChannelId;
            formData.ContentId = request.ContentId;
            formData.FormId = request.FormId;
            
            var form = await _formRepository.GetAsync(formData.SiteId, formData.FormId);
            if (form == null) return NotFound();
            if (form.IsClosed)
            {
                return this.Error("对不起，表单已被禁用");
            }

            if (form.IsTimeout && (form.TimeToStart > DateTime.Now || form.TimeToEnd < DateTime.Now))
            {
                return this.Error("对不起，表单只允许在规定的时间内提交");
            }

            var isSmsEnabled = await _smsManager.IsSmsEnabledAsync();
            if (isSmsEnabled && form.IsSms)
            {
                var codeCacheKey = GetSmsCodeCacheKey(form.Id, formData.Get<string>("SmsMobile"));
                var code = _cacheManager.Get<int>(codeCacheKey);
                if (code == 0 || TranslateUtils.ToInt(formData.Get<string>("SmsCode")) != code)
                {
                    return this.Error("输入的验证码有误或验证码已超时");
                }
            }

            var styles = await _formRepository.GetTableStylesAsync(form.Id);

            formData.Id = await _formDataRepository.InsertAsync(form, formData);
            await _formManager.SendNotifyAsync(form, styles, formData);

            return formData;
        }
    }
}
