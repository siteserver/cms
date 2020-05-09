using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SSCMS.Configuration;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class AuthManager : IAuthManager
    {
        private readonly ClaimsPrincipal _principal;
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly List<Permission> _permissions;

        public AuthManager(IHttpContextAccessor context, ISettingsManager settingsManager, IPluginManager pluginManager, IDatabaseManager databaseManager)
        {
            _principal = context.HttpContext.User;
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
            _permissions = pluginManager.GetPermissions();
        }

        private Administrator _admin;
        private User _user;
        private UserGroup _userGroup;

        public async Task<User> GetUserAsync()
        {
            if (!IsUser) return null;
            if (_user != null) return _user;

            _user = await _databaseManager.UserRepository.GetByUserNameAsync(UserName);
            return _user;
        }

        public void Init(Administrator administrator)
        {
            if (administrator != null && !administrator.Locked)
            {
                _admin = administrator;
            }
        }

        public async Task<Administrator> GetAdminAsync()
        {
            if (_admin != null) return _admin;

            if (IsAdmin)
            {
                _admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(AdminName);
            }
            else if (IsUser)
            {
                var user = await GetUserAsync();
                if (user != null && !user.Locked && user.Checked)
                {
                    _userGroup = await _databaseManager.UserGroupRepository.GetUserGroupAsync(user.GroupId);
                    if (_userGroup != null)
                    {
                        _admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(_userGroup.AdminName);
                    }
                }

                _admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(AdminName);
            }
            else if (IsApi)
            {
                var tokenInfo = await _databaseManager.AccessTokenRepository.GetByTokenAsync(ApiToken);
                if (tokenInfo != null)
                {
                    if (!string.IsNullOrEmpty(tokenInfo.AdminName))
                    {
                        var admin = await _databaseManager.AdministratorRepository.GetByUserNameAsync(tokenInfo.AdminName);
                        if (admin != null && !admin.Locked)
                        {
                            _admin = admin;
                        }
                    }
                }
            }

            Init(_admin);

            return _admin;
        }

        public bool IsAdmin => _principal != null && _principal.IsInRole(AuthTypes.Roles.Administrator);

        public int AdminId => IsAdmin
            ? TranslateUtils.ToInt(_principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)
            : 0;

        public string AdminName => IsAdmin ? _principal.Identity.Name : string.Empty;

        public bool IsUser => _principal != null && _principal.IsInRole(AuthTypes.Roles.User);

        public int UserId => IsUser
            ? TranslateUtils.ToInt(_principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)
            : 0;

        public string UserName => IsUser ? _principal.Identity.Name : string.Empty;

        public bool IsApi => _principal != null && _principal.IsInRole(AuthTypes.Roles.Api);

        public string ApiToken => IsApi ? _principal.Identity.Name : string.Empty;
    }
}