using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsSiteController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsSite))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var styles = await GetInputStylesAsync(request.SiteId);

            foreach (var style in styles)
            {
                var inputType = style.InputType;
                if (inputType == InputType.TextEditor)
                {
                    var value = request.Get(style.AttributeName, string.Empty);
                    value = await _pathManager.EncodeTextEditorAsync(site, value);
                    value = UEditorUtils.TranslateToStlElement(value);
                    site.Set(style.AttributeName, value);
                }
                else if (inputType == InputType.Image ||
                         inputType == InputType.Video ||
                         inputType == InputType.File)
                {
                    var count = request.Get(ColumnsManager.GetCountName(style.AttributeName), 0);
                    site.Set(ColumnsManager.GetCountName(style.AttributeName), count);
                    for (var n = 0; n <= count; n++)
                    {
                        site.Set(ColumnsManager.GetExtendName(style.AttributeName, n), request.Get(ColumnsManager.GetExtendName(style.AttributeName, n), string.Empty));
                    }
                }
                else if (inputType == InputType.CheckBox ||
                    style.InputType == InputType.SelectMultiple)
                {
                    var list = request.Get<List<object>>(style.AttributeName);
                    site.Set(style.AttributeName, ListUtils.ToString(list));
                }
                else
                {
                    var value = request.Get(style.AttributeName, string.Empty);
                    site.Set(style.AttributeName, value);
                }
            }

            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改站点设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}