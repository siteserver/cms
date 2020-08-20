using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Models;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    public partial class AudioController
    {
        [HttpPut, Route(Route)]
        public async Task<ActionResult<MaterialAudio>> Update([FromBody] UpdateRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                Types.SitePermissions.MaterialAudio))
            {
                return Unauthorized();
            }

            var lib = await _materialAudioRepository.GetAsync(request.Id);
            lib.Title = request.Title;
            lib.GroupId = request.GroupId;
            await _materialAudioRepository.UpdateAsync(lib);

            return lib;
        }
    }
}
