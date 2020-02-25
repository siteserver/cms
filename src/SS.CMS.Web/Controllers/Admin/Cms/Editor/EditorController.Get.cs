using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Core;
using SS.CMS.Framework;

namespace SS.CMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd) ||
                !await auth.AdminPermissions.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Unauthorized();
            }

            var site = await DataProvider.SiteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await DataProvider.ChannelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var groupNames = await DataProvider.ContentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await DataProvider.ContentTagRepository.GetTagNamesAsync(site.Id);

            var tableName = await DataProvider.ChannelRepository.GetTableNameAsync(site, channel);
            var allStyles = await DataProvider.TableStyleRepository.GetContentStyleListAsync(channel, tableName);
            var styles = allStyles.Where(style =>
                    !string.IsNullOrEmpty(style.DisplayName) && !StringUtils.ContainsIgnoreCase(ContentAttribute.MetadataAttributes.Value, style.AttributeName)).Select(
                x =>
                {
                    var style = x.Clone<TableStyle>();
                    style.AttributeName = StringUtils.LowerFirst(x.AttributeName);
                    return style;
                });

            var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(auth.AdminPermissions, site, site.Id);
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
