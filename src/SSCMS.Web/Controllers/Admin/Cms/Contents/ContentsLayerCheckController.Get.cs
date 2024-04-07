using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsLayerCheckController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromQuery] GetRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, MenuUtils.SitePermissions.ContentsCheck))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var summaries = new List<ChannelContentId>();
            if (!string.IsNullOrEmpty(request.ChannelContentIds))
            {
                summaries = ContentUtility.ParseChannelContentIds(request.ChannelContentIds);
            }
            else
            {
                var jsonFilePath = _pathManager.GetTemporaryFilesPath(request.FileName);
                if (FileUtils.IsFileExists(jsonFilePath))
                {
                    var json = await FileUtils.ReadTextAsync(jsonFilePath);
                    if (!string.IsNullOrEmpty(json))
                    {
                        summaries = TranslateUtils.JsonDeserialize<List<ChannelContentId>>(json);
                    }
                }
            }

            var contents = new List<Content>();
            foreach (var summary in summaries)
            {
                var channel = await _channelRepository.GetAsync(summary.ChannelId);
                var content = await _contentRepository.GetAsync(site, channel, summary.Id);
                if (content == null) continue;

                if (!await _authManager.HasContentPermissionsAsync(request.SiteId, content.ChannelId, MenuUtils.ContentPermissions.CheckLevel1))
                {
                    return Unauthorized();
                }

                var pageContent = content.Clone<Content>();
                pageContent.Set(ColumnsManager.CheckState, CheckManager.GetCheckState(site, content));
                contents.Add(pageContent);
            }

            var siteIdList = await _authManager.GetSiteIdsAsync();
            var transSites = await _siteRepository.GetSelectsAsync(siteIdList);

            var (isChecked, checkedLevel) = await CheckManager.GetUserCheckLevelAsync(_authManager, site, contents[0].ChannelId);
            var checkedLevels = ElementUtils.GetSelects(CheckManager.GetCheckedLevels(site, isChecked, checkedLevel, true));

            return new GetResult
            {
                Contents = contents,
                CheckedLevels = checkedLevels,
                CheckedLevel = checkedLevel,
                TransSites = transSites
            };
        }
    }
}