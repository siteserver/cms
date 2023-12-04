using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class FormsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<FormData>> Submit([FromQuery] SubmitRequest request, [FromBody] FormData formData)
        {
            if (!await _accessTokenRepository.IsScopeAsync(_authManager.ApiToken, Constants.ScopeForms))
            {
                return Unauthorized();
            }
            
            Form form = null;
            if (request.FormId > 0)
            {
                form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            }
            else if (!string.IsNullOrEmpty(request.FormName))
            {
                form = await _formRepository.GetByTitleAsync(request.SiteId, request.FormName);
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

            formData.SiteId = request.SiteId;
            formData.ChannelId = request.ChannelId;
            formData.ContentId = request.ContentId;
            formData.FormId = form.Id;

            formData.Id = await _formDataRepository.InsertAsync(form, formData);
            await _formManager.SendNotifyAsync(form, styles, formData);

            return formData;
        }
    }
}
