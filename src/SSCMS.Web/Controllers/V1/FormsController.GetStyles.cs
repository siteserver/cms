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
        [HttpGet, Route(RouteStyles)]
        public async Task<ActionResult<StylesResult>> GetStyles([FromQuery] FormRequest request)
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

            var styles = await _formRepository.GetTableStylesAsync(form.Id);

            var formData = await _formDataRepository.GetAsync(0, form.Id, styles);
            var isSmsEnabled = await _smsManager.IsSmsEnabledAsync();

            var site = await _siteRepository.GetAsync(request.SiteId);
            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);

            return new StylesResult
            {
                SiteUrl = siteUrl,
                Styles = styles,
                Title = form.Title,
                Description = form.Description,
                SuccessMessage = !string.IsNullOrEmpty(form.SuccessMessage) ? form.SuccessMessage : "表单提交成功！",
                SuccessCallback = form.SuccessCallback,
                IsSms = isSmsEnabled && form.IsSms,
                IsCaptcha = form.IsCaptcha,
                FormData = formData
            };
        }
    }
}
