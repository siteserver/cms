using System.Collections.Generic;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;
using Moq;
using SSCMS.Configuration;
using SSCMS.Core.Services;
using SSCMS.Core.Utils;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using Xunit;

namespace SSCMS.Web.Tests.Security
{
    public class AuthManagerPermissionsTests
    {
        [Fact]
        public async Task SiteAdminContentPermissionsDoNotCrossAllowedSites()
        {
            var channelRepository = new Mock<IChannelRepository>();
            channelRepository
                .Setup(x => x.GetParentIdAsync(2, 2))
                .ReturnsAsync(0);

            var authManager = CreateSiteAdminAuthManager(new List<int> { 1 }, channelRepository.Object);

            var allowed = await authManager.HasContentPermissionsAsync(2, 2, MenuUtils.ContentPermissions.Add);

            Assert.False(allowed);
        }

        [Fact]
        public async Task SiteAdminSitePermissionsDoNotCrossAllowedSites()
        {
            var authManager = CreateSiteAdminAuthManager(new List<int> { 1 }, Mock.Of<IChannelRepository>());

            var allowed = await authManager.HasSitePermissionsAsync(2);

            Assert.False(allowed);
        }

        private static AuthManager CreateSiteAdminAuthManager(List<int> siteIds, IChannelRepository channelRepository)
        {
            var context = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "site-admin"),
                    new Claim(ClaimTypes.Role, Types.Roles.Administrator)
                }, "Test"))
            };
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(context);

            var roles = new List<string> { "site-admin-role" };
            var administratorsInRolesRepository = new Mock<IAdministratorsInRolesRepository>();
            administratorsInRolesRepository
                .Setup(x => x.GetRolesForUserAsync("site-admin"))
                .ReturnsAsync(roles);

            var roleRepository = new Mock<IRoleRepository>();
            roleRepository
                .Setup(x => x.IsConsoleAdministrator(It.IsAny<IList<string>>()))
                .Returns(false);
            roleRepository
                .Setup(x => x.IsSystemAdministrator(It.IsAny<IList<string>>()))
                .Returns(true);

            var settingsManager = new Mock<ISettingsManager>();
            settingsManager
                .Setup(x => x.GetPermissions())
                .Returns(new List<Permission>
                {
                    new Permission
                    {
                        Id = MenuUtils.ContentPermissions.Add,
                        Type = new List<string> { Types.PermissionTypes.Channel }
                    }
                });
            settingsManager
                .Setup(x => x.GetSiteType(Types.SiteTypes.Web))
                .Returns(new SiteType { Id = Types.SiteTypes.Web });

            var siteRepository = new Mock<ISiteRepository>();
            siteRepository
                .Setup(x => x.GetAsync(1))
                .ReturnsAsync(new Site { Id = 1, SiteType = Types.SiteTypes.Web });

            var formRepository = new Mock<IFormRepository>();
            formRepository
                .Setup(x => x.GetFormsAsync(1))
                .ReturnsAsync(new List<Form>());

            var databaseManager = new Mock<IDatabaseManager>();
            databaseManager.SetupGet(x => x.AdministratorsInRolesRepository).Returns(administratorsInRolesRepository.Object);
            databaseManager.SetupGet(x => x.ChannelRepository).Returns(channelRepository);
            databaseManager.SetupGet(x => x.FormRepository).Returns(formRepository.Object);
            databaseManager.SetupGet(x => x.RoleRepository).Returns(roleRepository.Object);
            databaseManager.SetupGet(x => x.SiteRepository).Returns(siteRepository.Object);

            var authManager = new AuthManager(
                httpContextAccessor.Object,
                Mock.Of<IAntiforgery>(),
                new TestCacheManager(),
                settingsManager.Object,
                databaseManager.Object);

            SetAdmin(authManager, new Administrator
            {
                UserName = "site-admin",
                SiteIds = siteIds
            });

            return authManager;
        }

        private static void SetAdmin(AuthManager authManager, Administrator administrator)
        {
            var field = typeof(AuthManager).GetField("_admin", BindingFlags.Instance | BindingFlags.NonPublic);
            field.SetValue(authManager, administrator);
        }

        private class TestCacheManager : ICacheManager
        {
            private readonly Dictionary<string, object> _cache = new Dictionary<string, object>();

            public IReadOnlyCacheManagerConfiguration Configuration => null;

            public T Get<T>(string key)
            {
                return _cache.TryGetValue(key, out var value) ? (T)value : default;
            }

            public string GetByFilePath(string filePath)
            {
                return string.Empty;
            }

            public bool Exists(string key)
            {
                return _cache.ContainsKey(key);
            }

            public void AddOrUpdateSliding<T>(string key, T value, int minutes)
            {
                _cache[key] = value;
            }

            public void AddOrUpdateAbsolute<T>(string key, T value, int minutes)
            {
                _cache[key] = value;
            }

            public void AddOrUpdate<T>(string key, T value)
            {
                _cache[key] = value;
            }

            public void Remove(string key)
            {
                _cache.Remove(key);
            }

            public void Clear()
            {
                _cache.Clear();
            }
        }
    }
}
