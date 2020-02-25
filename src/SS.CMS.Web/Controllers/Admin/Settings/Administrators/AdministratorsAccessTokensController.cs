using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Framework;
using SS.CMS.Plugins;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsAccessTokens")]
    public partial class AdministratorsAccessTokensController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;

        public AdministratorsAccessTokensController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> GetList()
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            var adminNames = new List<string>();

            if (await auth.AdminPermissions.IsSuperAdminAsync())
            {
                adminNames = (await DataProvider.AdministratorRepository.GetUserNameListAsync()).ToList();
            }
            else
            {
                adminNames.Add(auth.AdminName);
            }

            var scopes = new List<string>(Constants.ScopeList);

            foreach (var service in await PluginManager.GetServicesAsync())
            {
                if (service.IsApiAuthorization)
                {
                    scopes.Add(service.PluginId);
                }
            }

            var tokens = await DataProvider.AccessTokenRepository.GetAccessTokenListAsync();

            return new ListResult
            {
                Tokens = tokens,
                AdminNames = adminNames,
                Scopes = scopes,
                AdminName = auth.AdminName
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<TokensResult>> Delete([FromBody]IdRequest request)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            await DataProvider.AccessTokenRepository.DeleteAsync(request.Id);
            var list = await DataProvider.AccessTokenRepository.GetAccessTokenListAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<TokensResult>> Submit([FromBody] AccessToken itemObj)
        {
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            if (itemObj.Id > 0)
            {
                var tokenInfo = await DataProvider.AccessTokenRepository.GetAsync(itemObj.Id);

                if (tokenInfo.Title != itemObj.Title && await DataProvider.AccessTokenRepository.IsTitleExistsAsync(itemObj.Title))
                {
                    return this.Error("保存失败，已存在相同标题的API密钥！");
                }

                tokenInfo.Title = itemObj.Title;
                tokenInfo.AdminName = itemObj.AdminName;
                tokenInfo.Scopes = itemObj.Scopes;

                await DataProvider.AccessTokenRepository.UpdateAsync(tokenInfo);

                await auth.AddAdminLogAsync("修改API密钥", $"Access Token:{tokenInfo.Title}");
            }
            else
            {
                if (await DataProvider.AccessTokenRepository.IsTitleExistsAsync(itemObj.Title))
                {
                    return this.Error("保存失败，已存在相同标题的API密钥！");
                }

                var tokenInfo = new AccessToken
                {
                    Title = itemObj.Title,
                    AdminName = itemObj.AdminName,
                    Scopes = itemObj.Scopes
                };

                await DataProvider.AccessTokenRepository.InsertAsync(tokenInfo);

                await auth.AddAdminLogAsync("新增API密钥", $"Access Token:{tokenInfo.Title}");
            }

            var list = await DataProvider.AccessTokenRepository.GetAccessTokenListAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }
    }
}
