using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.V1
{
    public partial class FormsController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<FormData>> Submit([FromQuery] SubmitRequest request, [FromBody] FormData formData)
        {
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
                var codeKey = GetSmsCodeCacheKey(form.Id, formData.Get<string>("SmsMobile"));
                var (isCodeValid, codeError) = SmsCodeGuard.Verify(_cacheManager, codeKey, formData.Get<string>("SmsCode"), PageUtils.GetIpAddress(Request));
                if (!isCodeValid)
                {
                    return this.Error(codeError);
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
