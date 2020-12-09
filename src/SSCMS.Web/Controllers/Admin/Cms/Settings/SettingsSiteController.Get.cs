using System.Threading.Tasks;
using Datory;
using Microsoft.AspNetCore.Mvc;
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
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.SettingsSite))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var entity = new Entity();
            var styles = await GetInputStylesAsync(request.SiteId);
            foreach (var style in styles)
            {
                if (style.InputType == InputType.Image ||
                    style.InputType == InputType.Video ||
                    style.InputType == InputType.File)
                {
                    var count = site.Get(ColumnsManager.GetCountName(style.AttributeName), 0);
                    entity.Set(ColumnsManager.GetCountName(style.AttributeName), count);
                    for (var n = 0; n <= count; n++)
                    {
                        var extendName = ColumnsManager.GetExtendName(style.AttributeName, n);
                        entity.Set(extendName, site.Get(extendName));
                    }
                }
                else if (style.InputType == InputType.CheckBox ||
                         style.InputType == InputType.SelectMultiple)
                {
                    var list = ListUtils.GetStringList(site.Get(style.AttributeName,
                        string.Empty));
                    entity.Set(style.AttributeName, list);
                }
                else
                {
                    entity.Set(style.AttributeName, site.Get(style.AttributeName));
                }
            }

            var siteUrl = await _pathManager.GetSiteUrlAsync(site, true);

            return new GetResult
            {
                SiteUrl = StringUtils.TrimEndSlash(siteUrl),
                Entity = entity,
                Styles = styles
            };
        }
    }
}