using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Web.Controllers.V1;
using Xunit;

namespace SSCMS.Web.Tests.Controllers.V1
{
    public class AdministratorsControllerTests
    {
        [Fact]
        public async Task LoginReturnsSanitizedAdministrator()
        {
            var administrator = new Administrator
            {
                Id = 1,
                Guid = "admin-guid",
                UserName = "admin",
                DisplayName = "Admin",
                Email = "admin@example.com",
                Mobile = "13800000000",
                LastActivityDate = DateTime.UtcNow
            };

            var administratorRepository = new Mock<IAdministratorRepository>();
            administratorRepository
                .Setup(x => x.ValidateAsync("admin", "password-md5", true))
                .ReturnsAsync((administrator, "admin", string.Empty));
            administratorRepository
                .Setup(x => x.GetByUserNameAsync("admin"))
                .ReturnsAsync(administrator);

            var authManager = new Mock<IAuthManager>();
            authManager
                .Setup(x => x.AuthenticateAdministrator(administrator, false))
                .Returns("token");

            var configRepository = new Mock<IConfigRepository>();
            configRepository.Setup(x => x.GetAsync()).ReturnsAsync(new Config());

            var controller = CreateController(
                administratorRepository.Object,
                authManager.Object,
                configRepository.Object,
                new TestCacheManager());

            var result = await controller.Login(new AdministratorsController.LoginRequest
            {
                Account = "admin",
                Password = "password-md5"
            });

            var value = Assert.IsType<AdministratorsController.LoginResult>(result.Value);
            Assert.IsType<AdministratorsController.LoginAdministrator>(value.Administrator);
            Assert.Equal("admin", value.Administrator.UserName);
            Assert.Null(value.Administrator.GetType().GetProperty(nameof(Administrator.Email)));
            Assert.Null(value.Administrator.GetType().GetProperty(nameof(Administrator.Mobile)));
        }

        [Fact]
        public async Task LoginRateLimitsRepeatedFailures()
        {
            var administratorRepository = new Mock<IAdministratorRepository>();
            administratorRepository
                .Setup(x => x.ValidateAsync("admin", "bad-password", true))
                .ReturnsAsync((null, "admin", "invalid"));

            var controller = CreateController(
                administratorRepository.Object,
                Mock.Of<IAuthManager>(),
                Mock.Of<IConfigRepository>(),
                new TestCacheManager());

            ActionResult lastResult = null;
            for (var i = 0; i < 11; i++)
            {
                var result = await controller.Login(new AdministratorsController.LoginRequest
                {
                    Account = "admin",
                    Password = "bad-password"
                });
                lastResult = result.Result;
            }

            Assert.IsType<BadRequestObjectResult>(lastResult);
            administratorRepository.Verify(x => x.ValidateAsync("admin", "bad-password", true), Times.Exactly(10));
        }

        [Fact]
        public async Task CreateRequiresSuperAdmin()
        {
            var administratorRepository = new Mock<IAdministratorRepository>();
            administratorRepository
                .Setup(x => x.InsertAsync(It.IsAny<Administrator>(), It.IsAny<string>()))
                .ReturnsAsync((true, string.Empty));

            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.ApiToken).Returns("token");
            authManager
                .Setup(x => x.HasAppPermissionsAsync(Core.Utils.MenuUtils.AppPermissions.SettingsAdministrators))
                .ReturnsAsync(true);
            authManager
                .Setup(x => x.IsSuperAdminAsync())
                .ReturnsAsync(false);

            var accessTokenRepository = new Mock<IAccessTokenRepository>();
            accessTokenRepository
                .Setup(x => x.IsScopeAsync("token", Configuration.Constants.ScopeAdministrators))
                .ReturnsAsync(true);

            var controller = CreateController(
                administratorRepository.Object,
                authManager.Object,
                Mock.Of<IConfigRepository>(),
                new TestCacheManager(),
                accessTokenRepository.Object);

            var result = await controller.Create(new Administrator
            {
                UserName = "new-admin",
                Password = "password-md5"
            });

            Assert.IsType<UnauthorizedResult>(result.Result);
            administratorRepository.Verify(
                x => x.InsertAsync(It.IsAny<Administrator>(), It.IsAny<string>()),
                Times.Never);
        }

        private static AdministratorsController CreateController(
            IAdministratorRepository administratorRepository,
            IAuthManager authManager,
            IConfigRepository configRepository,
            ICacheManager cacheManager,
            IAccessTokenRepository accessTokenRepository = null)
        {
            return new AdministratorsController(
                Mock.Of<ISettingsManager>(),
                authManager,
                configRepository,
                accessTokenRepository ?? Mock.Of<IAccessTokenRepository>(),
                administratorRepository,
                Mock.Of<IDbCacheRepository>(),
                Mock.Of<ILogRepository>(),
                Mock.Of<IStatRepository>(),
                cacheManager)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
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
