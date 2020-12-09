using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home.Write
{
    public partial class EditorController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery]GetRequest request)
        {
            var siteIds = await _authManager.GetSiteIdsAsync();

            if (siteIds.Count == 0)
            {
                return new GetResult
                {
                    Unauthorized = true
                };
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            var channel = await _channelRepository.GetAsync(request.ChannelId);

            var groupNames = await _contentGroupRepository.GetGroupNamesAsync(site.Id);
            var tagNames = await _contentTagRepository.GetTagNamesAsync(site.Id);

            var allStyles = await _tableStyleRepository.GetContentStylesAsync(site, channel);
            var styles = allStyles
                .Where(style =>
                    !string.IsNullOrEmpty(style.DisplayName) &&
                    !ListUtils.ContainsIgnoreCase(ColumnsManager.MetadataAttributes.Value, style.AttributeName))
                .Select(x => new InputStyle(x));

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

            var siteUrl = await _pathManager.GetSiteUrlAsync(site, false);

            return new GetResult
            {
                Unauthorized = false,
                Content = content,
                Site = site,
                Channel = channel,
                GroupNames = groupNames,
                TagNames = tagNames,
                Styles = styles,
                CheckedLevels = checkedLevels,
                SiteUrl = siteUrl
            };
        }
    }
}
