using System.Linq;
using System.Threading.Tasks;
using Datory.Utils;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Core;
using SS.CMS.Framework;
using SS.CMS.Plugins;

namespace SS.CMS.Web.Controllers.Admin.Plugins
{
    [Route("admin/plugins/add")]
    public partial class AddController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public AddController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.PluginsAdd))
            {
                return Unauthorized();
            }

            var dict = await PluginManager.GetPluginIdAndVersionDictAsync();
            var list = dict.Keys.ToList();
            var packageIds = Utilities.ToString(list);

            return new GetResult
            {
                IsNightly = WebConfigUtils.IsNightlyUpdate,
                PluginVersion = SystemManager.PluginVersion,
                PackageIds = packageIds
            };
        }
    }
}
