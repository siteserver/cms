using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPost, Route(RouteCensor)]
        public async Task<ActionResult<CensorSubmitResult>> Censor([FromBody] CensorRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId,
                    MenuUtils.ContentPermissions.Add, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            var site = await _siteRepository.GetAsync(request.SiteId);
            if (site == null) return NotFound();

            var channel = await _channelRepository.GetAsync(request.ChannelId);
            
            var content = await _pathManager.EncodeContentAsync(site, channel, request.Content);

            var fullContent = $"{content.Title}{content.SubTitle}{content.Summary}{content.Author}{content.Source}{content.Body}";

            var result = await _censorManager.ScanText(fullContent);

            return new CensorSubmitResult
            {
                Success = result.Suggestion == CensorSuggestion.Pass,
                TextResult = result
            };
        }
    }
}
