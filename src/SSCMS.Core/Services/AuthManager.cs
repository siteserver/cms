using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class AuthManager : IAuthManager
    {
        private readonly ClaimsPrincipal _principal;
        private readonly IOptionsMonitor<PermissionsOptions> _permissionsAccessor;
        private readonly ICacheManager<object> _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IDatabaseManager _databaseManager;

        public AuthManager(IHttpContextAccessor context, IOptionsMonitor<PermissionsOptions> permissionsAccessor, ICacheManager<object> cacheManager, ISettingsManager settingsManager, IDatabaseManager databaseManager)
        {
            _principal = context.HttpContext.User;
            _permissionsAccessor = permissionsAccessor;
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _databaseManager = databaseManager;
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

                _rolesKey = GetRolesCacheKey(_admin.UserName);
                _appPermissionsKey = GetAppPermissionsCacheKey(_admin.UserName);
                _sitePermissionDictKey = GetSitePermissionDictCacheKey(_admin.UserName);
                _channelPermissionDictKey = GetChannelPermissionDictCacheKey(_admin.UserName);
                _contentPermissionDictKey = GetContentPermissionDictCacheKey(_admin.UserName);
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

        public bool IsAdmin => _principal != null && _principal.IsInRole(Constants.RoleTypeAdministrator);

        public int AdminId => IsAdmin
            ? TranslateUtils.ToInt(_principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)
            : 0;

        public string AdminName => IsAdmin ? _principal.Identity.Name : string.Empty;

        public bool IsUser => _principal != null && _principal.IsInRole(Constants.RoleTypeUser);

        public int UserId => IsUser
            ? TranslateUtils.ToInt(_principal.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value)
            : 0;

        public string UserName => IsUser ? _principal.Identity.Name : string.Empty;

        public bool IsApi => _principal != null && _principal.IsInRole(Constants.RoleTypeApi);

        public string ApiToken => IsApi ? _principal.Identity.Name : string.Empty;

        public async Task AddSiteLogAsync(int siteId, string action)
        {
            await AddSiteLogAsync(siteId, 0, 0, action, string.Empty);
        }

        public async Task AddSiteLogAsync(int siteId, string action, string summary)
        {
            await AddSiteLogAsync(siteId, 0, 0, action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, string action, string summary)
        {
            await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, 0, await GetAdminAsync(), action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary)
        {
            await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, contentId, await GetAdminAsync(), action, summary);
        }

        public async Task AddAdminLogAsync(string action, string summary)
        {
            await _databaseManager.LogRepository.AddAdminLogAsync(await GetAdminAsync(), action, summary);
        }

        public async Task AddAdminLogAsync(string action)
        {
            await _databaseManager.LogRepository.AddAdminLogAsync(await GetAdminAsync(), action);
        }
    }
}