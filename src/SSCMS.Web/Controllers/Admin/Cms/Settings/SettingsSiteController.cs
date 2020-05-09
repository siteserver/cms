using System.Collections.Generic;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsSiteController : ControllerBase
    {
        private const string Route = "cms/settings/settingsSite";

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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.SettingsSite))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var styles = new List<InputStyle>();
            foreach (var style in await _tableStyleRepository.GetSiteStyleListAsync(request.SiteId))
            {
                styles.Add(new InputStyle(style));

                if (style.InputType == InputType.Image || 
                    style.InputType == InputType.Video ||
                    style.InputType == InputType.File)
                {
                    site.Set(ColumnsManager.GetCountName(style.AttributeName), site.Get(ColumnsManager.GetCountName(style.AttributeName), 0));
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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.SettingsSite))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var styles = await _tableStyleRepository.GetSiteStyleListAsync(request.SiteId);

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
                    for (var n = 1; n <= count; n++)
                    {
                        site.Set(ColumnsManager.GetExtendName(style.AttributeName, n), request.Get(ColumnsManager.GetExtendName(style.AttributeName, n), string.Empty));
                    }
                }
                else if (inputType == InputType.CheckBox || 
                    style.InputType == InputType.SelectMultiple)
                {
                    var list = request.Get<IEnumerable<object>>(style.AttributeName);
                    site.Set(style.AttributeName, Utilities.ToString(list));
                }
                else
                {
                    var value = request.Get(style.AttributeName, string.Empty);
                    site.Set(style.AttributeName, value);
                }

                if (style.IsFormatString)
                {
                    var formatStrong = request.Get($"{style.AttributeName}_formatStrong", false);
                    var formatEm = request.Get($"{style.AttributeName}_formatEM", false);
                    var formatU = request.Get($"{style.AttributeName}_formatU", false);
                    var formatColor = request.Get($"{style.AttributeName}_formatColor", string.Empty);
                    var formatString = ContentUtility.GetTitleFormatString(formatStrong, formatEm, formatU, formatColor);

                    site.Set(ColumnsManager.GetFormatStringAttributeName(style.AttributeName), formatString);
                }
            }

            site.SiteName = request.SiteName;
            site.ImageUrl = request.ImageUrl;
            site.Keywords = request.Keywords;
            site.Description = request.Description;
            
            await _siteRepository.UpdateAsync(site);

            await _authManager.AddSiteLogAsync(request.SiteId, "修改站点设置");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
