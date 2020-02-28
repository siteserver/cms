using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Request;
using SS.CMS.Plugins;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.Admin.Settings.Administrators
{
    [Route("admin/settings/administratorsAccessTokens")]
    public partial class AdministratorsAccessTokensController : ControllerBase
    {
        private const string Route = "";

        private readonly IAuthManager _authManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;

        public AdministratorsAccessTokensController(IAuthManager authManager, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository)
        {
            _authManager = authManager;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
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
                adminNames = await _administratorRepository.GetUserNameListAsync();
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

            var tokens = await _accessTokenRepository.GetAccessTokenListAsync();

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
            var auth = await _authManager.GetAdminAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissions.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAdministratorsAccessTokens))
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

                await auth.AddAdminLogAsync("修改API密钥", $"Access Token:{tokenInfo.Title}");
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

                await auth.AddAdminLogAsync("新增API密钥", $"Access Token:{tokenInfo.Title}");
            }

            var list = await _accessTokenRepository.GetAccessTokenListAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }
    }
}
