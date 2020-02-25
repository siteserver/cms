using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Cms.Settings
{
    [Route("admin/cms/settings/settingsCreateRuleLayerSet")]
    public partial class SettingsCreateRuleLayerSetController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public SettingsCreateRuleLayerSetController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<List<KeyValuePair<string, string>>>>> Get([FromQuery] GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            var dict = request.IsChannel
                ? await PathUtility.ChannelFilePathRules.GetDictionaryAsync(site, request.ChannelId)
                : await PathUtility.ContentFilePathRules.GetDictionaryAsync(site, request.ChannelId);
            var list = new List<KeyValuePair<string, string>>();

            foreach (var rule in dict)
            {
                list.Add(new KeyValuePair<string, string>(rule.Key, rule.Value));
            }

            return new ObjectResult<List<KeyValuePair<string, string>>>
            {
                Value = list
            };
        }
    }
}