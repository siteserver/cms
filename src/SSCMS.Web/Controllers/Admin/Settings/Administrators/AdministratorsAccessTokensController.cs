using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Administrators
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AdministratorsAccessTokensController : ControllerBase
    {
        private const string Route = "settings/administratorsAccessTokens";

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
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministratorsAccessTokens))
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

        [HttpDelete, Route(Route)]
        public async Task<ActionResult<TokensResult>> Delete([FromBody]IdRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministratorsAccessTokens))
            {
                return Unauthorized();
            }

            await _accessTokenRepository.DeleteAsync(request.Id);
            var list = await _accessTokenRepository.GetAccessTokensAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<TokensResult>> Submit([FromBody] AccessToken itemObj)
        {
            if (!await _authManager.HasAppPermissionsAsync(Types.AppPermissions.SettingsAdministratorsAccessTokens))
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

            var list = await _accessTokenRepository.GetAccessTokensAsync();

            return new TokensResult
            {
                Tokens = list
            };
        }
    }
}
