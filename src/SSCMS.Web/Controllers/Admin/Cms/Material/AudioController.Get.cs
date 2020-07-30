using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class AudioController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<QueryResult>> Get([FromQuery] QueryRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId, AuthTypes.SitePermissions.MaterialAudio))
            {
                return Unauthorized();
            }

            var isOpen = false;
            var account = await _openAccountRepository.GetBySiteIdAsync(request.SiteId);
            if (account.MpConnected)
            {
                isOpen = true;
            }

            var groups = await _materialGroupRepository.GetAllAsync(MaterialType.Audio);
            var count = await _materialAudioRepository.GetCountAsync(request.GroupId, request.Keyword);
            var items = await _materialAudioRepository.GetAllAsync(request.GroupId, request.Keyword, request.Page, request.PerPage);

            return new QueryResult
            {
                Groups = groups,
                Count = count,
                Items = items,
                IsOpen = isOpen
            };
        }
    }
}
