using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Editor
{

    [RoutePrefix("pages/cms/editor")]
    public partial class PagesEditorController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<ConfigResult> GetConfig([FromUri]ConfigRequest req)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();

            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(req.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(req.SiteId, req.ChannelId,
                    Constants.ChannelPermissions.ContentAdd))
            {
                return Request.Unauthorized<ConfigResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(req.SiteId);
            var channel = await ChannelManager.GetChannelAsync(req.SiteId, req.ChannelId);

            if (site == null)
            {
                return Request.BadRequest<ConfigResult>("指定的站点不存在");
            }
            if (channel == null)
            {
                return Request.BadRequest<ConfigResult>("指定的栏目不存在");
            }

            var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await DataProvider.ContentTagRepository.GetTagListAsync(site.Id);

            var allStyles = await TableStyleManager.GetContentStyleListAsync(site, channel);
            var styles = allStyles.Where(style =>
                    !StringUtils.ContainsIgnoreCase(ContentAttribute.MetadataAttributes.Value, style.AttributeName));

            var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissionsImpl, site, site.Id);
            var checkedLevels = CheckManager.GetCheckedLevelOptions(site, userIsChecked, userCheckedLevel, true);

            var content = new Content
            {
                Id = 0,
                SiteId = site.Id,
                ChannelId = channel.Id,
                AddDate = DateTime.Now,
                CheckedLevel = site.CheckContentDefaultLevel
            };
            if (req.ContentId != 0)
            {
                content = await DataProvider.ContentRepository.GetAsync(site, channel, req.ContentId);
            }

            var config = await DataProvider.ConfigRepository.GetAsync();

            var siteOptions = await DataProvider.SiteRepository.GetSiteOptionsAsync(0);
            var channelOptions = await ChannelManager.GetChannelOptionsAsync(site.Id);

            return new ConfigResult
            {
                User = auth.User,
                Config = config,
                Site = site,
                Channel = channel,
                GroupNames = groupNames,
                TagNames = tagNames,
                Styles = styles,
                CheckedLevels = checkedLevels,
                Content = content,
                SiteOptions = siteOptions,
                ChannelOptions = channelOptions
            };
        }
    }
}
