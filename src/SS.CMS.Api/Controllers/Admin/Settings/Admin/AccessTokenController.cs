using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Common;
using SS.CMS.Core.Common.Serialization;
using SS.CMS.Core.Services;

namespace SS.CMS.Api.Controllers.Admin.Settings.Admin
{
    [Route("admin/settings/admin")]
    [ApiController]
    public class AccessTokenController : ControllerBase
    {
        private const string Route = "access-token";

        private readonly IIdentityManager _identityManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly PluginManager _pluginManager;

        public AccessTokenController(IIdentityManager identityManager, IAccessTokenRepository accessTokenRepository, IAdministratorRepository administratorRepository, PluginManager pluginManager)
        {
            _identityManager = identityManager;
            _accessTokenRepository = accessTokenRepository;
            _administratorRepository = administratorRepository;
            _pluginManager = pluginManager;
        }

        /// <summary>
        /// List Access Tokens.
        /// </summary>
        /// <remarks>
        /// Sample response:
        ///
        ///     GET /api/admin/settings/admin/access-token
        ///     {
        ///        "id": 1,
        ///        "name": "Item1",
        ///        "isComplete": true
        ///     }
        ///
        /// </remarks>
        /// <returns>A newly created TodoItem</returns>
        /// <response code="200">Returns the newly created item</response>
        /// <response code="400">If the item is null</response>
        /// <response code="401">If admin is not authorized</response>
        [Consumes("application/json")]
        [Produces("application/json")]
        [ProducesResponseType(200)]     // Ok
        [ProducesResponseType(400)]     // BadRequest
        [ProducesResponseType(401)]     // Unauthorized
        [HttpGet(Route)]
        public async Task<ActionResult> List()
        {
            if (!_identityManager.IsAdminLoggin ||
                !_identityManager.AdminPermissions.HasSystemPermissions(MenuManager.SettingsPermissions.Admin))
            {
                return Unauthorized();
            }

            IEnumerable<string> adminNames;

            if (_identityManager.AdminPermissions.IsSuperAdmin())
            {
                adminNames = await _administratorRepository.GetUserNameListAsync();
            }
            else
            {
                adminNames = new List<string> { _identityManager.AdminName };
            }

            var scopes = new List<string>(_accessTokenRepository.ScopeList);

            foreach (var service in _pluginManager.Services)
            {
                if (service.IsApiAuthorization)
                {
                    scopes.Add(service.PluginId);
                }
            }

            return Ok(new
            {
                Value = await _accessTokenRepository.GetAllAsync(),
                adminNames,
                scopes,
                _identityManager.AdminName
            });
        }

        [HttpPost(Route)]
        public async Task<ActionResult> Create([FromBody] AccessTokenInfo accessTokenInfo)
        {
            if (!_identityManager.IsAdminLoggin ||
                !_identityManager.AdminPermissions.HasSystemPermissions(MenuManager.SettingsPermissions.Admin))
            {
                return Unauthorized();
            }

            if (await _accessTokenRepository.IsTitleExistsAsync(accessTokenInfo.Title))
            {
                return BadRequest("保存失败，已存在相同标题的API密钥！");
            }

            var tokenInfo = new AccessTokenInfo
            {
                Title = accessTokenInfo.Title,
                AdminName = accessTokenInfo.AdminName,
                Scopes = accessTokenInfo.Scopes
            };

            await _accessTokenRepository.InsertAsync(tokenInfo);

            LogUtils.AddAdminLog(_identityManager.IpAddress, _identityManager.AdminName, "新增API密钥", $"Access Token:{tokenInfo.Title}");

            return Ok(new
            {
                Value = await _accessTokenRepository.GetAllAsync()
            });
        }

        [HttpPut(Route)]
        public async Task<ActionResult> Update([FromBody] AccessTokenInfo accessTokenInfo)
        {
            if (!_identityManager.IsAdminLoggin ||
                !_identityManager.AdminPermissions.HasSystemPermissions(MenuManager.SettingsPermissions.Admin))
            {
                return Unauthorized();
            }

            var tokenInfo = await _accessTokenRepository.GetAsync(accessTokenInfo.Id);

            if (tokenInfo.Title != accessTokenInfo.Title && await _accessTokenRepository.IsTitleExistsAsync(accessTokenInfo.Title))
            {
                return BadRequest("保存失败，已存在相同标题的API密钥！");
            }

            tokenInfo.Title = accessTokenInfo.Title;
            tokenInfo.AdminName = accessTokenInfo.AdminName;
            tokenInfo.Scopes = accessTokenInfo.Scopes;

            await _accessTokenRepository.UpdateAsync(tokenInfo);

            LogUtils.AddAdminLog(_identityManager.IpAddress, _identityManager.AdminName, "修改API密钥", $"Access Token:{tokenInfo.Title}");

            return Ok(new
            {
                Value = await _accessTokenRepository.GetAllAsync()
            });
        }

        [HttpDelete(Route)]
        public async Task<ActionResult> Delete([FromBody] int id)
        {
            if (!_identityManager.IsAdminLoggin ||
                !_identityManager.AdminPermissions.HasSystemPermissions(MenuManager.SettingsPermissions.Admin))
            {
                return Unauthorized();
            }

            await _accessTokenRepository.DeleteAsync(id);

            return Ok(new
            {
                Value = await _accessTokenRepository.GetAllAsync()
            });
        }
    }
}
