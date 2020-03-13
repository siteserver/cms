using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsAccessTokens")]
    public partial class AdministratorsAccessTokensController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IPluginManager _pluginManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public AdministratorsAccessTokensController(IAuthManager authManager, IPluginManager pluginManager, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _pluginManager = pluginManager;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<ListResult>> GetList()
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            var adminName = await _authManager.GetAdminNameAsync();
            var adminNames = new List<string>();

            if (await _authManager.IsSuperAdminAsync())
            {
                adminNames = await _administratorRepository.GetUserNameListAsync();
            }
            else
            {
                adminNames.Add(adminName);
            }

            var scopes = new List<string>(Constants.ScopeList);

            foreach (var plugin in _pluginManager.GetPlugins())
            {
                if (plugin.IsApiAuthorization)
                {
                    scopes.Add(plugin.PluginId);
                }
            }

            var tokens = await _accessTokenRepository.GetAccessTokenListAsync();

            return new ListResult
            {
                Tokens = tokens,
                AdminNames = adminNames,
                Scopes = scopes,
                AdminName = adminName
            };
        }

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<TokensResult>> Delete([FromBody]IdRequest request)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            await _accessTokenRepository.DeleteAsync(request.Id);
            var list = await _accessTokenRepository.GetAccessTokenListAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<TokensResult>> Submit([FromBody] AccessToken itemObj)
        {
            
            if (!await _authManager.IsAdminAuthenticatedAsync() ||
                !await _authManager.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            if (itemObj.Id > 0)
            {
                var tokenInfo = await _accessTokenRepository.GetAsync(itemObj.Id);

                if (tokenInfo.Title != itemObj.Title && await _accessTokenRepository.IsTitleExistsAsync(itemObj.Title))
                {
                    return this.Error("保存失败，已存在相同标题的API密钥！");
                }

                tokenInfo.Title = itemObj.Title;
                tokenInfo.AdminName = itemObj.AdminName;
                tokenInfo.Scopes = itemObj.Scopes;

                await _accessTokenRepository.UpdateAsync(tokenInfo);

                await _authManager.AddAdminLogAsync("修改API密钥", $"Access Token:{tokenInfo.Title}");
            }
            else
            {
                if (await _accessTokenRepository.IsTitleExistsAsync(itemObj.Title))
                {
                    return this.Error("保存失败，已存在相同标题的API密钥！");
                }

                var tokenInfo = new AccessToken
                {
                    Title = itemObj.Title,
                    AdminName = itemObj.AdminName,
                    Scopes = itemObj.Scopes
                };

                await _accessTokenRepository.InsertAsync(tokenInfo);

                await _authManager.AddAdminLogAsync("新增API密钥", $"Access Token:{tokenInfo.Title}");
            }

            var list = await _accessTokenRepository.GetAccessTokenListAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }
    }
}
