using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Core.Common;
using SS.CMS.Core.Services;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services.IUserManager;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin.Settings.Admin
{
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route("admin/settings/admin")]
    [ApiController]
    public class AccessTokenController : ControllerBase
    {
        private const string Route = "access-token";

        private readonly IUserManager _userManager;
        private readonly IAccessTokenRepository _accessTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly PluginManager _pluginManager;

        public AccessTokenController(IUserManager userManager, IAccessTokenRepository accessTokenRepository, IUserRepository userRepository, ILogRepository logRepository, PluginManager pluginManager)
        {
            _userManager = userManager;
            _accessTokenRepository = accessTokenRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
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
            if (!_userManager.HasAppPermissions(AuthTypes.AppPermissions.SettingsAdmin))
            {
                return Unauthorized();
            }

            var userName = _userManager.GetUserName();

            IEnumerable<string> adminNames;

            if (_userManager.IsSuperAdministrator())
            {
                adminNames = await _userRepository.GetUserNameListAsync();
            }
            else
            {
                adminNames = new List<string> { userName };
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
                AdminName = userName
            });
        }

        [HttpPost(Route)]
        public async Task<ActionResult> Create([FromBody] AccessTokenInfo accessTokenInfo)
        {
            if (!_userManager.HasAppPermissions(AuthTypes.AppPermissions.SettingsAdmin))
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

            _logRepository.AddAdminLog(_userManager.GetIpAddress(), _userManager.GetUserName(), "新增API密钥", $"Access Token:{tokenInfo.Title}");

            return Ok(new
            {
                Value = await _accessTokenRepository.GetAllAsync()
            });
        }

        [HttpPut(Route)]
        public async Task<ActionResult> Update([FromBody] AccessTokenInfo accessTokenInfo)
        {
            if (!_userManager.HasAppPermissions(AuthTypes.AppPermissions.SettingsAdmin))
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

            _logRepository.AddAdminLog(_userManager.GetIpAddress(), _userManager.GetUserName(), "修改API密钥", $"Access Token:{tokenInfo.Title}");

            return Ok(new
            {
                Value = await _accessTokenRepository.GetAllAsync()
            });
        }

        [HttpDelete(Route)]
        public async Task<ActionResult> Delete([FromBody] int id)
        {
            if (!_userManager.HasAppPermissions(AuthTypes.AppPermissions.SettingsAdmin))
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
