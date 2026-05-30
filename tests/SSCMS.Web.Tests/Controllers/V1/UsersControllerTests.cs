using System.Collections.Generic;
using System.IO;
using System.Text;
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
    public class UsersControllerTests
    {
        [Fact]
        public async Task LoginRateLimitsRepeatedFailures()
        {
            var userRepository = new Mock<IUserRepository>();
            userRepository
                .Setup(x => x.ValidateAsync("user", "bad-password", true))
                .ReturnsAsync((null, "user", "invalid"));

            var controller = new UsersController(
                Mock.Of<IAuthManager>(),
                Mock.Of<IPathManager>(),
                Mock.Of<IConfigRepository>(),
                Mock.Of<IAccessTokenRepository>(),
                userRepository.Object,
                Mock.Of<ILogRepository>(),
                Mock.Of<IStatRepository>(),
                Mock.Of<IDbCacheRepository>(),
                Mock.Of<IUserGroupRepository>(),
                Mock.Of<IUsersInGroupsRepository>(),
                new TestCacheManager())
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            ActionResult lastResult = null;
            for (var i = 0; i < 11; i++)
            {
                var result = await controller.Login(new UsersController.LoginRequest
                {
                    Account = "user",
                    Password = "bad-password"
                });
                lastResult = result.Result;
            }

            Assert.IsType<BadRequestObjectResult>(lastResult);
            userRepository.Verify(x => x.ValidateAsync("user", "bad-password", true), Times.Exactly(10));
        }

        [Fact]
        public async Task UploadAvatarRejectsNonImageContent()
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.ApiToken).Returns("token");
            authManager
                .Setup(x => x.HasAppPermissionsAsync(It.IsAny<string>()))
                .ReturnsAsync(true);

            var accessTokenRepository = new Mock<IAccessTokenRepository>();
            accessTokenRepository
                .Setup(x => x.IsScopeAsync("token", It.IsAny<string>()))
                .ReturnsAsync(true);

            var userRepository = new Mock<IUserRepository>();
            userRepository
                .Setup(x => x.GetByUserIdAsync(1))
                .ReturnsAsync(new User
                {
                    Id = 1
                });

            var pathManager = new Mock<IPathManager>();
            pathManager.Setup(x => x.GetUserUploadFileName("payload.png")).Returns("payload.png");
            pathManager.Setup(x => x.GetUserUploadPath(1, "payload.png")).Returns("/tmp/payload.png");

            var controller = new UsersController(
                authManager.Object,
                pathManager.Object,
                Mock.Of<IConfigRepository>(),
                accessTokenRepository.Object,
                userRepository.Object,
                Mock.Of<ILogRepository>(),
                Mock.Of<IStatRepository>(),
                Mock.Of<IDbCacheRepository>(),
                Mock.Of<IUserGroupRepository>(),
                Mock.Of<IUsersInGroupsRepository>(),
                new TestCacheManager());

            var bytes = Encoding.UTF8.GetBytes("not an image");
            await using var stream = new MemoryStream(bytes);
            var result = await controller.UploadAvatar(1, new FormFile(stream, 0, bytes.Length, "file", "payload.png"));

            Assert.IsType<BadRequestObjectResult>(result.Result);
            pathManager.Verify(x => x.UploadAsync(It.IsAny<IFormFile>(), It.IsAny<string>()), Times.Never);
            userRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetRequiresSettingsUsersPermission()
        {
            var authManager = new Mock<IAuthManager>();
            authManager.Setup(x => x.ApiToken).Returns("token");
            authManager
                .Setup(x => x.HasAppPermissionsAsync(It.IsAny<string>()))
                .ReturnsAsync(false);

            var accessTokenRepository = new Mock<IAccessTokenRepository>();
            accessTokenRepository
                .Setup(x => x.IsScopeAsync("token", It.IsAny<string>()))
                .ReturnsAsync(true);

            var userRepository = new Mock<IUserRepository>();

            var controller = new UsersController(
                authManager.Object,
                Mock.Of<IPathManager>(),
                Mock.Of<IConfigRepository>(),
                accessTokenRepository.Object,
                userRepository.Object,
                Mock.Of<ILogRepository>(),
                Mock.Of<IStatRepository>(),
                Mock.Of<IDbCacheRepository>(),
                Mock.Of<IUserGroupRepository>(),
                Mock.Of<IUsersInGroupsRepository>(),
                new TestCacheManager());

            var result = await controller.Get("alice");

            Assert.IsType<UnauthorizedResult>(result.Result);
            userRepository.Verify(x => x.IsUserNameExistsAsync(It.IsAny<string>()), Times.Never);
            userRepository.Verify(x => x.GetByUserNameAsync(It.IsAny<string>()), Times.Never);
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
