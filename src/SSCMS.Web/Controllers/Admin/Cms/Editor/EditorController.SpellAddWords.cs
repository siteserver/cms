using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;
using SSCMS.Utils;
using SSCMS.Dto;

namespace SSCMS.Web.Controllers.Admin.Cms.Editor
{
    public partial class EditorController
    {
        [HttpPost, Route(RouteSpellAddWords)]
        public async Task<ActionResult<BoolResult>> SpellAddWords([FromBody] SpellAddWordsRequest request)
        {
            if (!await _authManager.HasSitePermissionsAsync(request.SiteId,
                    MenuUtils.SitePermissions.Contents) ||
                !await _authManager.HasContentPermissionsAsync(request.SiteId, request.ChannelId,
                    MenuUtils.ContentPermissions.Add, MenuUtils.ContentPermissions.Edit))
            {
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(request.Word))
            {
                var (success, errorMessage) = await _spellManager.AddSpellWhiteListAsync(request.Word);
                if (!success)
                {
                    return this.Error(errorMessage);
                }
            }

            return new BoolResult
            {
                Value = true
            };
        }
    }
}