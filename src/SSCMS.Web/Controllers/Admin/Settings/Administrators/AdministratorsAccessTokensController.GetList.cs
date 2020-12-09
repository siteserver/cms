using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    public partial class AdministratorsAccessTokensController
    {
        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> GetList()
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            var adminName = _authManager.AdminName;
            var adminNames = new List<string>();

            if (await _authManager.IsSuperAdminAsync())
            {
                adminNames = await _administratorRepository.GetUserNamesAsync();
            }
            else
            {
                adminNames.Add(adminName);
            }

            var scopes = new List<string>
            {
                Constants.ScopeChannels,
                Constants.ScopeContents,
                Constants.ScopeAdministrators,
                Constants.ScopeUsers,
                Constants.ScopeStl
            };

            //foreach (var plugin in _pluginManager.EnabledPlugins)
            //{
            //    scopes.Add(plugin.PluginId);
            //}

            var tokens = await _accessTokenRepository.GetAccessTokensAsync();

            return new ListResult
            {
                Tokens = tokens,
                AdminNames = adminNames,
                Scopes = scopes,
                AdminName = adminName
            };
        }
    }
}
