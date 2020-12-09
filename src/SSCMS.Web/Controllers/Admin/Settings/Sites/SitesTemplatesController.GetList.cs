using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    public partial class SitesTemplatesController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> GetList()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsSitesTemplates))
            {
                return Unauthorized();
            }

            return await GetListResultAsync();
        }
    }
}
