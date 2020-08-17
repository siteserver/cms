using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsSiteController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] SiteRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.SettingsSite))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var styles = new List<InputStyle>();
            foreach (var style in await _tableStyleRepository.GetSiteStylesAsync(request.SiteId))
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
                    var list = ListUtils.GetStringList(site.Get(style.AttributeName,
                        string.Empty));
                    site.Set(style.AttributeName, list);
                }
            }

            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);

            return new GetResult
            {
                SiteUrl = StringUtils.TrimEndSlash(siteUrl),
                Site = site,
                Styles = styles
            };
        }
    }
}