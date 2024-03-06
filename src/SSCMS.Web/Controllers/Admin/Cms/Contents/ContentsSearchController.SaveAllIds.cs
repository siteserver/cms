using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Configuration;
using SSCMS.Utils;
using SSCMS.Models;
using System.Collections.Generic;
using System.Linq;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsSearchController
    {
        [HttpPost, Route(RouteSaveAllIds)]
        public async Task<ActionResult<StringResult>> SaveAllIds([FromBody] ListRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.ContentsSearch))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            List<ContentSummary> summaries;
            if (request.IsAdvanced)
            {
                var isAdmin = false;
                var adminId = _authManager.AdminId;
                var isUser = false;
                if (request.SearchType == SearchType.Admin)
                {
                    isAdmin = true;
                }
                else if (request.SearchType == SearchType.User)
                {
                    isUser = true;
                }

                (_, summaries) = await _contentRepository.AdvancedSearchAsync(site, 0, request.ChannelIds, request.IsAllContents, request.StartDate, request.EndDate, request.Items, request.IsCheckedLevels, request.CheckedLevels, request.IsTop, request.IsRecommend, request.IsHot, request.IsColor, request.GroupNames, request.TagNames, isAdmin, adminId, isUser);
            }
            else
            {
                var channelId = request.ChannelIds.FirstOrDefault();
                var first = await _channelRepository.GetAsync(channelId);
                summaries = await _contentRepository.GetSummariesAsync(site, first, request.IsAllContents);
            }

            var fileName = await _pathManager.WriteTemporaryTextAsync(TranslateUtils.JsonSerialize(summaries));
            
            return new StringResult
            {
                Value = fileName
            };
        }
    }
}
