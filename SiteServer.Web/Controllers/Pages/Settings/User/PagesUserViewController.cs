using System;
using System.Threading.Tasks;
using System.Web.Http;
using NSwag.Annotations;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;

namespace SiteServer.API.Controllers.Pages.Settings.User
{
    [OpenApiIgnore]
    [RoutePrefix("pages/settings/userView")]
    public class PagesUserViewController : ApiController
    {
        private const string Route = "";

        [HttpGet, Route(Route)]
        public async Task<IHttpActionResult> Get()
        {
            try
            {
                var request = await AuthenticatedRequest.GetRequestAsync();
                if (!request.IsAdminLoggin ||
                    !await request.AdminPermissionsImpl.HasSystemPermissionsAsync(ConfigManager.SettingsPermissions.User))
                {
                    return Unauthorized();
                }

                var userId = request.GetQueryInt("userId");
                var user = await UserManager.GetUserByUserIdAsync(userId);
                if (user == null) return NotFound();

                var groupName = await UserGroupManager.GetUserGroupNameAsync(user.GroupId);

                return Ok(new
                {
                    Value = user,
                    GroupName = groupName
                });
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
