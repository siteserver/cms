using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsSite")]
    public partial class SettingsSiteController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public SettingsSiteController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> GetConfig([FromQuery] SiteRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigSite))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var styles = new List<Style>();
            foreach (var style in await _tableStyleRepository.GetSiteStyleListAsync(request.SiteId))
            {
                styles.Add(new Style
                {
                    Id = style.Id,
                    AttributeName = style.AttributeName,
                    DisplayName = style.DisplayName,
                    InputType = style.InputType.GetValue(),
                    Rules = TranslateUtils.JsonDeserialize<IEnumerable<TableStyleRule>>(style.RuleValues),
                    Items = style.Items
                });

                if (style.InputType == InputType.Image || 
                    style.InputType == InputType.Video ||
                    style.InputType == InputType.File)
                {
                    site.Set(EditorManager.GetCountName(style), site.Get(EditorManager.GetCountName(style), 0));
                }
                else if (style.InputType == InputType.CheckBox || 
                         style.InputType == InputType.SelectMultiple)
                {
                    var list = Utilities.GetStringList(site.Get(style.AttributeName,
                        string.Empty));
                    site.Set(style.AttributeName, list);
                }
            }

            return new GetResult
            {
                Site = site,
                Styles = styles
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody] SubmitRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId, Constants.SitePermissions.ConfigSite))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var styles = await _tableStyleRepository.GetSiteStyleListAsync(request.SiteId);

            foreach (var style in styles)
            {
                var value = request.Get(style.AttributeName, string.Empty);
                var inputType = style.InputType;
                if (inputType == InputType.TextEditor)
                {
                    value = await ContentUtility.TextEditorContentEncodeAsync(_pathManager, site, value);
                    value = UEditorUtils.TranslateToStlElement(value);
                }
                else if (inputType == InputType.Image || 
                         inputType == InputType.Video || 
                         inputType == InputType.File)
                {
                    var count = request.Get(EditorManager.GetCountName(style), 0);
                    site.Set(EditorManager.GetCountName(style), count);
                    for (var n = 1; n <= count; n++)
                    {
                        site.Set(EditorManager.GetExtendName(style, n), request.Get(EditorManager.GetExtendName(style, n), string.Empty));
                    }
                }

                if (inputType == InputType.CheckBox || 
                    style.InputType == InputType.SelectMultiple)
                {
                    var list = request.Get<IEnumerable<object>>(style.AttributeName);
                    site.Set(style.AttributeName, Utilities.ToString(list));
                }
                else
                {
                    site.Set(style.AttributeName, value);
                }

                if (style.IsFormatString)
                {
                    var formatStrong = request.Get($"{style.AttributeName}_formatStrong", false);
                    var formatEm = request.Get($"{style.AttributeName}_formatEM", false);
                    var formatU = request.Get($"{style.AttributeName}_formatU", false);
                    var formatColor = request.Get($"{style.AttributeName}_formatColor", string.Empty);
                    var formatString = ContentUtility.GetTitleFormatString(formatStrong, formatEm, formatU, formatColor);

                    site.Set(ContentAttribute.GetFormatStringAttributeName(style.AttributeName), formatString);
                }

                if (inputType == InputType.Image || inputType == InputType.File || inputType == InputType.Video)
                {
                    var attributeName = ContentAttribute.GetExtendAttributeName(style.AttributeName);
                    site.Set(attributeName, request.Get(attributeName, string.Empty));
                }
            }

            site.SiteName = request.SiteName;
            site.PageSize = request.PageSize;
            site.IsCreateDoubleClick = request.IsCreateDoubleClick;
            await _siteRepository.UpdateAsync(site);

            await auth.AddSiteLogAsync(request.SiteId, "修改站点设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
