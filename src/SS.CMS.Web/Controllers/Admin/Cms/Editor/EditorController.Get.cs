using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;

namespace SS.CMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSitePermissionsAsync(request.SiteId,
                    Constants.SitePermissions.Contents) ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentAdd) ||
                !await _authManager.HasChannelPermissionsAsync(request.SiteId, request.ChannelId, Constants.ChannelPermissions.ContentEdit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            if (channel == null) return NotFound();

            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(site.Id);

            var tableName = _channelRepository.GetTableName(site, channel);
            var allStyles = await _tableStyleRepository.GetContentStyleListAsync(channel, tableName);
            var styles = allStyles.Where(style =>
                    !string.IsNullOrEmpty(style.DisplayName) && !StringUtils.ContainsIgnoreCase(ColumnsManager.MetadataAttributes.Value, style.AttributeName)).Select(
                x =>
                {
                    var style = x.Clone<TableStyle>();
                    style.AttributeName = StringUtils.LowerFirst(x.AttributeName);
                    return style;
                });

            var (userIsChecked, userCheckedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, site.Id);
            var checkedLevels = CheckManager.GetCheckedLevelOptions(site, userIsChecked, userCheckedLevel, true);

            Content content;
            if (request.ContentId > 0)
            {
                content = await _pathManager.DecodeContentAsync(site, channel, request.ContentId);
            }
            else
            {
                content = new Content
                {
                    Id = 0,
                    SiteId = site.Id,
                    ChannelId = channel.Id,
                    AddDate = DateTime.Now,
                    CheckedLevel = site.CheckContentDefaultLevel
                };
            }

            //await ContentUtility.TextEditorContentDecodeAsync(parseManager.PathManager, pageInfo.Site, content.Get<string>(ContentAttribute.Content), pageInfo.IsLocal);

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
    }
}
