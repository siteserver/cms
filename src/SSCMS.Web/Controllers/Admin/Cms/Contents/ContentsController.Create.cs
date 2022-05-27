using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Configuration;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    public partial class ContentsController
    {
        [HttpPost, Route(RouteCreate)]
        public async Task<ActionResult<BoolResult>> Create([FromBody] CreateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return this.Error(Constants.ErrorNotFound);

            var summaries = ContentUtility.ParseSummaries(request.ChannelContentIds);
            var config = await _databaseManager.ConfigRepository.GetAsync();

            foreach (var summary in summaries)
            {
                await _createManager.CreateContentAsync(request.SiteId, summary.ChannelId, summary.Id);
                if (config.IsLogSite && config.IsLogSiteCreate)
                {
                    var filePath = await _pathManager.GetContentPageFilePathAsync(site, summary.ChannelId, summary.Id, 0);
                    await _authManager.AddSiteCreateLogAsync(request.SiteId, summary.ChannelId, summary.Id, filePath);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
