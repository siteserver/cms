using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.Abstractions.Dto.Result;
using SiteServer.API.Context;
using SiteServer.CMS.Core;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Settings
{
    [RoutePrefix("pages/cms/settings/settingsCreateRuleLayerSet")]
    public partial class PagesSettingsCreateRuleLayerSetController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<ObjectResult<List<KeyValuePair<string, string>>>> Get([FromUri] GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.ConfigCreateRule))
            {
                return Request.Unauthorized<ObjectResult<List<KeyValuePair<string, string>>>>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.BadRequest<ObjectResult<List<KeyValuePair<string, string>>>>("无法确定内容对应的站点");

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