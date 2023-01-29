using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    public partial class FormDataAddController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            var formPermission = MenuUtils.GetFormPermission(request.FormId);
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, formPermission))
            {
                return Unauthorized();
            }

            var form = await _formRepository.GetAsync(request.SiteId, request.FormId);
            if (form == null) return NotFound();

            var styles = await _formRepository.GetTableStylesAsync(form.Id);
            //var value = new Dictionary<string, object>();
            //if (request.DataId > 0)
            //{
            //    var content = await _dataRepository.GetDataInfoAsync(request.DataId);
            //    value = _dataRepository.GetDict(styles, content);
            //}
            var data = await _formDataRepository.GetAsync(request.DataId, form.Id, styles);

            //foreach (var style in styles)
            //{
            //    object value;
            //    if (style.InputType == InputType.CheckBox || style.InputType == InputType.SelectMultiple)
            //    {
            //        value = data != null
            //            ? TranslateUtils.JsonDeserialize<List<string>>(data.Get<string>(style.AttributeName))
            //            : new List<string>();
            //    }
            //    //else if (style.FieldType == InputType.Image.Value)
            //    //{
            //    //    value = data != null
            //    //        ? new List<string> {data.Get<string>(style.Title)}
            //    //        : new List<string>();
            //    //}
            //    else if (style.InputType == InputType.Date || style.InputType == InputType.DateTime)
            //    {
            //        value = data?.Get<DateTime>(style.AttributeName);
            //    }
            //    else
            //    {
            //        value = data?.Get<string>(style.AttributeName);
            //    }

            //    if (value == null)
            //    {
            //        value = string.Empty;
            //    }
            //}

            var site = await _siteRepository.GetAsync(request.SiteId);
            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);

            return new GetResult
            {
                SiteUrl = StringUtils.TrimEndSlash(siteUrl),
                Styles = styles,
                FormData = data
            };
        }
    }
}