using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormDataAddController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromQuery] int siteId, [FromBody] FormData request)
        {
            var formPermission = MenuUtils.GetFormPermission(request.FormId);
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, formPermission))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(siteId, request.FormId);
            if (form == null) return NotFound();

            //var data = request.DataId > 0
            //    ? await _dataRepository.GetDataAsync(request.DataId)
            //    : new Data
            //    {
            //        FormId = form.Id
            //    };

            var styles = await _formRepository.GetTableStylesAsync(form.Id);

            //foreach (var style in styles)
            //{
            //    var inputType = style.InputType;
            //    if (inputType == InputType.CheckBox ||
            //        style.InputType == InputType.SelectMultiple)
            //    {
            //        var list = request.Get<List<string>>(style.AttributeName);
            //        data.Set(style.AttributeName, ListUtils.ToString(list));
            //    }
            //    else
            //    {
            //        var value = request.Get(style.AttributeName, string.Empty);
            //        data.Set(style.AttributeName, value);
            //    }
            //}

            if (request.Id == 0)
            {
                request.SiteId = siteId;
                request.ChannelId = 0;
                request.ContentId = 0;
                request.Id = await _formDataRepository.InsertAsync(form, request);
                await _formManager.SendNotifyAsync(form, styles, request);
            }
            else
            {
                await _formDataRepository.UpdateAsync(request);
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
