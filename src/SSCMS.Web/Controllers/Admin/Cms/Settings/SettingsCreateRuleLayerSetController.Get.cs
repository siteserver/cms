using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils.PathRules;
using SSCMS.Dto;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    public partial class SettingsCreateRuleLayerSetController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ObjectResult<List<KeyValuePair<string, string>>>>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                MenuUtils.SitePermissions.SettingsCreateRule))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error("无法确定内容对应的站点");

            Dictionary<string, string> dict;
            if (request.IsChannel)
            {
                var rules = new ChannelFilePathRules(_pathManager, _databaseManager);
                dict = await rules.GetDictionaryAsync(request.ChannelId);
            }
            else
            {
                var rules = new ContentFilePathRules(_pathManager, _databaseManager);
                dict = await rules.GetDictionaryAsync(site, request.ChannelId);
            }
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