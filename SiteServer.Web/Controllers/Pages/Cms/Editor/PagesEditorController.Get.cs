using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Dto;
using SiteServer.CMS.Dto.Request;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Cms.Editor
{
    public partial class PagesEditorController
    {
        [HttpGet, Route(Route)]
        public async Task<GetResult> Get([FromUri]GetRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd) ||
                !await auth.AdminPermissionsImpl.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Request.Unauthorized<GetResult>();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return Request.NotFound<GetResult>();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return Request.NotFound<GetResult>();

            var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(site.Id);

            var allStyles = await DataProvider.TableStyleRepository.GetContentStyleListAsync(site, channel);
            var styles = allStyles.Where(style =>
                    !string.IsNullOrEmpty(style.DisplayName) && !StringUtils.ContainsIgnoreCase(ContentAttribute.MetadataAttributes.Value, style.AttributeName)).Select(
                x =>
                {
                    var style = x.Clone<TableStyle>();
                    style.AttributeName = StringUtils.LowerFirst(x.AttributeName);
                    return style;
                });

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
            if (request.ContentId != 0)
            {
                content = await DataProvider.ContentRepository.GetAsync(site, channel, request.ContentId);
            }

            return new GetResult
            {
                Content = content,
                Site = site,
                Channel = channel,
                GroupNames = groupNames,
                TagNames = tagNames,
                Styles = styles,
                CheckedLevels = checkedLevels
            };
        }

        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public Content Content { get; set; }
            public Site Site { get; set; }
            public Channel Channel { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public IEnumerable<TableStyle> Styles { get; set; }
            public List<Select<int>> CheckedLevels { get; set; }
        }
    }
}
