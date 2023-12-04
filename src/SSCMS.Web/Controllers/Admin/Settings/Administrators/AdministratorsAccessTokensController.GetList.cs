using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Models;

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
                Constants.ScopeSTL,
                Constants.ScopeForms,
                Constants.ScopeAdministrators,
                Constants.ScopeUsers,
                Constants.ScopeOthers,
            };

            var accessTokens = await _accessTokenRepository.GetAccessTokensAsync();
            var tokens = new List<AccessToken>();
            foreach (var token in accessTokens)
            {
               var admin = await _administratorRepository.GetByUserNameAsync(token.AdminName);
               if (admin != null)
               {
                    token.Set("AdminDisplay", _administratorRepository.GetDisplay(admin));
                    token.Set("AdminGuid", admin.Guid);
                    tokens.Add(token);
               }
            }

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
